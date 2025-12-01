---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/watermark-pdf-csharp.md)

---

# C#でPDFにウォーターマークを追加する：テキスト、画像、スタンプガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![最終更新日](https://img.shields.io/badge/Updated-November%202025-green)]()

> ウォーターマークは、文書を保護し、ステータスを示し、ブランディングを追加します。このガイドでは、テキストウォーターマーク、画像スタンプ、およびライブラリ比較を用いた機密マーキングについて説明します。

---

## 目次

1. [クイックスタート](#quick-start)
2. [ライブラリ比較](#library-comparison)
3. [テキストウォーターマーク](#text-watermarks)
4. [画像ウォーターマーク](#image-watermarks)
5. [HTMLウォーターマーク](#html-watermarks)
6. [位置決めとスタイリング](#positioning-and-styling)
7. [一般的な使用例](#common-use-cases)

---

## クイックスタート

### IronPDFでテキストウォーターマークを追加

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
pdf.ApplyWatermark("<h1 style='color:red;opacity:0.5;'>CONFIDENTIAL</h1>");
pdf.SaveAs("watermarked.pdf");
```

### 画像ウォーターマークを追加

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
pdf.ApplyWatermark("<img src='logo.png' style='opacity:0.3;width:200px;'>");
pdf.SaveAs("branded.pdf");
```

---

## ライブラリ比較

### ウォーターマーク機能

| ライブラリ | テキストウォーターマーク | 画像ウォーターマーク | HTMLウォーターマーク | 透明度コントロール | ページごと |
|---------|---------------|-----------------|----------------|-----------------|----------|
| **IronPDF** | ✅ シンプル | ✅ | ✅ ユニーク | ✅ | ✅ |
| iText7 | ✅ | ✅ | ❌ | ✅ | ✅ |
| Aspose.PDF | ✅ | ✅ | ❌ | ✅ | ✅ |
| PDFSharp | ✅ | ✅ | ❌ | ⚠️ | ✅ |
| PuppeteerSharp | ❌ | ❌ | ❌ | ❌ | ❌ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ | ❌ |

**主な利点:** IronPDFはHTML/CSSをウォーターマークに使用し、無限のスタイリングオプションを可能にします。

### コードの複雑さ

**IronPDF — 2行:**
```csharp
pdf.ApplyWatermark("<h1 style='color:gray;opacity:0.5;'>DRAFT</h1>");
```

**iText7 — 25+行:**
```csharp
// PdfCanvas、Rectangle計算、透明度グループ、
// フォントの読み込み、座標位置決め、適切なリソースの解放が必要
```

---

## テキストウォーターマーク

### シンプルテキスト

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("report.pdf");

// 赤い機密スタンプ
pdf.ApplyWatermark("<h1 style='color:red;'>CONFIDENTIAL</h1>");

pdf.SaveAs("confidential-report.pdf");
```

### 斜めテキスト

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 回転したウォーターマーク
pdf.ApplyWatermark(@"
    <h1 style='
        color: gray;
        opacity: 0.3;
        font-size: 72pt;
        transform: rotate(-45deg);
    '>DRAFT</h1>
");

pdf.SaveAs("draft-document.pdf");
```

### 複数行

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

pdf.ApplyWatermark(@"
    <div style='text-align:center;opacity:0.4;'>
        <h1 style='color:#333;'>INTERNAL USE ONLY</h1>
        <p style='color:#666;'>配布禁止</p>
    </div>
");

pdf.SaveAs("internal-document.pdf");
```

---

## 画像ウォーターマーク

### 会社ロゴ

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("invoice.pdf");

// 半透明のロゴを追加
pdf.ApplyWatermark(@"
    <img src='company-logo.png'
         style='opacity:0.2; width:300px;' />
");

pdf.SaveAs("branded-invoice.pdf");
```

### 背景画像

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("certificate.pdf");

// 大きな背景ウォーターマーク
pdf.ApplyWatermark(@"
    <img src='seal.png'
         style='opacity:0.1; width:80%;' />
", VerticalAlignment.Middle, HorizontalAlignment.Center);

pdf.SaveAs("certified-document.pdf");
```

### Base64エンコード画像

```csharp
using IronPdf;

byte[] logoBytes = File.ReadAllBytes("logo.png");
string base64Logo = Convert.ToBase64String(logoBytes);

var pdf = PdfDocument.FromFile("document.pdf");

pdf.ApplyWatermark($@"
    <img src='data:image/png;base64,{base64Logo}'
         style='opacity:0.3; width:200px;' />
");

pdf.SaveAs("watermarked.pdf");
```

---

## HTMLウォーターマーク

IronPDFのユニークな利点：完全なHTML/CSSウォーターマーク。

### 複雑なスタイルのウォーターマーク

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("contract.pdf");

pdf.ApplyWatermark(@"
    <div style='
        border: 3px solid red;
        padding: 20px;
        background: rgba(255,0,0,0.1);
        border-radius: 10px;
    '>
        <h1 style='color:red; margin:0;'>⚠️ DRAFT</h1>
        <p style='color:#666; margin:5px 0 0 0;'>
            法的拘束力なし - レビューが必要
        </p>
    </div>
");

pdf.SaveAs("draft-contract.pdf");
```

### 日付付きダイナミックウォーターマーク

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("report.pdf");

string watermarkHtml = $@"
    <div style='opacity:0.5; text-align:center;'>
        <p style='font-size:10pt; color:#999;'>
            印刷: {DateTime.Now:yyyy-MM-dd HH:mm}
        </p>
        <p style='font-size:8pt; color:#ccc;'>
            このコピーの有効期限は30日です
        </p>
    </div>
";

pdf.ApplyWatermark(watermarkHtml, VerticalAlignment.Bottom, HorizontalAlignment.Center);

pdf.SaveAs("time-stamped.pdf");
```

### QRコードウォーターマーク

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 検証ページへのリンクを含むQRコード
string qrCodeUrl = "https://api.qrserver.com/v1/create-qr-code/?size=100x100&data=https://verify.example.com/doc123";

pdf.ApplyWatermark($@"
    <div style='opacity:0.7;'>
        <img src='{qrCodeUrl}' style='width:80px;' />
        <p style='font-size:8pt; color:#333;'>検証のためにスキャン</p>
    </div>
", VerticalAlignment.Bottom, HorizontalAlignment.Right);

pdf.SaveAs("verifiable-document.pdf");
```

---

## 位置決めとスタイリング

### 位置オプション

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 中央（デフォルト）
pdf.ApplyWatermark("<h1>CENTER</h1>",
    VerticalAlignment.Middle,
    HorizontalAlignment.Center);

// 左上隅
pdf.ApplyWatermark("<h1>TOP LEFT</h1>",
    VerticalAlignment.Top,
    HorizontalAlignment.Left);

// 右下隅
pdf.ApplyWatermark("<h1>BOTTOM RIGHT</h1>",
    VerticalAlignment.Bottom,
    HorizontalAlignment.Right);
```

### 透明度コントロール

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 非常に微妙（10%の透明度）
pdf.ApplyWatermark("<h1 style='opacity:0.1;'>WATERMARK</h1>");

// 中程度（30%の透明度）
pdf.ApplyWatermark("<h1 style='opacity:0.3;'>WATERMARK</h1>");

// 目立つ（60%の透明度）
pdf.ApplyWatermark("<h1 style='opacity:0.6;'>WATERMARK</h1>");
```

### 特定のページのみ

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// 最初のページにのみウォーターマークを適用
var firstPage = pdf.CopyPage(0);
firstPage.ApplyWatermark("<h1>COVER PAGE</h1>");

// 残りのページに異なるウォーターマークを適用
for (int i = 1; i < pdf.PageCount; i++)
{
    var page = pdf.CopyPage(i);
    page.ApplyWatermark("<p style='opacity:0.2;'>Page " + (i + 1) + "</p>",
        VerticalAlignment.Bottom, HorizontalAlignment.Right);
}
```

---

## 一般的な使用例

### 機密文書

```csharp
using IronPdf;

public void ApplyConfidentialWatermark(string inputPath, string outputPath, string classification)
{
    var pdf = PdfDocument.FromFile(inputPath);

    string color = classification switch
    {
        "TOP SECRET" => "red",
        "SECRET" => "orange",
        "CONFIDENTIAL" => "blue",
        _ => "gray"
    };

    pdf.ApplyWatermark($@"
        <div style='text-align:center;'>
            <h1 style='color:{color}; opacity:0.4; font-size:48pt;
                       transform:rotate(-30deg);'>
                {classification}
            </h1>
        </div>
    ");

    pdf.SaveAs(outputPath);
}
```

### 下書き文書

```csharp
using IronPdf;

public void MarkAsDraft(string filePath)
{
    var pdf = PdfDocument.FromFile(filePath);

    pdf.ApplyWatermark(@"
        <div style='transform:rotate(-45deg); opacity:0.15;'>
            <h1 style='font-size:120pt; color:#333; margin:0;'>DRAFT</h1>
        </div>
    ");

    pdf.SaveAs(filePath.Replace(".pdf", "-draft.pdf"));
}
```

### 承認/却下スタンプ

```csharp
using IronPdf;

public void ApplyApprovalStamp(string filePath, bool approved, string approver)
{
    var pdf = PdfDocument.FromFile(filePath);

    string stampHtml = approved
        ? $@"<div style='border:3px solid green; padding:15px; background:rgba(0,255,0,0.1);'>
               <h2 style='color:green; margin:0;'>✓ 承認済み</h2>
               <p style='margin:5px 0 0 0;'>承認者: {approver}</p>
               <p style='margin:0; font-size:10pt;'>{DateTime.Now:yyyy-MM-dd}</p>
             </div>"
        : $@"<div style='border:3px solid red; padding:15px; background:rgba(255,0,0,0.1);'>
               <h2 style='color:red; margin:0;'>✗ 却下</h2>
               <p style='margin:5px 0 0 0;'>承認者: {approver}</p>
               <p style='margin:0; font-size:10pt;'>{DateTime.Now:yyyy-MM-dd}</p>
             </div>";

    pdf.ApplyWatermark(stampHtml,
        VerticalAlignment.Top,
        HorizontalAlignment.Right);

    pdf.SaveAs(filePath.Replace(".pdf", $"-{(approved ? "approved" : "rejected")}.pdf"));
}
```

### コピー保護

```csharp
using IronPdf;

public void ApplyCopyProtection(string filePath, string recipientName)
{
    var pdf = PdfDocument.FromFile(filePath);

    // 共有を抑止するための受信者固有のウォーターマークを追加
    pdf.ApplyWatermark($@"
        <div style='opacity:0.15; transform:rotate(-30deg);'>
            <p style='font-size:14pt; color:#333;'>
                ライセンス所有者: {recipientName}<br/>
                コピーID: {Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}
            </p>
        </div>
    ");

    // パスワード保護も追加
    pdf.SecuritySettings.OwnerPassword = "admin123";
    pdf.SecuritySettings.UserPassword = "";  // 閲覧は可能だが編集は不可

    pdf.SaveAs($"protected-{recipientName.Replace(" ", "-")}.pdf");
}
```

---

## 推奨事項

### ウォーターマークにIronPDFを選ぶべき時:
- ✅ HTML/CSSのスタイリングの柔軟性を求める場合
- ✅ 変数を含むダイナミックなウォーターマークが必要な場合
- ✅ 複雑なレイアウト（境界線、背景、QRコード）が必要な場合
- ✅ 他のPDF操作も行っている場合

### iText7を選ぶべき時:
- 非常に正確な座標位置決めが必要な場合
- すでにiTextエコシステム内にいる場合

### ウォーターマークができない場合:
- ❌ PuppeteerSharp — 生成のみ
- ❌ QuestPDF — 生成のみ
- ❌ wkhtmltopdf — 生成のみ

---

## 関連チュートリアル

- **[デジタル署名](digital-signatures-pdf-csharp.md)** — ウォーターマーク付き文書に署名
- **[PDFの結合](merge-split-pdf-csharp.md)** — 結合された文書にウォーターマークを追加
- **[PDF/Aコンプライアンス](pdf-a-compliance-csharp.md)** — アクセシブルなウォーターマーク
- **[最高のPDFライブラリ](best-pdf-libraries-dotnet-2025.md)** — 完全な比較

---

### その他のチュートリアル
- **[HTMLからPDFへ](html-to-pdf-csharp.md)** — ウォーターマークを追加するPDFを生成
- **[PDFから画像へ](pdf-to-image-csharp.md)** — ウォーターマーク付きPDFを変換
- **[ASP.NET Core](asp-net-core-pdf-reports.md)** — Webでのウォーターマーキング
- **[IronPDFガイド](ironpdf/)** — 完全なウォーターマークAPI

---

*「[Awesome .NET PDF Libraries 2025](README.md)」コレクション