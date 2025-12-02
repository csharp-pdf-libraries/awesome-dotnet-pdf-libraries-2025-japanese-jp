# Apryse（旧PDFTron）からIronPDFへの移行方法は？

## 目次
1. [ApryseからIronPDFへの移行理由](#apryseからironpdfへの移行理由)
2. [開始前に](#開始前に)
3. [クイックスタート（5分）](#クイックスタート5分)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティングガイド](#トラブルシューティングガイド)
9. [移行チェックリスト](#移行チェックリスト)
10. [追加リソース](#追加リソース)

---

## ApryseからIronPDFへの移行理由

### コスト比較

Apryse（旧PDFTron）は市場で最も高価なPDF SDKの一つであり、プレミアム価格でエンタープライズ顧客をターゲットにしています：

| 項目 | Apryse (PDFTron) | IronPDF |
|--------|-----------------|---------|
| **開始価格** | $1,500+/開発者/年（報告されている） | $749 一回払い（Lite） |
| **ライセンスモデル** | 年間サブスクリプション | 永久ライセンス |
| **ビューアライセンス** | 別途、追加コスト | 該当なし（標準ビューアを使用） |
| **サーバーライセンス** | エンタープライズ価格が必要 | ライセンス階層に含まれる |
| **合計3年間のコスト** | 開発者あたり$4,500+ | $749 一回払い |

### 複雑さとシンプルさ

| 機能 | Apryse | IronPDF |
|---------|--------|---------|
| **セットアップ** | モジュールパス、外部バイナリ | 単一のNuGetパッケージ |
| **初期化** | `PDFNet.Initialize()` にライセンスが必要 | 単純なプロパティ割り当て |
| **HTMLレンダリング** | 外部html2pdfモジュールが必要 | 組み込みのChromiumエンジン |
| **APIスタイル** | C++の遺産、複雑 | 現代的なC#慣習 |
| **依存関係** | 複数のDLL、プラットフォーム固有 | 自己完結型パッケージ |

### 移行するタイミング

**IronPDFに移行する場合：**
- HTML/URLからPDFへの変換が主に必要な場合
- よりシンプルなAPIと少ないボイラープレートを望む場合
- プレミアム価格が使用ケースに正当化されない場合
- PDFViewCtrlビューアコントロールが不要な場合
- サブスクリプションではなく一回払いのライセンスを好む場合

**Apryseを継続する場合：**
- ネイティブビューアコントロール（PDFViewCtrl）が必要な場合
- XODや独自フォーマットを広範囲に使用している場合
- 特定のエンタープライズ機能（高度な赤塗りなど）が必要な場合
- 組織がすでにエンタープライズライセンスを持っている場合

---

## 開始前に

### 前提条件

- **.NET Framework 4.6.2+** または **.NET Core 3.1+** または **.NET 5/6/7/8/9**
- **Visual Studio 2019+** または **VS Code** にC#拡張機能がある
- **NuGetパッケージマネージャー** アクセス
- **移行する既存のApryse（PDFTron）コードベース**

### すべてのApryse参照を見つける

移行前に、コードベース内のPDFTronの使用を特定します：

```bash
# すべてのpdftron usingステートメントを見つける
grep -r "using pdftron" --include="*.cs" .

# PDFNetの初期化を見つける
grep -r "PDFNet.Initialize\|PDFNet.SetResourcesPath" --include="*.cs" .

# PDFDocの使用を見つける
grep -r "new PDFDoc\|PDFDoc\." --include="*.cs" .

# HTML2PDFの使用を見つける
grep -r "HTML2PDF\|InsertFromURL\|InsertFromHtmlString" --include="*.cs" .

# ElementReader/Writerの使用を見つける
grep -r "ElementReader\|ElementWriter\|ElementBuilder" --include="*.cs" .
```

### 予想される変更点

| Apryseパターン | 必要な変更 |
|----------------|-----------------|
| `PDFNet.Initialize()` | `IronPdf.License.LicenseKey` に置き換え |
| `HTML2PDF` モジュール | 組み込みの `ChromePdfRenderer` |
| `ElementReader`/`ElementWriter` | IronPDFがコンテンツを内部的に処理 |
| `SDFDoc.SaveOptions` | 単純な `SaveAs()` メソッド |
| `PDFViewCtrl` | 外部PDFビューアを使用 |
| XODフォーマット | PDFまたは画像に変換 |
| モジュールパスの設定 | 不要 |

---

## クイックスタート（5分）

### ステップ1: NuGetパッケージを置き換える

```bash
# Apryse/PDFTronパッケージを削除
dotnet remove package PDFTron.NET.x64
dotnet remove package PDFTron.NET.x86
dotnet remove package pdftron

# IronPDFをインストール
dotnet add package IronPdf
```

またはパッケージマネージャーコンソール経由で：
```powershell
Uninstall-Package PDFTron.NET.x64
Install-Package IronPdf
```

### ステップ2: 名前空間を更新する

```csharp
// ❌ これらを削除
using pdftron;
using pdftron.PDF;
using pdftron.PDF.Convert;
using pdftron.SDF;
using pdftron.Filters;

// ✅ これらを追加
using IronPdf;
using IronPdf.Rendering;
```

### ステップ3: 初期化コードを削除する

**Before (Apryse):**
```csharp
// 複雑な初期化
PDFNet.Initialize("YOUR_LICENSE_KEY");
PDFNet.SetResourcesPath("path/to/resources");
// HTML2PDFのモジュールパスも...
```

**After (IronPDF):**
```csharp
// 単純なライセンス割り当て（開発用はオプション）
IronPdf.License.LicenseKey = "YOUR_LICENSE_KEY";
```

### ステップ4: 最初のPDFを変換する

**Before (Apryse):**
```csharp
using (PDFDoc doc = new PDFDoc())
{
    HTML2PDF converter = new HTML2PDF();
    converter.SetModulePath("path/to/html2pdf");
    converter.InsertFromHtmlString("<h1>Hello</h1>");
    if (converter.Convert(doc))
    {
        doc.Save("output.pdf", SDFDoc.SaveOptions.e_linearized);
    }
}
```

**After (IronPDF):**
```csharp
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello</h1>");
pdf.SaveAs("output.pdf");
```

---