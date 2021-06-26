using NetworkManager.TelnetCore;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ToolkitUI.TelnetCore;

namespace NetworkManager
{
    public partial class ETH_Tool : Window
    {
        TelnetCommander commander;
        Telnet telnet;
        TextWriter writer;

        public ETH_Tool()
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

            try { telnet.Login(login, password, 1000); }
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

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            commander.CloseConnection();
            ClearAll(sender, e);

            statusConnection.Content = "Disconnected";
        }

        public void Run(object sender, RoutedEventArgs e)
        {
            UpdateConfiguration(sender, e);
            DoCableTest(sender, e);
            ShowMacTable(sender, e);
            ShowPhysXErrors(sender, e);
        }

        public void UpdateConfiguration(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            if ((bool)faChck.IsChecked)
                commander.ShowConfigMES(data);
            else if ((bool)giChck.IsChecked)
                commander.ShowConfigAggregation(data);
            else if ((bool)ethChck.IsChecked)
                commander.ShowConfigQSW(data);
            else if ((bool)dlinkChck.IsChecked)
                commander.ShowConfigDlink(data);

            writer = new RedirectOutput(configWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void DoCableTest(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            if ((bool)faChck.IsChecked)
            {
                commander.DoCableTestMES(data);
                commander.SendCommand("y\n");
            }

            else if ((bool)ethChck.IsChecked)
                commander.DoCableTestQSW(data);

            else if ((bool)dlinkChck.IsChecked)
                commander.DoCableTestDLink(data);

            Thread.Sleep(2000);

            writer = new RedirectOutput(cableTest);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowMacTable(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            if ((bool)ethChck.IsChecked)
                commander.ShowMacTableQSW(data);
            else if ((bool)faChck.IsChecked)
                commander.ShowMacTableMES(data);
            else if ((bool)giChck.IsChecked)
                commander.ShowMacTableDlink(data);
            else if ((bool)dlinkChck.IsChecked)
                commander.ShowMacTableDlink(data);

            writer = new RedirectOutput(macTable);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ConfigureTerminal(object sender, RoutedEventArgs e)
        {
            commander.EnableAdminQSW();

            if ((bool)ethChck.IsChecked)
            {
                commander.SendCommand("interface ethernet1/0/" + configuringData.Text);
                commander.SendCommand("description " + description.Text);
                commander.SendCommand("lldp disable");
                commander.SendCommand("switchport mode hybrid");
                commander.SendCommand("switchport hybrid allowed vlan " + taggedVLAN.Text + " tag");
                commander.SendCommand("switchport hybrid allowed vlan " + pppoeVLAN.Text + " untag");
                commander.SendCommand("switchport hybrid native vlan " + nativeVLAN.Text);
                commander.SendCommand("switchport association multicast-vlan " + mcastVLAN.Text);
                commander.SendCommand("pppoe intermediate-agent");
                commander.SendCommand("loopback-detection specified-vlan " + loopsVLAN.Text);
                commander.SendCommand("loopback-detection control shutdown");
                commander.SendCommand("loopback-detection send packet number 2");
                commander.SendCommand("exit");
                commander.SendCommand("exit");
                commander.SendCommand("write");
                commander.SendCommand("y\n");
            }

            else if ((bool)faChck.IsChecked)
            {
                commander.SendCommand("interface fastethernet1/0/" + configuringData.Text);
                commander.SendCommand("loopback-detection enable");
                commander.SendCommand("switchport mode trunk");
                commander.SendCommand("switchport trunk allowed vlan add " + taggedVLAN.Text);
                commander.SendCommand("switchport trunk native vlan " + nativeVLAN.Text);
                commander.SendCommand("switchport protected-port");
                commander.SendCommand("bridge multicast unregistered filtering");
                commander.SendCommand("spanning-tree disable");
                commander.SendCommand("no lldp transmit");
                commander.SendCommand("no lldp receive");
                commander.SendCommand("switchport forbidden default-vlan");
                commander.SendCommand("switchport trunk multicast-tv vlan " + mcastVLAN.Text);
                commander.SendCommand("pppoe intermediate-agent");
                commander.SendCommand("multicast snooping add 1");
                commander.SendCommand("exit");
                commander.SendCommand("exit");
                commander.SendCommand("write");
                commander.SendCommand("y\n");
            }
                
            else if ((bool)giChck.IsChecked)
                commander.SendCommand("interface gigabitethernet1/0/" + configuringData.Text);



            writer = new RedirectOutput(configuringWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowPhysXErrors(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            if ((bool)faChck.IsChecked)
                commander.ShowPhysXErrorsMES(data);
            else if ((bool)giChck.IsChecked)
                commander.ShowPhysXErrorsAggregation(data);
            else if ((bool)ethChck.IsChecked)
            {
                commander.ShowPhysXErrorQSW(data);
                commander.SendCommand("q");
            }

            else if ((bool)dlinkChck.IsChecked)
                commander.ShowPhysXErrorDlink(data);

            writer = new RedirectOutput(physxError);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowVlanList(object sender, RoutedEventArgs e)
        {
            commander.ShowAllVlanList();
            commander.SendCommand("q");

            writer = new RedirectOutput(vlanList);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        public void ClearAll(object sender, RoutedEventArgs e)
        {
            configWND.Text = "";
            cableTest.Text = "";
            macTable.Text = "";
            physxError.Text = "";
            configuringWND.Text = "";
            vlanList.Text = "";
            interfacesDescrWND.Text = "";
            configWNDAggregate.Text = "";
            ipField.Text = "";
            loginField.Text = "";
            passField.Text = "";
            userData.Text = "";
        }

        private void ShowInterfacesDescription(object sender, RoutedEventArgs e)
        {
            commander.ShowInterfacesDescriptionAggregation();

            writer = new RedirectOutput(interfacesDescrWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        // For configuring aggregation (MES3124)
        private void ConfigureAggregate(object sender, RoutedEventArgs e)
        {
            commander.SendCommand("config");
            commander.SendCommand("interface GigabitEthernet 1/0/" + portFieldAggregate.Text);
            commander.SendCommand("description " + descAggr.Text);
            commander.SendCommand("exit");
            commander.SendCommand("write");
            commander.SendCommand("y");
            commander.SendCommand("exit");

            writer = new RedirectOutput(configWNDAggregate);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void LoginCentrum(object sender, RoutedEventArgs e)
        {

        }
    }
}