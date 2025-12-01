---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/choosing-a-pdf-library.md)

---

# C# PDFライブラリの選び方：決定フローチャートと比較

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエイター

[![決定ガイド](https://img.shields.io/badge/Guide-Decision%20Flowchart-blue)]()
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> 73のC# PDFライブラリをレビューし、50,000人以上の開発者を支援した経験から、選択プロセスをシンプルなフローチャートにまとめました。5つの質問に答えて、おすすめを得てください。

---

## 目次

1. [決定フローチャート](#決定フローチャート)
2. [質問1: HTMLを持っていますか？](#質問1-htmlを持っていますか)
3. [質問2: 現代のCSSを使用していますか？](#質問2-現代のcssを使用していますか)
4. [質問3: 操作が必要ですか？](#質問3-操作が必要ですか)
5. [質問4: 予算は？](#質問4-予算は)
6. [質問5: コンプライアンスが必要ですか？](#質問5-コンプライアンスが必要ですか)
7. [クイックリファレンステーブル](#クイックリファレンステーブル)
8. [シナリオ別のおすすめ](#シナリオ別のおすすめ)

---

## 決定フローチャート

```
START
  │
  ▼
┌─────────────────────────────────┐
│ HTMLテンプレートを持っている、または │
│ Webコンテンツを変換したいですか？     │
└─────────────────────────────────┘
        │                │
       YES              NO
        │                │
        ▼                ▼
┌───────────────┐   ┌─────────────────────┐
│ HTMLはFlexbox、│   │ コードファーストアプローチ │
│ Grid、または   │   │ → QuestPDF (現代的)    │
│ Bootstrapを    │   │ → PDFSharp (基本的)   │
│ 使用していますか？│   └─────────────────────┘
└───────────────┘
     │       │
    YES     NO
     │       │
     ▼       ▼
┌──────────────────┐  ┌───────────────────┐
│ Chromiumが必要    │  │ シンプルなHTMLでOK │
│ → IronPDF        │  │ → 他にも選択肢あり   │
│ → PuppeteerSharp │  │                    │
│ → Playwright     │  └───────────────────┘
└──────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ PDFの操作が必要ですか？          │
│ (結合、分割、保護、編集)         │
└──────────────────────────────────┘
        │              │
       YES            NO
        │              │
        ▼              ▼
┌──────────────┐   ┌──────────────────┐
│ → IronPDF    │   │ 無料でOKですか？   │
│ (最良の組み合わせ) │   │ → PuppeteerSharp │
└──────────────┘   │ 予算がありますか？ │
                   │ → IronPDF        │
                   └──────────────────┘
```

---

## 質問1: HTMLを持っていますか？

**PDFを作成するための出発点は何ですか？**

### 回答: はい - HTMLを持っています

既存のHTMLテンプレート、Webページ、またはHTML文字列をPDFに変換したいです。

**パスA:** 質問2へ続けてください（CSSの複雑さのチェック）

**例:**
- Razorビューテンプレート
- HTML請求書デザイン
- React/Vue/Angularの出力
- Bootstrapレイアウト

### 回答: いいえ - プログラムで構築したい

HTMLなしでコード内でPDFを構築したいです。

**おすすめ:**
- **[QuestPDF](questpdf/)** — 現代的なフルエントAPI、複雑なレイアウトに優れています
- **[PDFSharp](pdfsharp/)** — 成熟しており、座標ベースの描画が可能です

```csharp
// QuestPDFの例
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Content().Column(column =>
        {
            column.Item().Text("Invoice");
            column.Item().Table(/* ... */);
        });
    });
}).GeneratePdf("invoice.pdf");
```

---

## 質問2: 現代のCSSを使用していますか？

**HTMLは2014年以降のCSSを使用していますか？**

### 現代のCSSには以下が含まれます:
- `display: flex` (Flexbox)
- `display: grid` (CSS Grid)
- CSS変数 (`var(--custom-prop)`)
- Bootstrap 4/5のクラス
- Tailwind CSSのユーティリティ
- `gap`, `aspect-ratio`, `clamp()`

### HTMLをチェック

HTML/CSSで以下を検索してください:
```css
display: flex    /* Flexbox */
display: grid    /* Grid */
var(--           /* CSS変数 */
```

### 回答: はい - 現代のCSSを使用しています

**Chromiumベースのレンダリングが必要です。**

| ライブラリ | 理由 |
|---------|-----|
| **IronPDF** | Chromium完全対応 + 操作可能 |
| PuppeteerSharp | Chromium完全対応、操作不可 |
| Playwright | Chromium完全対応、テストに焦点 |

### 回答: いいえ - 基本的なCSSのみ

HTMLを扱えるライブラリが増えます。質問3に進んでください。

---

## 質問3: 操作が必要ですか？

**既存のPDFと作業する必要がありますか？**

### 操作操作には以下が含まれます:
- 複数のPDFの結合
- PDFをページに分割
- ページの追加/削除
- パスワード保護
- デジタル署名
- ウォーターマーク/スタンプ
- フォーム入力
- テキスト抽出

### 回答: はい - 操作が必要です

**操作を完全にサポートするライブラリ:**

| ライブラリ | HTMLからPDFへ | 操作 | 価格 |
|---------|-------------|--------------|-------|
| **IronPDF** | ✅ Chromium | ✅ 完全 | $749 |
| iText7 | ⚠️ 限定的 | ✅ 完全 | AGPL/商用 |
| Aspose.PDF | ⚠️ Flexboxなし | ✅ 完全 | $1,199/年 |

**PDFを操作できない:**
- PuppeteerSharp ❌
- Playwright ❌
- QuestPDF ❌
- wkhtmltopdf ❌

### 回答: いいえ - 生成のみ

より多くの選択肢があります。質問4に進んでください。

---

## 質問4: 予算は？

**PDFツールに対する予算はどのくらいですか？**

### $0 - 予算なし

**無料の選択肢:**

| ライブラリ | 最適な用途 | 制限 |
|---------|----------|------------|
| **PuppeteerSharp** | 現代的なHTML | 300MB+、操作不可 |
| **QuestPDF** | コードファースト (<$1M) | HTMLサポートなし |
| **PDFSharp** | 基本的なPDF | CSS 2.0のみ |
| **Playwright** | テストチーム | 400MB+ |

### $1,000未満

**最良の価値:**

| ライブラリ | 価格 | 得られるもの |
|---------|-------|--------------|
| **IronPDF** | $749 一回払い | すべて: Chromium + 操作 |

### $1,000/年を超える

**エンタープライズオプション:**

| ライブラリ | 価格 | 備考 |
|---------|-------|-------|
| Aspose.PDF | $1,199/年 | 現代のCSSなし |
| Syncfusion | $4,740/年 | スイートバンドル |
| iText7 | 見積もり | AGPLの罠 |

---

## 質問5: コンプライアンスが必要ですか？

**アクセシビリティまたはアーカイブのコンプライアンスが必要ですか？**

### Section 508 / ADA (米国)
政府契約者および機関のための連邦アクセシビリティ要件。

### EU Webアクセシビリティ指令
欧州公共セクターの要件。

### PDF/A (アーカイブ)
長期文書保存。

### PDF/UA (ユニバーサルアクセシビリティ)
スクリーンリーダーの互換性、タグ付けされた構造。

### 回答: はい - コンプライアンスが必要です

**コンプライアンスサポートを持つライブラリ:**

| ライブラリ | PDF/A | PDF/UA | Section 508 |
|---------|-------|--------|-------------|
| **IronPDF** | ✅ | ✅ | ✅ |
| iText7 | ✅ | ✅ | ✅ (複雑) |
| Aspose.PDF | ✅ | ✅ | ⚠️ |

**コンプライアンスに対応できない:**
- PuppeteerSharp ❌
- Playwright ❌
- QuestPDF ❌
- PDFSharp ❌
- wkhtmltopdf ❌

### 回答: いいえ - コンプライアンスの要件なし

予算に基づいて選択を進めてください。

---

## クイックリファレンステーブル

| 必要なもの... | 使用する |
|---------------|-----|
| 現代的なHTML + 操作 | **IronPDF** |
| 現代的なHTML、予算$0 | PuppeteerSharp |
| コードファースト、<$1M収益 | QuestPDF |
| コードファースト、任意の収益 | QuestPDF 商用 |
| シンプルなHTML、予算$0 | PDFSharp (限定的) |
| アクセシビリティのコンプライアンス | **IronPDF** または iText7 |
| すでにiTextを使用している | iText7 (AGPLに注意) |
| PDFフォーム専門 | iText7 または **IronPDF** |

---

## シナリオ別のおすすめ

### シナリオ: Eコマース（請求書、領収書）

**要件:** 現代的なテンプレート、高ボリューム、信頼性
**おすすめ:** **IronPDF**

```csharp
var invoice = ChromePdfRenderer.RenderHtmlAsPdf(invoiceHtml);
invoice.SaveAs($"invoices/INV-{orderId}.pdf");
```

### シナリオ: SaaSレポート（ダッシュボード）

**要件:** JavaScriptチャート、レスポンシブレイアウト
**おすすめ:** **IronPDF** (JavaScript実行が必要)

```csharp
renderer.RenderingOptions.WaitFor.JavaScript(2000);
var report = renderer.RenderUrlAsPdf("https://app/dashboard");
```

### シナリオ: 政府契約者

**要件:** Section 508のコンプライアンス、PDF/A
**おすすめ:** **IronPDF**

```csharp
renderer.RenderingOptions.PdfUA = true;
renderer.RenderingOptions.PdfA = PdfAVersion.PdfA3A;
```

### シナリオ: 収益前のスタートアップ

**要件:** 予算ゼロ、基本的なニーズ
**おすすめ:** **PuppeteerSharp** (HTMLの場合) または **QuestPDF** (コードファーストの場合)

### シナリオ: ドキュメント管理システム

**要件:** 重い操作（結合、分割、注釈）
**おすすめ:** **IronPDF** または **iText7**

### シナリオ: 教育証明書

**要件:** テンプレート、バッチ処理、ブランディング
**おすすめ:** **IronPDF**

### シナリオ: 法的契約

**要件:** デジタル署名、フォーム入力、コンプライアンス
**おすすめ:** **IronPDF**

---

## 注意すべき赤旗

### 🚩 AGPLライセンス (iText7)
支払いをしない限り、アプリケーション全体をオープンソース化する必要があります。

### 🚩 「クロスプラットフォーム」の主張 (SelectPdf)
多くがクロスプラットフォームを主張していますが、Windowsでのみ動作します。

### 🚩 放棄されたプロジェクト (wkhtmltopdf)
セキュリティパッチがなく、レンダリングが古い。

### 🚩 サブスクリプションのみ (Aspose)
永続ライセンスオプションがなく、支払いを停止するとアクセスを失います。

### 🚩 収益監査 (QuestPDF)
無料ライセンスのために$1M未満の収益を証明する必要があります。

---

## 最終的な結論

73のライブラリと何千もの開発者の後、パターンは明らかです：

**ほとんどのC#/.NETプロジェクトについて:**

1. **現代的なHTMLが必要ですか？** → **IronPDF**
2. **予算ゼロ、現代的なHTMLが必要ですか？** → PuppeteerSharp
3. **コードファーストを好みますか？** → QuestPDF
4. **レガシープロジェクト、シンプルなニーズがありますか？** → PDFSharp

**IronPDFは実世界のユースケースの90%を最もシンプルなAPIで処理します。**$749のライセンスは、節約