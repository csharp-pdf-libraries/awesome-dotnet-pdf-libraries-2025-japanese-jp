---
**  (Japanese Translation)**

 **English:** [itext-itextsharp/migrate-from-itext-itextsharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/itext-itextsharp/migrate-from-itext-itextsharp.md)
 **:** [itext-itextsharp/migrate-from-itext-itextsharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/itext-itextsharp/migrate-from-itext-itextsharp.md)

---
# iText / iTextSharpからIronPDFへの移行方法は？

## 目次
1. [iText/iTextSharpから移行する理由](#itexitextsharpから移行する理由)
2. [始める前に](#始める前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## iText/iTextSharpから移行する理由

### AGPLライセンスの罠

iTextは商用アプリケーションに対して深刻な法的およびビジネス上のリスクを提示します：

1. **AGPLバイラルライセンス**：iTextをWebアプリケーションで使用する場合、AGPLは**あなたのアプリケーション全体をオープンソース化することを要求します**—PDFコードだけでなく、コードベース全体
2. **永久ライセンスなし**：iTextは永久ライセンスを廃止し、年間サブスクリプションの更新を強制しています
3. **pdfHTMLアドオンのコスト**：HTMLからPDFへの変換には、追加コストで別途販売されているpdfHTMLアドオンが必要です
4. **複雑なライセンス監査**：エンタープライズ展開はライセンスの複雑さと監査リスクに直面します
5. **プログラム専用API**：`Paragraph`、`Table`、`Cell`を使った手動の低レベルPDF構築が必要です
6. **モダンWebレンダリングなし**：pdfHTMLでさえも、複雑なCSS/JavaScriptはかなりの労力を要します

### IronPDFの利点

| 特徴 | iText 7 / iTextSharp | IronPDF |
|---------|---------------------|---------|
| ライセンス | AGPL（バイラル）または高価なサブスクリプション | 商用、永久オプションあり |
| HTMLからPDFへ | 別途pdfHTMLアドオンが必要 | 組み込みのChromiumレンダラー |
| CSSサポート | 基本的なCSS | 完全なCSS3、Flexbox、Grid |
| JavaScript | なし | 完全実行 |
| APIパラダイム | プログラム（Paragraph、Table、Cell） | HTMLファーストでCSSを使用 |
| 学習曲線 | 急（PDF座標系） | Web開発者に優しい |
| オープンソースリスク | Webアプリをオープンソース化する必要がある | バイラル要件なし |
| 価格モデル | サブスクリプションのみ | 永久またはサブスクリプション |

### 移行の利点

- **AGPLリスクの排除**：プロプライエタリコードをクローズドソースのままに
- **PDF作成の簡素化**：プログラム構築の代わりにHTML/CSSを使用
- **モダンレンダリング**：ChromiumエンジンはあらゆるモダンWebコンテンツを処理
- **コスト削減**：HTMLからPDFへのライセンスが1つに含まれる（pdfHTMLアドオン不要）
- **永久ライセンス**：一回の購入オプション

---

## 始める前に

### 前提条件

1. **.NET環境**：.NET Framework 4.6.2+または.NET Core 3.1+ / .NET 5+
2. **NuGetアクセス**：NuGetパッケージをインストールできること
3. **IronPDFライセンス**：無料トライアルまたは購入したライセンスキー

### インストール

```bash
# iTextパッケージを削除
dotnet remove package itext7
dotnet remove package itext7.pdfhtml
dotnet remove package itextsharp

# IronPDFをインストール
dotnet add package IronPdf
```

### ライセンス設定

```csharp
// アプリケーションの起動時に追加（Program.csまたはStartup.cs）
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### iTextの使用箇所の特定

```bash
# すべてのiText参照を検索
grep -r "using iText\|using iTextSharp" --include="*.cs" .
grep -r "PdfWriter\|PdfDocument\|Document\|Paragraph\|Table\|Cell" --include="*.cs" .
grep -r "HtmlConverter\|ConverterProperties" --include="*.cs" .
```

---

## クイックスタート移行

### 最小変更例

**移行前 (iText 7):**
```csharp
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

public class ItextPdfService
{
    public byte[] CreateReport(ReportData data)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new PdfWriter(memoryStream))
            using (var pdfDoc = new PdfDocument(writer))
            using (var document = new Document(pdfDoc))
            {
                // ヘッダー
                var header = new Paragraph(data.Title)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(24)
                    .SetBold();
                document.Add(header);

                // テーブル
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 1 }))
                    .UseAllAvailableWidth();

                // ヘッダーセル
                table.AddHeaderCell(new Cell().Add(new Paragraph("ID")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Name")));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Value")));

                // データ行
                foreach (var item in data.Items)
                {
                    table.AddCell(new Cell().Add(new Paragraph(item.Id.ToString())));
                    table.AddCell(new Cell().Add(new Paragraph(item.Name)));
                    table.AddCell(new Cell().Add(new Paragraph(item.Value.ToString("C"))));
                }

                document.Add(table);
            }

            return memoryStream.ToArray();
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
    }

    public byte[] CreateReport(ReportData data)
    {
        // プログラム構築の代わりにHTML/CSSを使用
        string html = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; padding: 20px; }}
                    h1 {{ text-align: center; font-size: 24px; }}
                    table {{ width: 100%; border-collapse: collapse; }}
                    th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                    th {{ background-color: #4CAF50; color: white; }}
                    tr:nth-child(even) {{ background-color: #f2f2f2; }}
                </style>
            </head>
            <body>
                <h1>{data.Title}</h1>
                <table>
                    <tr><th>ID</th><th>Name</th><th>Value</th></tr>
                    {string.Join("", data.Items.Select(item => $@"
                        <tr>
                            <td>{item.Id}</td>
                            <td>{item.Name}</td>
                            <td>{item.Value:C}</td>
                        </tr>"))}
                </table>
            </body>
            </html>";

        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}
```

---