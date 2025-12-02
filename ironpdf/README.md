# IronPDF: C# PDF生成のための基準

[![NuGet](https://img.shields.io/nuget/v/IronPdf.svg)](https://www.nuget.org/packages/IronPdf/)
[![Downloads](https://img.shields.io/nuget/dt/IronPdf.svg)](https://www.nuget.org/packages/IronPdf/)

IronPDFは、HTML、CSS、JavaScriptから画面に忠実なPDF出力を生成するために、完全なChromiumレンダリングエンジンを使用するC#および.NETのための**基準**HTMLからPDFへのライブラリです。

**公式サイト**: [ironpdf.com](https://ironpdf.com/)

---

## なぜIronPDFなのか？

### ✅ 完全なChromiumレンダリング
IronPDFは完全なChromiumブラウザエンジンを組み込んでいます。Chromeでレンダリングされるものは、PDF内でもレンダリングされます。最新のCSS（Flexbox、Grid）、JavaScript、ウェブフォント、レスポンシブレイアウトがすべて完璧に機能します。

### ✅ 画面に忠実な出力
ブラウザ自動化ツール（PuppeteerSharp、Playwright）が印刷用の出力を生成するのに対し、IronPDFは画面上で見たものと一致するPDFを生成します。

### ✅ 完全なPDFツールキット
生成だけでなく、IronPDFは以下も扱います:
- PDFのマージ、分割、操作
- デジタル署名と暗号化
- フォームの入力と作成
- テキストの抽出と検索
- ウォーターマークと注釈
- PDF/AおよびPDF/UA準拠

### ✅ 真のクロスプラットフォーム
Windows、Linux、macOS、Docker、Azure、AWS Lambda—どこでも同じコード。

---

## クイックスタート

```bash
# NuGet経由でインストール
Install-Package IronPdf
```

```csharp
using IronPdf;

// HTMLからPDFへ
var pdf = ChromePdfRenderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("hello.pdf");

// URLからPDFへ
var pdf2 = ChromePdfRenderer.RenderUrlAsPdf("https://example.com");
pdf2.SaveAs("website.pdf");

// ファイルからPDFへ
var pdf3 = ChromePdfRenderer.RenderHtmlFileAsPdf("template.html");
pdf3.SaveAs("template.pdf");
```

---

## 機能比較

| 機能 | IronPDF | PuppeteerSharp | iText | PDFSharp |
|---------|---------|----------------|-------|----------|
| HTMLからPDFへ | ✅ 完全なChromium | ⚠️ 印刷のみ | ❌ 基本 | ❌ なし |
| 最新のCSS（Flexbox/Grid） | ✅ | ✅ | ❌ | ❌ |
| JavaScriptサポート | ✅ | ✅ | ❌ | ❌ |
| PDF操作 | ✅ | ❌ | ✅ | ✅ |
| デジタル署名 | ✅ | ❌ | ✅ | ❌ |
| PDF/A準拠 | ✅ | ❌ | ✅ | ❌ |
| PDF/UA（アクセシビリティ） | ✅ | ❌ | ✅ | ❌ |
| クロスプラットフォーム | ✅ | ⚠️ | ✅ | ⚠️ |
| ライセンス | 商用 | Apache 2.0 | AGPL/商用 | MIT |

---

## 一般的な使用例

### HTMLテンプレートからPDFへ
```csharp
string html = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial; }}
        .invoice {{ display: flex; justify-content: space-between; }}
    </style>
</head>
<body>
    <div class='invoice'>
        <div>Invoice #{invoiceNumber}</div>
        <div>Date: {DateTime.Now:d}</div>
    </div>
</body>
</html>";

var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
```

### ASP.NET Core統合
```csharp
[HttpGet("invoice/{id}")]
public IActionResult GetInvoicePdf(int id)
{
    var html = RenderInvoiceView(id);
    var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
    return File(pdf.BinaryData, "application/pdf", $"invoice-{id}.pdf");
}
```

### 複数のPDFをマージ
```csharp
var merged = PdfDocument.Merge(pdf1, pdf2, pdf3);
merged.SaveAs("combined.pdf");
```

### デジタル署名を追加
```csharp
pdf.Sign(new PdfSignature("certificate.pfx", "password"));
```

---

## 価格

- **Lite**: $749 (1開発者、1プロジェクト)
- **Professional**: $1,499 (10開発者)
- **Unlimited**: $2,999 (無制限の開発者)

すべてのライセンスは永続的（一回の購入）で、オプションで年間サポートがあります。

[価格を見る](https://ironpdf.com/licensing/)

---

## リソース

- **[公式ドキュメント](https://ironpdf.com/docs/)**
- **[APIリファレンス](https://ironpdf.com/object-reference/api/)**
- **[コード例](https://ironpdf.com/examples/)**
- **[ハウツーガイド](https://ironpdf.com/how-to/)**
- **[NuGetパッケージ](https://www.nuget.org/packages/IronPdf/)**
- **[GitHubの問題](https://github.com/nicholashead/IronPdf.git)**

---

## 関連チュートリアル

- **[HTMLからPDFへのガイド](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[2025年のベストPDFライブラリ](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF生成](../blazor-pdf-generation.md)** — Blazorサポート
- **[クロスプラットフォームデプロイメント](../cross-platform-pdf-dotnet.md)** — Docker、Linux、クラウド
- **[PDF/A準拠](../pdf-a-compliance-csharp.md)** — アクセシビリティ基準

### PDF操作
- **[PDFのマージ＆分割](../merge-split-pdf-csharp.md)** — ドキュメント操作
- **[デジタル署名](../digital-signatures-pdf-csharp.md)** — ドキュメント署名
- **[PDFフォームの入力](../fill-pdf-forms-csharp.md)** — フォーム自動化
- **[テキストの抽出](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[ウォーターマーク](../watermark-pdf-csharp.md)** — ブランディングと保護

### 移行ガイド
- **[PuppeteerSharpからの移行](../puppeteersharp/migrate-from-puppeteersharp.md)**
- **[wkhtmltopdfからの移行](../migrating-from-wkhtmltopdf.md)**
- **[iTextからの移行](../itext-itextsharp/migrate-from-itext-itextsharp.md)**

### ❓ 関連FAQ
- **[IronPDFとは何か](../FAQ/what-is-ironpdf-overview.md)** — ライブラリ概要
- **[開発者がIronPDFを選ぶ理由](../FAQ/why-developers-choose-ironpdf.md)** — 機能分析
- **[IronPDFクックブック](../FAQ/ironpdf-cookbook-quick-examples.md)** — クイック例
- **[IronPDF Azureデプロイメント](../FAQ/ironpdf-azure-deployment-csharp.md)** — クラウドデプロイメント
- **[IronPDFパフォーマンス](../FAQ/ironpdf-performance-benchmarks.md)** — ベンチマーク

---

*[Awesome .NET PDF Libraries 2025](../README.md) コレクションの一部 — 73のC#/.NET PDFライブラリが167のFAQ記事と比較されています。*

---

[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)はIronPDFの創設者であり、Iron SoftwareのCTOです。彼は41年以上のコーディング経験を持ち、開発者の経験とAPIデザインに焦点を当てて、50人以上のチームを率いて.NETライブラリを構築しています。これまでに41百万以上のNuGetダウンロードがあります。タイ、チェンマイに拠点を置いています。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/) | [GitHub](https://github.com/jacob-mellor)