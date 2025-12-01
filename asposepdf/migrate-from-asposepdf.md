---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [asposepdf/migrate-from-asposepdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/asposepdf/migrate-from-asposepdf.md)
🇯🇵 **日本語:** [asposepdf/migrate-from-asposepdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/asposepdf/migrate-from-asposepdf.md)

---

# 移行ガイド: Aspose.PDF for .NET → IronPDF

## 目次
1. [Aspose.PDFからIronPDFへの移行理由](#asposepdfからironpdfへの移行理由)
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

## Aspose.PDFからIronPDFへの移行理由

### コスト比較

Aspose.PDFは従来のエンタープライズライセンスモデルを使用しており、年次更新が必要です：

| 項目 | Aspose.PDF | IronPDF |
|--------|-----------|---------|
| **開始価格** | $1,199/開発者/年 | $749 一回払い（Lite） |
| **ライセンスモデル** | 年間サブスクリプション + 更新 | 永久ライセンス |
| **OEMライセンス** | $5,997+ 追加 | 高位層で含まれる |
| **サポート** | 追加コスト層 | 含まれる |
| **3年間の総コスト** | 開発者あたり $3,597+ | $749 一回払い |

### HTMLレンダリングエンジン比較

| 機能 | Aspose.PDF (Flying Saucer) | IronPDF (Chromium) |
|---------|---------------------------|-------------------|
| **CSS3サポート** | 限定的（古いCSS） | 完全なCSS3 |
| **Flexbox/Grid** | サポートされていない | 完全サポート |
| **JavaScript** | 非常に限定的 | 完全サポート |
| **Webフォント** | 部分的 | 完全 |
| **現代のHTML5** | 限定的 | 完全 |
| **レンダリング品質** | 可変 | ピクセル完璧 |

### パフォーマンス比較

ユーザーからは、顕著なパフォーマンスの違いが報告されています：

| 指標 | Aspose.PDF | IronPDF |
|--------|-----------|---------|
| **HTMLレンダリング** | 文書化された遅延（場合によっては30倍遅い） | 最適化されたChromiumエンジン |
| **大規模なドキュメント** | メモリ問題が報告されている | 効率的なストリーミング |
| **Linuxパフォーマンス** | 高CPU、メモリリークが報告されている | 安定 |
| **バッチ処理** | 可変 | 一貫性 |

### 移行するタイミング

**IronPDFに移行する場合：**
- 現代のHTML/CSSレンダリングが必要な場合
- アプリケーションのパフォーマンスが重要な場合
- ライセンスコストを削減したい場合
- Aspose.PDFのHTMLエンジンに問題がある場合
- 信頼性の高いクロスプラットフォームパフォーマンスが必要な場合

**Aspose.PDFを継続する場合：**
- IronPDFで利用できない特定のAspose.PDF機能を使用している場合
- ドキュメントがプログラムで構築されている場合（HTMLベースではない）
- 他のAspose製品との深い統合がある場合
- 移行コストがライセンス節約を上回る場合

---

## 開始する前に

### 前提条件

- **.NET Framework 4.6.2+** または **.NET Core 3.1+** または **.NET 5/6/7/8/9**
- **Visual Studio 2019+** または **VS Code** にC#拡張機能がある
- **NuGetパッケージマネージャー** アクセス
- 移行する**既存のAspose.PDFコードベース**

### Aspose.PDF参照の検索

移行する前に、コードベース内のすべてのAspose.PDFの使用を特定します：

```bash
# Aspose.Pdf usingステートメントをすべて検索
grep -r "using Aspose.Pdf" --include="*.cs" .

# Document使用を検索
grep -r "new Document\|Document\." --include="*.cs" .

# HtmlLoadOptions使用を検索
grep -r "HtmlLoadOptions\|HtmlFragment" --include="*.cs" .

# Facades使用を検索
grep -r "PdfFileEditor\|PdfFileMend\|PdfFileStamp" --include="*.cs" .

# TextAbsorber使用を検索
grep -r "TextAbsorber\|TextFragmentAbsorber" --include="*.cs" .
```

### 予想される変更点

| Aspose.PDFパターン | 必要な変更 |
|--------------------|-----------------|
| `new Document()` + `Pages.Add()` | HTMLレンダリングを代わりに使用 |
| `HtmlLoadOptions` | `ChromePdfRenderer.RenderHtmlAsPdf()` |
| `TextFragment` + 手動位置指定 | CSSに基づく位置指定 |
| `PdfFileEditor.Concatenate()` | `PdfDocument.Merge()` |
| `TextFragmentAbsorber` | `pdf.ExtractAllText()` |
| `ImageStamp` | HTMLベースの透かし |
| `.lic` ファイルライセンス | コードベースのライセンスキー |

---

## クイックスタート（5分）

### ステップ1: NuGetパッケージを置き換える

```bash
# Aspose.PDFを削除
dotnet remove package Aspose.PDF

# IronPDFをインストール
dotnet add package IronPdf
```

またはパッケージマネージャーコンソール経由で：
```powershell
Uninstall-Package Aspose.PDF
Install-Package IronPdf
```

### ステップ2: 名前空間を更新する

```csharp
// ❌ これらを削除
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Aspose.Pdf.Facades;
using Aspose.Pdf.Generator;

// ✅ これらを追加
using IronPdf;
using IronPdf.Rendering;
using IronPdf.Editing;
```

### ステップ3: ライセンス設定を更新する

**以前 (Aspose.PDF):**
```csharp
var license = new Aspose.Pdf.License();
license.SetLicense("Aspose.Pdf.lic");
```

**後 (IronPDF):**
```csharp
IronPdf.License.LicenseKey = "YOUR-IRONPDF-LICENSE-KEY";
```

### ステップ4: 最初のPDFを変換する

**以前 (Aspose.PDF):**
```csharp
string html = "<h1>Hello World</h1>";
using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
{
    var doc = new Document(stream, new HtmlLoadOptions());
    doc.Save("output.pdf");
}
```

**後 (IronPDF):**
```csharp
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

---