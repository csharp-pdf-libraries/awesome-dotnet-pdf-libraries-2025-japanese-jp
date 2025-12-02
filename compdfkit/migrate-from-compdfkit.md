# ComPDFKitからIronPDFへの移行方法は？

## 目次
1. [IronPDFへの移行理由](#ironpdfへの移行理由)
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

## IronPDFへの移行理由

### 主な利点

| 項目 | ComPDFKit | IronPDF |
|--------|-----------|---------|
| **HTMLからPDFへ** | 手動でのHTML解析が必要 | ネイティブのChromiumレンダリング |
| **市場の成熟度** | 新規参入者 | 10年以上の実績 |
| **コミュニティのサイズ** | 小規模、Stack Overflowでの情報が限られる | 大規模で活発なコミュニティ |
| **ドキュメント** | いくつかのギャップがある | 豊富なチュートリアルとガイド |
| **ダウンロード数** | 成長中 | 1000万以上のNuGetダウンロード |
| **APIスタイル** | C++の影響を受けた冗長なもの | モダンな.NETフルエントAPI |
| **メモリ管理** | 手動での`Release()`呼び出しが必要 | 自動GC処理 |

### 機能比較

| 機能 | ComPDFKit | IronPDF |
|---------|-----------|---------|
| HTMLからPDFへ | 基本/手動 | ✅ ネイティブのChromium |
| URLからPDFへ | 手動での実装が必要 | ✅ 組み込み |
| ゼロからPDFの作成 | ✅ | ✅ |
| PDF編集 | ✅ | ✅ |
| テキスト抽出 | ✅ | ✅ |
| マージ/分割 | ✅ | ✅ |
| デジタル署名 | ✅ | ✅ |
| 注釈 | ✅ | ✅ |
| フォーム入力 | ✅ | ✅ |
| PDF/A準拠 | ✅ | ✅ |
| ウォーターマーク | ✅ | ✅ |
| クロスプラットフォーム | Windows, Linux, macOS | Windows, Linux, macOS |
| .NET Core/.NET 5+ | ✅ | ✅ |

### 移行の利点

1. **優れたHTMLレンダリング**: IronPDFのChromiumエンジンは、最新のCSS3、JavaScript、レスポンシブレイアウトを処理
2. **成熟したエコシステム**: 10年以上の改良、豊富なドキュメント、実証された安定性
3. **シンプルなAPI**: 冗長なコードが不要、手動でのメモリ管理が不要
4. **.NETとのより良い統合**: ネイティブのasync/await、LINQサポート、フルエントインターフェース
5. **豊富なリソース**: Stack Overflowの回答やコミュニティの例が数千件
6. **プロフェッショナルサポート**: エンタープライズグレードのサポートと定期的なアップデート

---

## 開始前に

### 前提条件

- **.NETバージョン**: IronPDFは.NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9をサポート
- **NuGetアクセス**: nuget.orgからパッケージをインストールできることを確認
- **ライセンスキー**: [IronPDFウェブサイト](https://ironpdf.com/)から取得（無料トライアル利用可能）

### すべてのComPDFKit参照を見つける

```bash
# コードベース内のすべてのComPDFKitの使用箇所を検索
grep -r "using ComPDFKit" --include="*.cs" .
grep -r "CPDFDocument\|CPDFPage\|CPDFAnnotation" --include="*.cs" .

# NuGetパッケージ参照を検索
grep -r "ComPDFKit" --include="*.csproj" .
grep -r "ComPDFKit" --include="packages.config" .
```

### 変更点の概要

| 変更点 | ComPDFKit | IronPDF | 影響 |
|--------|-----------|---------|--------|
| **ドキュメントのロード** | `CPDFDocument.InitWithFilePath()` | `PdfDocument.FromFile()` | メソッド名の変更 |
| **保存** | `document.WriteToFilePath()` | `pdf.SaveAs()` | メソッド名の変更 |
| **メモリクリーンアップ** | `document.Release()`が必要 | 自動（GC） | 手動クリーンアップの削除 |
| **ページアクセス** | `document.PageAtIndex(i)` | `pdf.Pages[i]` | 配列スタイルアクセス |
| **ページインデックス** | 0ベース | 0ベース | 変更不要 |
| **HTMLレンダリング** | 手動実装 | `RenderHtmlAsPdf()` | 大幅な簡素化 |
| **テキスト抽出** | `textPage.GetText()` | `pdf.ExtractAllText()` | APIの簡素化 |
| **ページ数** | `document.PageCount`プロパティ | `pdf.PageCount` | 同じパターン |

---

## クイックスタート（5分）

### ステップ1: NuGetパッケージを更新

```bash
# ComPDFKitパッケージを削除
dotnet remove package ComPDFKit.NetCore
dotnet remove package ComPDFKit.NetFramework

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: Usingステートメントを更新

```csharp
// 以前
using ComPDFKit.PDFDocument;
using ComPDFKit.PDFPage;
using ComPDFKit.PDFAnnotation;
using ComPDFKit.Import;

// 以降
using IronPdf;
```

### ステップ3: ライセンスキーを適用

```csharp
// アプリケーションの起動時（Program.csまたはGlobal.asax）に追加
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ4: 基本的なコード移行

**以前 (ComPDFKit):**
```csharp
using ComPDFKit.PDFDocument;

var document = CPDFDocument.InitWithFilePath("input.pdf");
int pageCount = document.PageCount;
Console.WriteLine($"ページ数: {pageCount}");
document.WriteToFilePath("output.pdf");
document.Release();
```

**以降 (IronPDF):**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("input.pdf");
int pageCount = pdf.PageCount;
Console.WriteLine($"ページ数: {pageCount}");
pdf.SaveAs("output.pdf");
// Release()は不要 - GCがクリーンアップを処理
```

---