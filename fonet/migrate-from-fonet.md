---
**  (Japanese Translation)**

 **English:** [fonet/migrate-from-fonet.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/fonet/migrate-from-fonet.md)
 **:** [fonet/migrate-from-fonet.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/fonet/migrate-from-fonet.md)

---
# FoNet (FO.NET) から IronPDF への移行方法は？

## 目次
1. [IronPDF への移行理由](#ironpdf-への移行理由)
2. [開始前に](#開始前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全な API リファレンス](#完全な-api-リファレンス)
5. [XSL-FO から HTML への変換ガイド](#xsl-fo-から-html-への変換ガイド)
6. [コード例](#コード例)
7. [高度なシナリオ](#高度なシナリオ)
8. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
9. [トラブルシューティング](#トラブルシューティング)
10. [移行チェックリスト](#移行チェックリスト)

---

## IronPDF への移行理由

### FoNet (FO.NET) の課題

FoNet は XSL-FO から PDF へのレンダラーで、現代の開発において重大な制限があります：

1. **時代遅れの技術**：XSL-FO (Extensible Stylesheet Language Formatting Objects) は 2001 年の W3C 仕様で、2006 年以降更新されておらず、大部分が時代遅れと考えられています
2. **学習曲線が急**：XSL-FO は複雑な XML ベースのマークアップと特殊なフォーマットオブジェクト (fo:block, fo:table, fo:page-sequence など) の学習を要求します
3. **HTML/CSS のサポートなし**：HTML や CSS をレンダリングできず、HTML から XSL-FO マークアップへの手動変換が必要です
4. **放棄された/メンテナンスされていない**：元の CodePlex リポジトリは機能しておらず、GitHub のフォークもアクティブにメンテナンスされていません
5. **Windows のみ**：FoNet は System.Drawing に内部依存があり、Linux/macOS では動作しません
6. **限られた現代的な機能**：JavaScript のサポートがなく、CSS3、フレックスボックス/グリッド、現代的な Web フォントがありません
7. **URL レンダリング不可**：Web ページを直接レンダリングできず、HTML から XSL-FO への手動変換が必要です

### IronPDF の利点

| 項目 | FoNet (FO.NET) | IronPDF |
|--------|---------------|---------|
| 入力フォーマット | XSL-FO (時代遅れの XML) | HTML/CSS (現代の Web 標準) |
| 学習曲線 | 急 (XSL-FO の専門知識) | 緩やか (HTML/CSS の知識) |
| メンテナンス | 放棄された | 毎月アクティブにメンテナンスされている |
| プラットフォームサポート | Windows のみ | 真のクロスプラットフォーム |
| CSS サポート | なし | 完全な CSS3 (Flexbox, Grid) |
| JavaScript | なし | 完全な JavaScript サポート |
| URL レンダリング | サポートされていない | 組み込み |
| 現代的な機能 | 限定的 | ヘッダー、フッター、透かし、セキュリティ |
| ドキュメント | 時代遅れ | 包括的なチュートリアル |

### スイッチが理にかなっている理由

FoNet は XSL-FO がドキュメントフォーマットの標準になると予想された時に設計されました。それは起こりませんでした。HTML/CSS が普遍的なドキュメントフォーマットになりました：

- **開発者の 98% 以上** が HTML/CSS を知っています
- **開発者の < 1%** が XSL-FO を知っています
- ほとんどの XSL-FO リソースは 2005-2010 年からです

IronPDF を使用すると、すでに持っているスキルを使用してプロフェッショナルな PDF を作成できます。

---

## 開始前に

### 前提条件

1. **.NET 環境**：IronPDF は .NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9+ をサポートしています
2. **NuGet アクセス**：NuGet からパッケージをインストールできることを確認してください
3. **ライセンスキー**：本番使用のための IronPDF ライセンスキーを取得してください

### プロジェクトのバックアップ

```bash
# バックアップブランチを作成
git checkout -b pre-ironpdf-migration
git add .
git commit -m "FoNet から IronPDF への移行前のバックアップ"
```

### すべての FoNet 使用箇所を特定

```bash
# すべての FoNet 参照を検索
grep -r "FonetDriver\|Fonet\|\.fo\"\|xsl-region" --include="*.cs" --include="*.csproj" .

# すべての XSL-FO テンプレートファイルを検索
find . -name "*.fo" -o -name "*.xslfo" -o -name "*xsl-fo*"
```

### XSL-FO テンプレートを文書化

移行前に、すべての XSL-FO ファイルをカタログ化してください：
- ページの寸法とマージン
- 使用されているフォント
- テーブルとその構造
- ヘッダーとフッター (fo:static-content)
- ページ番号のパターン
- 画像参照

---