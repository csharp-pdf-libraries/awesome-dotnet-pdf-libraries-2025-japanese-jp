---
**  (Japanese Translation)**

 **English:** [FAQ/dotnet-10-linux-support.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/dotnet-10-linux-support.md)
 **:** [FAQ/dotnet-10-linux-support.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/dotnet-10-linux-support.md)

---
# .NET 10をLinuxで実行する方法は？（実践的な開発者FAQ）

Linuxでの.NET 10の実行はこれまでになくスムーズで生産的になりました。真のLTS、完全なC# 14サポート、主要なディストリビューション用の公式パッケージを備えて、Linuxは現在、一流の.NET開発およびデプロイメントプラットフォームです。このFAQは、インストール、クロスプラットフォームのヒント、一般的な落とし穴、実際のコードについてカバーしているので、すぐに実行を開始できます。

---

## なぜWindowsではなくLinuxで.NET 10を使用すべきですか？

Linuxは.NET開発のための強力なプラットフォームになりました。.NET 10を使用すると、長期サポート、最先端のパフォーマンス、C# 14を使用した現代的な開発体験を得ることができます。Linuxは、クラウドやコンテナのシナリオでWindowsに匹敵するかそれを上回ることがよくあり、マイクロサービス、API、あるいはいくつかのデスクトップアプリのデプロイを容易にします。最新のC#機能に興味がある場合は、[.NET 10のためのC# 14チュートリアル](dotnet-10-csharp-14-tutorial.md)をご覧ください。

---

## どのLinuxディストリビューションが.NET 10をサポートしていますか？

.NET 10は、いくつかの主要なディストリビューションを公式にサポートしています：

- **公式Microsoftパッケージ：** Ubuntu (22.04, 24.04), Debian (11, 12), RHEL (8.10+), Fedora (39+), openSUSE Leap (15.5+), Alpine Linux (3.18+), CentOS Stream 9。
- **コミュニティサポート（手動セットアップが必要な場合あり）：** Arch, Manjaro, Linux Mint, Pop!_OS。

