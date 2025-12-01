---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [foxit-sdk/migrate-from-foxit-sdk.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/foxit-sdk/migrate-from-foxit-sdk.md)
🇯🇵 **日本語:** [foxit-sdk/migrate-from-foxit-sdk.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/foxit-sdk/migrate-from-foxit-sdk.md)

---

# 移行ガイド: Foxit PDF SDK → IronPDF

## 目次
1. [IronPDFへ移行する理由](#ironpdfへ移行する理由)
2. [開始前に](#開始前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード例](#コード例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## IronPDFへ移行する理由

### Foxit SDKの課題

Foxit PDF SDKは強力なエンタープライズレベルのライブラリですが、大きな複雑さを伴います：

1. **複雑なライセンスシステム**: 複数の製品、SKU、ライセンスタイプ（開発者ごと、サーバーごと、OEMなど）があり、適切なオプションを選択することが困難です
2. **エンタープライズ価格**: 大規模な組織向けに価格設定されており、小規模なチームにとっては禁止的な場合があります
3. **手動インストール**: DLLの参照やプライベートNuGetフィードが必要で、簡単な公開NuGetパッケージはありません
4. **冗長なAPI**: ライブラリの初期化、エラーコードのチェック、明示的なリソースのクリーンアップがボイラープレートを追加します
5. **別途HTML変換アドオンが必要**: HTMLからPDFへの変換には追加のアドオン購入が必要です
6. **複雑な設定**: 設定には詳細なオブジェクト設定が必要です（例：`HTML2PDFSettingData`）
7. **C++の遺産**: APIパターンはC++の起源を反映しており、現代のC#では自然に感じられません

### IronPDFの利点

| 項目 | Foxit SDK | IronPDF |
|------|-----------|---------|
| インストール | 手動DLL/プライベートフィード | 簡単なNuGetパッケージ |
| ライセンシング | 複雑、エンタープライズ向け | 透明、全サイズ |
| 初期化 | `Library.Initialize(sn, key)` | ライセンスキーを一度設定 |
| エラー処理 | ErrorCode enums | 標準の.NET例外 |
| HTMLからPDFへ | 別途アドオン | 組み込みChromium |
| APIスタイル | C++の遺産、冗長 | 現代的な.NETパターン |
| リソースのクリーンアップ | 手動の`Close()`/`Release()` | IDisposable/自動 |
| ドキュメント | エンタープライズドキュメント | 公開チュートリアル |

### コストベネフィット分析

Foxit SDKからIronPDFへの移行は以下を提供します：
- **複雑さの削減**: よりシンプルなAPI、少ないボイラープレート
- **開発の高速化**: 直感的なメソッド、少ない設定
- **現代の.NET**: async/await、LINQ互換性、標準パターン
- **HTMLファーストアプローチ**: WebスキルをPDF生成に使用
- **包括的な機能**: HTML変換用の別途アドオンは不要

---

## 開始前に

### 前提条件

1. **.NET環境**: IronPDFは.NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9+をサポートしています
2. **NuGetアクセス**: NuGetからパッケージをインストールできることを確認してください
3. **ライセンスキー**: 本番用のIronPDFライセンスキーを取得してください

### プロジェクトのバックアップ

```bash
# バックアップブランチを作成
git checkout -b pre-ironpdf-migration
git add .
git commit -m "Backup before Foxit SDK to IronPDF migration"
```

### すべてのFoxit SDKの使用を特定

```bash
# すべてのFoxit SDKの参照を検索
grep -r "foxit\|PDFDoc\|PDFPage\|Library.Initialize\|Library.Release" --include="*.cs" --include="*.csproj" .

# Foxit DLLの参照を検索
find . -name "*.csproj" | xargs grep -l "Foxit\|fsdk"
```

### 現在の機能を文書化

移行前にカタログ：
- 使用しているFoxitの機能（HTML変換、注釈、フォーム、セキュリティ）
- ライセンスキーの場所と初期化コード
- カスタム設定と設定
- エラー処理パターン

---

## クイックスタート移行

### ステップ 1: NuGetパッケージの更新

```bash
# Foxit SDKは通常、DLL参照の手動削除が必要です
# .csprojでFoxitの参照を確認し、それらを削除してください

# IronPDFをインストール
dotnet add package IronPdf
```

**.csprojにFoxitの参照がある場合：**
```xml
<!-- これらを手動で削除 -->
<Reference Include="fsdk_dotnet">
    <HintPath>..\libs\Foxit\fsdk_dotnet.dll</HintPath>
</Reference>
```

### ステップ 2: 名前空間の更新

```csharp
// 以前
using foxit;
using foxit.common;
using foxit.common.fxcrt;
using foxit.pdf;
using foxit.pdf.annots;
using foxit.addon.conversion;

// 以後
using IronPdf;
using IronPdf.Rendering;
using IronPdf.Editing;
```

### ステップ 3: IronPDFの初期化

```csharp
// 以前 (Foxit)
string sn = "YOUR_SERIAL_NUMBER";
string key = "YOUR_LICENSE_KEY";
ErrorCode error_code = Library.Initialize(sn, key);
if (error_code != ErrorCode.e_ErrSuccess)
{
    throw new Exception("Failed to initialize Foxit SDK");
}
// ... あなたのコード ...
Library.Release();

// 以後 (IronPDF)
IronPdf.License.LicenseKey = "YOUR-IRONPDF-LICENSE-KEY";
// それだけです！Release()は必要ありません
```

### ステップ 4: 基本的な変換パターン

```csharp
// 以前 (Foxit)
Library.Initialize(sn, key);
HTML2PDFSettingData settings = new HTML2PDFSettingData();
settings.page_width = 612.0f;
settings.page_height = 792.0f;
using (HTML2PDF html2pdf = new HTML2PDF(settings))
{
    html2pdf.Convert(htmlContent, "output.pdf");
}
Library.Release();

// 以後 (IronPDF)
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(htmlContent);
pdf.SaveAs("output.pdf");
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| Foxitの名前空間 | IronPDFの同等物 | 備考 |
|-----------------|-------------------|-------|
| `foxit` | `IronPdf` | メインの名前空間 |
| `foxit.common` | `IronPdf` | 共通タイプ |
| `foxit.common.fxcrt` | N/A | 低レベル（不要） |
| `foxit.pdf` | `IronPdf` | PDFドキュメント操作 |
| `foxit.pdf.annots` | `IronPdf.Editing` | 注釈 |
| `foxit.pdf.actions` | `IronPdf` | アクション（リンクなど） |
| `foxit.pdf.graphics` | `IronPdf.Drawing` | グラフィック操作 |
| `foxit.addon.conversion` | `IronPdf.Rendering` | HTML/画像変換 |

### コアクラスマッピング

| Foxit SDKクラス | IronPDFの同等物 | 備考 |
|-----------------|-------------------|-------|
| `Library` | N/A | IronPDFは自動管理 |
| `PDFDoc` | `PdfDocument` | メインドキュメントクラス |
| `PDFPage` | `PdfDocument.Pages[i]` | ページアクセス |
| `HTML2PDF` | `ChromePdfRenderer` | HTML変換 |
| `Convert` | `ChromePdfRenderer` | 変換 |
| `TextPage` | `pdf.ExtractTextFromPage(i)` | テキスト抽出 |
| `Watermark` | `TextStamper` / `ImageStamper` | ウォーターマーク |
| `Security` | `SecuritySettings` | PDFセキュリティ |
| `Annotation` | Various stampers | 注釈 |
| `Form` | `pdf.Form` | フォームフィールド |
| `Bookmark` | `pdf.BookMarks` | ブックマーク/アウトライン |
| `Metadata` | `pdf.MetaData` | ドキュメントメタデータ |

### ライブラリの初期化

| Foxitメソッド | IronPDFの同等物 | 備考 |
|--------------|-------------------|-------|
| `Library.Initialize(sn, key)` | `IronPdf.License.LicenseKey = "key"` | 一度のセットアップ |
| `Library.Release()` | N/A | 不要 |
| ErrorCodeチェック | Try/catch | 標準例外 |

### PDFDocメソッド

| FoxitのPDFDoc | IronPDFのPdfDocument | 備考 |
|--------------|---------------------|-------|
| `new PDFDoc(path)` | `PdfDocument.FromFile(path)` | ファイルからロード |
| `doc.LoadW(password)` | `PdfDocument.FromFile(path, password)` | パスワード保護 |
| `doc.Load("")` | N/A | ロードは自動 |
| `doc.GetPageCount()` | `pdf.PageCount` | ページ数プロパティ |
| `doc.GetPage(index)` | `pdf.Pages[index]` | インデックスでページを取得 |
| `doc.CreatePage()` | HTMLレンダリングがページを作成 | または既存のマージ |
| `doc.SaveAs(path, flags)` | `pdf.SaveAs(path)` | ドキュメントを保存 |
| `doc.Close()` | `pdf.Dispose()`またはusingステートメント | クリーンアップ |
| `doc.InsertDocument()` | `PdfDocument.Merge()` | ドキュメントをマージ |
| `doc.RemovePage(index)` | `pdf.RemovePages(indices)` | ページを削除 |
| `doc.GetMetadata()` | `pdf.MetaData` | プロパティアクセス |
| `doc.SetMetadata()` | `pdf.MetaData.Title = "..."` | メタデータを設定 |

### PDFPageメソッド

| FoxitのPDFPage | IronPDFの同等物 | 備考 |
|---------------|-------------------|-------|
| `page.GetWidth()` | `pdf.Pages[i].Width` | ページ幅 |
| `page.GetHeight()` | `pdf.Pages[i].Height` | ページ高さ |
| `page.GetRotation()` | `pdf.Pages[i].Rotation` | ページ回転 |
| `page.StartParse()` | N/A | 自動解析 |
| `page.StartEditing()` | スタンパーを使用 | スタンパー経由で編集 |
| `page.EndEditing()` | N/A | 自動終了 |
| `page.DrawText()` | `TextStamper` | テキストを描画 |
| `page.AddAnnot()` | Various stampers | 注釈を追加 |
| `new TextPage(page)` | `pdf.ExtractTextFromPage(i)` | ページからテキストを抽出 |

### HTML2PDF / 変換

| FoxitのHTML2PDF | IronPDFの同等物 | 備考 |
|----------------|-------------------|-------|
| `new HTML2PDFSettingData()` | `new ChromePdfRenderer()` | レンダラーを作成 |
| `settings.page_width` | `RenderingOptions.PaperSize` | 標準サイズ |
| `settings.page_height` | `RenderingOptions.SetCustomPaperSize()` | カスタムサイズ |
| `settings.page_mode` | N/A | デフォルトでマルチページ |
| `html2pdf.Convert(html, path)` | `renderer.RenderHtmlAsPdf(html)` | HTML文字列 |
| `html2pdf.ConvertFromURL(url, path)` | `renderer.RenderUrlAsPdf(url)` | URL変換 |
| `html2pdf.ConvertFromFile(path, out)` | `renderer.RenderHtmlFileAsPdf(path)` | HTMLファイル |

### RenderingOptions (設定)

| Foxitの設定 | IronPDFのRenderingOptions | 備考 |
|-------------|-------------------------|-------|
| `page_width` (ポイント) | `PaperSize`または`SetCustomPaperSize(w, h)` | 標準/カスタム |
| `page_height` (ポイント) | 上記と同様 | カスタムの場合はmmで |
| `page_margin_top/bottom/left/right` | `MarginTop/Bottom/Left/Right` | mm単位 |
| `scale_mode` | `Zoom` | ズーム率 |
| `page_orientation` | `PaperOrientation` | 縦/横 |
| JavaScriptタイムアウト | `Timeout` | 秒単位 |
| メディアタイプ | `CssMediaType` | 印刷/スクリーン |

### セキュリティ設定

| Foxitのセキュリティ | IronPDFのSecuritySettings | 備考 |
|---------------------|-------------------------|-------|
| `doc.Encrypt(password)` | `pdf.SecuritySettings.OwnerPassword` | オーナーパスワード |
| ユーザーパスワード | `pdf.SecuritySettings.UserPassword` | ユーザーパスワード |
| 印刷許可 | `AllowUserPrinting` | 印刷権限 |
| コピー許可 | `AllowUserCopyPasteContent` | コピー権限 |
| 変更許可 | `AllowUserEdits` | 編集権限 |
| `EncryptionLevel` | 自動（AES-256） | 暗号化レベル |

### ウォーターマーク

| Foxitのウォーターマーク | IronPDFの同等物 | 備考 |
|-----------------------|-------------------|-------|
| `new Watermark(doc, text, font, size, color)` | `new TextStamper()` | テキストウォーターマーク |
| `WatermarkSettings.position` | `VerticalAlignment`