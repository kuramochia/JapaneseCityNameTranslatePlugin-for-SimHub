using System;
using System.Collections.Generic;

namespace Kuramochia.JapaneseCityNamePlugin
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    public class JapaneseCityNamePluginSettings
    {
        public string Url { get; set; } = "https://api.cognitive.microsofttranslator.com/";

        public string Region { get; set; } = "japaneast";

        public string Secrets { get; set; } = "";

        public Dictionary<string, string> TranslatedCities { get; set; } = new Dictionary<string, string>();
    }
}