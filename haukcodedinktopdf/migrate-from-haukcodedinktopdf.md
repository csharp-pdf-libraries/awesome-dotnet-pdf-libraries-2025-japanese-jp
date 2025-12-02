---
**  (Japanese Translation)**

 **English:** [haukcodedinktopdf/migrate-from-haukcodedinktopdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/haukcodedinktopdf/migrate-from-haukcodedinktopdf.md)
 **:** [haukcodedinktopdf/migrate-from-haukcodedinktopdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/haukcodedinktopdf/migrate-from-haukcodedinktopdf.md)

---
# Haukcode.DinkToPdfからIronPDFへの移行方法は？

## 目次

1. [Haukcode.DinkToPdfからIronPDFへ移行する理由](#haukcodedinktopdfからironpdfへ移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [セキュリティの改善](#セキュリティの改善)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## Haukcode.DinkToPdfからIronPDFへ移行する理由

### 重大なセキュリティ問題

Haukcode.DinkToPdfは、放棄されたDinkToPdfプロジェクトのフォークであり、wkhtmltopdfバイナリをラップしています。**wkhtmltopdfには、プロジェクトが放棄されたために修正されることのない重大なセキュリティ脆弱性があります。**

#### CVE-2022-35583 - 重大なSSRF脆弱性 (CVSS 9.8)

wkhtmltopdfライブラリ（およびHaukcode.DinkToPdfを含むすべてのラッパー）は、**サーバーサイドリクエストフォージェリ（SSRF）**に対して脆弱です：

- **攻撃ベクトル**：悪意のあるHTMLコンテンツがサーバーに内部リソースをフェッチさせる
- **AWSメタデータ攻撃**：`http://169.254.169.254`にアクセスしてAWSクレデンシャルを盗む
- **内部ネットワークアクセス**：内部サービスをスキャンしてアクセスする
- **ローカルファイルインクルージョン**：`file://`プロトコルを介してローカルファイルを読み取る
- **影響**：インフラストラクチャの完全な乗っ取りが可能

**この脆弱性に対する修正はありません**。wkhtmltopdfは放棄されており（2023年1月にアーカイブされています）。

### wkhtmltopdfの追加問題

1. **放棄されたプロジェクト**：最後のリリースは2020年の0.12.6；プロジェクトは2023年にアーカイブされました
2. **古いWebKitエンジン**：~2015年のQt WebKitを使用しており、数年分のセキュリティパッチが欠けています
3. **HTML5/CSS3のサポートなし**：現代のウェブ標準の限定的なレンダリング
4. **ネイティブバイナリの依存**：プラットフォーム固有のバイナリ（Windows/Linux/macOS）を配布する必要があります
5. **スレッドセーフティの問題**：`SynchronizedConverter`シングルトンパターンが必要
6. **JavaScriptの制限**：限定的かつ安全でないJavaScriptの実行

### 代わりにIronPDFが提供するもの

| 項目 | Haukcode.DinkToPdf | IronPDF |
|------|-------------------|---------|
| 基盤エンジン | wkhtmltopdf (Qt WebKit ~2015) | Chromium (定期的に更新) |
| セキュリティ状況 | CVE-2022-35583 (重大、修正不可) | アクティブにパッチ適用 |
| プロジェクト状況 | 放棄されたプロジェクトのフォーク | アクティブに開発中 |
| HTML5/CSS3 | 限定的 | 完全サポート |
| JavaScript | 限定的、安全でない | 完全なV8エンジン |
| ネイティブバイナリ | 必要（プラットフォーム固有） | 自己完結型 |
| スレッドセーフティ | シングルトンパターンが必要 | 設計によるスレッドセーフ |
| サポート | コミュニティのみ | プロフェッショナルサポート |
| アップデート | 予定なし | 定期的なリリース |

---

## 開始する前に

### 前提条件

1. **.NETバージョン**：IronPDFは.NET Framework 4.6.2+および.NET Core 3.1+ / .NET 5+をサポートします
2. **ライセンスキー**：[IronPDFウェブサイト](https://ironpdf.com/licensing/)から取得
3. **ネイティブバイナリの削除**：wkhtmltopdf DLL/バイナリを削除する計画を立てる

### Haukcode.DinkToPdfの使用箇所の特定

コードベース内のすべてのDinkToPdfの使用箇所を見つけます：

```bash
# DinkToPdf名前空間の使用箇所を検索
grep -r "using DinkToPdf\|using Haukcode" --include="*.cs" .

# コンバーターの使用箇所を検索
grep -r "SynchronizedConverter\|BasicConverter\|HtmlToPdfDocument" --include="*.cs" .

# ネイティブライブラリの読み込みを検索
grep -r "wkhtmltopdf\|libwkhtmltox" --include="*.cs" --include="*.csproj" .

# GlobalSettings/ObjectSettingsの使用箇所を検索
grep -r "GlobalSettings\|ObjectSettings\|MarginSettings" --include="*.cs" .
```

### 依存関係の監査

プロジェクトファイルでDinkToPdfパッケージをチェックします：

```bash
grep -r "DinkToPdf\|Haukcode" --include="*.csproj" .
```

一般的なパッケージ名：
- `DinkToPdf`
- `Haukcode.DinkToPdf`
- `Haukcode.WkHtmlToPdf-DotNet`

---

## クイックスタート移行

### ステップ1：DinkToPdfとネイティブバイナリを削除

```bash
# NuGetパッケージを削除
dotnet remove package DinkToPdf
dotnet remove package Haukcode.DinkToPdf
dotnet remove package Haukcode.WkHtmlToPdf-DotNet

# IronPDFをインストール
dotnet add package IronPdf
```

**ネイティブバイナリを削除：**
- `libwkhtmltox.dll` (Windows)
- `libwkhtmltox.so` (Linux)
- `libwkhtmltox.dylib` (macOS)

### ステップ2：コードを更新

**更新前 (Haukcode.DinkToPdf):**
```csharp
using DinkToPdf;
using DinkToPdf.Contracts;

public class PdfService
{
    private readonly IConverter _converter;

    public PdfService()
    {
        _converter = new SynchronizedConverter(new PdfTools());
    }

    public byte[] GeneratePdf(string html)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10 }
            },
            Objects = {
                new ObjectSettings {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        return _converter.Convert(doc);
    }
}
```

**更新後 (IronPDF):**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        _renderer.RenderingOptions.MarginTop = 10;
        _renderer.RenderingOptions.MarginBottom = 10;
    }

    public byte[] GeneratePdf(string html)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}

// シングルトンは不要 - IronPDFはスレッドセーフです！
```

### ステップ3：依存性注入を更新

**更新前 (Haukcode.DinkToPdf):**
```csharp
// Startup.cs - スレッドセーフティの問題のためシングルトンが必須
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
}
```

**更新後 (IronPDF):**
```csharp
// Startup.cs - シングルトンでもトランジェントでもどちらでも動作（どちらも安全）
public void ConfigureServices(IServiceCollection services)
{
    IronPdf.License.LicenseKey = Configuration["IronPdf:LicenseKey"];
    services.AddSingleton<IPdfService, IronPdfService>();
    // または services.AddTransient<IPdfService, IronPdfService>() - どちらも安全です！
}
```

---