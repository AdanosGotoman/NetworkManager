using NetworkManager.SNMP;
using NetworkManager.UI.xPON;
using NetworkManager.Utilities;
using System.Windows;

namespace NetworkManager.UI
{
    public partial class Selector : Window
    {
        public Selector() { InitializeComponent(); }

        private void OpenHuawei(object sender, RoutedEventArgs e)
        {
            HuaweiWnd wnd = new HuaweiWnd();
            wnd.Show();
            //Application.Current.Windows[0].Close();
        }

        private void OpenEltex(object sender, RoutedEventArgs e)
        {
            EltexWnd wnd = new EltexWnd();
            wnd.Show();
            //Application.Current.Windows[0].Close();
        }

        private void OpenEthernet(object sender, RoutedEventArgs e)
        {
            ETH_Tool wnd = new ETH_Tool();
            wnd.Show();
            //Application.Current.Windows[0].Close();
        }

        private void OpenSNMP(object sender, RoutedEventArgs e)
        {
            LoggerSNMP wnd = new LoggerSNMP();
            wnd.Show();
        }

        private void OpenSubChecker(object sender, RoutedEventArgs e)
        {
            SubscribersChecker wnd = new SubscribersChecker();
            wnd.Show();
        }
    }
}