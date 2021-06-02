using NetworkManager.TelnetCore;
using System;
using System.IO;
using System.Windows;
using ToolkitUI.TelnetCore;

namespace NetworkManager.UI.xPON
{
    public partial class HuaweiWnd : Window
    {
        public HuaweiWnd()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }

        TelnetCommander commander;
        Telnet telnet;
        TextWriter writer;
        string auth;

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
            commander.CloseConnectionHuawei();
            commander.SendCommand("y");

            writer = new RedirectOutput(logger);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());

            MessageBox.Show("Connection closed successful");
            ClearAll(sender, e);
        }

        // To get all types of configuration at session start
        private void Run(object sender, RoutedEventArgs e)
        {

        }

        // To update a specific configuration 
        private void UpdateConfiguration(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            commander.EnableAdmin_V1();
            commander.EnableAdmin_V2();
            commander.ShowConfigHuawei(data);
            commander.CloseInterfaceMode();
            commander.DisableAdmin();

            writer = new RedirectOutput(configWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateState(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            commander.EnableAdmin_V1();
            commander.EnableAdmin_V2();
            commander.ShowStateHuawei(data);
            commander.CloseInterfaceMode();
            commander.DisableAdmin();

            writer = new RedirectOutput(stateWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowListONTBySlot(object sender, RoutedEventArgs e)
        {
            string data = userData.Text;

            commander.EnableAdmin_V1();
            commander.EnableAdmin_V2();
            commander.EnableAdmin_V3(data);
            commander.AutofindONTBySlot(slotNumber.Text);
            commander.CloseInterfaceMode();
            commander.DisableAdmin();

            writer = new RedirectOutput(ontListBySlot);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void AutofindAllONT(object sender, RoutedEventArgs e)
        {
            commander.EnableAdmin_V1();
            commander.AutofindAllONT();
            commander.DisableAdmin();

            writer = new RedirectOutput(ontListAutofind);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowSrvPortsList(object sender, RoutedEventArgs e)
        {
            string data = dataForSrvPorts.Text;
            string ID = ontID.Text;

            commander.EnableAdmin_V1();
            commander.ShowServicePortsHuawei(data, ID);
            commander.SendCommand("\n");
            commander.DisableAdmin();

            writer = new RedirectOutput(servicePortsList);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowMacTable(object sender, RoutedEventArgs e)
        {
            string data = dataForMAC.Text;
            string ID = macID.Text;

            commander.EnableAdmin_V1();
            commander.ShowMacTableHuawei(data, ID);
            commander.DisableAdmin();

            writer = new RedirectOutput(macTable);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowInfoBySlot(object sender, RoutedEventArgs e)
        {
            string slot = ontSlot.Text;

            commander.EnableAdmin_V1();
            commander.ShowInfoONTBySlot(slot);
            commander.DisableAdmin();

            writer = new RedirectOutput(infoTable);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowONTByMAC(object sender, RoutedEventArgs e)
        {
            string mac = macAddress.Text;

            commander.EnableAdmin_V1();
            commander.SearchONTByMacHuawei(mac);


            writer = new RedirectOutput(searchByMac);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        public void ClearAll(object sender, RoutedEventArgs e)
        {
            logger.Text = "";
            configWND.Text = "";
            stateWND.Text = "";
            ontListAutofind.Text = "";
            servicePortsList.Text = "";
            macTable.Text = "";
            infoTable.Text = "";
            searchByMac.Text = "";
            stateWNDbyPort.Text = "";
            ontListBySlot.Text = "";
            connectionHistory.Text = "";
            lineParams.Text = "";
            profilesWND.Text = "";
            ipField.Text = "";
            loginField.Text = "";
            passField.Text = "";
            userData.Text = "";
        }

        private void ContinueLog(object sender, RoutedEventArgs e)
        {
            commander.SendCommand("\n");

            writer = new RedirectOutput(infoTable);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void ShowConnectionHistory(object sender, RoutedEventArgs e)
        {
            string portID = port_id.Text;
            string data = userData.Text;

            commander.EnableAdmin_V1();
            commander.EnableAdmin_V2();
            commander.EnableAdmin_V3(data);
            commander.ShowConnectionHistoryHuawei(portID);
            commander.CloseInterfaceMode();
            commander.DisableAdmin();

            writer = new RedirectOutput(connectionHistory);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateStateByPort(object sender, RoutedEventArgs e)
        {
            string port = portField.Text;
            string data = userData.Text;

            commander.EnableAdmin_V1();
            commander.EnableAdmin_V2();
            commander.EnableAdmin_V3(data);
            commander.ShowPhysXErrorHuawei(port);

            commander.CloseInterfaceMode();
            commander.DisableAdmin();

            writer = new RedirectOutput(stateWNDbyPort);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateProfiles(object sender, RoutedEventArgs e)
        {
            commander.EnableAdmin_V1();
            commander.ShowProfilesDSL();
            commander.SendCommand("\n");
            commander.DisableAdmin();

            writer = new RedirectOutput(profilesWND);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }

        private void UpdateLineParams(object sender, RoutedEventArgs e)
        {
            commander.EnableAdmin_V1();
            commander.ShowLineParameters(userData.Text);
            commander.SendCommand("\n" + "y\n");

            commander.DisableAdmin();

            writer = new RedirectOutput(lineParams);
            Console.SetOut(writer);
            Console.WriteLine(telnet.Read());
        }
    }
}