リストにないディストリビューションを使用している場合でも、Snapパッケージや手動インストールスクリプトをほぼ常に使用できます。Archのようなローリングリリースの場合は、[AURパッケージ](https://aur.archlinux.org/packages?O=0&K=dotnet-sdk-10.0)を確認してください。

---

## 自分のLinuxディストリビューションに.NET 10をインストールする方法は？

主要なディストリビューションのための簡単な概要です。ほとんどの場合、Microsoftの公式リポジトリが最も最新のパッケージを提供します。

### UbuntuまたはDebianにインストールする最良の方法は？

推奨される方法は、Microsoft自身のパッケージフィードを介しています：

```bash
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
```
または、よりシンプル（ただし時には古い）方法で：
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
```
インストール後、バージョンを確認してください：
```bash
dotnet --version
```

#### LinuxでIronPDFのような.NETライブラリを使用できますか？

もちろんです！[IronPDF](https://ironpdf.com)を使用した例はこちらです：

```csharp
// Install-Package IronPdf (via NuGet)
using IronPdf;

var pdfGen = new HtmlToPdf();
pdfGen.RenderHtmlAsPdf("<h2>Hello from Linux & .NET 10!</h2>")
      .SaveAs("linux-hello.pdf");
Console.WriteLine("PDF created: linux-hello.pdf");
```
PDF/A準拠に興味がある場合は、[C#でのPDF/Aガイド](pdf-a-compliance-csharp.md)をご覧ください。

---

### Red HatまたはFedoraに.NET 10をインストールする方法は？

**RHEL**または**Fedora**の場合、DNFを使用します：

```bash
sudo dnf install dotnet-sdk-10.0
```
アップデートは迅速で、MicrosoftとRed Hatが密接に協力しているため、統合は堅牢です。

#### Linuxで最小限のASP.NET Core APIを開始するには？

これを試してみてください：

```bash
dotnet new webapi -n LinuxApiDemo
cd LinuxApiDemo
dotnet run
```
新しいAPIにアクセスするには、[http://localhost:5000/weatherforecast](http://localhost:5000/weatherforecast)を参照してください。

---

### Alpine Linuxにインストールする最も簡単な方法は？

Alpineはコンテナに最適です：

```bash
apk add dotnet10-sdk
```
または、Docker内で：
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/YourApp.dll"]
```
Alpineベースのイメージは非常に小さく、本番環境に理想的です。

---

### Arch、Manjaro、Mint、またはPop!_OSに.NET 10をインストールするには？

**Arch/Manjaro**の場合：  
お気に入りのAURヘルパーを使用してください：
```bash
yay -S dotnet-sdk-10.0
```
**Mint/Pop!_OS**の場合：  
Ubuntuの指示に従ってください。必要に応じて、universe/multiverseリポジトリを有効にします。

---

### ディストリビューションがリストにない場合、Snap、手動スクリプト、またはDockerを使用できますか？

はい！普遍的な方法には以下が含まれます：

- **Snap：**  
  ```bash
  sudo snap install dotnet-sdk --classic --channel=10.0
  ```
- **手動スクリプト：**  
  ```bash
  wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --channel 10.0
  ```
- **Docker：**  
  Microsoftの公式イメージを使用してください：
  ```dockerfile
  FROM mcr.microsoft.com/dotnet/sdk:10.0
  ```

これらのアプローチは、ほぼすべてのLinuxシステムで機能します。

---

## Linux開発者にとって.NET 10の新機能は何ですか？

.NET 10は以下をもたらします：

- **長期サポート：** 2028年まで維持されます。
- **パフォーマンスの向上：** JITが速く、特にコンテナ用にメモリが少なくなります。
- **モダンなC# 14：** パターンマッチング、改善されたレコードなど（[完全なC# 14チュートリアルを見る](dotnet-10-csharp-14-tutorial.md)）。
- **ポスト量子暗号化：** 将来の証明のための新しい安全なアルゴリズム。
- **直接C#スクリプト実行：** `.cs`ファイルを直接実行する—クイックスクリプトに最適。

### LinuxでC#スクリプトを直接実行するにはどうすればよいですか？

コードを書いて実行するだけです：
```bash
echo 'System.Console.WriteLine("Hello from C# script!");' > demo.cs
dotnet run demo.cs
```

### 新しい暗号化プリミティブを使用できますか？

はい、新しいAPIがサポートされています。例（APIは変更される可能性があります）：

```csharp
// Install-Package System.Security.Cryptography.PostQuantum
using System.Security.Cryptography.PostQuantum;

var keyPair = MLKEM.GenerateKeyPair();
var encrypted = MLKEM.Encrypt("SecretData", keyPair.PublicKey);
var decrypted = MLKEM.Decrypt(encrypted, keyPair.PrivateKey);
Console.WriteLine($"Decrypted: {decrypted}");
```

---

## .NET 10用のLinux開発環境をどのようにセットアップすればよいですか？

選択肢があります：

- **VS Code + C# Dev Kit：**  
  ```bash
  sudo snap install code --classic
  ```
  豊富な編集とデバッグのために「C# Dev Kit」拡張機能をインストールします。
- **Rider IDE：**  
  [Rider](https://www.jetbrains.com/rider/)は大規模プロジェクトに優れています。
- **ターミナルのみ：**  
  CLIは強力です。`dotnet new`、`dotnet build`、`dotnet run`とよく組み合わせるツールとして[neovim](https://neovim.io/)があります。

.NET UIフレームワークの比較については、[Avalonia vs MAUI for .NET 10](avalonia-vs-maui-dotnet-10.md)をご覧ください。

---

## LinuxからWindows、Linux、macOS向けのアプリをビルドして公開するにはどうすればよいですか？

クロスプラットフォームの公開は簡単です：

```bash
# Windows x64
dotnet publish -r win-x64 --self-contained -c Release -o out/win

# Linux x64
dotnet publish -r linux-x64 --self-contained -c Release -o out/linux

# macOS ARM64
dotnet publish -r osx-arm64 --self-contained -c Release -o out/mac
```
各OS用のネイティブ実行可能ファイルが得られます。

### 一貫したビルドのためにDockerを使用できますか？

はい！2段階のDockerビルドはこちらです：

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -r linux-x64 --self-contained -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./YourAppExecutable"]
```

Blazorアプリのビルドについては、[.NET 10のBlazor FAQ](dotnet-10-blazor.md)をチェックしてください。

---

## .NET 10はLinuxまたはWindowsで速いですか？

多くのワークロードで、LinuxはWindowsに匹敵するか、それを上回ることがあります。  
- **Web API：** オーバーヘッドが低く、コンテナ密度が良い。
- **ネットワーキング：** KestrelはLinuxネイティブの機能を活用します。
- **メモリ：** Linuxコンテナではしばしばフットプリントが小さい。

### Linuxでパフォーマンスをベンチマークするにはどうすればよいですか？

[wrk](https://github.com/wg/wrk)でのクイックベンチマーク：

```csharp
// Install-Package Microsoft.AspNetCore.App
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var app = WebApplication.Create();
app.MapGet("/", () => "Hi from Linux!");
app.Run();
```
アプリを起動したら、次を実行します：
```bash
wrk -t4 -c100 -d30s http://localhost:5000/
```

---

## Linuxで.NET 10を使用してネイティブのデスクトップアプリをビルドできますか？

Linuxでの.NET GUIは進行中です：

- **Avalonia：** クロスプラットフォームで現在うまく機能しています。
- **MAUI：** Linuxサポートは作業中です—[AvaloniaとMAUIの比較](avalonia-vs-maui-dotnet-10.md)をご覧ください。
- **Uno Platform：** 別のクロスプラットフォームオプション。

### Avaloniaアプリを作成するにはどうすればよいですか？

```bash
dotnet new avalonia.app -n LinuxGuiApp
cd LinuxGuiApp
dotnet run
```
より多くの洞察については、[.NET 10でのクロスプラットフォームGUIガイド](avalonia-vs-maui-dotnet-10.md)をご覧ください。

---

## Linuxで.NET 10を使用する際の一般的な落とし穴は何ですか？

### なぜ`dotnet: command not found`が発生するのですか？

.NETがインストールされていないか、`$PATH`にありません。  
確認してください：
```bash
ls /usr/share/dotnet/
export PATH=$PATH:/usr/share/dotnet
```
または手動インストールの場合：
```bash
export PATH=$PATH:$HOME/.dotnet
```

### SSL証明書エラーをどのように修正しますか？

システム証明書をインストールします：
```bash
# Ubuntu/Debian
sudo apt-get install ca-certificates

# Alpine
apk add ca-certificates
update-ca-certificates
```

### 不足している依存関係に関するエラーが発生した場合はどうすればよいですか？

一部の.NETライブラリ（[IronPDF](https://ironpdf.com)など）は、ICUのような追加のLinuxライブラリを必要とすることがあります：

```bash
# Ubuntu/Debian
sudo apt-get install libicu-dev

# Alpine
apk add icu-libs
```
奇妙なグローバリゼーションやレンダリングエラーが発生している場合、これがよくある解決策です。

### 公開されたアプリのファイル権限をどのように扱いますか？

公開された実行可能ファイルの場合：
```bash
chmod +x ./YourAppExecutable
```

### アプリが1024以下のポートにバインドできないのはなぜですか？

デフォルトでは、rootのみがこれを行うことができます。高いポート（例えば、8080）を使用するか、次を実行してください：
```bash
sudo setcap 'cap_net_bind_service=+ep' ./YourAppExecutable
```

---

## クイックリファレンスコマンドとDockerイメージをどこで見つけることができますか？

**インストールコマンドテーブル：**

| ディストリビューション    | コマンド                                      |
|