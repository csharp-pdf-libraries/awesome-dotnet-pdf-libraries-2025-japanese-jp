---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [htmldoc/migrate-from-htmldoc.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/htmldoc/migrate-from-htmldoc.md)
🇯🇵 **日本語:** [htmldoc/migrate-from-htmldoc.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/htmldoc/migrate-from-htmldoc.md)

---

# 移行ガイド: HTMLDOC → IronPDF

## 目次
1. [HTMLDOCから移行する理由](#HTMLDOCから移行する理由)
2. [始める前に](#始める前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## HTMLDOCから移行する理由

### HTMLDOCの現実

HTMLDOCは1990年代後半/2000年代初頭のレガシーテクノロジーで、根本的な制限があります:

1. **先史時代のWeb標準**: CSSがWebデザインに不可欠になる前に構築されたため、CSS3、HTML5、Flexbox、Gridに対応していません
2. **JavaScriptのサポートなし**: JavaScriptを実行できず、動的コンテンツが不可能です
3. **GPLライセンスの懸念**: ウイルス性のGPLライセンスは、組み込まれたソフトウェアもGPLであることを要求します。商用製品にとって問題です
4. **コマンドラインのみ**: ネイティブの.NETライブラリーがなく、プロセスの生成、一時ファイル、出力の解析が必要です
5. **非推奨のレンダリング**: 簡易HTMLパーサーは、現代のWebレイアウトに苦戦します
6. **非同期サポートなし**: 同期プロセスの実行がスレッドをブロックします
7. **限定的なフォントサポート**: 基本的なフォント処理で、手動での埋め込みが必要です
8. **プラットフォーム依存性**: ターゲットシステムにHTMLDOCバイナリのインストールが必要です

### IronPDFの利点

| 機能 | HTMLDOC | IronPDF |
|---------|---------|---------|
| レンダリングエンジン | カスタムHTMLパーサー (1990年代) | 現代のChromium |
| HTML/CSSサポート | HTML 3.2、最小限のCSS | HTML5、CSS3、Flexbox、Grid |
| JavaScript | なし | 完全実行 |
| .NET統合 | なし (コマンドライン) | ネイティブライブラリ |
| 非同期サポート | なし | 完全なasync/await |
| ライセンス | GPL (ウイルス性) | 商用 (許容的) |
| メンテナンス | 最小限の更新 | アクティブな開発 |
| サポート | コミュニティのみ | プロフェッショナルサポート |
| デプロイメント | バイナリのインストール | NuGetパッケージ |

### 移行の利点

- **現代的なレンダリング**: Chromiumエンジンは、あらゆる現代のWebコンテンツを処理します
- **ネイティブ統合**: プロセスの生成、一時ファイル、シェルエスケープが不要です
- **スレッドセーフ**: マルチスレッドサーバー環境に安全です
- **非同期パターン**: ノンブロッキングPDF生成
- **商用ライセンス**: プロプライエタリソフトウェアでのデプロイが可能
- **アクティブサポート**: 定期的な更新とセキュリティパッチ

---

## 始める前に

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
// アプリケーションの起動時に追加 (Program.cs または Global.asax)
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### HTMLDOCの使用状況の特定

HTMLDOCはコマンドラインツールであるため、プロセス実行パターンを検索します:

```bash
# HTMLDOCの使用パターンを検索
grep -r "htmldoc\|HTMLDOC\|ProcessStartInfo" --include="*.cs" .
grep -r "Process\.Start\|CreateNoWindow" --include="*.cs" .
```

---