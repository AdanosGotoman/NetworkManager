using NetworkManager.TelnetCore;
using System;
using System.IO;
using System.Windows;
using ToolkitUI.TelnetCore;

namespace NetworkManager
{
    public partial class ETH_Tool : Window
    {
        TelnetCommander commander;
        Telnet telnet;
        TextWriter writer;
        string auth;

        public ETH_Tool() { InitializeComponent(); WindowState = WindowState.Maximized; }

        private void Connect(object sender, RoutedEventArgs e)
        {
            string ip = ipField.Text;
            string login = loginField.Text;
            string password = passField.Text;

            commander = new TelnetCommander(ip);
            telnet = commander.GetTelnet();

            auth = telnet.Login(login, password, 1200);
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

        private void ConfigureTerminal(object sender, RoutedEventArgs e) // QSW only!
        {
            commander.EnableAdminQSW();
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
                commander.ShowPhysXErrorQSW(data);
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
            logger.Text = "";
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

            writer = new RedirectOutput(configWNDAggregate);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());

        }

        private void SendDescription(object sender, RoutedEventArgs e)
        {
            commander.SendCommand("description " + descAggr.Text);
            commander.SendCommand("exit");
            commander.SendCommand("write");
            commander.SendCommand("y");

            writer = new RedirectOutput(configWNDAggregate);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void CloseConfig(object sender, RoutedEventArgs e)
        {
            commander.SendCommand("exit");

            writer = new RedirectOutput(configWNDAggregate);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }
    }
}