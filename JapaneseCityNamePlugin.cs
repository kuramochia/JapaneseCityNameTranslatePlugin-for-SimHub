using GameReaderCommon;
using SimHub.Plugins;
using System.Threading;
using System.Windows.Media;

namespace Kuramochia.JapaneseCityNamePlugin
{
    [PluginName("Japanese City Name Translate Plugin")]
    [PluginDescription("地名を日本語に翻訳して表示します")]
    [PluginAuthor("kuramochia")]
    public class JapaneseCityNamePlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public JapaneseCityNamePluginSettings Settings;
        public CancellationTokenSource EndTokenSource = new CancellationTokenSource();

        //public PJLocalization Localization { get; private set; }

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "Japanese City Name Translate Plugin";

        private JapaneseCityNameLocalization _localization;

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            EndTokenSource.Cancel();
            EndTokenSource.Dispose();
            _localization?.Dispose();
            Save();
        }

        public void Save()
        {
            this.SaveCommonSettings("Settings", Settings);
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager) => new JapaneseCityNamePluginSettingsControl(this);

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            // Load settings
            Settings = this.ReadCommonSettings<JapaneseCityNamePluginSettings>("Settings", () => new JapaneseCityNamePluginSettings());
            _localization = new JapaneseCityNameLocalization(this);
        }

        void IDataPlugin.DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // Update Data
            if (data.OldData != null)
            {
                if (data.GameName == "ETS2" || data.GameName == "ATS")
                {
                    _localization.DataUpdate();
                }
            }
        }
    }
}