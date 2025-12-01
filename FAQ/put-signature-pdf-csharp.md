---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/put-signature-pdf-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/put-signature-pdf-csharp.md)
🇯🇵 **日本語:** [FAQ/put-signature-pdf-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/put-signature-pdf-csharp.md)

---

# C#でPDFにデジタル署名と手書き署名を追加する方法は？

C#でPDFにデジタルまたはビジュアル署名を追加することは、思っているよりも簡単です。これにより、契約の承認、ドキュメントのワークフロー、コンプライアンスが劇的に合理化されます。このFAQでは、暗号署名の作成、手書き署名画像のオーバーレイ、記入可能な署名フィールドの設定、バッチ署名、証明書管理、規制コンプライアンスなどの高度な要件の処理方法について学びます。すべての例では、開発者向けに構築された人気のあるC# PDFライブラリである[IronPDF](https://ironpdf.com)を使用しています。

---

## C#で使用できるPDF署名の主なタイプは何ですか？

PDF署名は一般的に3つのカテゴリーに分類され、ニーズに応じて個別にまたは組み合わせて使用できます：

1. **デジタル（暗号）署名：**  
   これらは見えないものですが、デジタル証明書を使用してPDFにあなたの身元を安全に結びつけます。最も強力な法的証拠と改ざん検出を提供します。

2. **可視署名画像：**  
   署名のスキャンまたは描画された画像で、ページ上に視覚的に配置されます。まるで紙にペンで書いたように。

3. **インタラクティブ署名フィールド：**  
   受信者が後で自分のデジタル署名を入力できる空白のフィールドで、Adobe Readerや類似のツールを使用します。

こちらはIronPDFを使用してすべての3つを行う実用的なC#スニペットです：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using IronPdf.Forms;
using System.Drawing;

// 既存のPDFをロード
var pdf = PdfDocument.FromFile("document.pdf");

// 1. デジタル署名（見えない）
pdf.SignWithFile("mycert.pfx", "certPassword");

// 2. 可視署名画像を追加
using (var signImg = new Bitmap("signature.png"))
{
    pdf.DrawBitmap(signImg, 400, 120, 0, 180, 60);
}

// 3. 他の人が署名するための署名フィールドを挿入
var sigField = new SignatureFormField
{
    Name = "recipient_signature",
    PageIndex = 0,
    X = 100,
    Y = 180,
    Width = 220,
    Height = 60
};
pdf.Form.Fields.Add(sigField);

pdf.SaveAs("signed-and-ready.pdf");
```

署名前にXMLやXAMLなどのソースからPDFを生成する方法に興味がある場合は、[C#でXMLをPDFに変換する方法は？](xml-to-pdf-csharp.md)と[.NET MAUIでXAMLからPDFを生成する方法は？](xaml-to-pdf-maui-csharp.md)をご覧ください。

---

## C#で証明書を使用してPDFにデジタル署名する方法は？

PDFにデジタル署名を付けると、著者の暗号学的証拠が添付され、改ざん検出が保証されます。こちらはPFX証明書を使用して素早く行う方法です：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf

var pdf = PdfDocument.FromFile("contract.pdf");
pdf.SignWithFile("signing-cert.pfx", "password123");
pdf.SaveAs("contract-signed.pdf");
```

### PDFにデジタル署名をするとどうなりますか？

IronPDFはドキュメントの安全なハッシュを作成し、あなたの秘密鍵でそれを暗号化し、PDFに埋め込みます。署名後にPDFが変更されると、署名は無効になります。

### PDF署名用の証明書はどこで入手できますか？

PDFにデジタル署名をするには、証明書（PFXファイル）とそのパスワードが必要です。主に2つのオプションがあります：

#### 証明書局（CA）による署名証明書を購入すべきですか？

- **最適な用途：** 外部契約、顧客向けドキュメント、法的およびコンプライアンスのニーズ。
- **ベンダー：** DigiCert、GlobalSign、Sectigoなど。
- **メリット：** Adobeおよびほとんどの主要なPDFビューアに信頼されています。
- **デメリット：** 身元確認と年間コストが発生します。

