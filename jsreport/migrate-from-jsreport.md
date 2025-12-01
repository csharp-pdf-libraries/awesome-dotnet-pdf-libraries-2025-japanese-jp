---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [jsreport/migrate-from-jsreport.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/jsreport/migrate-from-jsreport.md)
🇯🇵 **日本語:** [jsreport/migrate-from-jsreport.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/jsreport/migrate-from-jsreport.md)

---

# jsreportからIronPDFへの移行方法は？

## 目次
1. [jsreportから移行する理由](#jsreportから移行する理由)
2. [開始前に](#開始前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## jsreportから移行する理由

### jsreportの課題

jsreportは、純粋な.NET環境には属さない複雑さを導入します：

1. **Node.jsの依存関係**：Node.jsのランタイムとバイナリが必要で、インフラの複雑さが増します
2. **外部バイナリの管理**：プラットフォーム固有のバイナリ（Windows、Linux、OSX）をダウンロードして管理する必要があります
3. **別のサーバープロセス**：ユーティリティまたはWebサーバーとして実行される—追加のプロセス管理が必要
4. **JavaScriptテンプレート**：Handlebars、JsRender、または他のJSテンプレートシステムの学習を強制されます
5. **複雑なリクエスト構造**：`RenderRequest`が冗長で、`Template`オブジェクトがネストされています
6. **ライセンスの制限**：無料層はテンプレート数を制限し、スケーリングには商用ライセンスが必要です
7. **ストリームベースの出力**：手動でファイル操作が必要なストリームを返します

### IronPDFの利点

| 機能 | jsreport | IronPDF |
|---------|----------|---------|
| ランタイム | Node.js + .NET | 純粋な.NET |
| バイナリ管理 | 手動 (jsreport.Binaryパッケージ) | 自動 |
| サーバープロセス | 必要 (ユーティリティまたはWebサーバー) | プロセス内 |
| テンプレート | JavaScript (Handlebarsなど) | C# (Razor、文字列補間) |
| APIスタイル | 冗長なリクエストオブジェクト | クリーンな流暢なメソッド |
| 出力 | ストリーム | PdfDocumentオブジェクト |
| PDF操作 | 限定的 | 広範囲 (マージ、分割、編集) |
| 非同期サポート | 主要 | 同期および非同期の両方 |

### 移行の利点

- **Node.jsを排除**：Node.jsのバイナリやバージョンを管理する必要がなくなります
- **よりシンプルなAPI**：20行以上を3-5行のコードに置き換えます
- **ネイティブC#**：JavaScriptの代わりにC#テンプレートを使用します
- **プロセス内**：サーバー管理や起動遅延がありません
- **豊富なPDF操作**：完全なマージ、分割、ウォーターマーク、フォーム、セキュリティサポート
- **より小さいフットプリント**：単一のNuGetパッケージ、プラットフォーム固有のバイナリなし

---

## 開始前に

### 前提条件

1. **.NET環境**：.NET Framework 4.6.2+または.NET Core 3.1+ / .NET 5+
2. **NuGetアクセス**：NuGetパッケージをインストールする能力
3. **IronPDFライセンス**：無料試用版または購入したライセンスキー

### インストール

```bash
# jsreportパッケージを削除
dotnet remove package jsreport.Binary
dotnet remove package jsreport.Binary.Linux
dotnet remove package jsreport.Binary.OSX
dotnet remove package jsreport.Local
dotnet remove package jsreport.Types
dotnet remove package jsreport.Client

# IronPDFをインストール
dotnet add package IronPdf
```

### ライセンス設定

```csharp
// アプリケーションの起動時に追加 (Program.csまたはStartup.cs)
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### jsreportの使用を特定する

```bash
# すべてのjsreport参照を検索
grep -r "using jsreport\|LocalReporting\|RenderRequest\|RenderAsync" --include="*.cs" .
grep -r "JsReportBinary\|Template\|Recipe\|Engine\." --include="*.cs" .
```

---