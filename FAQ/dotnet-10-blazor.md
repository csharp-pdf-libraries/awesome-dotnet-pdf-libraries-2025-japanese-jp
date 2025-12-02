# .NET 10 Blazorの新機能と開発者が利用する方法

.NET 10はBlazorに大幅なアップグレードをもたらし、バンドルサイズの大幅な縮小、簡単な永続的状態、パスワードレス認証、より良いフォーム検証などが含まれます。現代のC# Webアプリを構築している場合、これらの機能は時間の節約とより良いユーザーエクスペリエンスの提供に役立ちます。ここでは、最も重要な変更とそれらを今日どのように使用するかについての実用的なFAQを紹介します。

---

## .NET 10でBlazorバンドルサイズはどれくらい小さくなりましたか？

BlazorのコアJavaScriptバンドルは、現在わずか43 KB（183 KBからダウン）で、76%の削減です。これは自動的です：アップグレードすると、ユーザーはすぐにより速いページの読み込みと改善されたキャッシングを得ます。設定やコードの調整は必要ありません。

ドキュメントが多いアプリ（例：PDF、レポート）を構築している場合、この小さなバンドルは[IronPDF](https://ironpdf.com)と特によく組み合わせることができ、UIが迅速にロードされる間に複雑な処理がサーバー側で行われます。その他のエコシステムの更新については、[Dotnet 10 Linux Support](dotnet-10-linux-support.md)を参照してください。

---

## Blazor Serverで永続的状態はどのように機能しますか？

.NET 10 Blazor Serverは、切断またはサーバーの再起動を越えて状態を永続化することができるようになりました。不安定なWiFiのためにユーザーのセッションが失われることはもうありません。

状態を持つプロパティに`[PersistentState]`を装飾するだけです：

```csharp
// NuGet: Microsoft.AspNetCore.Components.Server
using Microsoft.AspNetCore.Components;

[PersistentState]
public string Theme { get; set; } = "Light";
```

この属性はBlazorに、再接続後にそのプロパティを自動的に保存および復元するように指示します。複雑なオブジェクトも機能します。それらに`[PersistentState]`をマークするだけです。

### JavaScriptから状態永続性を制御できますか？

はい！.NET 10では、JavaScriptからBlazor「回路」を一時停止および再開することができます。これは高度なシナリオに便利です：

```javascript
// Blazorを一時停止
Blazor.pause();
// Blazorを再開
Blazor.resume();
```

これは、デバッグ中や重要なアクション中にUIを凍結する際に便利です。

---

## Blazorでパスキー（パスワードレス）認証をどのように使用しますか？

.NET 10は、FIDO2/WebAuthnパスキーを箱から出してすぐにサポートしており、指紋、Face ID、またはハードウェアキーログインを簡単に有効にすることができます。

セットアップで、Identityにパスキーサポートを追加します：

```csharp
// NuGet: Microsoft.AspNetCore.Identity
using Microsoft.AspNetCore.Identity;

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddPasskeys();
```

次に、パスキーサインインをトリガーするUIを作成します：

```csharp
private async Task SignInWithPasskey()
{
    var result = await AuthenticationService.RequestPasskeySignInAsync();
    // 結果を処理
}
```

覚えておいてください：パスキー認証にはHTTPSと最新のIdentityセットアップが必要です。セキュアな認証についての詳細は、[Dotnet 10 Csharp 14 Tutorial](dotnet-10-csharp-14-tutorial.md)をチェックしてください。

---

## ネストされたモデルのフォーム検証はどのように改善されましたか？

Blazorは現在、複雑でネストされた検証を直接サポートしているため、深く構造化されたフォームに対して複雑なカスタムロジックは不要です。

`Program.cs`で検証のための型を登録します：

```csharp
// NuGet: Microsoft.Extensions.DependencyInjection
builder.Services.AddValidatableTypes();
```

次に、モデルで`[ValidatableType]`と`[ValidateComplexType]`を使用します：

```csharp
[ValidatableType]
public class Order {
    [Required] public string CustomerName { get; set; }
    [ValidateComplexType] public Address ShippingAddress { get; set; } = new();
}
```

これで、Blazorはネストされたすべてのプロパティを自動的に検証します。これは、複雑なデータを持つ実際のフォームにとって素晴らしいことです。

---

## .NET 10 Blazorで404ページをどのように処理しますか？

「見つからない」ルートの処理は現在、単一の行です：

```csharp
NavigationManager.NotFound();
```

また、`NotFound.razor`を編集することで404ページを完全にカスタマイズすることもできます：

```razor
@page "/not-found"
<h1>404: ページが見つかりません</h1>
```

404 UIを中央集権化することで、すっきりと一貫性を保つことができます。

---

## BlazorでJavaScript Interopは何が新しいですか？

もう面倒なJSラッパーは不要です：.NET 10では、C#から直接JavaScriptプロパティを取得/設定できます。

```csharp
// NuGet: Microsoft.JSInterop
using Microsoft.JSInterop;

await JS.SetValueAsync("document.title", "New Title");
string currentTitle = await JS.GetValueAsync<string>("document.title");
```

また、JSオブジェクト参照を簡単に作成および管理することもできます：

```csharp
var jsObj = await JS.InvokeAsync<IJSObjectReference>("SomeFactory.create");
await jsObj.DisposeAsync();
```

高度な統合パターンについては、[Iron Softwareの開発者ブログ](https://ironsoftware.com)を参照してください。

---

## .NET 10での主なQuickGridの改善点は何ですか？

Blazorのオープンソースグリッドは現在、以下をサポートしています：

- **条件付き行スタイリング：** 行データに基づいてCSSクラスを適用します。
- **プログラムによる列制御：** コードから直接列を非表示/表示にします。

例：

```razor
<QuickGrid Items="@accounts" RowClass="GetRowClass">
    <PropertyColumn Property="@(a => a.Name)" Title="Name" />
</QuickGrid>
@code {
    private string GetRowClass(Account a) => a.Balance < 0 ? "negative" : "";
}
```

ドキュメントテーブルとPDFグリッドについての詳細は、[Pdf Headers Footers Csharp](pdf-headers-footers-csharp.md)を参照してください。

---

## NavLinkのアクティブ状態処理はどのように改善されましたか？

NavLinkは現在、クエリ文字列やフラグメントがあっても正確にルートをマッチさせます：

```razor
<NavLink href="/dashboard" Match="NavLinkMatch.All">Dashboard</NavLink>
```

これは、`/dashboard`、`/dashboard?tab=x`、および`/dashboard#section`がすべてNavLinkをアクティブに設定することを意味し、長年の悩みを解決します。

---

## Blazorの再接続とプリローディング機能に何が新しいですか？

新しい`ReconnectModal`コンポーネントはカスタマイズ可能でCSPフレンドリーです。インラインスタイルやスクリプトは必要ありません。接続の問題中にUXを向上させるために、簡単にスタイルを変更したり拡張したりできます。

Blazor WebAssemblyは、より速い起動のために改善されたプリローディングサポートも追加しています：

```html
<link rel="preload" href="_framework/blazor.webassembly.js" as="script" />
```

---

## プロジェクトを.NET 10 Blazorにアップグレードするにはどうすればよいですか？

1. **.csprojファイルを更新します：**  
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```
2. **.NET 10 SDKをインストールします：**  
   ```bash
   dotnet --version
   ```
3. **復元してビルドします：**  
   ```bash
   dotnet restore
   dotnet build
   ```
4. **JS interopやサードパーティライブラリを使用している場合は、アプリを特にテストします。**  
5. **依存関係を更新します：** [IronPDF](https://ironpdf.com)やグリッドなどのツールは新しいバージョンが必要になるかもしれません。

クロスプラットフォームのアップグレードのヒントについては、[Avalonia Vs Maui Dotnet 10](avalonia-vs-maui-dotnet-10.md)を参照してください。

---

## アップグレード時に注意すべき落とし穴は何ですか？

- **ビルドエラーが発生した場合は？** すべてのNuGet依存関係を更新してください。
- **永続的状態が復元されない場合は？** 関連するすべてのプロパティに`[PersistentState]`があることを確認してください。
- **パスキーサインインが機能しない場合は？** HTTPSを使用し、`.AddPasskeys()`を再確認してください。
- **カスタム404が表示されない場合は？** `NotFound.razor`のルートとロジックを見直してください。
- **JS interopが失敗する場合は？** プロパティパスとJSランタイムの可用性を検証してください。

PDF内のフォント管理については、[Manage Fonts Pdf Csharp](manage-fonts-pdf-csharp.md)を参照してください。

---

## .NET 10、C# 14、およびBlazorについてもっと学ぶには？

より深いチュートリアルとエコシステムの洞察については、[Dotnet 10 Csharp 14 Tutorial](dotnet-10-csharp-14-tutorial.md)を見るか、[Iron Software](https://ironsoftware.com)および[IronPDF](https://ironpdf.com)を訪問してください。

---

*[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)はIron SoftwareのCTOで、.NETドキュメント処理ツールを構築する50人以上のエンジニアのチームを率いています。彼はもともとIronPDFを作成しました。*

---

## 関連リソース

### 📚 チュートリアル & ガイド
- **[HTML to PDF Guide](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[Best PDF Libraries 2025](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[Beginner Tutorial](../csharp-pdf-tutorial-beginners.md)** — 最初のPDFを5分で
- **[Decision Flowchart](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[Merge & Split PDFs](../merge-split-pdf-csharp.md)** — ドキュメントの結合
- **[Digital Signatures](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[Extract Text](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[Fill PDF Forms](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF Generation](../blazor-pdf-generation.md)** — Blazorサポート
- **[Cross-Platform Deployment](../cross-platform-pdf-dotnet.md)** — Docker、Linux、Cloud

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*「[Awesome .NET PDF Libraries 2025](../README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を提供。*

---

**Jacob Mellor** — IronPDFの創設者、Iron SoftwareのCTO。41年間のコーディング、50人以上のチーム、4100万以上のNuGetダウンロード。土木工学の学位を持つソフトウェアパイオニア。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/) | [GitHub](https://github.com/jacob-mellor)