#### 自己署名証明書で十分ですか？

- **最適な用途：** 社内使用、テスト、開発、またはリスクの低いドキュメント。
- **メリット：** 無料で即座に生成できます。
- **デメリット：** 組織外の受信者には信頼されません（Adobeは警告を表示します）。

##### Windowsで自己署名証明書を作成するにはどうすればよいですか？

PowerShellを使用して自分の証明書を作成します：

```powershell
$cert = New-SelfSignedCertificate -Subject "CN=MyApp" -Type CodeSigningCert
Export-PfxCertificate -Cert $cert -FilePath mycert.pfx -Password (ConvertTo-SecureString "password123" -AsPlainText -Force)
```

本番環境では、常にCA発行の証明書を好むべきです。ファイルからのPDF生成と署名についての詳細は、[C#でHTMLファイルをPDFに変換する方法は？](html-file-to-pdf-csharp.md)をご覧ください。

---

## 自己署名証明書とCA発行証明書をいつ使用すべきか？

**自己署名証明書を使用する場合：** 社内文書、プロトタイプ、または開発シナリオに適しています。ただし、受信者が証明書を手動で信頼しない限り、「信頼されていない」という警告が表示されることを覚えておいてください。

**CA署名証明書を使用する場合：** クライアント、パートナーに署名されたPDFを送信する場合や、法的記録の保持には、追加の手順なしで主流のPDFビューアでの信頼を保証する唯一の方法です。

---

## PDFに手書き署名画像を追加するにはどうすればよいですか？

PDFに署名画像をスタンプすることは、完全な暗号学的証明が必要ない場合でも、見慣れた「インク」の外観を求める場合に最適です。

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using System.Drawing;

var pdf = PdfDocument.FromFile("report.pdf");
using (var handSig = new Bitmap("my-signature.png"))
{
    pdf.DrawBitmap(handSig, x: 350, y: 80, pageIndex: 0, width: 160, height: 60);
}
pdf.SaveAs("report-with-visual-signature.pdf");
```

### ビジュアル署名画像が十分な場合は？

- 低リスクの承認や社内記録に適しています
- 個人プロジェクトや非公式の合意に
- 関係者が外観を気にする場合、法的な有効性ではなく

より強固な真正性を求める場合は、以下に示すようにデジタル署名と組み合わせてください。

---

### PDF上に署名画像を正確に配置するにはどうすればよいですか？

PDFの座標はポイントで測定されます（1ポイント = 1/72インチ）、左下が（0, 0）です。署名を右下に配置するには、次のように計算します：

```csharp
int pageW = 612;   // レターサイズの幅（ポイント）
int pageH = 792;   // レターサイズの高さ（ポイント）
int sigW = 160;
int sigH = 60;

int x = pageW - sigW - 25; // 25ptのマージン
int y = 30; // 下から30pt

using (var sigBmp = new Bitmap("sig.png"))
{
    pdf.DrawBitmap(sigBmp, x, y, 0, sigW, sigH);
}
```

PDFビューアでPDFを開き、マウスを動かして座標を確認し、完璧な配置のために必要に応じて調整してください。

---

## 1つのPDFにデジタル署名とビジュアル署名を組み合わせることはできますか？

はい！可視署名画像と暗号学的デジタル署名の両方を追加することで、見た目とセキュリティの両方を提供できます。

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using System.Drawing;

var pdf = PdfDocument.FromFile("agreement.pdf");

using (var visSig = new Bitmap("ceo-sign.png"))
{
    pdf.DrawBitmap(visSig, 380, 90, 0, 180, 70);
}

pdf.SignWithFile("ceo-cert.pfx", "ceoPassword");

pdf.SaveAs("agreement-fully-signed.pdf");
```

この方法で、PDFは署名されたように見え、また法的に保護されます。

---

## 後で他の人が記入できる署名フィールドをPDFに追加するにはどうすればよいですか？

クライアントやマネージャーにドキュメントに自分で署名してもらう必要がある場合は、インタラクティブな署名フィールドを追加します：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using IronPdf.Forms;

var pdf = PdfDocument.FromFile("blank-form.pdf");

var sigPlaceholder = new SignatureFormField
{
    Name = "customer_signature",
    PageIndex = 0,
    X = 110,
    Y = 140,
    Width = 200,
    Height = 60
};

pdf.Form.Fields.Add(sigPlaceholder);

pdf.SaveAs("blank-form-with-sig-field.pdf");
```

Adobe Readerや互換アプリで開いたとき、受信者はクリック可能な署名スロットを見ることができます。

---

## C#でWindows証明書ストアを使用してPDFに署名する方法は？

組織が証明書を中央で管理している場合や、ハードウェアトークンを使用している場合は、Windows証明書ストアからの証明書を使用して署名できます：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using System.Security.Cryptography.X509Certificates;

var pdf = PdfDocument.FromFile("secure-doc.pdf");

var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
store.Open(OpenFlags.ReadOnly);

var certs = store.Certificates.Find(X509FindType.FindBySubjectName, "MyCompany", false);
if (certs.Count == 0)
    throw new Exception("No matching certificate found.");

var cert = certs[0];
pdf.SignWithCertificate(cert);

pdf.SaveAs("store-signed.pdf");
```

これにより、機密性の高いPFXファイルの配布を避け、ITチームが簡単に鍵をローテーションできます。

---

## 署名メタデータ（理由、場所、連絡先など）を追加するにはどうすればよいですか？

署名に豊富なメタデータを追加することで、誰が、なぜ、どのように連絡を取るかを明確にすることができます：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using IronPdf.Signing;

var pdf = PdfDocument.FromFile("offer.pdf");

var sig = new PdfSignature("hr-cert.pfx", "securePass")
{
    SigningReason = "Offer accepted",
    SigningLocation = "Berlin HQ",
    SigningContactInfo = "hr@example.com"
};

pdf.Sign(sig);

pdf.SaveAs("offer-signed.pdf");
```

署名されたPDFをAdobeで開くと、署名パネルにこれらの詳細が表示されます。

---

## 複数の署名や複数当事者のドキュメントをどのように処理すればよいですか？

複数の署名者がいる合意には、各署名を順番に追加します：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using System.Drawing;

var pdf = PdfDocument.FromFile("partnership.pdf");

// CEOの署名
using (var ceo = new Bitmap("ceo-sign.png"))
{
    pdf.DrawBitmap(ceo, 120, 160, 0, 170, 60);
    pdf.SignWithFile("ceo-cert.pfx", "ceoPass");
}

// CFOの署名、インクリメンタルに追加
using (var cfo = new Bitmap("cfo-sign.png"))
{
    pdf.DrawBitmap(cfo, 350, 160, 0, 170, 60);
    pdf.SignWithFile("cfo-cert.pfx", "cfoPass");
}

pdf.SaveAs("fully-signed-partnership.pdf");
```

**ヒント：** 新しい署名を追加する前に、常に最新のPDF（すべての以前の署名が含まれている）を使用してください。元の未署名のドキュメントから始めないでください。

---

## 複数のPDFを自動的に一括署名するにはどうすればよいですか？

数百のドキュメントを処理する必要がある場合は、ファイルをループして署名ロジックを適用します：

```csharp
using IronPdf; // NuGet: Install-Package IronPdf
using System.IO;

string certPath = "company.pfx";
string certPass = Environment.GetEnvironmentVariable("PDF_CERT_PASS");

var inputDir = "to-sign";
var outputDir = "signed-files";
Directory.CreateDirectory(outputDir);

foreach (var file in Directory.GetFiles(inputDir, "*.pdf"))
{
    var pdf = PdfDocument