# PDF/Aおよびアクセシビリティコンプライアンス in C#: セクション508、EU指令、およびPDF/UA

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![コンプライアンス](https://img.shields.io/badge/Compliance-Section%20508%20%7C%20PDF%2FUA%20%7C%20WCAG-green)]()
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> ほとんどのPDFライブラリはアクセシビリティ監査に失敗します。政府、医療、教育、またはEU公共セクターにサービスを提供する場合、コンプライアンスのPDFが必要です。ここでは、コンプライアンスが実際に何を要求しているのか、どのライブラリが提供しているのかを説明します。

---

## 目次

1. [アクセシビリティコンプライアンスが重要な理由](#アクセシビリティコンプライアンスが重要な理由)
2. [基準の理解](#基準の理解)
3. [ライブラリコンプライアンス比較](#ライブラリコンプライアンス比較)
4. [IronPDFを使用したPDF/Aの実装](#IronPDFを使用したPDFAの実装)
5. [タグ付けされたPDF構造](#タグ付けされたPDF構造)
6. [コンプライアンスのテスト](#コンプライアンスのテスト)
7. [一般的なコンプライアンスの失敗](#一般的なコンプライアンスの失敗)

---

## アクセシビリティコンプライアンスが重要な理由

### 法的要件

**アメリカ合衆国:**
- **セクション508** — 連邦機関はアクセシブルな電子文書を使用しなければならない
- **ADA** — アメリカ障害者法はデジタルコンテンツに適用される
- **罰金:** 初回違反で最大$75,000、その後は$150,000

**欧州連合:**
- **Webアクセシビリティ指令 (2016/2102)** — 公共セクターのウェブサイトおよびアプリ
- **ヨーロッパアクセシビリティ法** — 2025年に民間セクターに拡大
- **罰金:** 会員国によって異なる、最大€100,000+

**その他:**
- **AODA (カナダ)** — オンタリオ州のアクセシビリティ
- **DDA (英国)** — 障害者差別法

### コンプライアンスが必要な対象

| セクター | 要件 | 基準 |
|--------|------------|----------|
| 連邦政府 (米国) | 必須 | セクション508, PDF/UA |
| 州/地方政府 | 通常は必須 | セクション508 |
| 医療 | HIPAAに隣接 | セクション508 |
| 教育 | 必須 (連邦資金) | セクション508, WCAG |
| EU公共セクター | 必須 | EN 301 549, WCAG |
| 金融サービス | 要求が増加 | さまざま |
| 訴訟を抱える組織 | 和解条件 | WCAG 2.1 AA |

---

## 基準の理解

### PDF/A (アーカイブ)

PDF/AはISO 19005 — 長期保存のための基準です:

| バージョン | 使用例 | 主な特徴 |
|---------|----------|--------------|
| **PDF/A-1** | 基本的なアーカイブ | 自己完結型、フォント埋め込み |
| **PDF/A-2** | 現代的なアーカイブ | JPEG 2000、透明性 |
| **PDF/A-3** | アタッチメント付きアーカイブ | 任意のファイルタイプの埋め込み |

PDF/Aは数十年後も文書が読めるように保証します。

### PDF/UA (ユニバーサルアクセシビリティ)

PDF/UAはISO 14289 — PDFのアクセシビリティ基準です:

**要件:**
- タグ付けされた文書構造
- 読み取り順序の指定
- 画像の代替テキスト
- 言語の特定
- 適切な見出し階層
- フォームフィールドのラベル

### WCAG (ウェブコンテンツアクセシビリティガイドライン)

WCAG 2.1は、それらが「ウェブコンテンツ」と見なされる場合にPDFに適用されます:

| レベル | 説明 |
|-------|-------------|
| **A** | 最低限のアクセシビリティ |
| **AA** | 標準コンプライアンスターゲット |
| **AAA** | 最高のアクセシビリティ |

ほとんどの規制はWCAG 2.1 AAを要求します。

### セクション508

セクション508はWCAG 2.0 AAを参照し、特定のPDF要件を追加します:
- タグ付けされたPDF構造
- 論理的な読み取り順序
- テキスト代替
- 色のコントラスト (最小4.5:1)
- キーボードナビゲーション

---

## ライブラリコンプライアンス比較

### 現実のチェック

| ライブラリ | PDF/A | PDF/UA | セクション508 | タグ付けされたPDF |
|---------|-------|--------|-------------|------------|
| **IronPDF** | ✅ 1a, 2a, 2b, 3a, 3b | ✅ 完全 | ✅ 合格 | ✅ 自動 |
| iText7 | ✅ | ✅ | ✅ | ⚠️ 手動タグ付け |
| Aspose.PDF | ✅ | ✅ | ⚠️ | ⚠️ 手動タグ付け |
| PuppeteerSharp | ❌ | ❌ | ❌ | ❌ |
| Playwright | ❌ | ❌ | ❌ | ❌ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ |
| PDFSharp | ❌ | ❌ | ❌ | ❌ |
| wkhtmltopdf | ❌ | ❌ | ❌ | ❌ |

**重要な洞察:** ほとんどの無料ライブラリはアクセシブルなPDFを生成できません。これは簡単に追加できる機能ではなく、アーキテクチャのサポートが必要です。

### PuppeteerSharp/Playwrightが失敗する理由

これらのライブラリはChromeのPDF出力を直接使用します。Chromeは生成しません:
- 適切な文書タグ付け
- 読み取り順序メタデータ
- PDF/UA構造
- アクセシブルなフォームフィールド

これらは印刷に適した「フラット」なPDFを生成しますが、アクセシビリティには適していません。

### QuestPDF/PDFSharpが失敗する理由

これらはアクセシビリティインフラストラクチャのないコードファーストのライブラリです:
- 意味的構造がない
- 代替テキストサポートがない
- 言語仕様がない
- 読み取り順序がない

### iText/Asposeが作業を必要とする理由

これらはアクセシビリティをサポートしていますが、手動の労力が必要です:
- 各要素をタグ付けする必要があります
- 読み取り順序を明示的に設定する必要があります
- タグ付けのための複雑なAPI
- 要件を見逃しやすい

### IronPDFがリードする理由

IronPDFは自動的にアクセシブルなPDFを生成します:
- HTMLの意味的構造がPDFタグに変換されます
- 読み取り順序はDOMに従います
- AltテキストはHTMLから転送されます
- 言語はHTMLの`lang`属性から取得されます

---

## IronPDFを使用したPDF/Aの実装

### 基本的なPDF/A生成

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// PDF/Aコンプライアンスを有効にする
renderer.RenderingOptions.PdfA = PdfAVersion.PdfA3;

var html = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <title>アクセシブルレポート</title>
</head>
<body>
    <h1>月次レポート</h1>
    <p>この文書はPDF/A-3に準拠しています。</p>
</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("accessible-report.pdf");
```

### アクセシビリティのためのPDF/UA

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// PDF/UAを有効にする
renderer.RenderingOptions.PdfUA = true;

var html = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <title>アクセシブル請求書</title>
</head>
<body>
    <main>
        <h1>請求書 #12345</h1>

        <img src='logo.png' alt='会社ロゴ - Acme Corporation' />

        <table>
            <caption>請求書の項目</caption>
            <thead>
                <tr>
                    <th scope='col'>説明</th>
                    <th scope='col'>金額</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>コンサルティングサービス</td>
                    <td>$1,500.00</td>
                </tr>
            </tbody>
        </table>
    </main>
</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("accessible-invoice.pdf");
```

### PDF/A-3 + PDF/UAの組み合わせ

```csharp
var renderer = new ChromePdfRenderer();

// 最大限のコンプライアンス
renderer.RenderingOptions.PdfA = PdfAVersion.PdfA3A;  // A = アクセシブル
renderer.RenderingOptions.PdfUA = true;

var pdf = renderer.RenderHtmlAsPdf(accessibleHtml);
pdf.SaveAs("fully-compliant.pdf");
```

---

## タグ付けされたPDF構造

### HTMLからPDFタグへのマッピング

| HTML要素 | PDFタグ | 目的 |
|--------------|---------|---------|
| `<h1>` - `<h6>` | `<H1>` - `<H6>` | 見出し階層 |
| `<p>` | `<P>` | 段落 |
| `<table>` | `<Table>` | データテーブル |
| `<th>` | `<TH>` | テーブルヘッダー |
| `<td>` | `<TD>` | テーブルデータ |
| `<ul>`, `<ol>` | `<L>` | リスト |
| `<li>` | `<LI>` | リストアイテム |
| `<img alt="...">` | `<Figure>` | 代替テキスト付き画像 |
| `<a>` | `<Link>` | ハイパーリンク |
| `<main>` | `<Document>` | メインコンテンツ |
| `<article>` | `<Art>` | 記事 |
| `<section>` | `<Sect>` | セクション |

### アクセシブルHTMLのベストプラクティス

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>記述的な文書タイトル</title>
</head>
<body>
    <!-- 意味的構造を使用する -->
    <main>
        <!-- 適切な見出し階層 -->
        <h1>文書タイトル</h1>

        <section aria-label="導入">
            <h2>導入</h2>
            <p>意味のあるテキストでコンテンツ段落。</p>
        </section>

        <!-- 画像には代替テキストが必要 -->
        <img src="chart.png"
             alt="売上成長を示す棒グラフ: Q1 $100k, Q2 $150k, Q3 $200k"
             title="四半期ごとの売上" />

        <!-- テーブルには構造が必要 -->
        <table>
            <caption>四半期収益の概要</caption>
            <thead>
                <tr>
                    <th scope="col">四半期</th>
                    <th scope="col">収益</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th scope="row">Q1</th>
                    <td>$100,000</td>
                </tr>
            </tbody>
        </table>

        <!-- リンクは記述的であるべき -->
        <a href="https://example.com/report">
            完全な年次報告書をダウンロードする (PDF, 2.5 MB)
        </a>
    </main>
</body>
</html>
```

---

## コンプライアンスのテスト

### 自動テストツール

| ツール | 費用 | テスト |
|------|------|-------|
| **[PAC 2024](https://pdfua.foundation/en/pdf-accessibility-checker-pac)** | 無料 | PDF/UA, WCAG |
| **Adobe Acrobat Pro** | サブスクリプション | PDF/UA, セクション508 |
| **[PAVE](https://pave-pdf.org/)** | オンライン無料 | PDF/UA |
| **[CommonLook](https://commonlook.com/)** | 商用 | 完全コンプライアンス |
| **[axe-core](https://github.com/dequelabs/axe-core)** | 無料 | ソースHTML WCAG |

### PACテストワークフロー

```csharp
using IronPdf;

// コンプライアントなPDFを生成
var renderer = new ChromePdfRenderer();