---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/dotnet-cross-platform-development.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/dotnet-cross-platform-development.md)
🇯🇵 **日本語:** [FAQ/dotnet-cross-platform-development.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/dotnet-cross-platform-development.md)

---

# なぜ2024年に真のクロスプラットフォーム開発のために.NETを選ぶべきなのか？

絶対に—.NETはもはやWindows専用ではありません。今日の.NETでは、同じコードベースからWindows、macOS、Linux、モバイル、ウェブ向けの高性能アプリを構築できます。MacからAPIを起動し、クラウド内のLinuxにデプロイし、モバイルデバイスをターゲットにする場合でも、.NETは真のポータビリティを提供します。このFAQでは、.NETがどのようにして真のクロスプラットフォーム開発を実現するか、どのプラットフォームをターゲットにできるか、一般的な落とし穴、および次のプロジェクトのための実用的なコード例を通して案内します。

---

## .NETは実際に「一度書けばどこでも実行」をどのように達成しているのか？

.NETを使用すると、C#でコアロジックを書き、すべての主要なプラットフォームでネイティブにデプロイできます。ランタイムはOSとハードウェアの詳細を抽象化するため、異なる環境のためにコードを微調整する必要はほとんどありません。

例えば、こちらはWindows、Mac、Linuxのどれでも同じように動作する最小限のWeb APIです：

```csharp
// Install-Package Microsoft.AspNetCore.App
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from .NET—any OS, any time!");

app.Run();
```

好みのOSで開発し、コード変更なしに別のOSにデプロイできます。それが今の.NETのリアルワールド体験です。

### .NETでクロスプラットフォームのPDF生成はできますか？

