# GotenbergからIronPDFへの移行方法は？

## 目次

1. [GotenbergからIronPDFへ移行する理由](#gotenbergからironpdfへ移行する理由)
2. [始める前に](#始める前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンス比較](#パフォーマンス比較)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## GotenbergからIronPDFへ移行する理由

### Gotenbergのアーキテクチャ問題

Gotenbergは、PDF生成のためのDockerベースのマイクロサービスアーキテクチャです。強力で柔軟性がありますが、重要な複雑さを導入します：

1. **インフラストラクチャのオーバーヘッド**：Docker、コンテナオーケストレーション（Kubernetes/Docker Compose）、サービスディスカバリ、ロードバランシングが必要
2. **ネットワーク遅延**：すべてのPDF操作には別のサービスへのHTTP呼び出しが必要で、リクエストごとに10-100ms以上が追加される
3. **コールドスタートの問題**：コンテナの起動には最初のリクエストに2-5秒を追加することがあり、ウォームコンテナでもネットワークオーバーヘッドがある
4. **運用の複雑さ**：コンテナの健康、スケーリング、ログ、監視を別の関心事として管理する必要がある
5. **障害点**：ネットワークタイムアウト、サービスの利用不可、コンテナのクラッシュがすべて問題になる
6. **マルチパートフォームデータ**：すべてのリクエストにはmultipart/form-dataペイロードの構築が必要で、冗長でエラーが発生しやすい
7. **バージョン管理**：Gotenbergイメージはアプリケーションとは別に更新されるため、APIの変更が統合を壊す可能性がある
8. **リソースの分離**：コンテナ内で別のChromiumインスタンスを実行すると、大量のメモリを消費する

### 代わりにIronPDFが提供するもの

| アスペクト | Gotenberg | IronPDF |
|--------|-----------|---------|
| デプロイメント | Dockerコンテナ + オーケストレーション | 単一のNuGetパッケージ |
| 遅延 | ネットワーク往復（10-100ms+） | プロセス内（< 1msオーバーヘッド） |
| コールドスタート | コンテナの初期化（2-5秒） | 最初のレンダリングのみ（約1-2秒） |
| インフラストラクチャ | Docker、Kubernetes、ロードバランサー | 不要 |
| 障害モード | ネットワーク、コンテナ、サービスの障害 | 標準の.NET例外 |
| APIスタイル | REST multipart/form-data | ネイティブC#メソッド呼び出し |
| スケーリング | 水平（コンテナを増やす） | 垂直（プロセス内） |
| リソース管理 | 別のコンテナリソース | アプリケーションリソースを共有 |
| デバッグ | 分散トレースが必要 | 標準デバッガー |
| バージョン管理 | コンテナイメージタグ | NuGetパッケージバージョン |

---

## 始める前に

### 前提条件

1. **.NETバージョン**：IronPDFは.NET Framework 4.6.2+および.NET Core 3.1+ / .NET 5+をサポート
2. **ライセンスキー**：[IronPDFウェブサイト](https://ironpdf.com/licensing/)から入手
3. **インフラストラクチャの削除**：移行後にGotenbergコンテナを廃止する計画

### Gotenbergの使用状況を特定する

コードベース内のすべてのGotenberg API呼び出しを見つける：

```bash
# Gotenbergへの直接HTTP呼び出しを検索
grep -r "gotenberg\|/forms/chromium\|/forms/libreoffice\|/forms/pdfengines" --include="*.cs" .

# GotenbergSharpApiClientの使用を検索
grep -r "GotenbergSharpClient\|Gotenberg.Sharp\|ChromiumRequest\|MergeBuilder" --include="*.cs" .

# Docker/KubernetesのGotenberg設定を検索
grep -r "gotenberg/gotenberg\|gotenberg:" --include="*.yml" --include="*.yaml" .
```

### 依存関係の監査

使用しているGotenbergクライアントライブラリを確認する：

```bash
# NuGetパッケージを確認
grep -r "Gotenberg.Sharp.API.Client" --include="*.csproj" .

# package.jsonまたは他の設定を確認
grep -r "gotenberg" --include="*.json" .
```

---