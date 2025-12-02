---
**  (Japanese Translation)**

 **English:** [activepdf/migrate-from-activepdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/activepdf/migrate-from-activepdf.md)
 **:** [activepdf/migrate-from-activepdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/activepdf/migrate-from-activepdf.md)

---
# ActivePDFからIronPDFへの移行方法は？

> **移行の複雑さ：** 中
> **見積もり時間：** 典型的なプロジェクトで2-4時間
> **最終更新：** 2024年12月

## 目次

- [IronPDFへの移行理由](#ironpdfへの移行理由)
- [開始する前に](#開始する前に)
- [クイックスタート（5分）](#クイックスタート5分)
- [完全なAPIリファレンス](#完全なapiリファレンス)
- [コード移行例](#コード移行例)
- [高度なシナリオ](#高度なシナリオ)
- [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
- [トラブルシューティングガイド](#トラブルシューティングガイド)
- [移行チェックリスト](#移行チェックリスト)
- [追加リソース](#追加リソース)

---

## IronPDFへの移行理由

ActivePDF Toolkitは強力なPDF操作ライブラリでしたが、Foxitによる買収は、プラットフォームに依存している開発者にとって大きな不確実性を生み出しました。

### ActivePDFを離れる理由

**不確かな製品の未来**：FoxitによるActivePDFの買収は、製品の長期的な開発軌道について疑問を投げかけます。開発者は、ツールキットがレガシー製品になり、サポートとイノベーションが減少する可能性のリスクに直面しています。

**ライセンスの複雑さ**：買収後の移行期間は、ライセンスの不確実性をもたらしました。マシンロックされたライセンスは、特にクラウドやコンテナ化された環境での展開を複雑にする可能性があります。

**レガシーコードベースの懸念**：ActivePDFのアーキテクチャは、古い設計哲学を反映しており、現代の.NET慣習とは合致しない状態フルなツールキットパターン（`OpenOutputFile`、`CloseOutputFile`）を採用しています。

**インストールの複雑さ**：ActivePDFは、NuGetベースのパッケージ管理とは異なり、手動でのDLL参照やパス設定（`new Toolkit(@"C:\Program Files\ActivePDF\...")`）がしばしば必要です。

**限定的な現代の.NETサポート**：ActivePDFは.NETをサポートしていますが、その焦点は歴史的に.NET Frameworkにあり、.NET Coreや.NET 5+に対するサポートは変動しています。

### IronPDFが提供するもの

| ActivePDFの制限 | IronPDFの解決策 |
|----------------|----------------|
| 不確かな未来（Foxit買収） | 独立した会社、明確なロードマップ |
| マシンロックされたライセンス | コードベースのライセンスキー |
| 手動DLLインストール | シンプルなNuGetパッケージ |
| 状態フルなOpen/Closeパターン | フルーエント、機能的API |
| レガシー.NET Frameworkの焦点 | .NET Framework 4.6.2から.NET 9 |
| 複雑な設定パス | 設定不要 |

---

## 開始する前に

### 前提条件

- .NET Framework 4.6.2+ または .NET Core 3.1+ / .NET 5-9
- Visual Studio 2019+ または JetBrains Rider
- NuGetパッケージマネージャーへのアクセス
- IronPDFライセンスキー（無料試用版は [ironpdf.com](https://ironpdf.com) で利用可能）

### すべてのActivePDF参照を見つける

ソリューションディレクトリでこれらのコマンドを実行して、ActivePDFを使用しているすべてのファイルを見つけます：

```bash
grep -r "using ActivePDF" --include="*.cs" .
grep -r "using APToolkitNET" --include="*.cs" .
grep -r "APToolkitNET" --include="*.csproj" .
```

### 予想される重大な変更

| カテゴリ | ActivePDFの振る舞い | IronPDFの振る舞い | 移行アクション |
|----------|-------------------|------------------|------------------|
| オブジェクトモデル | 単一の`Toolkit`オブジェクト | `ChromePdfRenderer` + `PdfDocument` | 懸念を分離 |
| ファイル操作 | `OpenOutputFile()`/`CloseOutputFile()` | 直接の`SaveAs()` | open/close呼び出しを削除 |
| DLL参照 | 手動パス参照 | NuGetパッケージ | パス設定を削除 |
| ページ作成 | `NewPage()`メソッド | HTMLから自動 | ページ作成呼び出しを削除 |
| 戻り値 | 整数エラーコード | 例外 | try/catchを使用 |
| ライセンス | マシンロック | コードベースのキー | `License.LicenseKey`を追加 |

---

## クイックスタート（5分）

### ステップ1：NuGetパッケージを更新

```bash
# ActivePDFパッケージを削除
dotnet remove package APToolkitNET

# IronPDFをインストール
dotnet add package IronPdf
```

またはパッケージマネージャーコンソール経由で：
```powershell
Uninstall-Package APToolkitNET
Install-Package IronPdf
```

### ステップ2：ライセンスキーを設定（Program.csまたはStartup）

```csharp
// アプリケーションの起動時、IronPDF操作の前にこれを追加
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ3：グローバル検索＆置換

| 検索 | 置換 |
|------|------|
| `using ActivePDF.Toolkit;` | `using IronPdf;` |
| `using APToolkitNET;` | `using IronPdf;` |
| `using APToolkitNET.PDFObjects;` | `using IronPdf;` |
| `using APToolkitNET.Common;` | `using IronPdf;` |

### ステップ4：基本操作を確認

**移行前（ActivePDF）：**
```csharp
using ActivePDF.Toolkit;

Toolkit toolkit = new Toolkit();
if (toolkit.OpenOutputFile("output.pdf") == 0)
{
    toolkit.AddHTML("<h1>Hello World</h1>");
    toolkit.CloseOutputFile();
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| ActivePDF 名前空間 | IronPDF 名前空間 | 目的 |
|---------------------|-------------------|---------|
| `ActivePDF.Toolkit` | `IronPdf` | コア機能 |
| `APToolkitNET` | `IronPdf` | 主要なPDF操作 |
| `APToolkitNET.PDFObjects` | `IronPdf` | PDFドキュメントオブジェクト |
| `APToolkitNET.Common` | `IronPdf` | 共通ユーティリティ |
| `APToolkitNET.Extractor` | `IronPdf` | テキスト抽出 |
| `APToolkitNET.Rasterizer` | `IronPdf` | PDFから画像へ |

### ドキュメント作成メソッド

| ActivePDF メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `new Toolkit()` | `new ChromePdfRenderer()` | レンダラーがPDFを作成 |
| `new Toolkit(path)` | `new ChromePdfRenderer()` | パスは不要 |
| `toolkit.OpenOutputFile(path)` | 不要 | 最後にSaveAsを呼び出すだけ |
| `toolkit.CloseOutputFile()` | 不要 | 自動クリーンアップ |
| `toolkit.AddHTML(html)` | `renderer.RenderHtmlAsPdf(html)` | PdfDocumentを返す |
| `toolkit.AddURL(url)` | `renderer.RenderUrlAsPdf(url)` | URLからPDFへ |
| `toolkit.NewPage()` | HTMLから自動 | ページは自動的に作成される |
| `toolkit.SaveAs(path)` | `pdf.SaveAs(path)` | ファイルに保存 |

### ファイル操作メソッド

| ActivePDF メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `toolkit.OpenInputFile(path)` | `PdfDocument.FromFile(path)` | 既存のPDFをロード |
| `toolkit.AddPDF(path)` | `PdfDocument.Merge()` | マージのため |
| `toolkit.Merge(path)` | `PdfDocument.Merge(pdfs)` | 複数のPDFをマージ |
| `toolkit.GetPDFInfo()` | `pdf.MetaData` | ドキュメントのメタデータ |
| `toolkit.GetPageCount()` | `pdf.PageCount` | ページ数プロパティ |

### ページ操作メソッド

| ActivePDF メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `toolkit.SetPageSize(w, h)` | `RenderingOptions.PaperSize` | 列挙型またはカスタムを使用 |
| `toolkit.SetOrientation("Portrait")` | `RenderingOptions.PaperOrientation` | 列挙値 |
| `toolkit.SetMargins(t, b, l, r)` | `RenderingOptions.MarginTop/Bottom/Left/Right` | 個々のプロパティ |
| `toolkit.CopyPage(src, dest)` | `pdf.CopyPages(indices)` | ページをコピー |
| `toolkit.DeletePage(index)` | `pdf.RemovePages(index)` | ページを削除 |
| `toolkit.RotatePage(degrees)` | `pdf.Pages[i].Rotation` | ページごとの回転 |

### コンテンツメソッド

| ActivePDF メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `toolkit.AddText(text, x, y)` | HTML/CSSの位置指定を使用 | HTMLアプローチ |
| `toolkit.AddImage(path, x, y)` | `ImageStamper`またはHTMLの`<img>` | スタンピング |
| `toolkit.AddWatermark(text)` | `pdf.ApplyWatermark(html)` | HTMLウォーターマーク |
| `toolkit.AddBookmark(title)` | `pdf.BookMarks.AddBookMarkAtStart()` | ブックマークAPI |
| `toolkit.AddAnnotation(...)` | `pdf.Annotations` | アノテーションAPI |

### テキスト抽出メソッド

| ActivePDF メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `toolkit.GetText()` | `pdf.ExtractAllText()` | ドキュメント全体のテキスト |
| `toolkit.GetTextFromPage(page)` | `pdf.Pages[i].Text` | ページごとの抽出 |
| `extractor.ExtractToFile(path)` | `File.WriteAllText(path, pdf.ExtractAllText())` | ファイルに保存 |

### セキュリティメソッド

| ActivePDF メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `toolkit.Encrypt(password)` | `pdf.SecuritySettings.OwnerPassword` | オーナーパスワード |
| `toolkit.SetUserPassword(pwd)` | `pdf.SecuritySettings.UserPassword` | ユーザーパスワード |
| `toolkit.SetPermissions(flags)` | `pdf.SecuritySettings.AllowUserXxx` | 個々の権限 |
| `toolkit.RemoveEncryption()` | パスワードでロードし、パスワードなしで保存 | 直接的な方法なし |

### 設定オプションマッピング

| ActivePDF 設定 | IronPDF 相当 | デフォルト |
|-----------------|-----------------|---------|
| `toolkit.SetPageSize(612, 792)` | `RenderingOptions.PaperSize = PdfPaperSize.Letter` | A4 |
| `toolkit.SetPageSize(595, 842)` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | A4 |
| `toolkit.SetOrientation("Landscape")` | `RenderingOptions.PaperOrientation = Landscape` | 縦向き |
| `toolkit.SetDPI(300)` | `RenderingOptions.Dpi` | 96 |
| `toolkit.SetCompression(true)` | `pdf.CompressImages()` | レンダリング後 |
| `toolkit.EnableJavaScript(true)` | `RenderingOptions.EnableJavaScript` | true |

---

## コード移行例

### 例1：HTML文字列からPDFへ

**移行前（ActivePDF）：**
```csharp
using ActivePDF.Toolkit;

public void CreatePdfFromHtml(string html, string outputPath)
{
    Toolkit toolkit = new Toolkit();

    if (toolkit.OpenOutputFile(outputPath) == 0)
    {
        toolkit.SetPageSize(612, 792); // Letterサイズ
        toolkit.SetMargins(72, 72, 72, 72); // 1インチのマージン
        toolkit.AddHTML(html);
        toolkit.CloseOutputFile();
    }
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public void CreatePdfFromHtml(string html, string outputPath)
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
    renderer.RenderingOptions.MarginTop = 25;  // mm
    renderer.RenderingOptions.MarginBottom = 25;
    renderer.RenderingOptions.MarginLeft = 25;
    renderer.RenderingOptions.MarginRight = 25;

    using var pdf = renderer.RenderHtmlAsPdf(html);
    pdf.SaveAs(outputPath);
}
```

### 例2：URLからPDFへ

**移行前（ActivePDF）：**
```csharp
using ActivePDF.Toolkit;

public void ConvertUrlToPdf(string url, string outputPath)
{
    Toolkit toolkit = new Toolkit();

    if (toolkit.OpenOutputFile(outputPath) == 0)
    {
        toolkit.AddURL(url);
        toolkit.CloseOutputFile();
        Console.WriteLine("PDFが正常に作成されました");
    }
    else
    {
        Console.WriteLine("PDFの作成に失敗しました");
    }
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public void ConvertUrlToPdf(string url, string outputPath)
{
    var renderer = new ChromePdfRenderer();

    try
    {
        using var pdf = renderer.RenderUrlAsPdf(url);
        pdf.SaveAs(outputPath);
        Console.WriteLine("PDFが正常に作成されました");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"PDFの作成に失敗しました: {ex.Message}");
    }
}
```

### 例3：