using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace NetworkManager
{
    public partial class App : Application
    {
        private static List<CultureInfo> m_Langs = new List<CultureInfo>();
        public static EventHandler LangChanged;

        public static List<CultureInfo> Langs
        {
            get { return m_Langs; }
        }

        public App()
        {
            InitializeComponent();
            m_Langs.Clear();
            m_Langs.Add(new CultureInfo("en-US"));
            m_Langs.Add(new CultureInfo("ru-RU"));
        }

        public static CultureInfo Lang
        {
            get { return Thread.CurrentThread.CurrentUICulture; }

            set 
            {
                if (value == null) throw new ArgumentException("value");
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                // Change lang
                Thread.CurrentThread.CurrentCulture = value;

                // Create Res dictionary for new culture
                ResourceDictionary dictionary = new ResourceDictionary();

                switch (value.Name)
                {
                    case "ru-RU": 
                        dictionary.Source = new Uri(string.Format("UI/lang.{0}.xaml", value.Name), UriKind.Relative); break;
                    default:
                        dictionary.Source = new Uri("UI/lang.en-US.xaml", UriKind.Relative); break;
                }

                // Find and delete old dictionary. Add new...
                ResourceDictionary oldDictionary = (from d in Current.Resources.MergedDictionaries where d.Source != null && d.Source.OriginalString.StartsWith("UI/lang.") select d).First();

                if (oldDictionary != null)
                {
                    int index = Current.Resources.MergedDictionaries.IndexOf(oldDictionary);
                    Current.Resources.MergedDictionaries.Remove(oldDictionary);
                    Current.Resources.MergedDictionaries.Insert(index, dictionary);
                }
                else
                    Current.Resources.MergedDictionaries.Add(dictionary);

                // Create new event for signal all wnd
                LangChanged(Current, new EventArgs());
            }
        }
    }
}
