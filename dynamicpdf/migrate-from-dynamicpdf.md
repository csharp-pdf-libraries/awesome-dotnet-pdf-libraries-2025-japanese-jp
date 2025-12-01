---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [dynamicpdf/migrate-from-dynamicpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/dynamicpdf/migrate-from-dynamicpdf.md)
🇯🇵 **日本語:** [dynamicpdf/migrate-from-dynamicpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/dynamicpdf/migrate-from-dynamicpdf.md)

---

# DynamicPDFからIronPDFへの移行方法は？

## 目次

1. [DynamicPDFからIronPDFへ移行する理由](#DynamicPDFからIronPDFへ移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード例](#コード例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## DynamicPDFからIronPDFへ移行する理由

### 製品の断片化問題

DynamicPDFは**別々の製品として販売され、別々のライセンスが必要です**：

1. **DynamicPDF Generator**: スクラッチからPDFを作成
2. **DynamicPDF Merger**: 既存のPDFをマージ、分割、操作
3. **DynamicPDF Core Suite**: GeneratorとMergerの組み合わせ
4. **DynamicPDF ReportWriter**: レポート生成
5. **DynamicPDF HTML Converter**: HTMLからPDFへの変換（別のアドオン）
6. **DynamicPDF Print Manager**: プログラムでPDFを印刷

**各製品には別々のライセンスが必要です。** 完全なPDFソリューションのコストは、予想よりも3-5倍になることがあります。

### DynamicPDFとIronPDFの比較

| 項目 | DynamicPDF | IronPDF |
|--------|------------|---------|
| **製品モデル** | 断片化された（5+製品） | 一体型ライブラリ |
| **ライセンシング** | 複数のライセンスが必要 | 単一ライセンス |
| **HTMLからPDFへ** | 別のアドオン購入が必要 | 組み込み、Chromiumベース |
| **CSSサポート** | 限定的（アドオンが必要） | 完全なCSS3サポート、Flexbox/Grid含む |
| **JavaScript** | 限定的なサポート | 完全なES6+サポート |
| **APIスタイル** | 座標ベースの位置指定 | HTML/CSS + 操作API |
| **学習曲線** | 急（複数のAPI） | 緩やか（Web技術） |
| **モダン.NET** | .NET Standard 2.0 | .NET 6/7/8/9+ ネイティブ |
| **ドキュメント** | 製品ごとに分散 | 統一されたドキュメント |
| **価格の明確さ** | 複雑な階層 | 透明な価格設定 |

### 主な移行の利点

1. **単一パッケージ**: 3-5つのDynamicPDFパッケージを1つのNuGetパッケージで置き換え
2. **モダンなレンダリング**: Chromiumエンジン対レガシーレンダリング
3. **Web技術の使用**: 座標ベースの位置指定の代わりにHTML/CSSを使用
4. **シンプルなAPI**: コードが少なく、読みやすく、メンテナンスが容易
5. **アドオン購入不要**: HTML、マージ、セキュリティが全て含まれている
6. **パフォーマンスの向上**: モダンな.NETランタイムに最適化

---

## 開始する前に

### 1. DynamicPDFの使用状況を調査

使用しているDynamicPDF製品を特定します：

```bash
# DynamicPDFの参照をすべて見つける
grep -r "ceTe.DynamicPDF\|DynamicPDF" --include="*.cs" --include="*.csproj" .

# NuGetパッケージをチェック
dotnet list package | grep -i dynamic
```

**よく見られるパッケージ:**
- `ceTe.DynamicPDF.CoreSuite.NET`
- `ceTe.DynamicPDF.Generator.NET`
- `ceTe.DynamicPDF.Merger.NET`
- `ceTe.DynamicPDF.HtmlConverter.NET`

### 2. 現在の機能を文書化

使用している機能のチェックリストを作成します：

- [ ] スクラッチからのPDF生成
- [ ] HTMLからPDFへの変換
- [ ] PDFのマージ/分割
- [ ] フォーム入力
- [ ] テキスト抽出
- [ ] デジタル署名
- [ ] 暗号化/パスワード
- [ ] ウォーターマーク
- [ ] バーコード
- [ ] テーブル（Table2）
- [ ] ヘッダー/フッター
- [ ] ページ番号

### 3. IronPDFのセットアップ

```bash
# DynamicPDFパッケージを削除
dotnet remove package ceTe.DynamicPDF.CoreSuite.NET
dotnet remove package ceTe.DynamicPDF.Generator.NET
dotnet remove package ceTe.DynamicPDF.Merger.NET
dotnet remove package ceTe.DynamicPDF.HtmlConverter.NET

# IronPDFをインストール
dotnet add package IronPdf
```

### 4. ライセンスの設定

```csharp
// アプリケーションの起動時（Program.csまたはStartup.cs）に
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## クイックスタート移行

### パラダイムシフト

DynamicPDFは**座標ベースの位置指定**を使用し、特定のX,Y座標に要素を配置します。IronPDFは**HTML/CSSレンダリング**を使用し、Web技術でデザインします。

**DynamicPDFのアプローチ:**
```csharp
Document document = new Document();
Page page = new Page(PageSize.Letter);
Label label = new Label("Hello", 100, 200, 300, 50, Font.Helvetica, 12);
page.Elements.Add(label);
document.Pages.Add(page);
document.Draw("output.pdf");
```

**IronPDFのアプローチ:**
```csharp
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1 style='margin-left:100px'>Hello</h1>");
pdf.SaveAs("output.pdf");
```

### 最小限の移行例

**以前（DynamicPDF）:**
```csharp
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;

public byte[] GenerateInvoice(Invoice invoice)
{
    Document document = new Document();
    Page page = new Page(PageSize.A4);

    // ヘッダー
    Label title = new Label("INVOICE", 0, 0, 595, 30, Font.HelveticaBold, 24);
    title.Align = TextAlign.Center;
    page.Elements.Add(title);

    // 顧客情報
    Label customer = new Label($"Bill To: {invoice.CustomerName}", 40, 60, 300, 20);
    page.Elements.Add(customer);

    // ページを追加して生成
    document.Pages.Add(page);
    return document.Draw();
}
```

**後（IronPDF）:**
```csharp
using IronPdf;

public byte[] GenerateInvoice(Invoice invoice)
{
    var html = $@"
        <html>
        <head>
            <style>
                body {{ font-family: Helvetica, sans-serif; padding: 40px; }}
                h1 {{ text-align: center; font-size: 24pt; }}
                .customer {{ margin-top: 30px; }}
            </style>
        </head>
        <body>
            <h1>INVOICE</h1>
            <div class='customer'>Bill To: {invoice.CustomerName}</div>
        </body>
        </html>";

    var renderer = new ChromePdfRenderer();
    var pdf = renderer.RenderHtmlAsPdf(html);
    return pdf.BinaryData;
}
```

---