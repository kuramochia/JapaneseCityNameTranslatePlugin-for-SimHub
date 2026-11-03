using log4net.Plugin;
using SimHub.Plugins;
using System;
using System.Configuration;
using System.Diagnostics;
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
            secretsTextBox.Password = plugin.Settings.Secrets;
        }

        private void updateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Plugin.Settings.Url = urlTextBox.Text;
            this.Plugin.Settings.Region = regionTextBox.Text;
            this.Plugin.Settings.Secrets = secretsTextBox.Password;
            this.Plugin.Save();
            MessageBox.Show("Settings updated successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void maintenanceJson_Click(object sender, RoutedEventArgs e)
        {
            // 設定の保存
            this.Plugin.Save();
            // Json の手動編集画面を開く
            var jsonPath = this.Plugin.GetPluginJsonPath();
            
            // Json パスを直接プロセスに渡して起動
            using(var p = new Process())
            {
                p.StartInfo.FileName = jsonPath;
                p.Start();
            }
        }

        // 設定の再読み込みボタン
        private void reloadJson_Click(object sender, RoutedEventArgs e)
        {
            this.Plugin.LoadSettings();
        }
    }   
}