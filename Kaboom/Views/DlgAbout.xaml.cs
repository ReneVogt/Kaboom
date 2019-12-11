using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace Com.Revo.Games.Kaboom.Views
{
    /// <summary>
    /// Interaktionslogik für DlgAbout.xaml
    /// </summary>
    public partial class DlgAbout
    {
        public DlgAbout()
        {
            InitializeComponent();
        }

        void DlgAbout_OnLoaded(object sender, RoutedEventArgs e)
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            lbVersion.Content = $"Kaboom v{v.Major}.{v.Minor}.{v.Build}";
        }
        void OnLinkClicked(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
