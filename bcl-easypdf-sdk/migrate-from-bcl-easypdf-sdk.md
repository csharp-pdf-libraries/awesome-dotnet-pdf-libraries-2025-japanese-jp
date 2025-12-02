# BCL EasyPDF SDKからIronPDFへの移行方法は？

## 目次
1. [BCL EasyPDF SDKからIronPDFへ移行する理由](#bcl-easypdf-sdkからironpdfへ移行する理由)
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

## BCL EasyPDF SDKからIronPDFへ移行する理由

### BCL EasyPDF SDKの問題点

BCL EasyPDF SDKは、いくつかの問題のある技術に依存しており、デプロイメントの悪夢を引き起こします：

| 問題 | 影響 |
|-------|--------|
| **Windows専用** | Linux、macOS、Docker、クラウドコンテナへのデプロイができない |
| **Microsoft Officeが必要** | 文書変換のために、すべてのサーバーにOfficeをインストールする必要がある |
| **COMインターオプ** | DLLの読み込みエラー、クラッシュ、バージョンの競合が発生する |
| **仮想プリンタードライバー** | サーバー上での対話型ユーザーセッションが必要 |
| **レガシーアーキテクチャ** | .NET Core/.NET 5+のサポートが限定的 |
| **複雑なインストール** | MSIインストーラー、GAC登録、レジストリの変更が必要 |

### BCL EasyPDF SDKの一般的なエラー

開発者は頻繁にこれらの問題に遭遇します：

- `bcl.easypdf.interop.easypdfprinter.dll error loading`
- `COM object that has been separated from its underlying RCW cannot be used`
- `Timeout expired waiting for print job to complete`
- `The printer operation failed because the service is not running`
- `Error: Access denied`（対話型セッションが必要）
- `Cannot find printer: BCL easyPDF Printer`

### IronPDFの利点

| 機能 | BCL EasyPDF SDK | IronPDF |
|---------|-----------------|---------|
| **プラットフォーム** | Windows専用 | Windows, Linux, macOS, Docker |
| **Office依存性** | 必要 | なし |
| **インストール** | 複雑なMSI + ドライバー | シンプルなNuGetパッケージ |
| **HTMLレンダリング** | 基本（Officeベース） | 完全なChromium（CSS3、JS、Flexbox） |
| **サーバーデプロイメント** | 対話型セッションが必要 | ヘッドレスで実行 |
| **.NETサポート** | .NET Coreのサポートが限定的 | .NET 5/6/7/8/9を完全サポート |
| **非同期サポート** | コールバックベース | ネイティブのasync/await |
| **コンテナサポート** | なし | Docker/Kubernetesを完全サポート |
| **ライセンス** | サーバーごと | 開発者ごと |

---

## 開始する前に

### 前提条件

- **.NET Framework 4.6.2+** または **.NET Core 3.1+** または **.NET 5/6/7/8/9**
- **Visual Studio 2019+** または **VS Code** にC#拡張機能がある
- **NuGetパッケージマネージャー**へのアクセス
- 移行する**既存のBCL EasyPDF SDKコードベース**

### すべてのBCL EasyPDF SDK参照を見つける

移行する前に、コードベース内のすべてのBCL EasyPDFの使用を特定します：

```bash
# すべてのBCL usingステートメントを見つける
grep -r "using BCL" --include="*.cs" .

# Printer/PDFDocumentの使用を見つける
grep -r "Printer\|PDFDocument\|PDFConverter\|HTMLConverter" --include="*.cs" .

# COMインターオプ参照を見つける
grep -r "easyPDF\|BCL.easyPDF" --include="*.csproj" .

# 設定を見つける
grep -r "PageOrientation\|TimeOut\|PrintOffice" --include="*.cs" .
```

### 予想される変更点

| BCL EasyPDFパターン | 必要な変更 |
|---------------------|-----------------|
| `new Printer()` | `ChromePdfRenderer`を使用 |
| `PrintOfficeDocToPDF()` | Office変換が異なる方法で処理される |
| `RenderHTMLToPDF()` | `RenderHtmlAsPdf()` |
| COMインターオプ参照 | 完全に削除 |
| プリンタードライバー設定 | 不要 |
| `BeginPrintToFile()`コールバック | ネイティブのasync/await |
| 対話型セッション要件 | ヘッドレスで実行 |

---

## クイックスタート（5分）

### ステップ1：BCL EasyPDF SDKを削除する

BCL EasyPDF SDKは通常、以下の方法でインストールされます：
- MSIインストーラー
- 手動DLL参照
- GAC登録

すべての参照を削除します：
1. プログラムと機能からBCL EasyPDF SDKをアンインストール
2. プロジェクトからDLL参照を削除
3. COMインターオプ参照を削除
4. 存在する場合はGACエントリをクリーンアップ

### ステップ2：IronPDFをインストール

```bash
# IronPDFをインストール
dotnet add package IronPdf
```

またはパッケージマネージャーコンソール経由で：
```powershell
Install-Package IronPdf
```

### ステップ3：名前空間を更新

```csharp
// ❌ これらを削除
using BCL.easyPDF;
using BCL.easyPDF.Interop;
using BCL.easyPDF.PDFConverter;
using BCL.easyPDF.Printer;

// ✅ これらを追加
using IronPdf;
using IronPdf.Rendering;
```

### ステップ4：最初のPDFを変換

**変換前（BCL EasyPDF SDK）：**
```csharp
using BCL.easyPDF;
using BCL.easyPDF.Interop;

Printer printer = new Printer();
printer.Configuration.TimeOut = 120;

try
{
    printer.RenderHTMLToPDF("<h1>Hello</h1>", "output.pdf");
}
finally
{
    printer.Dispose();
}
```

**変換後（IronPDF）：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.Timeout = 120000; // ミリ秒

var pdf = renderer.RenderHtmlAsPdf("<h1>Hello</h1>");
pdf.SaveAs("output.pdf");
```

---