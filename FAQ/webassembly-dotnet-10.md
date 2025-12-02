# .NET 10 WebAssemblyを使用してブラウザでC#を実行する方法は？（Blazor、パフォーマンス、および実用的な例）

かつてブラウザで直接C#を実行することは不可能に思えましたが、.NET 10の改善されたWebAssembly (Wasm) サポートにより、現在では迅速かつ実用的で、本番環境に対応しています。豊かなインタラクティブUI、クライアント側のデータ処理、またはWebアプリに新機能を追加したい場合でも、.NET 10のBlazor WebAssemblyはゲームチェンジャーです。このFAQでは、WebAssemblyの理解からBlazor WASMアプリの構築、デプロイ、最適化、さらには実世界の経験に基づくプロのヒントやトラブルシューティングまで、基本を順を追って説明します。

---

## WebAssemblyとは何か、なぜ.NET開発者が気にかけるべきか？

WebAssembly (Wasm) は、C#、C++、Rustなどの言語をブラウザでほぼネイティブの速度で直接実行できる低レベルのバイナリ命令フォーマットです。JavaScriptに限定されることなく、C#コードをコンパイルしてクライアント側で実行できるようになりました。これにより、Chrome、Firefox、Edge、Safariなどの現代のブラウザ全体で実現します。

### これがC#および.NET開発者にどのような利点をもたらすか？

- C#の専門知識を活用して、高速でインタラクティブなWebアプリを構築します。
- 重い処理（例：データ解析、画像またはPDFの操作）をクライアントにオフロードして、サーバーの負荷を軽減します。
- オフラインシナリオとリアルタイムのブラウザ体験を可能にします。
- 新しいアプリタイプ：ゲーム、シミュレーション、暗号化、ドキュメント処理などをブラウザ内で完全に実行します。

**ブラウザ内のJavaScriptからC#コードを呼び出す例**

```csharp
// using Microsoft.JSInterop;
// Install-Package Microsoft.AspNetCore.Components.WebAssembly

public class MathHelper
{
    public static int Multiply(int a, int b) => a * b;
}

// From JavaScript:
// DotNet.invokeMethod('MyBlazorApp', 'Multiply', 4, 5);
// Returns: 20
```

ブラウザ内であなたのC#コードは即座に実行されます。サーバーへの往復はありません。

