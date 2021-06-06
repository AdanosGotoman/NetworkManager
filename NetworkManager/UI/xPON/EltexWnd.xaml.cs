using NetworkManager.TelnetCore;
using System;
using System.IO;
using System.Windows;
using ToolkitUI.TelnetCore;

namespace NetworkManager.UI.xPON
{
    public partial class EltexWnd : Window
    {
        TelnetCommander commander;
        Telnet telnet;
        TextWriter writer;
        string auth;

        public EltexWnd() { InitializeComponent(); WindowState = WindowState.Maximized; }

        private void Connect(object sender, RoutedEventArgs e)
        {
            string ip = ipField.Text;
            string login = loginField.Text;
            string password = passField.Text;

            commander = new TelnetCommander(ip);
            telnet = commander.GetTelnet();

            auth = telnet.Login(login, password, 1000);
            logger.Text = auth;
        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            commander.CloseConnection();

            writer = new RedirectOutput(logger);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
            ClearAll(sender, e);

            MessageBox.Show("Connection closed successful");
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

            commander.ShowUnactivatedEltex(dataSlot);

            writer = new RedirectOutput(unactivatedSlot);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowBySlotPort(object sender, RoutedEventArgs e)
        {
            string dataSlotPort = slot_port.Text;

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
            logger.Text = "";
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
        }
    }
}