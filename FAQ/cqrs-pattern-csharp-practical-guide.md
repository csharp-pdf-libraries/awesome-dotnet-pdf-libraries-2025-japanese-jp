---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/cqrs-pattern-csharp-practical-guide.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/cqrs-pattern-csharp-practical-guide.md)
🇯🇵 **日本語:** [FAQ/cqrs-pattern-csharp-practical-guide.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/cqrs-pattern-csharp-practical-guide.md)

---

# 実際のドキュメントおよびPDFワークフローでCQRSとMediatRをC#でどのように実装するか？

CQRS（Command Query Responsibility Segregation）は、特にアプリのレポーティング、ドキュメント管理、またはPDF生成の要件が増加し始めたときに、読み取り操作と書き込み操作を分離するための強力なアーキテクチャパターンです。このFAQでは、CQRSをC#でMediatRを使用して実際にどのように実装するか、それがどこで輝くのか（そしてどこでそうでないか）、PDF作成などの実用的なシナリオにどのように適用するかを学びます。実践的なコードを通して歩き、ベストプラクティスを共有し、重要な落とし穴に注意を促します。

関連する深掘りについては、[C#でPDFを作成する方法は？](create-pdf-csharp-complete-guide.md)、[C#でPDF DOMにアクセスする方法は？](access-pdf-dom-object-csharp.md)、および[C#でPDFに添付ファイルを追加する方法は？](add-attachments-pdf-csharp.md)を参照してください。

---

## C#プロジェクトでCQRSをいつ使用すべきですか？その実際の利点は何ですか？

CQRSは、アプリケーションが読み取りと書き込みに対して重いまたは複雑な操作を異なる方法で処理する必要がある場合に最適です。たとえば、複雑なデータ構造をロードするためにレポーティングダッシュボードが遅い場合、またはPDFエクスポートプロセスがAPIロジックと密接に結合している場合、CQRSはこれらのパスを独立してスケールアップし、合理化するのに役立ちます。

**CQRSを使用する場合:**
- 読み取り（例：ダッシュボード、レポート）と書き込み（例：データ入力、PDF生成）を別々に最適化する必要がある場合。
- 書き込みロジックが複雑である場合—複数ステップのワークフロー、検証、またはイベント処理を考えてください。
- 読み取り要件が非正規化されたモデルや高度に最適化されたモデルを含む場合。

**CQRSをスキップする場合:**
- 読み取りと書き込みの両方に同様のロジックがあるシンプルなCRUDアプリケーションの場合。
- スケーリングが問題ではないトラフィックの少ないアプリの場合。
- メッセージベースまたはイベント駆動の考え方にチームが慣れていないプロジェクトの場合—学習曲線があります。

CQRSの適合性に関するさらに多くのヒントを知りたい場合は、[完全なCQRSパターンガイド](https://ironpdf.com)を参照してください（イベントソーシング、MediatRパイプライン、およびより高度なシナリオをカバーしています）。

---

## C#でMediatRを使用したCQRSはどのように機能しますか？ハイレベルのアプローチは何ですか？

MediatRを使用すると、各コマンド（書き込み）とクエリ（読み取り）を個別のメッセージとして扱います。ハンドラクラスがこれらのメッセージを処理し、コントローラを薄く保ち、ビジネスロジックを整理します。

**コマンド**は状態を変更するアクションを表します（PDFを生成するか、レコードを作成するなど）。
**クエリ**はデータを取得します（請求書のリストを取得するなど）。

たとえば：
- 「請求書PDFを生成」リクエストはコマンドです—ビジネスロジックをトリガーし、新しいファイルを生成します。
- 「請求書をリストする」クエリは読み取り操作であり、表示用のDTOを返します。

この分離により、それぞれを独立してスケールアップし、コードベースをはるかに保守しやすくすることができます。

---

## ASP.NET CoreプロジェクトでCQRSとMediatRを設定するにはどうすればよいですか？

MediatR NuGetパッケージをインストールする必要があります。PDFワークフローの場合、IronPDFも必要になります。

```bash
dotnet add package MediatR
dotnet add package IronPdf // 高度なPDF生成用
```

**MediatRとサービスを登録します：**

```csharp
using IronPdf; // NuGet: IronPdf
using MediatR;

// Program.csまたは同等のセットアップファイル内
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 最高のパフォーマンスのためにChromePdfRendererをシングルトンとして登録
builder.Services.AddSingleton<ChromePdfRenderer>();

// リポジトリを登録
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

var app = builder.Build();
```

### PDFレンダリングエンジンをシングルトンとして使用する理由は？

`ChromePdfRenderer`などのPDFエンジンは初期化にリソースを多く消費します。シングルトンとして登録すると、起動コストは一度だけで済み、並行リクエストに対してスレッドセーフになります。

### CQRSでどのようなモデルを使用すべきですか？

- **ドメインモデル：** コマンド（書き込み）に使用され、検証とビジネスロジックを含みます。
- **DTO/ビューモデル：** クエリ（読み取り）に使用され、フラット化されてシリアライズの準備ができています。

高度なPDF機能については、[C#でPDF DOMにアクセスする方法は？](access-pdf-dom-object-csharp.md)および[C#でWebGLサイトをPDFにレンダリングする方法は？](render-webgl-pdf-csharp.md)をチェックしてください。

---

## CQRSとMediatRでコマンドをどのように書き、処理しますか？

典型的なコマンドとして請求書の作成を通して見てみましょう。

**コマンドを定義します：**

```csharp
using MediatR;

public record CreateInvoiceCommand(string Customer, decimal Amount) : IRequest<int>;
```

ここで、`record`はそれを不変に保ち、`IRequest<int>`はハンドラが新しい請求書IDを返すことを意味します。

**ハンドラを実装します：**

```csharp
using MediatR;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _repo;

    public CreateInvoiceHandler(IInvoiceRepository repo) => _repo = repo;

    public async Task<int> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = new Invoice
        {
            Customer = command.Customer,
            Amount = command.Amount,
            CreatedUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(invoice, cancellationToken);
        return invoice.Id;
    }
}
```

**コントローラでディスパッチします：**

```csharp
using Microsoft.AspNetCore.Mvc;
using MediatR;

[ApiController]
[Route("invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    // ... その他のエンドポイント
}
```

コントローラはビジネスロジックを含まず、コマンドをディスパッチするだけでスリムなままであることに注意してください。

---

## CQRSで高速な読み取りのためのクエリをどのように実装しますか？

クエリは速度のために最適化され、必要なもののみを返すべきです。ここに設定方法があります：

**クエリとDTOを定義します：**

```csharp
using MediatR;
using System.Collections.Generic;

public record GetInvoicesQuery(int Page = 1, int PageSize = 50) : IRequest<IReadOnlyList<InvoiceDto>>;

public record InvoiceDto(int Id, string Customer, decimal Amount, DateTime CreatedUtc);
```

**クエリハンドラを書きます：**

```csharp
using MediatR;

public class GetInvoicesHandler : IRequestHandler<GetInvoicesQuery, IReadOnlyList<InvoiceDto>>
{
    private readonly IInvoiceRepository _repo;

    public GetInvoicesHandler(IInvoiceRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<InvoiceDto>> Handle(GetInvoicesQuery query, CancellationToken cancellationToken)
    {
        var invoices = await _repo.GetPagedAsync(query.Page, query.PageSize, cancellationToken);

        return invoices.Select(i => new InvoiceDto(
            i.Id,
            i.Customer,
            i.Amount,
            i.CreatedUtc
        )).ToList();
    }
}
```

**コントローラエンドポイント：**

```csharp
[HttpGet]
public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int size = 50)
{
    var query = new GetInvoicesQuery(page, size);
    var invoices = await _mediator.Send(query);
    return Ok(invoices);
}
```

**クエリにDTOを使用する理由は？**
DTOはドメインロジックをクリーンに保ち、UIまたはAPIの消費者に出力をカスタマイズすることを可能にします。専用の読み取りストア（Elasticsearchなど）に切り替える場合は、ハンドラのみを更新します。

PDF出力をカスタマイズする方法については、[C#でPDFに添付ファイルを追加する方法は？](add-attachments-pdf-csharp.md)をチェックしてください。

---

## C#でPDF生成にCQRSとMediatRをどのように使用できますか？

PDF生成は、CQRSにとって典型的な「コマンド」の使用例です。IronPDFを使用して請求書データからPDFを作成しましょう：

**1. PDF生成コマンドを定義します：**

```csharp
using MediatR;

public record GenerateInvoicePdfCommand(int InvoiceId) : IRequest<byte[]>;
```

**2. IronPDFを使用してコマンドハンドラを書きます：**

```csharp
using MediatR;
using IronPdf; // NuGet: IronPdf

public class GenerateInvoicePdfHandler : IRequestHandler<GenerateInvoicePdfCommand, byte[]>
{
    private readonly IInvoiceRepository _repo;
    private readonly ChromePdfRenderer _renderer;

    public GenerateInvoicePdfHandler(IInvoiceRepository repo, ChromePdfRenderer renderer)
    {
        _repo = repo;
        _renderer = renderer;
    }

    public async Task<byte[]> Handle(GenerateInvoicePdfCommand command, CancellationToken cancellationToken)
    {
        var invoice = await _repo.GetByIdAsync(command.InvoiceId, cancellationToken) 
                      ?? throw new Exception("Invoice not found");

        var htmlContent = $@"
            <h1>Invoice #{invoice.Id}</h1>
            <p>Customer: {invoice.Customer}</p>
            <p>Amount: {invoice.Amount:C}</p>
            <p>Date: {invoice.CreatedUtc:yyyy-MM-dd}</p>";

        var pdfDoc = _renderer.RenderHtmlAsPdf(htmlContent);

        return pdfDoc.BinaryData;
    }
}
```

**3. コントローラから生成されたPDFを返します：**

```csharp
[HttpPost("{id}/pdf")]
public async Task<IActionResult> GeneratePdf(int id)
{
    var pdfBytes = await _mediator.Send(new GenerateInvoicePdfCommand(id));
    return File(pdfBytes, "application/pdf", $"invoice-{id}.pdf");
}
```

**ボーナス：生成されたPDFをBlobストレージに保存するにはどうすればよいですか？**

PDFをアップロードしてダウンロードURLを返すようにハンドラを拡張します：

```csharp
public class GenerateInvoicePdfHandler : IRequestHandler<GenerateInvoicePdfCommand, string>
{
    // ...依存関係

    public async Task<string> Handle(GenerateInvoicePdfCommand command, CancellationToken cancellationToken)
    {
        // ...上記のようにPDFを生成

        var fileName = $"invoices/invoice-{invoice.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var url = await _storage.UploadAsync(fileName, pdfDoc.BinaryData, cancellationToken);

        return url;
    }
}
```

完全なPDF作成オプションについては、[C#でPDFを作成する方法は？](create-pdf-csharp-complete-guide.md)を参照してください。

---

## C#での一般的なCQRSの落とし穴とそれを避ける方法は？

**1. シンプルなシナリオの過剰なアーキテクチャ**

アプリが基本的なCRUDである場合、CQRSは不要な複雑さを追加する可能性があります。読み取りと書き込みを分割する価値がある場合にのみ行ってください。

**2. コマンドでの検証の欠如**

ビジネス検証はコントローラではなくハンドラに属します。再利用可能な検証には、MediatRパイプライン動作またはFluentValidationを使用してください。

**検証の例：**

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

**3. 太ったコントローラ、薄いハンドラ**

ビジネスロジックをコントローラに移動している場合、CQRSのポイント