.NET 10の改善点の概要については、[What's New in Dotnet 10](whats-new-in-dotnet-10.md)を参照してください。

---

## .NET 10 WebAssemblyの新機能とその重要性は？

.NET 10は、Blazorアプリをこれまで以上に小さく、速く、そしてより能力が高いものにするWebAssemblyの大幅な強化を導入しています。

### .NET 10 Blazor WASMアプリはどれくらい速く、小さくなったか？

起動時間はほぼ30%改善され、ランタイムのダウンロードサイズは以前のバージョンと比較してほぼ30%軽量化されました。エンドユーザーは、特にモバイルや遅いネットワーク上で、より速いロード時間と低いメモリ使用量を体験するでしょう。

**起動時間の測定例**

```csharp
@code {
    protected override void OnInitialized()
    {
        var timestamp = DateTime.Now;
        Console.WriteLine($"Blazor WASM initialized at {timestamp:HH:mm:ss.fff}");
    }
}
```

実際のパフォーマンス向上を確認するために、このタイミングを以前のバージョンと比較します。

.NET 10のインストールの詳細については、[How To Install Dotnet 10](how-to-install-dotnet-10.md)を参照してください。

### Blazor WebAssemblyのAhead-of-Time (AOT) コンパイルとは何か？

AOTは、ブラウザでのジャストインタイム（JIT）解釈の必要性を排除し、事前に完全にWebAssemblyにC#コードをコンパイルします。これにより、CPU集中型のワークロードのランタイムが最大50%高速化されますが、ダウンロードサイズが約500KB増加し、ビルド時間がわずかに長くなります。

**AOTを有効にする方法は？**

プロジェクトの `.csproj` にこれを追加します：

```xml
<PropertyGroup>
  <RunAOTCompilation>true</RunAOTCompilation>
</PropertyGroup>
```

その後、以下で公開します：

```bash
dotnet publish -c Release
```

**AOTスピードアップのベンチマーク例**

```csharp
<button @onclick="RunMathBenchmark">Benchmark</button>
<p>Elapsed: @msElapsed ms</p>

@code {
    private long msElapsed;

    void RunMathBenchmark()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        double total = 0;
        for (int i = 0; i < 1_000_000; i++)
        {
            total += Math.Sqrt(i);
        }
        sw.Stop();
        msElapsed = sw.ElapsedMilliseconds;
    }
}
```

AOTビルドと非AOTビルドを比較して、顕著なパフォーマンスの違いを確認します。

---

## .NET 10でBlazor WebAssemblyを使用してマルチスレッディングは使用できますか？

はい！.NET 10は、SharedArrayBufferサポートとWebワーカーを使用してブラウザでの真の並列処理を可能にし、CPUバウンドのタスクを複数のコアで実行できます。

**並列計算の例**

```csharp
@page "/thread-demo"
<h3>マルチスレッド計算</h3>
<button @onclick="CalculateSum">実行</button>
<p>結果: @sum</p>

@code {
    long sum = 0;

    private async Task CalculateSum()
    {
        var numbers = Enumerable.Range(1, 10_000_000).ToArray();
        sum = await Task.Run(() =>
            numbers.AsParallel().Sum(x => (long)x)
        );
    }
}
```

ブラウザのDevToolsでマルチコア使用を確認できます。実際の並列処理が行われています。

注意：一部のブラウザ（特にiOS Safari）はスレッディングを完全にサポートしていない場合があるため、常にクロスプラットフォームでテストしてください。

---

## Blazor WebAssemblyアプリをゼロから構築するにはどうすればよいですか？

Blazor WASMを始めるのは簡単です。ここでは、最初のアプリをスキャフォールドして実行する方法を説明します：

```bash
dotnet new blazorwasm -n MyFirstWasmApp
cd MyFirstWasmApp
dotnet run
```

デフォルトのカウンターアプリを見るために[http://localhost:5000](http://localhost:5000)を開きます。

### 起動中に何が起こるか？

- ブラウザは.NET WebAssemblyランタイム、アプリのDLL、およびBlazorフレームワークをダウンロードします。
- .NETランタイムがブラウザで起動し、あなたのC#コードを実行します。
- すべてのUI更新はクライアント側で行われます。継続的なサーバー接続は必要ありません。

ASP.NETアプリケーションの詳細については、[How To Develop Aspnet Applications Dotnet 10](how-to-develop-aspnet-applications-dotnet-10.md)を参照してください。

---

## Blazor WASMでインタラクティブコンポーネントを構築するにはどうすればよいですか？

Blazorを使用すると、C#だけで動的でレスポンシブなUIを作成できます。ここでは、リアルタイムで更新されるストップウォッチコンポーネントの例を紹介します：

```razor
@page "/stopwatch"
<h1>ストップウォッチ</h1>
<p>経過時間: @seconds s</p>
<button @onclick="Start">開始</button>
<button @onclick="Stop">停止</button>
<button @onclick="Reset">リセット</button>

@code {
    private System.Timers.Timer? timer;
    private int seconds = 0;

    void Start()
    {
        if (timer == null)
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                seconds++;
                InvokeAsync(StateHasChanged);
            };
        }
        timer.Start();
    }

    void Stop() => timer?.Stop();
    void Reset()
    {
        Stop();
        seconds = 0;
    }
}
```

JavaScriptは不要です。タイマーロジックは純粋なC#でブラウザで実行されます。

---

## Blazor WebAssemblyでJavaScriptとC#を統合するにはどうすればよいですか？

BlazorのJavaScriptインタロップを使用すると、C#からJSを呼び出し、C#メソッドをJavaScriptに公開できます。

### C#からJavaScript関数を呼び出すにはどうすればよいですか？

例えば、クリップボードにテキストをコピーしたい場合：

**wwwroot/script.js**
```javascript
window.copyToClipboard = (text) => {
    navigator.clipboard.writeText(text);
};
```

**Blazorコンポーネント**
```razor
@inject IJSRuntime JS

<button @onclick="CopyToClipboard">コピー</button>

@code {
    private async Task CopyToClipboard()
    {
        await JS.InvokeVoidAsync("copyToClipboard", "Hello from Blazor!");
    }
}
```

### JavaScriptからC#メソッドを呼び出す方法は？

`[JSInvokable]`を使用して、JSからC#メソッドを呼び出せるようにします：

```csharp
using Microsoft.JSInterop;

public class JsInteropDemo
{
    [JSInvokable]
    public static string GetTime()
    {
        return DateTime.Now.ToLongTimeString();
    }
}
```

JSから：

```javascript
DotNet.invokeMethodAsync('MyFirstWasmApp', 'GetTime').then(console.log);
```

より高度な解析やテキスト抽出については、[Parse Pdf Extract Text Csharp](parse-pdf-extract-text-csharp.md)を参照してください。

---

## Blazor WebAssemblyでどのような実世界のアプリを構築できますか？

Blazor WASMは、以前はサーバーロジックを必要としていた多くのクライアント側アプリシナリオの扉を開きます。

### ブラウザで大きなCSVファイルを処理するには？

Blazor WASMは、遅いアップロードを避けるために、クライアント側で大きなCSVを解析できます：

```razor
@page "/csv-upload"
<h3>CSVアップローダー</h3>
<InputFile OnChange="HandleCsv" />
<p>処理された行数: @rowCount</p>
<p>時間: @msElapsed ms</p>

@code {
    int rowCount = 0;
    long msElapsed = 0;

    async Task HandleCsv(InputFileChangeEventArgs e)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        using var stream = e.File.OpenReadStream(10_000_000);
        using var reader = new StreamReader(stream);

        rowCount = 0;
        while (!reader.EndOfStream)
        {
            await reader.ReadLineAsync();
            rowCount++;
        }
        msElapsed = sw.ElapsedMilliseconds;
    }
}
```

### リアルタイムチャートやアニメーションを構築できますか？

はい、Blazorと小さなJSヘルパーを使用して、60 FPSでグラフィックスをレンダリングできます：

```razor
@page "/realtime-chart"
<canvas id="chartCanvas" width="800" height="400"></canvas>
<button @onclick="AnimateChart">アニメーション</button>

@inject IJSRuntime JS

@code {
    private bool animating = false;
    private double angle = 0;

    private async Task AnimateChart()
    {
        animating = true;
        while (animating)
        {
            await JS.InvokeVoidAsync("drawSineWave", angle);
            angle += 0.1;
            await Task.Delay(16); // 約60 FPS
        }
    }
}
```

**wwwroot/script.js**
```javascript
window.drawSineWave = function(phase) {
    const ctx = document.getElementById('chartCanvas').getContext('2d');
    ctx.clearRect(0, 0, 800, 400);
    ctx.beginPath();
    for (let x = 0; x < 800; x++) {
        const y = 200 + Math.sin((x + phase * 50) * 0.02) * 100;
        ctx.lineTo(x, y);
    }
    ctx.stroke();
};
```

### ブラウザでの画像処理は可能ですか？

絶対に可能です。画像をアップロードして、完全にクライアント側で処理します：

```razor
@page "/gray-image"
<InputFile OnChange="OnImageUpload" accept="image/*" />
@if (!string.IsNullOrEmpty(imageDataUrl))
{
    <img src="@imageDataUrl" alt="Filtered Image" />
}

@code {
    private string? imageDataUrl;

    async Task OnImageUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        var buffer = new byte[file.Size];
        await using var stream = file.OpenReadStream(5_000_000);
        await stream.ReadAsync(buffer);

        imageDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
    }
}
```

より高度なフィルターには、Wasmにコンパイルされた[SkiaSharp](https://github.com/mono/SkiaSharp)を試してみてください。

### ブラウザでのPDF生成や解析は可能ですか？

クライアント側のPDF生成は視野に入っています。[IronPDF](https://ironpdf.com)は既にサーバー側のC# PDFタスクのリーダーであり、Wasmサポートは積極的に探求中です。最新情報については[Iron Software](https://ironsoftware.com)をフォローしてください。C#でPDFからテキストを