# macOSでVisual Studio for Macがなくなった今、どうやって.NETアプリケーションを開発するのか？

2025年にMacで最新の.NET開発環境を設定する方法を探していますか？あなたは一人ではありません—Visual Studio for Macが廃止されて以来、多くの開発者がツールやワークフローを変更しています。良いニュースは、.NETがmacOSで盛んになっており、適切なセットアップがあればAPIからクロスプラットフォームのGUIアプリまで何でも構築できることです。始め方、使用するツール、実用的なコードのヒントについてはこちらです。

---

## Visual Studio for Macに何が起こったのか、代わりに何を使うべきか？

Visual Studio for Macは、2024年8月31日をもって公式に廃止されました。推奨される代替品は、軽量な体験のための[VS Code with the C# Dev Kit](https://code.visualstudio.com/docs/languages/dotnet)、またはフル機能のIDEである[JetBrains Rider](https://www.jetbrains.com/rider/)です。どちらもmacOS上でスムーズに動作し、.NET開発を完全に実現可能にしています。

クロスプラットフォームのワークフローに関するさらなる戦略については、[Dotnet Cross Platform Development](dotnet-cross-platform-development.md)を参照してください。

---

## 2025年のmacOSで最高の.NET IDEは何ですか？

現在、いくつかの堅実なエディター選択肢があり、それぞれに強みがあります：

- **VS Code + C# Dev Kit**：高速、無料、そして小さな編集や小規模プロジェクト、スクリプティングに最適です。深いリファクタリングやデザイナー機能には欠けますが、日常のコーディングには優れています。
- **JetBrains Rider**：高度なリファクタリング、デバッグ、ホットリロード、堅牢なNuGetワークフローを備えた包括的なIDE（個人/非商用利用は無料）。大規模プロジェクトやチームに最適です。
- **Neovim/Vim + OmniSharp**：ターミナルベースのワークフローやカスタマイズが好きなら、このオプションがあなたに適していますが、セットアップのカーブが急であることを期待してください。

一般的に、小規模/中規模プロジェクトにはVS Codeから始め、複雑またはチームベースの開発にはRiderに移行します。より詳しい比較については、[VS Code vs Rider: The Real-World Showdown](#vs-code-vs-rider-the-real-world-showdown)をチェックしてください。

---

## macOSで.NETを正しい方法でインストールするには？

ほとんどの開発者にとって、[Homebrew](https://brew.sh/)を使用するのがインストールし、更新を維持する最も簡単な方法です：

```bash
brew install dotnet-sdk
```

代わりに、[dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)から直接インストーラーをダウンロードします。動作を確認するには、次を実行します：

```bash
dotnet --version
```

SDKバージョンを切り替える必要がある場合は、[dotnet-install scripts](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script)を使用してください。

---

## .NET開発用のVS Codeを設定するにはどうすればよいですか？

まず、これらの拡張機能をインストールします：
- **C# Dev Kit**（Microsoft）言語サポート、インテリセンス、デバッグ用。
- **.NET Install Tool** および（オプションで）NuGet Package Manager 依存関係の管理用。

新しいプロジェクトを開始するには：

```bash
dotnet new webapi -n MyApi
cd MyApi
code .
```

VS Codeはパッケージを復元し、デバッグを即座に提供します。ホットリロードには、使用します：

```bash
dotnet watch run
```

MVCプロジェクトからビューをレンダリングしたり、PDFをエクスポートしたりする場合は、[Mvc View To Pdf Csharp](mvc-view-to-pdf-csharp.md)を参照してください。

---

## macOSで.NETにJetBrains Riderを検討する理由は？

[Rider](https://www.jetbrains.com/rider/)は、深いコード分析、高度なリファクタリング、または優れたマルチプラットフォームデバッグが必要な場合に理想的です。任意の`.sln`または`.csproj`を開くだけで動作し、ホットリロードや組み込みターミナルなどの機能があります。こちらは、PDFジェネレータの簡単な例です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new HtmlToPdf();
        renderer.RenderHtmlAsPdf("<h1>Hello, IronPDF on macOS!</h1>")
                .SaveAs("example.pdf");
    }
}
```

IronPDF ([docs](https://ironpdf.com))はmacOS上でシームレスに動作し、RiderのNuGet UIはパッケージの追加を容易にします。

トップPDFライブラリの比較については、[2025 Html To Pdf Solutions Dotnet Comparison](2025-html-to-pdf-solutions-dotnet-comparison.md)を参照してください。

---

## Macで.NETプロジェクトを作成、ビルド、クロスコンパイルするにはどうすればよいですか？

コンソールアプリを作成します：

```bash
dotnet new console -n HelloMac
cd HelloMac
dotnet run
```

他のプラットフォーム用にビルドするには：

```bash
dotnet publish -r win-x64 --self-contained
dotnet publish -r linux-x64 --self-contained
dotnet publish -r osx-arm64 --self-contained
```

これにより、ユーザーが.NETランタイムを必要としない各プラットフォームのネイティブバイナリが生成されます。

マルチプラットフォームアプリ戦略についての詳細は、[Dotnet Cross Platform Development](dotnet-cross-platform-development.md)と[Avalonia Vs Maui Dotnet 10](avalonia-vs-maui-dotnet-10.md)を参照してください。

---

## macOSでASP.NET Coreアプリケーションを開発できますか？

もちろんです！ASP.NET CoreはmacOS上でネイティブに実行されます。こちらは、最小限のAPIの例です：

```csharp
// NuGet: Microsoft.AspNetCore.App
using Microsoft.AspNetCore.Builder;

var app = WebApplication.CreateBuilder(args).Build();
app.MapGet("/", () => "Hello from ASP.NET Core on macOS!");
app.Run();
```

HTTPS開発証明書を信頼するには：

```bash
dotnet dev-certs