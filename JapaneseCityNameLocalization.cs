using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kuramochia.JapaneseCityNamePlugin
{
    public class JapaneseCityNameLocalization : IDisposable
    {
        private const string JobCitySourcePropertyName = "Job.CitySource";
        private const string JobCityDestinationPropertyName = "Job.CityDestination";

        private readonly JapaneseCityNamePlugin _plugin;

        public HttpClient _httpclient;

        // 追加: Dispose 管理用フラグ
        private bool _disposed;

        public JapaneseCityNameLocalization(JapaneseCityNamePlugin plugin)
        {
            _plugin = plugin;
            _plugin.PluginManager.AddProperty(JobCitySourcePropertyName, _plugin.GetType(), "", "日本語に自動翻訳された配送元都市名（ゲーム内表記と同じとは限りません）");
            _plugin.PluginManager.AddProperty(JobCityDestinationPropertyName, _plugin.GetType(), "", "日本語に自動翻訳された配送先都市名（ゲーム内表記と同じとは限りません）");

            _httpclient = new HttpClient();
        }

        private Dictionary<string, Task<Tuple<string, string>>> _translationTasks = new Dictionary<string, Task<Tuple<string, string>>>();

        public void DataUpdate()
        {
            if (_plugin.PluginManager.GameName == "ETS2" || _plugin.PluginManager.GameName == "ATS")
            {
                // 標準の都市名
                string defaultCitySourceName = _plugin.PluginManager.GetPropertyValue("GameRawData.JobValues.CitySource")?.ToString();
                string defaultCityDestinationName = _plugin.PluginManager.GetPropertyValue("GameRawData.JobValues.CityDestination")?.ToString();

                if (!string.IsNullOrEmpty(defaultCitySourceName))
                {
                    // CitySource の確認
                    if (_plugin.Settings.TranslatedCities.TryGetValue(defaultCitySourceName, out string translatedCitySource))
                    {
                        // 翻訳済みの都市名が存在する場合はそれを使用
                        _plugin.PluginManager.SetPropertyValue(JobCitySourcePropertyName, _plugin.GetType(), translatedCitySource);
                    }
                    else if (_translationTasks.TryGetValue(defaultCitySourceName, out Task<Tuple<string, string>> existingTask))
                    {
                        // 既に翻訳タスクが存在する場合はそれを使用
                        if (existingTask.IsCompleted)
                        {
                            // タスクが完了している場合は結果を取得して設定
                            var result = existingTask.Result;
                            _plugin.PluginManager.SetPropertyValue(JobCitySourcePropertyName, _plugin.GetType(), result.Item2 ?? result.Item1);
                        }
                        else
                        {
                            // タスクがまだ完了していない場合は、元の都市名を設定
                            _plugin.PluginManager.SetPropertyValue(JobCitySourcePropertyName, _plugin.GetType(), defaultCitySourceName);
                        }
                    }
                    else
                    {
                        // 未実行の翻訳タスクを作成する
                        var t = Task<Task<Tuple<string, string>>>.Run(async () =>
                        {
                            var result = await TranslateAsync(defaultCitySourceName);
                            if (result.Item2 != null)
                            {
                                _plugin.Settings.TranslatedCities[defaultCitySourceName] = result.Item2;
                                _plugin.PluginManager.SetPropertyValue(JobCitySourcePropertyName, _plugin.GetType(), result.Item2);
                                _translationTasks.Remove(defaultCitySourceName);
                            }
                            return result;
                        });
                        // タスクを辞書に追加して管理
                        _translationTasks[defaultCitySourceName] = t;
                        // 元の都市名を設定しておく
                        _plugin.PluginManager.SetPropertyValue(JobCitySourcePropertyName, _plugin.GetType(), defaultCitySourceName);
                    }
                }

                if (!string.IsNullOrEmpty(defaultCityDestinationName))
                {
                    // CityDestination の確認
                    if (_plugin.Settings.TranslatedCities.TryGetValue(defaultCityDestinationName, out string translatedCityDestination))
                    {
                        // 翻訳済みの都市名が存在する場合はそれを使用
                        _plugin.PluginManager.SetPropertyValue(JobCityDestinationPropertyName, _plugin.GetType(), translatedCityDestination);
                    }
                    else if (_translationTasks.TryGetValue(defaultCityDestinationName, out Task<Tuple<string, string>> existingTask))
                    {
                        // 既に翻訳タスクが存在する場合はそれを使用
                        if (existingTask.IsCompleted)
                        {
                            // タスクが完了している場合は結果を取得して設定
                            var result = existingTask.Result;
                            _plugin.PluginManager.SetPropertyValue(JobCityDestinationPropertyName, _plugin.GetType(), result.Item2 ?? result.Item1);
                        }
                        else
                        {
                            // タスクがまだ完了していない場合は、元の都市名を設定
                            _plugin.PluginManager.SetPropertyValue(JobCityDestinationPropertyName, _plugin.GetType(), defaultCityDestinationName);
                        }
                    }
                    else
                    {
                        // 未実行の翻訳タスクを作成する
                        var t = Task<Task<Tuple<string, string>>>.Run(async () =>
                        {
                            var result = await TranslateAsync(defaultCityDestinationName);
                            // item2 が null でない場合のみ翻訳結果を保存する
                            if (result.Item2 != null)
                            {
                                _plugin.Settings.TranslatedCities[defaultCityDestinationName] = result.Item2;
                                _plugin.PluginManager.SetPropertyValue(JobCityDestinationPropertyName, _plugin.GetType(), result.Item2);
                                _translationTasks.Remove(defaultCityDestinationName);
                            }
                            return result;
                        });
                        // タスクを辞書に追加して管理
                        _translationTasks[defaultCityDestinationName] = t;
                        // 元の都市名を設定しておく
                        _plugin.PluginManager.SetPropertyValue(JobCityDestinationPropertyName, _plugin.GetType(), defaultCityDestinationName);
                    }
                }
            }
        }

        private async Task<Tuple<string, string>> TranslateAsync(string cityName)
        {
            // 非同期で翻訳を行う処理をここに実装する
            // 例: Microsoft Translator API を使用して cityName を翻訳する
            // 翻訳結果を Tuple<string, string> として返す
            using (var request = new HttpRequestMessage(HttpMethod.Post, _plugin.Settings.Url + "translate?api-version=3.0&to=ja-JP"))
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", _plugin.Settings.Secrets);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _plugin.Settings.Region);
                request.Content = new StringContent($"[{{\"text\": \"{cityName}\"}}]", System.Text.Encoding.UTF8, "application/json");

                try
                {
                    using (var response = await _httpclient.SendAsync(request, _plugin.EndTokenSource.Token))
                    {
                        // EnsureSuccessStatusCode() を呼び出して、HTTP ステータスコードが成功でない場合に例外をスローする
                        response.EnsureSuccessStatusCode();
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        // 例: [{"detectedLanguage":..., "translations":[{"text":"東京"...}]}]
                        // translations 配列の先頭要素の text のみを取得する
                        var translatedResult = JArray.Parse(jsonResponse);
                        var translatedCityName = translatedResult[0]?["translations"]?[0]?["text"]?.Value<string>() ?? cityName;

                        return new Tuple<string, string>(cityName, translatedCityName);
                    }
                }
                catch (Exception ex)
                {
                    SimHub.Logging.Current.Error("Translation API request failed", ex);
                    return new Tuple<string, string>(cityName, null);
                }
            }

        }

        // IDisposable 実装
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // マネージリソースの解放
                _httpclient?.Dispose();
                _httpclient = null;
            }
            // アンマネージリソースがあればここで解放

            _disposed = true;
        }
    }
}