確かに。[IronPDF](https://ironpdf.com)のようなライブラリは、任意のOSでPDFをシームレスに生成することを可能にします。例えば：

```csharp
// Install-Package IronPdf
using IronPdf;

var pdfMaker = new HtmlToPdf();
var document = pdfMaker.RenderHtmlAsPdf("<h1>PDF Generation Anywhere</h1>");
document.SaveAs("cross-platform.pdf");
```

この同じコードはWindows、macOS、Linux、さらにはコンテナ内でも—プラットフォーム固有の調整は必要ありません。PDFツールについてさらに深く掘り下げるには、[2025 HTML to PDF Solutions for .NET Comparison](2025-html-to-pdf-solutions-dotnet-comparison.md)をご覧ください。

---

## .NETはかつてWindows専用だったのでは？何が変わったのか？

はい、元々の.NET FrameworkはWindows専用でした。しかし、.NET Core（そして現在の.NET 5+）以来、.NETは完全にクロスプラットフォーム、オープンソース、そして現代的に再設計されました。

- .NET 10（最新）はWindows、macOS（Apple Siliconを含む）、Linuxでネイティブに動作します。
- ARMサポートが組み込まれています—Raspberry Pi、AWS Graviton、新しいMacでコードを実行できます。
- コンテナはネイティブにサポートされており、すべてのシナリオの公式イメージがあります。

.NETを「Windows専用」のスタックとしてまだ想像しているなら、視点を更新する時です。

---

## 2024年に.NETでターゲットにできるプラットフォームは？

.NETは膨大な範囲のターゲットをサポートしており、最も多用途性のあるスタックの一つです。

### サポートされているデスクトップとサーバーOSは？

| プラットフォーム                       | サポートレベル   |
|---------------------------------|----------------|
| Windows x64 / ARM64             | 完全           |
| macOS x64 / ARM64 (Apple Silicon) | 完全        |
| Linux x64 / ARM64               | 完全           |
| Alpine Linux                    | 完全           |

ARMデバイスを含む、すべての主要なプラットフォーム向けのデスクトップおよびサーバーアプリケーションを構築できます。

### モバイルプラットフォームについてはどうですか？

.NET MAUIを使用すると、次のネイティブアプリを出荷できます：

- iOS
- Android
- iPadOS

UIを一度書いて、AppleとGoogleのモバイルエコシステムの両方にデプロイします。クロスプラットフォームUIのためにMAUIとAvaloniaを比較している場合は、[Avalonia vs MAUI for .NET 10](avalonia-vs-maui-dotnet-10.md) FAQをチェックしてください。

### サーバー側とクライアント側の両方で.NETをWeb上で使用できますか？

絶対に。使用する：

- サーバーサイドのWebアプリケーションとAPIのためのASP.NET Core。
- ブラウザ内でC#コードを実行するBlazor WebAssembly（JavaScript不要）。

C#でインタラクティブなWeb UIを構築する方法の詳細については、[Blazor in .NET 10](dotnet-10-blazor.md) FAQをチェックしてください。

### 真のクロスプラットフォームデスクトップGUIのオプションはありますか？

はい！Avaloniaは、Windows、macOS、Linuxでネイティブに実行される現代的なデスクトップUIのための際立った選択肢です。MAUIもデスクトップをサポートしていますが、特にLinuxサポートに関しては、Avaloniaが最良の選択です。[Avalonia vs MAUI](avalonia-vs-maui-dotnet-10.md)の比較でさらに深く掘り下げてください。

---

## .NETは複数のプラットフォームで実際にどのように機能するのか？

秘密のソースはビルドプロセスにあります：

1. C#コードを書きます。
2. 中間言語（IL）—中立的なバイトコードにコンパイルされます。
3. 実行時に、.NETランタイム（CoreCLRまたはMono）がILをホストOSおよびCPUのネイティブマシンコードにコンパイルします。

これは、コードベースがポータブルであり、ランタイムがOS固有の詳細をすべて処理することを意味します。醜い`#ifdef`ブロックやプラットフォームシムは不要です。

---

## 任意のOSから任意のプラットフォーム向けにビルドおよび公開できますか？

はい、それが.NETでの最大の生産性の勝利の一つです。サポートされている任意のOSからクロスコンパイルできます：

```bash
# macOSまたはLinuxからWindows .exeをビルド
dotnet publish -r win-x64 --self-contained

# WindowsまたはMacからLinuxアプリをビルド
dotnet publish -r linux-x64 --self-contained

# LinuxまたはWindowsからApple Siliconターゲットをビルド
dotnet publish -r osx-arm64 --self-contained
```

`--self-contained`フラグは.NETランタイムをバンドルするため、ターゲットシステムに.NETがインストールされていなくてもアプリが実行されます。

特にMacのためのプラットフォーム固有のセットアップのヒントについては、[macOSでの.NET開発ガイド](dotnet-development-macos.md)をチェックしてください。

---

## .NETのパフォーマンスはJava、Node.js、Goと比較してどうですか？

.NETは速い—Webサーバーの[TechEmpower benchmarks](https://www.techempower.com/benchmarks/#section=data-r20&hw=ph&test=plaintext)でしばしばトップに立ち、デスクトップおよびモバイルのネイティブスタックと競合します。

実際の用語では：

- LinuxホストのAPIは、そこにあるものと同じくらい速く実行されます。
- デスクトップアプリはWindows、Mac、Linuxでネイティブのように感じます。
- .NET MAUIモバイルアプリはネイティブアプリと同じくらいのパフォーマンスを発揮します。

ビジネスロジック、ドキュメント処理、APIのために、.NETはトップパフォーマーです。特に.NETベースのPDF生成については、プラットフォーム全体で高速なドキュメント作成のために[IronPDF](https://ironpdf.com)をチェックしてください。

---

## .NET UIの実際のクロスプラットフォームストーリー：MAUIとAvaloniaは？

[MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui)と[Avalonia](https://avaloniaui.net/)の両方が、デスクトップとモバイルUIのための単一のコードベースを書くことを可能にします。

- **MAUI**は、Windows、macOS、iOS、Androidを一つのプロジェクトでターゲットにし、ネイティブコントロールとパフォーマンスを求める場合に最適です。
- **Avalonia**は、特にLinuxのファーストクラスのサポートを持つ、現代的なクロスプラットフォームデスクトップUIを求める場合に輝きます。

詳細については、[Avalonia vs MAUI for .NET 10 development](avalonia-vs-maui-dotnet-10.md)でアプローチを比較してください。

#### 例：最小限のMAUI To-Doリスト

```csharp
// Install-Package Microsoft.Maui
using Microsoft.Maui.Controls;

public class MainPage : ContentPage
{
    public MainPage()
    {
        var tasks = new ObservableCollection<string>();
        var input = new Entry { Placeholder = "Type a task..." };
        var list = new ListView { ItemsSource = tasks };
        var add = new Button { Text = "Add" };

        add.Clicked += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(input.Text))
            {
                tasks.Add(input.Text);
                input.Text = "";
            }
        };

        Content = new StackLayout { Padding = 30, Children = { input, add, list } };
    }
}
```

この1ページは、4つの主要なプラットフォームすべてで動作します—調整は必要ありません。

#### 例：シンプルなAvaloniaファイルリーダー

```csharp
// Install-Package Avalonia
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var txtBox = this.FindControl<TextBox>("ContentBox");
        var btn = this.FindControl<Button>("OpenBtn");

        btn.Click += async (s, e) =>
        {
            var picker = new OpenFileDialog();
            var files = await picker.ShowAsync(this);
            if (files?.Length > 0)
                txtBox.Text = File.ReadAllText(files[0]);
        };
    }
    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
```

上記は、Windows、Mac、Linuxのデスクトップで動作します—Wineや互換性レイヤーは必要ありません。

---

## .NETをコンテナとクラウド環境で使用できますか？

はい—.NETはコンテナの世界で第一級の市民です。MicrosoftはSDK、ランタイム、ASP.NETの公式Dockerイメージを公開しています。

Alpineベースイメージを使用した.NET 10 Web APIの典型的なDockerfileは次のとおりです：

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app --self-contained -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

なぜAlpineか？イメージサイズを減らし、デプロイを高速化し、クラウドコストを低減します。Azure、AWS、GCP、オンプレミスのいずれの主要クラウドでも.NETコンテナを実行できます。

---

## クロスプラットフォーム.NETのチームおよびワークフローの利点は何ですか？

### .NETは混在OS開発チームをどのように扱いますか？

Windows（Visual Studio）、macOS（Rider）、Linux（VS Code）の開発者が同じ.NETコードベースで作業できます。ビルド、テスト、デプロイメントはすべてのプラットフォームで一貫しています—もう「私のマシンでは動作する」という問題はありません。

### .NETでのCI/CDはどれほど簡単ですか？

非常に簡単です。すべての主要なCI/CDプラットフォーム（GitHub Actions、Azure DevOps、Jenkins、GitLab CI）は、Windows、Linux、macOSランナーで.NETビルドをサポートしています。任意のビルドエージェントから任意のターゲットOSに公開できます。

### クラウド、サーバーレス、ARMに対する柔軟なデプロイメントについては？

.NETはサポートしています：

- 効率的なクラウドホスティングのためのLinuxコンテナ
- 企業ニーズのためのクラシックなWindowsサーバー
- Azure/AWS/Google Cloud上のサーバーレス
- コスト効率が高く、エネルギー効率の良いクラウドVMのためのARMターゲット

---

## 他のスタックよ