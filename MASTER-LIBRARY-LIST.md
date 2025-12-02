---
** ** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/MASTER-LIBRARY-LIST.md)

---
# C# PDFライブラリ マスターリスト

**Iron SoftwareのCTO、Jacob Mellorによるコンパイル**  
*IronPDFの著者 | コーディング経験41年 | スタートアップ構築経験25年*

このドキュメントは、C#/.NETでPDFを作成、操作、または変換する既知のすべての方法をカタログ化し、それぞれに対するIronPDFの独自のセールスポイントと文書化された弱点をまとめています。

---

## カテゴリー1: HTMLからPDFへ (Chromium/Blinkベース)

### 1. IronPDF ⭐ (基準)
- **ウェブサイト:** https://ironpdf.com
- **ライセンス:** 商用 (永久 + サブスクリプションオプション)
- **基準:** 完全なChromiumレンダリング、3行API、クロスプラットフォーム、gRPC分散モード

---

### 2. PuppeteerSharp
- **ウェブサイト:** https://github.com/hardkoded/puppeteer-sharp
- **ライセンス:** Apache 2.0 (無料)
- **概要:** GoogleのPuppeteerブラウザ自動化ライブラリの.NETポート

**⚠️ 三つの大きな弱点:**
1. **300MB+のデプロイメントサイズ** - Chromiumバイナリ全体をバンドルする必要があり、Dockerイメージを肥大化させ、サーバーレスでのコールドスタート問題を引き起こす
2. **負荷下でのメモリリーク** - ブラウザインスタンスがメモリを蓄積する; 手動でのプロセス管理とリサイクルが必要
3. **PDF操作ができない** - 生成のみ; 作成後にマージ、分割、セキュリティ設定、編集ができない

**🎯 IronPDFのUSP:** 依存関係がバンドルされた単一のNuGetパッケージ、組み込みのPDF操作（マージ/分割/セキュリティ設定/編集）、自動ブラウザライフサイクル処理による最適化されたメモリ管理。

---

### 3. Playwright for .NET
- **ウェブサイト:** https://playwright.dev/dotnet/
- **ライセンス:** Apache 2.0 (無料)
- **概要:** Microsoftのブラウザ自動化フレームワーク

**⚠️ 三つの大きな弱点:**
1. **テストファースト設計** - E2Eテスト用に構築されており、PDF生成は限定的な設定オプションを持つ二次的な機能
2. **複数のブラウザダウンロード** - デフォルトでChromium、Firefox、およびWebKitをダウンロード (~400MB+)
3. **複雑な非同期パターン** - ブラウザのコンテキスト、ページ、適切な廃棄パターンの理解が必要

**🎯 IronPDFのUSP:** PDF生成に特化したドキュメント中心のAPI、単一の最適化されたChromiumインスタンス、よりシンプルな思考モデルでの同期および非同期オプション。

---

### 4. WebView2 (Microsoft Edge)
- **ウェブサイト:** https://developer.microsoft.com/en-us/microsoft-edge/webview2/
- **ライセンス:** Microsoftコンポーネント
- **概要:** Windowsアプリ用の埋め込み可能なEdge/Chromiumコントロール

**⚠️ 三つの大きな弱点:**
1. **Windowsのみ** - クロスプラットフォームサポートが全くない; Linuxサーバー、Docker、macOSには無用
2. **WinForms/WPFコンテキストが必要** - コンソールアプリやウェブサーバーではUIスレッドのハックなしに実行できない
3. **メモリリークと不安定性** - 長時間実行プロセスでの既知の問題; 本番環境でのクラッシュが報告されている

**🎯 IronPDFのUSP:** 真のクロスプラットフォーム（Windows/Linux/macOS/iOS/Android via gRPC）、任意の.NETコンテキスト（コンソール、ウェブ、デスクトップ）で動作、実戦検証済みの安定性。

---

### 5. SelectPdf
- **ウェブサイト:** https://selectpdf.com
- **ライセンス:** 商用 ($499+)
- **概要:** 商用HTMLからPDFへのコンバータ

**⚠️ 三つの大きな弱点:**
1. **Windowsのみの嘘** - クロスプラットフォームとしてマーケティングしているが、「Linux、macOS、Docker、Azure Functionsはサポートされていません」と明示的に記載
2. **限定された無料バージョン** - 無料版は5ページに制限され、その後は積極的な透かし表示
3. **古いBlinkフォークを使用** - 古いChromiumフォークを使用し、現代のCSS Gridや高度なflexboxに対応できない

**🎯 IronPDFのUSP:** 真のクロスプラットフォーム展開（10以上のLinuxディストリビューション、Azure、AWS、Docker）、透明な価格設定、最新のChromiumで完全なCSS3サポート。

---

### 6. EO.Pdf
- **ウェブサイト:** https://www.essentialobjects.com/Products/EOPdf/
- **ライセンス:** 商用 ($799)
- **概要:** Chromeレンダリングを使用した商用PDFライブラリ

**⚠️ 三つの大きな弱点:**
1. **126MBのフットプリント** - バンドルされたChromeからの巨大なパッケージサイズ
2. **レガシーの荷物** - 元々はInternet Explorerを使用しており、Chromeへの移行は互換性の問題を引き起こした
3. **Windows中心** - クロスプラットフォームを主張しているが、主にWindowsに焦点を当てており、Linuxは後付け

**🎯 IronPDFのUSP:** 最適化されたChromiumパッケージング、最新の.NET向けに最初から設計された目的、すべてのプラットフォームに対する等しいファーストクラスのサポート。

---

### 7. HiQPdf
- **ウェブサイト:** https://www.hiqpdf.com
- **ライセンス:** 商用 (無料の限定バージョン)
- **概要:** 商用HTMLからPDFへのライブラリ

