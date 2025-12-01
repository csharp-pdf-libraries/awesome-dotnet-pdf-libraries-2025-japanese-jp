---
**🌐 日本語版** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/wkhtmltopdf-security-risks.md)

---

# wkhtmltopdfを置き換えるべき理由：重大なセキュリティ脆弱性と現代のPDF生成への道

## エグゼクティブサマリー

**wkhtmltopdfは放棄され、安全でなく、サーバーに対する重大な攻撃ベクトルを作り出します。**

お使いの組織がwkhtmltopdfを直接使用している場合、またはDinkToPdf、Rotativa、TuesPechkin、HaukCodeDinkToPdfなどのラッパーを介して使用している場合、以下の問題を抱えたソフトウェアを実行しています。

- **CVE-2022-35583**: 修正されることのないCRITICAL（CVSS 9.8）SSRF脆弱性
- **2020年以降放棄されている**: セキュリティ更新、バグ修正、サポートなし
- **2015年の古いWebKitエンジン**: 現代のウェブ標準に対応していない
- **既知の攻撃ベクトル**: SSRF、LFI、および標的型攻撃の初期アクセスで積極的に悪用されている

この記事では、リスクを説明し、IronPDFへの明確な移行方法を提供します。

---

## 目次

1. [wkhtmltopdfの現状](#wkhtmltopdfの現状)
2. [重大なセキュリティ脆弱性](#重大なセキュリティ脆弱性)
3. [攻撃シナリオ](#攻撃シナリオ)
4. [あなたの組織にとってこれが重要な理由](#あなたの組織にとってこれが重要な理由)
5. [影響を受けるツールとライブラリ](#影響を受けるツールとライブラリ)
6. [解決策：IronPDFへの移行](#解決策ironpdfへの移行)
7. [移行ガイド](#移行ガイド)
8. [追加リソース](#追加リソース)

---

## wkhtmltopdfの現状

### 公式の放棄

wkhtmltopdfは、そのメンテナーによって公式に放棄されました。[公式ステータスページ](https://wkhtmltopdf.org/status.html)より：

> "このプロジェクトは放棄され、現在は安全ではありません。"

プロジェクトの衰退は以下の時点で始まりました：

1. **2013年**: GoogleがWebKitをBlinkにフォーク
2. **2015年**: QtがQtWebKitを非推奨に
3. **2016年**: QtがQtWebKitを完全に削除
4. **2020年**: 最後のwkhtmltopdfリリース（0.12.6）
5. **2022年**: 修正されない重大なCVE-2022-35583が公表された

### 放棄された理由

wkhtmltopdfはQtWebKitに依存しており、QtWebKit自体が放棄されました：

- QtWebKitは非推奨になった後、何年も「生命維持装置」にあった
- JavaScriptエンジンはES6をサポートしておらず、現代のウェブコンテンツを破壊する
- レンダリングエンジンは2015年頃のWebKitに基づいており、現代のブラウザより10年遅れている
- FlexboxやGridのようなCSS3機能との互換性がない
- JavaScriptの実行が一貫していない

### 「放棄されたソフトウェア」の分類

wkhtmltopdfは現在、**放棄されたソフトウェア**として分類されています。これは、開発者によって更新、維持、またはサポートされていないソフトウェアを意味します。これは以下を意味します：

- 新たに発見された脆弱性に対するセキュリティパッチがない
- バグ修正がない
- 互換性の更新がない
- 悪用を監視する人がいない

---

## 重大なセキュリティ脆弱性

### CVE-2022-35583: サーバーサイドリクエストフォージェリ（SSRF）

| 属性 | 値 |
|-----------|-------|
| **CVE ID** | CVE-2022-35583 |
| **CVSSスコア** | **9.8 CRITICAL** |
| **ステータス** | **未修正** |
| **影響を受けるバージョン** | 0.12.6（最新）を含むすべてのバージョン |

#### 説明

[GitHubアドバイザリーデータベース](https://github.com/advisories/GHSA-v2fj-q75c-65mr)より：

> "wkhtmlTOpdf 0.12.6はSSRFに対して脆弱であり、攻撃者が初期アセットIPアドレスをソースに持つiframeタグを注入することで、対象のシステムへの初期アクセスを取得することができます。これにより、攻撃者は内部アセットにアクセスすることで、インフラ全体を乗っ取ることができます。"

#### 影響

攻撃者は以下を行うことができます：

1. **内部サービスへのアクセス**: ファイアウォールの背後にあるデータベース、API、管理パネルに到達する
2. **AWS認証情報の抽出**: `169.254.169.254`にアクセスしてIAMロール認証情報を盗む
3. **ローカルファイルの読み取り**: `file://`プロトコルを使用して`/etc/passwd`、プライベートキー、および設定ファイルを読み取る
4. **内部ネットワークのマッピング**: 内部サービスとIPアドレスを発見する
5. **攻撃のピボット**: 初期アクセスを使用して横方向の移動と権限昇格を行う

### ローカルファイルインクルージョン（LFI）

[Virtue Securityの分析](https://www.virtuesecurity.com/kb/wkhtmltopdf-file-inclusion-vulnerability-2/)より：

> "wkhtmltopdfはサーバーサイドでHTMLコンテンツをレンダリングするため、サーバーサイドリクエストフォージェリ（SSRF）およびローカルファイルインクルージョン（LFI）の脆弱性に対して高リスクの対象となります。"

攻撃者は任意のファイルを読むことができます：

```html
<iframe src="file:///etc/passwd" width="800" height="600"></iframe>
```

```html
<img src="x" onerror="document.write('<iframe src=file:///etc/shadow></iframe>')">
```

### 同一生成元ポリシーのバイパス

[Ubuntu Security Notice USN-6232-1](https://ubuntu.com/security/notices/USN-6232-1)より：

> "wkhtmltopdfは、特定のHTMLファイルを処理する際に同一生成元ポリシーを適切に強制していませんでした。wkhtmltopdfを使用しているユーザーまたは自動システムが、特別に作成されたHTMLファイルを処理するようにだまされた場合、攻撃者はこの問題を利用して機密情報を露呈させる可能性があります。"

---

## 攻撃シナリオ

### シナリオ1：AWS認証情報の盗難

**対象**: AWS EC2上でwkhtmltopdfを使用している任意のアプリケーション

**攻撃ベクトル**:
```html
<iframe src="http://169.254.169.254/latest/meta-data/iam/security-credentials/" width="800" height="600"></iframe>
```

**影響**:
- 攻撃者がIAMロール認証情報を取得
- ロールがアクセスできるすべてのAWSサービスへのアクセス（S3、RDS、Lambdaなど）
- クラウドインフラストラクチャの完全な妥協の可能性

### シナリオ2：内部ネットワークの発見

**対象**: PDF生成を備えた企業アプリケーション

**攻撃ベクトル**:
```html
<script>
  for (let i = 1; i <= 255; i++) {
    new Image().src = `http://192.168.1.${i}:8080/probe`;
  }
</script>
<iframe src="http://192.168.1.1:8080/admin" width="800" height="600"></iframe>
```

**影響**:
- 内部ネットワークトポロジーのマッピング
- 内部サービスの発見（Jenkins、GitLab、データベース）
- 横方向の移動のためのターゲットの発見

### シナリオ3：ランサムウェアへの初期アクセス

**対象**: パブリックフェーシングのPDF生成を備えた企業

**攻撃チェーン**:
1. 攻撃者がPDFコンバーターに悪意のあるHTMLを提出
2. SSRFが内部サービスと認証情報を明らかにする
3. 攻撃者が内部ネットワークに足場を築く
4. ドメインコントローラーへの横方向の移動
5. 組織全体にランサムウェアを展開

**これは理論的なものではありません**：SSRF脆弱性は、初期アクセスのために高度な持続的脅威によって使用される[MITRE ATT&CKテクニック（T1190）](https://attack.mitre.org/techniques/T1190/)です。

### シナリオ4：設定ファイルの抽出

**対象**: wkhtmltopdfを搭載した任意のサーバー

**攻撃ベクトル**:
```html
<iframe src="file:///var/www/app/.env" width="800" height="600"></iframe>
<iframe src="file:///app/config/database.yml" width="800" height="600"></iframe>
<iframe src="file:///root/.ssh/id_rsa" width="800" height="600"></iframe>
```

**影響**:
- データベース認証情報
- APIキー
- SSHプライベートキー
- JWTシークレット

---

## あなたの組織にとってこれが重要な理由

### コンプライアンス違反

放棄された、脆弱なソフトウェアを使用することは、以下を違反する可能性があります：

- **PCI DSS**: 要件6.2 - 既知の脆弱性からシステムコンポーネントを保護する
- **HIPAA**: ソフトウェアメンテナンスに関するセキュリティルール要件
- **SOC 2**: CC6.1 - セキュリティ脆弱性が特定され、対処される
- **ISO 27001**: A.12.6.1 - 技術的脆弱性の管理
- **GDPR**: 第32条 - 適切なセキュリティ対策

### 監査の赤旗

セキュリティ監査員は特に以下を探します：

- 既知の未修正CVEを持つソフトウェア
- 放棄された依存関係
- SSRF脆弱性を持つインターネット向けサービス

wkhtmltopdfはこれらすべての条件を満たします。

### 保険の影響

サイバー保険ポリシーはしばしば除外します：

- 既知の未修正の脆弱性から生じる侵害
- セキュリティ更新の維持に失敗
- 不注意なセキュリティ実践

wkhtmltopdfの実行は、保険の適用を無効にする可能性があります。

### 実際の露出

wkhtmltopdfは広く展開されています：

- 数百万台のLinuxサーバーにインストールされている
- 人気のフレームワーク（Rails、Django、Laravel、.NET）で使用されている
- Dockerイメージにバンドルされている
- CI/CDパイプラインで展開されている

お使いの組織には複数のインスタンスが存在する可能性が高いです。

---

## 影響を受けるツールとライブラリ

wkhtmltopdfは直接および数多くのラッパーを通じて使用されています。これらを使用している場合、脆弱性があります：

### .NET / C#

| ライブラリ | ステータス | 移行ガイド |
|---------|--------|-----------------|
| **DinkToPdf** | 放棄された（2018年） | [IronPDFへの移行](dinktopdf/migrate-from-dinktopdf.md) |
| **Rotativa** | wkhtmltopdfを使用 | [IronPDFへの移行](rotativa/migrate-from-rotativa.md) |
| **TuesPechkin** | 放棄された | [IronPDFへの移行](tuespechkin/migrate-from-tuespechkin.md) |
| **HaukCodeDinkToPdf** | DinkToPdfのフォーク | [IronPDFへの移行](haukcodedinktopdf/migrate-from-haukcodedinktopdf.md) |
| **WickedPdf**