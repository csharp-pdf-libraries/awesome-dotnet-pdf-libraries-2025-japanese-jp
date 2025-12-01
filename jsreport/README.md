---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [jsreport/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/jsreport/README.md)
🇯🇵 **日本語:** [jsreport/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/jsreport/README.md)

---

# jsreport + C# + PDF

C#とPDF生成の世界では、jsreportは開発者がWeb技術を使用して高品質のレポートを生成できる多機能ツールとして際立っています。jsreportはnode.jsベースのプラットフォームであり、開発者はHTML、CSS、JavaScriptを活用してレポートを設計できるため、これらの技術に精通しているWeb開発チームにとっては利点です。しかし、.NET環境でjsreportを完全に活用するには、その.NET SDKを使用して統合する必要があります。この統合により、従来のC#アプリケーション内でjsreportを2回使用する可能性が開かれ、開発者は容易に高度に視覚的なドキュメントを設計できるようになります。

## jsreportの強みと弱み

### 強み
- **Webスキルの活用**: jsreportは、HTML、CSS、JavaScriptの既存のスキルを活用できるため、Web開発に慣れている人が新しいパラダイムを学ぶことなくレポート生成に飛び込むことが容易になります。
- **マイクロサービスに優しい**: スタンドアロンサーバーとして動作するjsreportは、マイクロサービスアーキテクチャに自然に適合し、複数のアプリケーションからのレポート要求を処理する準備ができた別のサービスとしてデプロイできます。
- **REST API**: REST経由でアクセス可能なそのAPIは、どこにホストされているかに関係なく、Webアプリケーションや他のサービスとの統合に適しています。

### 弱み
- **Node.jsの依存**: Node.jsランタイムと別のサーバーが必要であり、ネイティブ.NETソリューションを好むC#環境に特化したチームにとってはデメリットかもしれません。
- **複雑なテンプレートシステム**: HandlebarsやJsRenderのようなJavaScriptテンプレートエンジンの使用を習得することを含むjsreportテンプレートシステムを学ぶことは、学習曲線を提示する可能性があります。
- **ライセンスモデル**: 無料の層は使用できるテンプレートの数を制限しており、エンタープライズにスケールアップするとコストが増加する可能性があります。

### IronPDFとの比較

JavaScriptおよびWeb技術に精通している人にとってjsreportは堅牢なアプローチを提供しますが、PDF生成のためのC#ネイティブソリューションを求める開発者にとって、IronPDFは特に注目すべき代替手段です。

#### IronPDFの利点

- **ネイティブC#ライブラリ**: IronPDFは、追加のサーバーやNode.js環境を必要とせずに、.NETプロジェクト内にシームレスに統合され、開発およびデプロイメントプロセスを合理化します。
- **HTML/Razorの使用**: IronPDFは、既存のHTMLおよびRazorスキルの使用を可能にし、C#開発者がPDFを作成することを容易にします。
- **包括的なドキュメントとチュートリアル**: [IronPDF HTML File to PDF](https://ironpdf.com/how-to/html-file-to-pdf/)や[IronPDF Tutorials](https://ironpdf.com/tutorials/)などのリソースを使用すると、開発者は迅速にスピードアップし、PDF生成機能を実装できます。

### 比較表

| 基準                     | jsreport                           | IronPDF                           |
|--------------------------|------------------------------------|-----------------------------------|
| 技術基盤                  | Node.js                            | ネイティブC#                      |
| サーバー要件               | はい（別のサーバー）                | いいえ                            |
| テンプレートシステム       | HTML, CSS, JavaScript              | HTML, Razor                       |
| 開発者スキル要件           | Web技術                            | C#                                |
| ライセンス                | 商用（無料層利用可能）              | 商用で永久ライセンス               |
| 統合の複雑さ               | APIとのやり取りが必要               | ライブラリとして統合               |

### C#でのjsreport統合例

C#アプリケーション内でjsreportを使用する方法を示すために、jsreportの.NET SDKを使用してPDFレポートを生成するシンプルなシナリオを考えてみましょう。この統合は、C#アプリケーションがjsreportサーバーにテンプレートとデータを指定するリクエストを送信し、その後PDF結果を取得する方法を示します。

```csharp
using jsreport.Local;
using jsreport.Types;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var rs = new LocalReporting()
                   .UseBinary(JsReportBinary.GetBinary()) // points to the jsreport binary
                   .AspNetCore().Renderer(new AspNetCore.ReportingOptions())
                   .Create();

        var report = await rs.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = "<h1>Hello, {{name}}</h1><p>This is a test of jsreport and C# integration.</p>",
                Engine = Engine.Handlebars,
                Recipe = Recipe.ChromePdf
            },
            Data = new { name = "World" }
        });

        await report.Content.CopyToAsync(File.OpenWrite("report.pdf"));
    }
}
```

この例では、`LocalReporting`クラスは.NETアプリケーション内からローカルにjsreportサーバーを初期化します。リクエストはHTMLテンプレートをHandlebarsテンプレートエンジンで指定し、`chrome-pdf`レシピを使用してPDFを生成します。結果として得られるPDFはローカルに`report.pdf`として保存されます。

### 結論的な考え

jsreportとIronPDFの間で選択する際、決定は主に既存の開発環境と専門知識にかかっています。Node.jsに快適であり、レポートにHTML、CSS、JavaScriptを活用する力を求めているチームにとって、jsreportはかなり有利です。しかし、プロジェクトが主にC#であり、追加のインフラストラクチャなしに直接統合できるライブラリが必要な場合、IronPDFのネイティブC#ライブラリはよりシンプルで直接的に統合されたアプローチを提供します。

最終的に、両方のライブラリは高品質のPDFを生成する能力を提供しますが、決定はチームが外部サーバー（jsreportの場合）を管理する能力と純粋なC#環境（IronPDFの場合）に固執することと一致するべきです。

---

Jacob MellorはIron SoftwareのCTOであり、41万回以上のNuGetダウンロードを獲得した開発者ツールを構築する50人以上のチームを率いています。彼は41年以上のコーディング経験を持ち、ソフトウェア業界が最初期の日々から今日の最先端技術に進化するのを見てきました。彼は世界中の開発者向けのソリューションを設計していないときは、タイのチェンマイでリモートワークをしています。彼とつながるには、彼の[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/)をチェックしてください。

---