---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/install-nuget-powershell.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/install-nuget-powershell.md)
🇯🇵 **日本語:** [FAQ/install-nuget-powershell.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/install-nuget-powershell.md)

---

# .NETプロジェクトでNuGetパッケージをPowerShellを使用して管理する方法は？

Visual Studio内のNuGetと格闘したり、GUI、CLI、謎のスクリプト間を行き来したりするのに疲れたら、あなただけではありません。PowerShellを介してNuGetを管理することで、スクリプト可能で、再現性があり、クロスプラットフォームのワークフローを解除します。これは、現代の.NET開発、CI/CDパイプライン、およびDocker環境にとって重要です。このFAQでは、.NETパッケージ管理を合理化するための基本、トラブルシューティングのヒント、および実用的なスクリプトを紹介します。IDEは必要ありません。

## なぜNuGetパッケージを管理するためにPowerShellを使用すべきですか？

PowerShellを使用すると、ワークステーション、ビルドサーバー、Dockerコンテナー、またはLinux/macOSなど、任意の環境からNuGetパッケージを管理できます。このアプローチは高度にスクリプト化可能であり、インストール、アップグレード、ロールバックを完全に自動化して信頼できるものにします。これはCI、デプロイメント、再現可能なビルドにとって重要です。.NET 6以降に移行する場合は、環境が設定されていることを確認してください（[How To Install Dotnet 10](how-to-install-dotnet-10.md)を参照）。

## PowerShellのNuGetプロバイダーとNuGetモジュールの違いは何ですか？

最初は混乱するかもしれません：
- **NuGetプロバイダー**：これは、NuGetフィード（NuGet.orgやプライベートソースなど）との通信を可能にするPowerShellの"エンジン"です。
- **NuGetモジュール**：これは、実際のパッケージ管理のための`Install-Package`や`Update-Package`などのPowerShellコマンドを追加します。

両方が必要です：プロバイダーで接続し、モジュールでパッケージを制御します。

## PowerShellでNuGetを設定するにはどうすればよいですか？

始めるには、プロバイダーとモジュールの両方をインストールするという2つのステップが必要です。

### NuGetプロバイダーをインストールするにはどうすればよいですか？

以下をPowerShellセッションで実行して、NuGetプロバイダーをシステム全体に追加します：

```powershell
Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force
```
管理者権限がない場合は、現在のユーザーのみにインストールします：
```powershell
Install-PackageProvider -Name NuGet -Scope CurrentUser
```

### NuGet PowerShellモジュールをインストールするにはどうすればよいですか？

次に、NuGetモジュールを追加します：

```powershell
Install-Module -Name NuGet -Force
```
PowerShellギャラリーを信頼するかどうかを尋ねられた場合は、`Y`で確認します。インストールを確認します：
```powershell
Get-Module -ListAvailable -Name NuGet
```

## PowerShellからNuGetパッケージをインストール、更新、または削除するにはどうすればよいですか？

PowerShellを使用すると、高度なシナリオでさえも、依存関係を簡単に管理できます。

### パッケージを名前またはバージョンでインストールするにはどうすればよいですか？

パッケージをインストールするには（例：IronPdf）：

```powershell
Install-Package IronPdf
```
特定のバージョンの場合：
```powershell
Install-Package IronPdf -RequiredVersion 2024.1.6
```
インストールされたパッケージを確認します：
```powershell
Get-Package IronPdf
```
バージョンを固定することで、再現可能なビルドを作成できます（これが重要な理由については、[2025 Html To Pdf Solutions Dotnet Comparison](2025-html-to-pdf-solutions-dotnet-comparison.md)を参照してください）。

### NuGetパッケージを更新またはアンインストールするにはどうすればよいですか？

最新バージョンに更新するには：
```powershell
Update-Package IronPdf
```
パッケージを削除するには：
```powershell
Uninstall-Package IronPdf
```
すべてのパッケージを更新します（注意して）：
```powershell
Update-Package
```
Newtonsoft.Jsonなどの人気ライブラリの一般的なタスクをスクリプト化できます：
```powershell
Install-Package Newtonsoft.Json -RequiredVersion 13.0.3
Update-Package Newtonsoft.Json
Uninstall-Package Newtonsoft.Json
```
PDFファイルにアクセスしたり操作したりする関連タスクについては、[Access Pdf Dom Object Csharp](access-pdf-dom-object-csharp.md)および[Add Copy Delete Pdf Pages Csharp](add-copy-delete-pdf-pages-csharp.md)を参照してください。

## カスタムパッケージソースまたはプライベートフィードをPowerShellで操作するにはどうすればよいですか？

NuGet.orgに限定されることはありません。PowerShellはプライベート/内部フィードをサポートします。

- **ソースをリストする**：
    ```powershell
    Get-PackageSource
    ```
- **新しいソースを追加する**：
    ```powershell
    Register-PackageSource -Name MyPrivateFeed -Location https://myfeed.example.com/nuget -ProviderName NuGet
    ```
- **ソースを削除する**：
    ```powershell
    Unregister-PackageSource -Name MyPrivateFeed
    ```
- **特定のソースからインストールする**：
    ```powershell
    Install-Package MySecretLibrary -Source MyPrivateFeed
    ```

## CI/CDパイプラインまたはビルドスクリプトでNuGetを使用するにはどうすればよいですか？

PowerShellスクリプトは、サーバー、Docker、またはクラウドビルドの自動化を簡単にします。

