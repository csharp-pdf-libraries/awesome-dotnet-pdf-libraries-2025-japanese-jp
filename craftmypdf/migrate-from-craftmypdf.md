---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [craftmypdf/migrate-from-craftmypdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/craftmypdf/migrate-from-craftmypdf.md)
🇯🇵 **日本語:** [craftmypdf/migrate-from-craftmypdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/craftmypdf/migrate-from-craftmypdf.md)

---

# 移行ガイド: CraftMyPDF → IronPDF

## 目次
1. [IronPDFへの移行理由](#ironpdfへの移行理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート（5分）](#クイックスタート5分)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティングガイド](#トラブルシューティングガイド)
9. [移行チェックリスト](#移行チェックリスト)
10. [追加リソース](#追加リソース)

---

## IronPDFへの移行理由

### クラウドベースのPDF APIの問題点

CraftMyPDFのような多くのクラウドPDFサービスは、多くの本番環境に不適切な根本的な問題を引き起こします：

1. **データがシステムを離れる**：すべてのHTMLテンプレートとJSONデータペイロードがCraftMyPDFのサーバーに送信されます。請求書、契約書、医療記録、またはあらゆる機密ビジネスデータについて、これはコンプライアンスリスク（HIPAA、GDPR、SOC2）を生み出します。

2. **ネットワークの遅延と信頼性**：各PDF生成にはクラウドへの往復が必要です。PDFごとに1.5-30秒の平均遅延（CraftMyPDFの独自のドキュメントによる）対ローカル生成のミリ秒。

3. **継続的なコスト**：PDFごとの課金（1クレジット/ PDF）が積み上がります。月に10,000 PDF = かなりの繰り返しコスト対一回限りのライセンス。

4. **印刷最適化、画面精度ではない**：クラウドPDF APIは通常、印刷用に最適化されており、「インク」を節約するために背景を削減し、色を単純化します。出力は画面上のHTMLのようには見えません。

5. **テンプレートのロックイン**：CraftMyPDFは独自のドラッグアンドドロップエディターを必要とします。標準のHTML/CSSを自由に使用することはできません。

### IronPDFの主な利点

| 項目 | CraftMyPDF | IronPDF |
|--------|------------|---------|
| **データの場所** | クラウド（データがシステムを離れる） | オンプレミス（データは離れない） |
| **遅延** | PDFごとに1.5-30秒 | ミリ秒 |
| **価格** | PDFごとのサブスクリプション（$0.01-0.05/PDF） | 一回限りの永久ライセンス |
| **テンプレートシステム** | 独自のドラッグアンドドロップのみ | 任意のHTML/CSS/JavaScript |
| **出力品質** | 印刷最適化（インク削減） | ピクセルパーフェクトな画面レンダリング |
| **API依存性** | インターネットが必要 | オフラインで動作 |
| **レンダリングエンジン** | クラウドレンダラー | ローカルChromium（Chrome品質） |
| **コンプライアンス** | データが組織を離れる | SOC2/HIPAAフレンドリー |

### 機能比較

| 機能 | CraftMyPDF | IronPDF |
|---------|------------|---------|
| HTMLからPDFへ | APIテンプレート経由 | ✅ ネイティブ |
| URLからPDFへ | API経由 | ✅ ネイティブ |
| カスタムテンプレート | 独自エディターのみ | ✅ 任意のHTML |
| CSS3サポート | 限定 | ✅ 完全 |
| JavaScriptレンダリング | 限定 | ✅ 完全 |
| PDFのマージ/分割 | API経由 | ✅ ネイティブ |
| フォーム入力 | API経由 | ✅ ネイティブ |
| デジタル署名 | API経由 | ✅ ネイティブ |
| ウォーターマーク | API経由 | ✅ ネイティブ |
| オフラインで動作 | ❌ | ✅ |
| セルフホスティング | ❌ | ✅ |

### 実際のコスト比較

**CraftMyPDFのコスト（月額）:**
- ライトプラン: $19/月で1,200 PDF
- プロフェッショナル: $49/月で5,000 PDF
- エンタープライズ: $99/月で15,000 PDF
- 大規模利用: 100,000 PDF = 約$500-600/月

**IronPDFのコスト（一回限り）:**
- ライトライセンス: $749（1開発者、1プロジェクト）
- プロフェッショナル: $1,499（無制限のプロジェクト）
- 一回の支払い後は無制限のPDF

**損益分岐点: ~2-3ヶ月（ボリュームによる）**

---

## 開始する前に

### 前提条件

- **.NETバージョン**: IronPDFは.NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9をサポート
- **NuGetアクセス**: nuget.orgからパッケージをインストールできることを確認
- **ライセンスキー**: [IronPDFウェブサイト](https://ironpdf.com/)から取得（無料トライアル利用可能）

### すべてのCraftMyPDF参照を見つける

```bash
# コードベース内のすべてのCraftMyPDFの使用箇所を検索
grep -r "CraftMyPdf\|craftmypdf\|api.craftmypdf.com" --include="*.cs" .
grep -r "X-API-KEY" --include="*.cs" .

# APIキー参照を検索
grep -r "your-api-key\|template-id\|template_id" --include="*.cs" .

# NuGetパッケージ参照を検索
grep -r "CraftMyPdf\|RestSharp" --include="*.csproj" .
```

### 変更点の概要

| 変更 | CraftMyPDF | IronPDF | 影響 |
|--------|------------|---------|--------|
| **アーキテクチャ** | クラウドREST API | ローカル.NETライブラリ | HTTP呼び出しを削除 |
| **テンプレート** | 独自エディター | 標準HTML | テンプレートをHTMLに変換 |
| **APIキー** | すべての呼び出しに必要 | 起動時のライセンス | APIキー処理を削除 |
| **非同期パターン** | 必須（HTTP） | オプション | 好みでawaitを削除 |
| **画像処理** | API経由でアップロード | HTMLに埋め込み | インライン画像 |
| **エラー処理** | HTTPステータスコード | 例外 | try/catchパターンを変更 |
| **データバインディング** | JSONテンプレート | 文字列補間 | データバインディングを簡素化 |

---

## クイックスタート（5分）

### ステップ1: NuGetパッケージを更新

```bash
# CraftMyPDF（SDKを使用している場合）とRestSharpを削除
dotnet remove package RestSharp
# CraftMyPDFパッケージは存在しない - RESTのみ

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: Usingステートメントを更新

```csharp
// 以前
using RestSharp;
using System.IO;

// 以後
using IronPdf;
```

### ステップ3: ライセンスキーを適用（起動時に一度）

```csharp
// アプリケーションの起動時（Program.csまたはGlobal.asax）に追加
// これですべてのX-API-KEYヘッダーが置き換わる
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ4: 基本的なコード移行

**以前（CraftMyPDF）:**
```csharp
using RestSharp;
using System.IO;

var client = new RestClient("https://api.craftmypdf.com/v1/create");
var request = new RestRequest(Method.POST);
request.AddHeader("X-API-KEY", "your-api-key");
request.AddJsonBody(new
{
    template_id = "your-template-id",
    data = new { name = "John Doe", amount = "$1,000" }
});

var response = await client.ExecuteAsync(request);
if (response.IsSuccessful)
{
    File.WriteAllBytes("output.pdf", response.RawBytes);
}
```

**以後（IronPDF）:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

var html = @"
    <h1>Invoice</h1>
    <p>Name: John Doe</p>
    <p>Amount: $1,000</p>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
// APIキーなし、HTTP呼び出しなし、await不要！
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| CraftMyPDFパターン | IronPDF名前空間 | メモ |
|--------------------|-------------------|-------|
| `RestSharp.RestClient` | `IronPdf` | HTTPクライアント不要 |
| `api.craftmypdf.com/v1/*` | `IronPdf.*` | すべての操作がローカルで行われる |
| `X-API-KEY`ヘッダー | `License.LicenseKey` | 起動時に一度設定 |
| JSONデータバインディング | C#文字列補間 | 標準の.NETパターン |

### APIエンドポイントマッピング

| CraftMyPDFエンドポイント | IronPDFメソッド | メモ |
|--------------------|----------------|-------|
| `POST /v1/create` | `renderer.RenderHtmlAsPdf(html)` | テンプレート → HTML |
| `POST /v1/create` (URLから) | `renderer.RenderUrlAsPdf(url)` | 直接URLレンダリング |
| `POST /v1/merge` | `PdfDocument.Merge(pdfs)` | ローカルでマージ |
| `POST /v1/add-watermark` | `pdf.ApplyWatermark(html)` | HTMLベースのウォーターマーク |
| `POST /v1/pdf-to-image` | `pdf.RasterizeToImageFiles()` | ローカル変換 |
| テンプレートエディター | 標準HTML/CSS | 任意のエディターを使用 |
| Webhookコールバック | 不要 | デフォルトで同期 |

### 設定マッピング

| CraftMyPDFオプション | IronPDF相当 | メモ |
|-------------------|-------------------|-------|
| `template_id` | HTML文字列 | 自分のHTMLを使用 |
| `data` JSON | C#補間 | `$"Hello {name}"` |
| `page_size: "A4"` | `PaperSize = PdfPaperSize.A4` | |
| `orientation: "landscape"` | `PaperOrientation = Landscape` | |
| `margin_top: 20` | `MarginTop = 20` | ミリメートル単位 |
| `header` | `HtmlHeader` | 完全なHTMLサポート |
| `footer` | `HtmlFooter` | 完全なHTMLサポート |
| `output: "pdf"` | デフォルト | 常にPDF |
| `async: true` | `*Async()`メソッドを使用 | オプション |

### テンプレート機能マッピング

| CraftMyPDF機能 | IronPDF相当 |
|--------------------|-------------------|
| `{%name%}` テンプレート変数 | `$"{name}"` C#補間 |
| `{%loop items%}` | LINQ + `string.Join()` |
| `{%if condition%}` | C#三項演算子またはif文 |
| ドラッグアンドドロップレイアウト | CSS Flexbox/Grid |
| 画像コンポーネント | `<img src="...">` |
| QRコードコンポーネント | QRライブラリ + `<img>`を使用 |
| バーコードコンポーネント | バーコードライブラリ + `<img>`を使用 |
| チャートコンポーネント | チャートライブラリまたはSVGを使用 |

---

## コード移行例

### 例1: シンプルなHTMLからPDFへ

**以前（CraftMyPDF）:**
```csharp
using RestSharp;
using System.IO;

class Program
{
    static async Task Main()
    {
        var client = new RestClient("https://api.craftmypdf.com/v1/create");
        var request = new RestRequest(Method.POST);
        request.AddHeader("X-API-KEY", "your-api-key");
        request.AddJsonBody(new
        {
            template_id = "simple-template-id",
            data = new
            {
                title = "Hello World",
                body = "This is a PDF from CraftMyPDF"
            }
        });

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            File.WriteAllBytes("output.pdf", response.RawBytes);
            Console.WriteLine("PDF created!");
        }
        else
        {
            Console.WriteLine($"Error: {response.ErrorMessage}");
        }
    }
}
```

**以後（IronPDF）:**
```csharp
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();

        var html = @"
            <html>
            <body>
                <h1>Hello World</h1>
                <p>This is a PDF from IronPDF</p>
            </body>
            </html>";

        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("output.pdf");
        Console.WriteLine("PDF created!");
        // HTTP問題に関するエラー処理は不要！
    }
}
```

### 例2: URLからPDFへ