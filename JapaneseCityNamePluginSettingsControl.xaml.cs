using System;
using System.Windows;
using System.Windows.Controls;

namespace Kuramochia.JapaneseCityNamePlugin
{
    /// <summary>
    /// Logique d'interaction pour SettingsControlDemo.xaml
    /// </summary>
    public partial class JapaneseCityNamePluginSettingsControl : UserControl
    {
        public JapaneseCityNamePlugin Plugin { get; }

        public JapaneseCityNamePluginSettingsControl()
        {
            InitializeComponent();
        }

        public JapaneseCityNamePluginSettingsControl(JapaneseCityNamePlugin plugin) : this()
        {
            this.Plugin = plugin;
            urlTextBox.Text = plugin.Settings.Url;
            regionTextBox.Text = plugin.Settings.Region;
            secretsTextBox.Text = plugin.Settings.Secrets;
        }

        private void updateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Plugin.Settings.Url = urlTextBox.Text;
            this.Plugin.Settings.Region = regionTextBox.Text;
            this.Plugin.Settings.Secrets = secretsTextBox.Text;
            this.Plugin.Save();
            MessageBox.Show("Settings updated successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }   
}