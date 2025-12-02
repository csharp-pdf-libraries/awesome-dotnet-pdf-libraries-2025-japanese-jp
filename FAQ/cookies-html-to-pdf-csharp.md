# C#でIronPDFを使用して認証済みHTMLからPDFへの変換でクッキーをどう扱うか？

C#で認証が必要なWebページをPDFに変換することは、強力なワークフローです。しかし、期待しているプライベートダッシュボードやパーソナライズされたデータの代わりにPDFでログイン画面や一般的なコンテンツが表示される場合、クッキーが欠けている可能性があります。このFAQでは、IronPDFで安全かつ確実にクッキーを送信し、複雑な認証を処理し、一般的な問題をトラブルシューティングし、堅牢なHTMLからPDFへの自動化のためのエッジケースをマスターする方法について説明します。

---

## WebページをPDFに変換する際、クッキーが重要な理由は？

IronPDF（または類似のツール）を使用してライブURLや[HTMLをPDFに変換](https://ironpdf.com/how-to/pixel-perfect-html-to-pdf/)する場合、それは新しく、クリーンなブラウザを起動するようなものです。そのプロセスに正しいクッキーがなければ、保護されたコンテンツやパーソナライズされたコンテンツにアクセスできません。これは、ログインされていない空のChromeプロファイルと同じです。

### クッキーが不足しているとPDF生成でどのような問題が発生しますか？

正しいクッキーを提供しない場合：

- **認証されたページ**はログイン画面にリダイレクトされます。
- **ユーザーダッシュボード**はサインインプロンプトのみを表示し、パーソナライズされた情報は表示されません。
- **CSRF保護フォーム**はエラーを表示したり、不完全にロードされることがあります。
- **ログインユーザー向けに意図された動的コンテンツ**は一般的なものとして表示されます。
- **APIやリソースエンドポイント**は「401 Unauthorized」や空のデータを返すことがあります。

基本的に、セッション、認証、またはカスタマイズのためにクッキーを使用しているサイトでは、PDFをレンダリングする際にそれらのクッキーを渡す必要があります。

HTMLからPDFへのワークフローについて詳しくは、[Convert Html To Pdf Csharp](convert-html-to-pdf-csharp.md)をご覧ください。

---

## C#でIronPDFにカスタムクッキーを渡すにはどうすればいいですか？

IronPDFは、レンダリングのためのカスタムクッキーを簡単に提供できるようにしています。クッキー名と値で辞書を作成し、IronPDFが各リクエストにそれらを追加します。まさにブラウザが行うように。

```csharp
using IronPdf; // Install-Package IronPdf

var pdfGenerator = new ChromePdfRenderer();

// ここで認証/セッションクッキーを定義します
pdfGenerator.RenderingOptions.CustomCookies = new Dictionary<string, string>
{
    { ".AspNetCore.Session", "example_session_token" },
    { "auth_token", "user_jwt_here" }
};

var resultPdf = pdfGenerator.RenderUrlAsPdf("https://yourdomain.com/dashboard");
resultPdf.SaveAs("dashboard.pdf");
```

**ヒント：**  
レンダリング中に行われるすべてのHTTPリクエスト（画像、CSS、AJAXを含む）にこれらのクッキーが含まれます。より高度なHTMLおよびPDFシナリオについては、[Advanced Html To Pdf Csharp](advanced-html-to-pdf-csharp.md)をチェックしてください。

---

## 複数のクッキーを送信する必要がある場合、順序は重要ですか？

心配無用です。必要なだけ多くのクッキーを追加できます。辞書内のアイテムの順序は関係ありません。重要なのは、アプリケーションが期待するすべてのクッキーを含めることです。

```csharp
using IronPdf; // Install-Package IronPdf

var pdfRenderer = new ChromePdfRenderer();

pdfRenderer.RenderingOptions.CustomCookies = new Dictionary<string, string>
{
    { "sessionId", "sess_ABC123" },
    { "csrf", "token_value" },
    { "theme", "dark" }
};

var pdf = pdfRenderer.RenderUrlAsPdf("https://example.com/profile");
pdf.SaveAs("profile.pdf");
```

クッキーは認証からUIの好みまで何でもカバーできます。名前を正確に記述し、現在の値を提供してください。

---

## ASP.NETまたはASP.NET Coreの認証クッキーをどう扱えばいいですか？

ASP.NETアプリは通常、`.AspNetCore.Identity.Application`、`.AspNetCore.Cookies`、`ASP.NET_SessionId`のような名前のクッキーを使用します。これらは認証済みWebアプリにとって重要です。

### 正しいクッキーを見つけて使用するにはどうすればいいですか？

使用するクッキーを特定するには、ブラウザでWebアプリにログインし、DevTools（ネットワーク > クッキー）を開いて、関連するクッキー名と値をコピーします。

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.CustomCookies = new Dictionary<string, string>
{
    { ".AspNetCore.Identity.Application", "real_auth_token" },
    { "ASP.NET_SessionId", "session_id" }
};

var pdf = renderer.RenderUrlAsPdf("https://myapp.com/admin/reports");
pdf.SaveAs("admin-report.pdf");
```

ASP.NETアプリ内でPDFを生成している場合は、次のセクションでクッキーの自動転送について説明します。

異なる認証フローを扱う方法については、[Advanced Html To Pdf Csharp](advanced-html-to-pdf-csharp.md)をご覧ください。

---

## ASP.NET Coreからユーザーのクッキーを自動的に使用できますか？

もちろんです！ASP.NET Coreコントローラ内にいて、PDFが現在のユーザーのセッションを反映させたい場合は、受信リクエストからクッキーをコピーします。

```csharp
using IronPdf;
using Microsoft.AspNetCore.Http;
// Install-Package IronPdf

public class PdfController : Controller
{
    public IActionResult DownloadDashboard()
    {
        var pdfRenderer = new ChromePdfRenderer();

        var cookiesToSend = new Dictionary<string, string>();
        foreach (var cookie in Request.Cookies)
        {
            cookiesToSend[cookie.Key] = cookie.Value;
        }

        pdfRenderer.RenderingOptions.CustomCookies = cookiesToSend;
        var pdf = pdfRenderer.RenderUrlAsPdf($"{Request.Scheme}://{Request.Host}/dashboard");
        return File(pdf.BinaryData, "application/pdf", "dashboard.pdf");
    }
}
```

これにより、PDFがログインしているユーザーのビューと一致することが保証されます。これは、パーソナライズされたダッシュボードやマルチユーザーレポートに非常に便利です。

---

## IronPDFでのリクエストコンテキストがクッキー処理にどのように影響しますか？

IronPDFは「リクエストコンテキスト」を使用して、ブラウザセッションとクッキーがレンダリング間でどのように持続するかを制御します。

- **Isolated**（デフォルト）：毎回新しいブラウザ状態から始まります（以前のクッキーはありません）。
- **Global**：複数のレンダリング間でクッキー/セッションデータを共有します（ログインしてからレンダリングするなど、複数ステップのフローに最適）。
- **Auto**：IronPDFに決定させます（通常はIsolatedがデフォルト）。

### どのコンテキストをいつ選択すべきですか？

- 複数のレンダリングをまたいでログイン状態やセッション状態を維持する必要がある場合は、**Global**を使用します。
- クッキーが持続しない単一のステートレスレンダリングには、**Isolated**を使用します。

```csharp
using IronPdf; // Install-Package IronPdf

var pdfRenderer = new ChromePdfRenderer();
pdfRenderer.RenderingOptions.RequestContext = RequestContexts.Global; // または Isolated
```

複雑なワークフロー（レポートをレンダリングする前にログインするなど）には、Globalコンテキストが必須です。これについては、[Advanced Html To Pdf Csharp](advanced-html-to-pdf-csharp.md)で詳しく説明しています。

---

## PDFレンダリング前に自動的にログインするにはどうすればいいですか？

最初にログインページをレンダリングして（必要に応じてフォーム送信を自動化した後）、同じセッションで保護されたコンテンツをレンダリングすることで、ログインシーケンスをスクリプト化できます。

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.RequestContext = RequestContexts.Global;

renderer.RenderUrlAsPdf("https://example.com/login"); // 必要に応じてフォーム送信を自動化

var pdf = renderer.RenderUrlAsPdf("https://example.com/protected/dashboard");
pdf.SaveAs("protected-dashboard.pdf");
```

これにより、IronPDFが認証クッキー、リダイレクト、セッション管理を処理できます。ワークフローとセッション状態を制御するためのさらなるヒントについては、[Convert Html To Pdf Csharp](convert-html-to-pdf-csharp.md)をチェックしてください。

---

## 認証済みPDF変換のためにJWTベアラートークンを渡すにはどうすればいいですか？

多くのAPIやSPAでは、クッキーやHTTPヘッダーでJWTを認証に使用しています。

### JWTがクッキーにある場合はどうすればいいですか？

クッキー辞書に含めるだけです：

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.CustomCookies = new Dictionary<string, string>
{
    { "jwt", "eyJhbGciOiJIUzI1NiIsIn..." },
    { "refresh_token", "refresh_token_here" }
};

var pdf = renderer.RenderUrlAsPdf("https://api.example.com/reports/view");
pdf.SaveAs("jwt-cookie-report.pdf");
```

### JWTがHTTPヘッダーで必要な場合はどうすればいいですか？

`CustomHttpHeaders`オプションを使用します：

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.CustomHttpHeaders = a new Dictionary<string, string>
{
    { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsIn..." }
};

var pdf = renderer.RenderUrlAsPdf("https://api.example.com/reports/view");
pdf.SaveAs("jwt-header-report.pdf");
```

**注意：**  
JWTは頻繁に期限切れになります。常に有効で現在のトークンを使用しないと、未承認のエラーが発生します。

PDFのセキュリティと認証について詳しくは、[Understanding Pdf Security Net 10 Ironpdf](understanding-pdf-security-net-10-ironpdf.md)をご覧ください。

---

## PDFがコンテンツの代わりにログインページを表示する場合、どうデバッグすればいいですか？

PDFが意図した認証済みコンテンツを表示していない場合、デバッグが不可欠です。

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.RenderDelay = 3000;

renderer.RenderingOptions.CustomCookies = new Dictionary<string, string>
{
    { "debug_session", "test_value" }
};

var pdf = renderer.RenderUrlAsPdf("https://secure.example.com/protected");

// PDFテキストを抽出して検査
var text = pdf.ExtractAllText();
Console.WriteLine(text.Substring(0, Math.Min(500, text.Length)));
```

「ログインしてください」などの手がかりを探します。ログインプロンプトが表示される場合は、以下をダブルチェックしてください：

- クッキー名と値（タイプミスや現在のものか？）
- クッキーのドメイン/パス（レンダリングされるURLと一致する必要があります）
- クッキーが期限切れまたは不正形式でないか
- Secure/HttpOnly設定

アプリが相対URLに依存している場合は、[Base Url Html To Pdf Csharp](base-url-html-to-pdf-csharp.md)も参照してください。

---

## IronPDFでSecureおよびHttpOnlyクッキーをどう扱うべきですか？

「Secure」クッキーはHTTPS経由でのみ送信され、「HttpOnly」クッキーはクライアントサイドのスクリプトからアクセスできません。IronPDFはそれらを以下のように扱います：

- **Secureクッキー**：HTTPSリクエストのみに含まれます。URLが`https://`で始まることを確認してください。
- **HttpOnlyクッキー**：IronPDFでプログラム的に設定できますが、JavaScript経由で取得することはできません。サーバーサイドのコードやブラウザの開発ツールを使用してください。

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = a new ChromePdfRenderer();

renderer.RenderingOptions.CustomCookies = new Dictionary<string, string>
{
    { "secure_session", "secure_val" },     // HTTPS専用
    { "httponly_auth", "httponly_val" }     // コードで設定可能
};

var pdf = renderer.RenderUrlAsPdf("https://secure.example.com/secret");
pdf.SaveAs("secure-content.pdf");
```

セキュリテ