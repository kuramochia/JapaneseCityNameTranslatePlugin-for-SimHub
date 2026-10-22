using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Kuramochia.JapaneseCityNamePlugin
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    public class JapaneseCityNamePluginSettings
    {
        public string Url { get; set; } = "https://api.cognitive.microsofttranslator.com/";

        public string Region { get; set; } = "japaneast";

        [JsonConverter(typeof(DpapiStringJsonConverter))]
        public string Secrets { get; set; } = "";

        public Dictionary<string, string> TranslatedCities { get; set; } = new Dictionary<string, string>();
    }

    public sealed class DpapiStringJsonConverter : JsonConverter
    {
        private const string EncryptedValuePrefix = "dpapi:";

        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = (string)reader.Value;
            if (value == null || !value.StartsWith(EncryptedValuePrefix, StringComparison.Ordinal))
            {
                return value;
            }

            var encryptedBytes = Convert.FromBase64String(value.Substring(EncryptedValuePrefix.Length));
            var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var plainBytes = Encoding.UTF8.GetBytes((string)value);
            var encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
            writer.WriteValue(EncryptedValuePrefix + Convert.ToBase64String(encryptedBytes));
        }
    }
}