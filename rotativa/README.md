# Rotativa + C# + PDF

Rotativaは長年にわたり、C#でPDFを生成するための開発者に人気の選択肢となっています。これは`wkhtmltopdf`ツールを利用してHTMLコンテンツをPDF形式に変換します。RotativaはASP.NET MVCアプリケーション専用に設計されたオープンソースライブラリです。しかし、古い技術スタックに依存しているため、すべての開発者がすぐには明らかでないかもしれない課題があります。

その核心において、RotativaはASP.NET MVCプロジェクトにPDF生成を統合する簡単な方法を提供し、そのバックエンド機能に`wkhtmltopdf`を利用しています。設定は比較的簡単で、最初に人気になったときに多くの開発者を引きつけました。

```csharp
public ActionResult ExportToPdf()
{
    var pdf = new ActionAsPdf("ActionName")
    {
        FileName = "Report.pdf"
    };
    return pdf;
}
```

## Rotativaの強みと弱み

その初期の人気と使いやすさにもかかわらず、Rotativaはその狭い焦点と古い技術への依存によっていくらか制限されています。ここにその強みと弱みの内訳があります：

### 強み

- **MVC統合**: RotativaはASP.NET MVCプロジェクトにシームレスに統合され、このフレームワークを利用している開発者にとって魅力的な選択肢となりました。
- **使いやすさ**: 最小限のコードで、開発者はすぐにMVCビューからPDFドキュメントを生成することができます。
- **オープンソース**: MITライセンスの下でライセンスされているRotativaは、趣味の開発者や小企業にも自由に使用や変更が許可されています。

### 弱み

- **ASP.NET MVCのみ**: Rotativaの主な制限の一つは、ASP.NET MVCに限定されていることで、Razor Pages、Blazor、ミニマルAPI、その他の.NET Coreアプリケーションで構築されたプロジェクトには適していません。
- **メンテナンス放棄**: Rotativaは何年もの間、更新やメンテナンスを受けておらず、そのユーザーを機能的およびセキュリティの脆弱性にさらしています。
- **セキュリティ上の懸念**: Rotativaのすべてのユーザーは、CVE-2022-35583のような顕著な問題を含む`wkhtmltopdf`の固有のセキュリティ脆弱性の影響を受けます。
- **スレッド問題**: ユーザーは`wkhtmltopdf`の同期制限によるスレッド問題を報告しています。

## IronPDF: 現代的な代替品

IronPDFは、HTMLからPDFを生成したいC#開発者にとって、より汎用性が高く堅牢なソリューションを提供します。MVC、Razor Pages、Blazor、API、コンソールアプリケーション、さらにはデスクトッププロジェクトなど、あらゆる.NETプロジェクトタイプと高い互換性を持っています。

### IronPDFの主な利点

- **包括的な互換性**: IronPDFはあらゆるプロジェクトタイプで機能し、Rotativaと比較してより柔軟性を提供します。
- **アクティブなメンテナンス**: Rotativaとは異なり、IronPDFは機能性とセキュリティを対象とした定期的な更新が行われています。
- **簡単な統合と使用**: IronPDFは、MVC専用の制約がないRotativaと同様に、実装が簡単に設計されています。

.NETプロジェクトでIronPDFを実装する方法を簡単に見てみましょう：

```csharp
using IronPdf;

public void CreatePdfFromHtml()
{
    var Renderer = new HtmlToPdf();
    var Pdf = Renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
    Pdf.SaveAs("output.pdf");
}
```

IronPDFを使用してPDFを生成する方法についての詳細は、以下のリソースを参照してください：

- [IronPDFでHTMLファイルをPDFに変換する](https://ironpdf.com/how-to/html-file-to-pdf/)
- [IronPDFのチュートリアルとガイド](https://ironpdf.com/tutorials/)

---

## C#でRotativaを使ってHTMLをPDFに変換する方法は？

**Rotativa**での処理方法は以下の通りです：

```csharp
// NuGet: Install-Package Rotativa.Core
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Threading.Tasks;

namespace RotativaExample
{
    public class PdfController : Controller
    {
        public async Task<IActionResult> GeneratePdf()
        {
            var htmlContent = "<h1>Hello World</h1><p>This is a PDF document.</p>";
            
            // RotativaはMVCコントローラーからViewAsPdf結果を返すことを要求します
            return new ViewAsPdf()
            {
                ViewName = "PdfView",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
    }
}
```

**IronPDF**を使用すると、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

namespace IronPdfExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var renderer = new ChromePdfRenderer();
            var htmlContent = "<h1>Hello World</h1><p>This is a PDF document.</p>";
            
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            pdf.SaveAs("output.pdf");
            
            Console.WriteLine("PDF generated successfully!");
        }
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケールアップを容易にします。

---