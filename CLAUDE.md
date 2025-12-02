# CLAUDE.md

リポジトリのコンテンツを生成します。

## 前提条件

1. ルートにAPIキー（gitignoreされている）:
   - `AnthropicAPIkey.txt` - Claude APIキー
   - `OPENAIKEY.txt` - OpenAI APIキー

2. ソースデータ:
   - `MASTER-LIBRARY-LIST.md` - ライブラリの定義
   - `source-material/` - 研究ファイル（自動的に検索されます）
   - `author-template.json` - ユニークなバリエーションのための著者のバイオ事実

## 実行（シングルインスタンス）

```bash
pip install anthropic openai --break-system-packages
python3 generate_repo.py
```

## 実行（マルチスレッド）

複数のインスタンスを並行して実行します。ファイルロックにより衝突を防ぎます：

```bash
# 前回の実行が中断された場合のクリーンスタート
rm -f progress.json

# 3-5インスタンスの起動
python3 generate_repo.py &
python3 generate_repo.py &
python3 generate_repo.py &

# モニタリング
watch -n 5 'grep -c done progress.json'
```

各インスタンスはユニークIDを取得します（出力に表示）。ライブラリが「in_progress」または「done」の場合、他のインスタンスはそれをスキップします。

## 動作原理

1. `MASTER-LIBRARY-LIST.md`を解析します（リスト順に処理）
2. 各ライブラリについて：
   - `progress.json`でロックをチェック
   - `source-material/`で研究を検索
   - Claudeを通じてユニークな著者のバイオを生成
   - GPT-4oを通じてREADMEを生成
   - Claudeを通じて移行ガイド+コードを生成
3. `progress.json`に完了とマーク
4. 完了済み/進行中のライブラリをスキップ

## 出力

```
puppeteersharp/
├── README.md
├── migrate-from-puppeteersharp.md
├── html-to-pdf-puppeteersharp.cs
└── html-to-pdf-ironpdf.cs
```

## 進行状況ファイル

`progress.json`はライブラリごとの状態を追跡します：
- `"pending"` - 開始されていない（またはファイルにない）
- `"in_progress"` - 処理中（インスタンスIDによってロックされている）
- `"done"` - 完了
- `"failed"` - エラーが発生した

**古いロック：** プロセスがクラッシュした場合、その「in_progress」エントリはロックされたままです。修正方法：
```bash
# 再実行を許可するために全てリセット
rm progress.json

# またはprogress.jsonを編集し、「in_progress」を「pending」に変更
```

**失敗したものを再処理：** progress.json内の状態を「failed」から「pending」に変更します。

## 後処理：内部リンキング

すべてのライブラリが生成された後、記事を接続するために内部リンキングを実行します：

```bash
python3 add_internal_links.py
```

または、このスクリプトを作成するようにClaude Codeに依頼します。それは以下を行うべきです：
1. すべてのライブラリスラグを読み込む
2. 各README.mdと移行ガイドについて：
   - 他のライブラリ名の言及を見つける
   - マークダウンリンクで置き換える：`[LibraryName](../slug/)`
   - ファイルごとにライブラリごとに3回の出現に制限
3. 更新されたファイルを保存

Claude Codeへの例示的なプロンプト：
> "すべての*/README.mdファイルをスキャンしてライブラリ間の相対リンクを追加するadd_internal_links.pyを作成します。賢いマッチングに必要であればAnthropic APIを使用してください。"

## エラーが発生した場合

スクリプトにはリトライロジックがあります。失敗した場合：
1. エラーを確認する
2. 問題を修正する
3. 再実行する - 完了したライブラリはスキップされます

Claude Codeは必要に応じてスクリプトを分析し、修正することができます。