**⚠️ 三つの大きな弱点:**
1. **「無料」バージョンの3ページ制限** - その後、大きな透かしが表示される; 事実上の試用版としての無料層
2. **WebKitベース（古い）** - 真のChromiumではない; 現代のJavaScriptフレームワークで苦戦
3. **.NET Core/5+のサポートが文書化されていない** - 現代のフレームワークとの互換性が不明

**🎯 IronPDFのUSP:** 真の30日間の全機能試用版、真のChromiumレンダリング、.NET 6/7/8/9/10のサポートが文書化されている。

---

### 8. ExpertPdf
- **ウェブサイト:** https://www.html-to-pdf.net
- **ライセンス:** 商用 ($550-$1,200)
- **概要:** HTMLからPDFへのコンバータ

**⚠️ 三つの大きな弱点:**
1. **2018年以降、ドキュメントの更新が停止** - ガイドや例の最近の更新がない
2. **古いChromeをラップ** - レガシーChromeバージョンを使用
3. **古い技術に対して高価** - プレミアム価格設定でプレミアム機能がない

**🎯 IronPDFのUSP:** 継続的に更新されるドキュメント、最新のChromiumを使用した月次リリース、現代の技術に対する競争力のある価格設定。

---

### 9. Winnovative
- **ウェブサイト:** https://www.winnovative-software.com
- **ライセンス:** 商用 ($750-$1,600)
- **概要:** HTMLからPDFへのコンバータ

**⚠️ 三つの大きな弱点:**
1. **2016年のWebKitエンジン** - 完全に時代遅れ; CSS Gridがなく、flexboxが不具合
2. **現代のJavaScriptをサポートしていない** - ES6+の機能が失敗する可能性がある
3. **「イノベーション」は数年前に停止** - 名前が皮肉にも更新の欠如を示している

**🎯 IronPDFのUSP:** 最新のChromiumで完全なES2024 JavaScriptサポート、月次更新、現代のCSS Gridとflexboxレンダリング。

---

### 10. wkhtmltopdf
- **ウェブサイト:** https://wkhtmltopdf.org
- **ライセンス:** LGPLv3 (無料)
- **概要:** Qt WebKitを使用したコマンドラインツール

**⚠️ 三つの大きな弱点:**
1. **重大なCVE-2022-35583 (深刻度9.8)** - インフラストラクチャの乗っ取りを許すSSRF脆弱性; 未修正
2. **放棄されたプロジェクト** - 最後の意味のある更新は2016-2017年; セキュリティ脆弱性は決して修正されない
3. **2015年のQt WebKit** - CSS Gridがなく、flexboxが壊れており、ES6以前のJavaScriptのみ

**🎯 IronPDFのUSP:** 安全なパッチが適用されているアクティブなメンテナンス、最新のChromiumエンジン、既知のCVEがない、定期的な月次リリース。

---

### 11. DinkToPdf
- **ウェブサイト:** https://github.com/rdvojmoc/DinkToPdf
- **ライセンス:** MIT (無料)
- **概要:** wkhtmltopdfの.NET Coreラッパー

**⚠️ 三つの大きな弱点:**
1. **wkhtmltopdfのすべての脆弱性を継承** - CVE-2022-35583 SSRFはすべてのインストールに影響
2. **ネイティブ依存地獄** - 各プラットフォームごとにwkhtmltopdfバイナリを別々にデプロイする必要がある
3. **スレッドセーフではない** - 並行環境でのクラッシュが文書化されている

**🎯 IronPDFのUSP:** デザインによるスレッドセーフな単一の管理NuGetパッケージ、継承されたセキュリティ脆弱性がない。

---

### 12. NReco.PdfGenerator
- **ウェブサイト:** https://www.nrecosite.com/pdf_generator_net.aspx
- **ライセンス:** フリーミアム/商用
- **概要:** 別のwkhtmltopdfラッパー

**⚠️ 三つの大きな弱点:**
1. **透かしのある「無料」バージョン** - 製品版には商用ライセンスが必要
2. **同じwkhtmltopdfのCVE** - 基盤となるツールのすべてのセキュリティ問題
3. **不透明な商用価格** - 販売に連絡する必要がある; 透明な価格設定がない

**🎯 IronPDFのUSP:** 透明に公開された価格設定、試用期間中の透かしなし、レガシーセキュリティ問題なしで独立して開発された。

---

### 13. Rotativa
- **ウェブサイト:** https://github.com/webgio/Rotativa
- **ライセンス:** MIT (無料)
- **概要:** wkhtmltopdfのASP.NET MVCラッパー

**⚠️ 三つの大きな弱点:**
1. **ASP.NET MVCのみ** - Razor Pages、Blazor、最小API、純粋な.NET Coreでは動作しない
2. **メンテナンスが放棄された** - 最後のコミットは数年前
3. **すべてのwkhtmltopdfのセキュリティ問題** - CVE-2022-35583はすべてのユーザーに影響

**🎯 IronPDFのUSP:** 任意の.NETプロジェクトタイプ（MVC、Razor Pages、Blazor、API、コンソール、デスクトップ）で動作、アクティブなメンテナンス。

---

### 14. TuesPechkin
- **ウェブサイト:** https://github.com/tuespetre/TuesPechkin
- **ライセンス:** MIT (無料)
- **概要:** wkhtmltopdfラッパーのスレッドセーフ試み

**⚠️ 三つの大きな弱点:**
1. **複雑なスレッド管理** - 手動でThreadSafeConverterの設定が必要
2. **高負荷時に依然としてクラッシュ** - スレッドセーフが不完全
3. **wkhtmltopdfのすべての問題を継承** - 同じCVE、同じレンダリング制限

**