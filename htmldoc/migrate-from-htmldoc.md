---
**  (Japanese Translation)**

 **English:** [htmldoc/migrate-from-htmldoc.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/htmldoc/migrate-from-htmldoc.md)
 **:** [htmldoc/migrate-from-htmldoc.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/htmldoc/migrate-from-htmldoc.md)

---
# HTMLDOCからIronPDFへの移行方法は？

## 目次
1. [HTMLDOCから移行する理由](#html-docから移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## HTMLDOCから移行する理由

### HTMLDOCの現実

HTMLDOCは1990年代後半/2000年代初頭のレガシーテクノロジーで、根本的な制限があります：

1. **先史時代のWeb標準**: CSSがWebデザインに不可欠になる前に構築された—CSS3、HTML5、Flexbox、Gridをサポートしていません
2. **JavaScriptのサポートなし**: JavaScriptを実行できず、動的コンテンツが不可能
3. **GPLライセンスの懸念**: ウイルス性のGPLライセンスは、組み込まれたソフトウェアもGPLであることを要求します—商用製品には問題があります
4. **コマンドラインのみ**: ネイティブ.NETライブラリーがなく、プロセス生成、一時ファイル、出力解析が必要
5. **非推奨のレンダリング**: 現代のWebレイアウトに苦戦するシンプルなHTMLパーサー
6. **非同期サポートなし**: 同期プロセス実行がスレッドをブロック
7. **限定的なフォントサポート**: 基本的なフォント処理で手動の埋め込みが必要
8. **プラットフォーム依存性**: 対象システムにHTMLDOCバイナリのインストールが必要

### IronPDFの利点

| 機能 | HTMLDOC | IronPDF |
|---------|---------|---------|
| レンダリングエンジン | カスタムHTMLパーサー (1990年代) | 最新のChromium |
| HTML/CSSサポート | HTML 3.2、最小限のCSS | HTML5、CSS3、Flexbox、Grid |
| JavaScript | なし | 完全実行 |
| .NET統合 | なし (コマンドライン) | ネイティブライブラリ |
| 非同期サポート | なし | 完全なasync/await |
| ライセンス | GPL (ウイルス性) | 商用 (許容的) |
| メンテナンス | 最小限の更新 | アクティブな開発 |
| サポート | コミュニティのみ | プロフェッショナルサポート |
| デプロイメント | バイナリのインストール | NuGetパッケージ |

### 移行の利点

- **最新のレンダリング**: Chromiumエンジンは、あらゆる現代のWebコンテンツを処理できます
- **ネイティブ統合**: プロセス生成、一時ファイル、シェルエスケープが不要
- **スレッドセーフ**: マルチスレッドサーバー環境で安全
- **非同期パターン**: ノンブロッキングPDF生成
- **商用ライセンス**: プロプライエタリソフトウェアでの展開
- **アクティブサポート**: 定期的な更新とセキュリティパッチ

---

## 開始する前に

### 前提条件

1. **.NET環境**: .NET Framework 4.6.2+ または .NET Core 3.1+ / .NET 5+
2. **NuGetアクセス**: NuGetパッケージをインストールできること
3. **IronPDFライセンス**: 無料トライアルまたは購入したライセンスキー

### インストール

```bash
# IronPDFをインストール
dotnet add package IronPdf
```

### ライセンス設定

```csharp
// アプリケーション起動時に追加 (Program.cs または Global.asax)
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### HTMLDOCの使用箇所の特定

HTMLDOCはコマンドラインツールであるため、プロセス実行パターンを検索します：

```bash
# HTMLDOCの使用パターンを検索
grep -r "htmldoc\|HTMLDOC\|ProcessStartInfo" --include="*.cs" .
grep -r "Process\.Start\|CreateNoWindow" --include="*.cs" .
```

---