```powershell
Install-PackageProvider -Name NuGet -Force
Install-Module -Name NuGet -Force
Install-Package IronPdf -Source https://api.nuget.org/v3/index.json
Install-Package Newtonsoft.Json
dotnet restore ./MyProject.sln
dotnet build ./MyProject.csproj -c Release
dotnet pack ./MyProject.csproj -o ./artifacts
```
これにより、すべての環境が正確な依存関係を取得し、信頼できるビルドが必須になります。CIでウェブページやASPXをPDFに変換する場合は、[Convert Aspx To Pdf Csharp](convert-aspx-to-pdf-csharp.md)を参照してください。

## PowerShellを使用して.NETでIronPDFでPDFを生成できますか？

もちろんです！PowerShell経由でIronPDFをインストールした後、C#コードでHTMLからPDFを作成できます：

```powershell
Install-Package IronPdf
```
```csharp
using IronPdf; // NuGet: Install-Package IronPdf

var pdfRenderer = new ChromePdfRenderer();
string html = "<h1>Hello, PDF from PowerShell!</h1>";
var doc = pdfRenderer.RenderHtmlAsPdf(html);
doc.SaveAs("output.pdf");
```
完全なウォークスルーについては、この[PDF生成ビデオ](https://ironpdf.com/blog/videos/how-to-generate-pdf-files-in-dotnet-core-using-ironpdf/)をチェックしてください。

## オフラインまたはエアギャップ環境でNuGetパッケージをインストールするにはどうすればよいですか？

まず、接続されたマシンでパッケージをダウンロードします：
```powershell
Save-Package IronPdf -Path C:\NuGetPackages
```
そのフォルダーをオフラインサーバーにコピーし、ローカルにインストールします：
```powershell
Install-Package IronPdf -Source C:\NuGetPackages
```
このアプローチは、セキュアまたは切断されたデプロイメントにとって非常に貴重です。

## PowerShell CoreとWindows PowerShellでNuGetは同じように機能しますか？

ほとんどは、はい！PowerShell Core（7+）はクロスプラットフォームなので、Windows、Linux、またはmacOSでこれらのコマンドを使用できます。非Windowsシステムでは`powershell`の代わりに`pwsh`を使用してください。非常に古いモジュールはサポートされない場合がありますが、NuGetとIronPDFは問題なく動作します。

## 一般的なNuGet PowerShellの問題とその修正方法は何ですか？

- **依存関係のエラー**：`-AllowClobber -Force -Scope CurrentUser`でモジュールをインストールします。
- **古いWindowsのTLSエラー**：TLS 1.2を強制します：
    ```powershell
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    ```
- **権限の問題**：必要に応じて`-Scope CurrentUser`を使用します。
- **モジュールが見つからない**：`Import-Module NuGet`を試してください。
- **ソースの問題**：`Get-PackageSource`でフィードを確認し、URLが正しいことを確認してください。

インストール後にPDFファイルを操作する必要がある場合は、[Access Pdf Dom Object Csharp](access-pdf-dom-object-csharp.md)を参照してください。

## 最も便利なPowerShell NuGetコマンドは何ですか？

こちらはクイックリファレンスです：

| タスク                      | コマンド                                          |
|---------------------------|--------------------------------------------------|
| プロバイダーのインストール          | `Install-PackageProvider -Name NuGet -Force`     |
| モジュールのインストール            | `Install-Module -Name NuGet -Force`              |
| パッケージのインストール           | `Install-Package PackageName`                    |
| 特定のバージョンのインストール  | `Install-Package PackageName -RequiredVersion X` |
| パッケージの更新            | `Update-Package PackageName`                     |
| パッケージのアンインストール         | `Uninstall-Package PackageName`                  |
| インストール済みのリスト            | `Get-Package`                                    |
| ソースの追加/削除         | `Register/Unregister-PackageSource ...`           |
| オフライン用に保存          | `Save-Package PackageName -Path Folder`          |

さらに詳しくは、[2025 Html To Pdf Solutions Dotnet Comparison](2025-html-to-pdf-solutions-dotnet-comparison.md)を参照してください。

---

*質問がありますか？[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、Iron Softwareの最高技術責任者はいつでも喜んでお手伝いします。以下にコメントを残すか、より多くのチュートリアルについて[IronPDF](https://ironpdf.com)をチェックしてください。*
---

## 関連リソース

### 📚 チュートリアル＆ガイド
- **[HTML to PDF Guide](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[Best PDF Libraries 2025](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[Beginner Tutorial](../csharp-pdf-tutorial-beginners.md)** — 最初のPDFを5分で
- **[Decision Flowchart](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[Merge & Split PDFs](../merge-split-pdf-csharp.md)** — 文書の結合
- **[Digital Signatures](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[Extract Text](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[Fill PDF Forms](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF Generation](../blazor-pdf-generation.md)** — Blazorサポート
- **[Cross-Platform Deployment](../cross-platform-pdf-dotnet.md)** — Docker、Linux、クラウド

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*「[Awesome .NET PDF Libraries 2025](../README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を提供。*


---

[ジェイコブ・メロー](https://ironsoftware.com/about-us/authors/jacobmellor/)によって執筆、Iron Softwareの最高技術責任者。ジェイコブは6歳でコーディングを始め、実際のPDFの課題を解決するためにIronPDFを作成しました。タイのチェンマイに拠点を置く。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/) | [GitHub](https://github.com/jacob-mellor)