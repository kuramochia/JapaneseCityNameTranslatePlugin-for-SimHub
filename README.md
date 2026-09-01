# Japanese City Name Translate Plugin for SimHub

## これは何？

Euro Truck Simulator 2（ETS2）および American Truck Simulator（ATS）の配送元・配送先都市名を、Microsoft Azure Translator を使って日本語に翻訳する [SimHub](https://www.simhubdash.com/) プラグインです。

翻訳した都市名は SimHub のプロパティとして追加されるため、ダッシュボードの NCalc や JavaScript から利用できます。一度翻訳に成功した都市名はキャッシュされ、次回から翻訳 API へ再度問い合わせずに表示されます。

> [!IMPORTANT]
> このプラグインを利用するには、Microsoft Azure の Translator リソースと API キーが必要です。Translator の利用料金や無料利用枠については、Microsoft Azure の最新情報を確認してください。

## 対応ゲーム

- Euro Truck Simulator 2
- American Truck Simulator

## インストール方法

1. [Releases](https://github.com/kuramochia/JapaneseCityNameTranslatePlugin-for-SimHub/releases) から `kuramochia.JapaneseCityNamePlugin.dll` をダウンロードします。
2. DLL を SimHub のインストール先にコピーします。通常は `C:\Program Files (x86)\SimHub` です。
3. SimHub を再起動します。
4. プラグインをロードする確認ダイアログが表示された場合は、内容を確認して有効化します。

## Azure Translator の準備

1. [Azure portal](https://portal.azure.com/) で Translator リソースを作成します。
2. リソースの「キーとエンドポイント」画面から、キー、リージョン、エンドポイントを確認します。
3. API キーは第三者に公開しないでください。

## 設定方法

SimHub の左メニューから **Japanese City Name Translate Plugin** を開き、次の項目を入力します。

| 項目 | 設定内容 | 例 |
| :--- | :--- | :--- |
| URL | Azure Translator のエンドポイント。通常は変更不要です。末尾の `/` を含めてください。 | `https://api.cognitive.microsofttranslator.com/` |
| Region | Translator リソースのリージョン | `japaneast` |
| Secrets | Translator リソースの API キー | Azure portal で取得したキー |

入力後、**設定保存** ボタンをクリックしてください。

## 追加されるプロパティ

| プロパティ名 | 内容 | 翻訳前・翻訳失敗時 |
| :--- | :--- | :--- |
| `JapaneseCityNamePlugin.Job.CitySource` | 日本語に翻訳された配送元都市名 | 元の配送元都市名 |
| `JapaneseCityNamePlugin.Job.CityDestination` | 日本語に翻訳された配送先都市名 | 元の配送先都市名 |

翻訳は非同期で実行されます。初めて表示する都市では、翻訳が完了するまでゲームから取得した元の都市名が表示されます。翻訳に成功すると、プロパティの値が日本語へ切り替わります。

翻訳結果はプラグイン設定に都市名ごとに保存されます。すでに翻訳済みの都市名では、保存された結果が利用されます。

## 使用例

SimHub の JavaScript で配送先都市名を取得する例です。

```javascript
return $prop('JapaneseCityNamePlugin.Job.CityDestination');
```

NCalc では、プロパティ一覧から `JapaneseCityNamePlugin.Job.CitySource` または `JapaneseCityNamePlugin.Job.CityDestination` を選択して利用できます。

## 注意事項

- 翻訳結果は機械翻訳のため、ゲーム内の日本語表記や一般的な地名表記と一致しない場合があります。
- 翻訳 API への接続に失敗した場合は、元の都市名が表示されます。SimHub のログに `Translation API request failed` が記録されるため、URL、リージョン、API キー、ネットワーク接続を確認してください。
- API キーを含む設定ファイルや画面のスクリーンショットを公開しないでください。

## 開発・ビルド

このプロジェクトは .NET Framework 4.8 を対象としています。ビルドには SimHub がインストールされた Windows 環境が必要です。

環境変数 `SIMHUB_INSTALL_PATH` に SimHub のインストール先を設定してから、Visual Studio または MSBuild でソリューションをビルドしてください。ビルド後、生成されたプラグインファイルは `SIMHUB_INSTALL_PATH` へコピーされます。

## ライセンス

[MIT License](LICENSE.txt)