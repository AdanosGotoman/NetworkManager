using NetworkManager.TelnetCore;
using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ToolkitUI.TelnetCore;

namespace NetworkManager.UI.xPON
{
    public partial class EltexWnd : Window
    {
        TelnetCommander commander;
        Telnet telnet;
        TextWriter writer;

        public EltexWnd()
        {
            InitializeComponent(); 
            WindowState = WindowState.Maximized;
            App.LangChanged += LangChanged;
            CultureInfo currLang = App.Lang;

            langItem.Items.Clear();
            foreach (var lang in App.Langs)
            {
                MenuItem menulang = new MenuItem();
                menulang.Header = lang.DisplayName;
                menulang.Tag = lang;
                menulang.IsChecked = lang.Equals(currLang);
                menulang.Click += ChangeLang;
                langItem.Items.Add(menulang);
            }
        }

        public void ChangeLang(object sender, RoutedEventArgs e)
        {
            langItem = sender as MenuItem;

            if (langItem != null)
            {
                CultureInfo lang = langItem.Tag as CultureInfo;
                if (lang != null)
                    App.Lang = lang;
            }
        }

        private void LangChanged(object sender, EventArgs e)
        {
            CultureInfo currLang = App.Lang;

            foreach (MenuItem langItem in menu.Items)
            {
                CultureInfo info = langItem.Tag as CultureInfo;
                langItem.IsChecked = info != null && info.Equals(currLang);
            }
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            string ip = ipField.Text;
            string login = loginField.Text;
            string password = passField.Text;

            commander = new TelnetCommander(ip);
            telnet = commander.GetTelnet();

            try  { telnet.Login(login, password, 1000); }
            catch (Exception exc) 
            {
                if (telnet.IsConnected)
                    statusConnection.Content = "Connected";
                else
                {
                    statusConnection.Content = "Connection ERROR";
                    MessageBox.Show("Error: " + exc.Message);
                }
            }
        }

        public async Task DetectConnectionLoop()
        {
            byte[] buffer = new byte[1];
            ArraySegment<byte> arrSegment = new ArraySegment<byte>(buffer, 0, 0);
            SocketFlags flags = SocketFlags.None;

            CancellationTokenSource src = new CancellationTokenSource();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            while (!src.Token.IsCancellationRequested)
            {
                try
                {
                    await socket.SendAsync(arrSegment, flags);
                    await Task.Delay(500);
                }
                catch (Exception exc)
                {
                    src.Cancel();
                    MessageBox.Show("Error: " + exc.Message);

                    Application.Current.Windows[0].Close();
                    Selector wnd = new Selector();
                    wnd.Show();
                    MessageBox.Show("Please, retry connecting with Centrum");

                    return;
                }
            }
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            commander.CloseConnection();
            ClearAll(sender, e);

            statusConnection.Content = "Disconnected";
        }

        // To get all types of configuration at session start
        private void Run(object sender, RoutedEventArgs e)
        {
            UpdateConfiguration(sender, e);
            UpdateState(sender, e);
            UpdateConnectHistory(sender, e);
            UpdateConnectionState(sender, e);
        }

        // To update a specific configuration 
        private void UpdateConfiguration(object sender, RoutedEventArgs e)
        {
            commander.ShowConfigEltex(userData.Text);

            writer = new RedirectOutput(configWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateState(object sender, RoutedEventArgs e)
        {
            commander.ShowStateEltex(userData.Text);

            writer = new RedirectOutput(stateWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateConnectHistory(object sender, RoutedEventArgs e)
        {
            commander.ShowConnectionsEltex(userData.Text);

            writer = new RedirectOutput(connectionHistory);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateConnectionState(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            commander.ShowOnlineEltex(data);
            commander.ShowOfflineEltex(data);

            writer = new RedirectOutput(connectionState);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowBySlot(object sender, RoutedEventArgs e)
        {
            string dataSlot = slot.Text;

            if (dataSlot.Contains("9"))
                MessageBox.Show("MA4000 doesn't contains more 8 slots");
            else
                commander.ShowUnactivatedEltex(dataSlot);

            writer = new RedirectOutput(unactivatedSlot);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowBySlotPort(object sender, RoutedEventArgs e)
        {
            string dataSlotPort = slot_port.Text;

            if (dataSlotPort.Contains("1/8"))
                MessageBox.Show("MA4000 doesn't contains more 8 ports");
            else
                commander.ShowUnactivatedEltex(dataSlotPort);

            writer = new RedirectOutput(unactivatedSlotPort);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void DeleteONT(object sender, RoutedEventArgs e)
        {
            string userData = configuringData.Text;

            commander.SendCommand("configure terminal");
            commander.SendCommand("interface ont " + userData);
            commander.SendCommand("no serial");
            commander.SendCommand("no description");
            commander.SendCommand("do commit");
            commander.SendCommand("do confirm");
            commander.SendCommand("exit");
            commander.SendCommand("exit");

            writer = new RedirectOutput(configuringWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void RestoreONT(object sender, RoutedEventArgs e)
        {
            string userData = configuringData.Text;

            commander.SendCommand("send omci restore interface ont " + userData);

            writer = new RedirectOutput(configuringWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ConfigureTerminal(object sender, RoutedEventArgs e)
        {
            string userData = configuringData.Text;

            commander.SendCommand("configure terminal");
            commander.SendCommand("interface ont " + userData);
            commander.SendCommand("description " + ontDescription.Text);
            commander.SendCommand("serial " + ontSerial.Text);
            commander.SendCommand("service 0 profile cross-connect" + " HSI");
            commander.SendCommand("service 0 profile dba" + " dba_HSI");
            commander.SendCommand("service 1 profile cross-connect" + " VOIP");
            commander.SendCommand("service 1 profile dba" + " dba_VOIP");
            commander.SendCommand("service 3 profile cross-connect" + " IPTV");
            commander.SendCommand("service 3 profile dba" + " dba_IPTV");
            commander.SendCommand("service 4 profile cross-connect" + " TR-069");
            commander.SendCommand("service 4 profile dba" + " dba_TR-069");
            commander.SendCommand("profile ports" + " ports-IOP");
            commander.SendCommand("profile management" + " management-IOP");
            commander.SendCommand("do commit");
            commander.SendCommand("do confirm");
            commander.SendCommand("exit");
            commander.SendCommand("exit");

            writer = new RedirectOutput(configuringWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowMacTable(object sender, RoutedEventArgs e)
        {
            commander.ShowMacTableEltex(userData.Text);

            writer = new RedirectOutput(macTable);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        public void ClearAll(object sender, RoutedEventArgs e)
        {
            configWND.Text = "";
            stateWND.Text = "";
            connectionHistory.Text = "";
            connectionState.Text = "";
            unactivatedSlot.Text = "";
            unactivatedSlotPort.Text = "";
            configuringWND.Text = "";
            configuringData.Text = "";
            ontDescription.Text = "";
            macTable.Text = "";
            ipField.Text = "";
            loginField.Text = "";
            passField.Text = "";
            userData.Text = "";
        }
    }
}