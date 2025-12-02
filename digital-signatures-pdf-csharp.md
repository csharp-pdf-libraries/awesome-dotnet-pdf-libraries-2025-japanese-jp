---
** ** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/digital-signatures-pdf-csharp.md)

---
# C#でのPDFへのデジタル署名：完全実装ガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![セキュリティ](https://img.shields.io/badge/Security-Digital%20Signatures-green)]()
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> デジタル署名は、PDFに法的有効性、改ざん検出、署名者認証を提供します。このガイドでは、セキュリティのベストプラクティスを交えながら、異なるC#ライブラリを使用した実装について説明します。

---

## 目次

1. [デジタル署名の重要性](#デジタル署名の重要性)
2. [ライブラリ比較](#ライブラリ比較)
3. [IronPDFでのクイックスタート](#IronPDFでのクイックスタート)
4. [証明書の種類](#証明書の種類)
5. [可視署名と不可視署名](#可視署名と不可視署名)
6. [複数の署名](#複数の署名)
7. [タイムスタンプ機関](#タイムスタンプ機関)
8. [検証](#検証)
9. [セキュリティのベストプラクティス](#セキュリティのベストプラクティス)

---

## デジタル署名の重要性

デジタル署名は以下を提供します：

1. **認証** — 文書に署名した人を確認
2. **完全性** — 署名後に文書が変更されたかどうかを検出
3. **否認防止** — 署名者は署名を否認できない
4. **法的有効性** — ほとんどの管轄区域で手書きの署名と同等

### 法的認識

| 地域 | 法律 | 状態 |
|--------|-----|--------|
| アメリカ | ESIGN Act, UETA | 法的に同等 |
| EU | eIDAS規則 | 資格のある署名が拘束力を持つ |
| 英国 | 電子通信法 | 法的に認識される |
| インド | IT法 2000 | 法的に有効 |
| オーストラリア | 電子取引法 | 法的に認識される |

---

## ライブラリ比較

### デジタル署名機能

| ライブラリ | PDF署名 | 検証 | 可視署名 | タイムスタンプ | 複数署名 | PAdES |
|---------|---------|--------|------------|-----------|---------------|-------|
| **IronPDF** | ✅ シンプル | ✅ | ✅ | ✅ | ✅ | ✅ |
| iText7 | ✅ 複雑 | ✅ | ✅ | ✅ | ✅ | ✅ |
| Aspose.PDF | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| PDFSharp | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| PuppeteerSharp | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

**重要な洞察:** PDFSharp、PuppeteerSharp、QuestPDFは文書に署名することができません。

### コードの複雑さ

**IronPDF — 3行:**
```csharp
var pdf = PdfDocument.FromFile("contract.pdf");
pdf.Sign(new PdfSignature("certificate.pfx", "password"));
pdf.SaveAs("signed-contract.pdf");
```

**iText7 — 30+行:**
```csharp
// かなり複雑 - PdfSigner、証明書チェーン、
// ダイジェストアルゴリズムの指定、署名フィールドの作成などが必要。
```

---

## IronPDFでのクイックスタート

### 基本的な署名

```csharp
using IronPdf;
using IronPdf.Signing;

// 証明書を読み込む
var signature = new PdfSignature("certificate.pfx", "password")
{
    SigningContact = "legal@company.com",
    SigningLocation = "New York, NY",
    SigningReason = "契約承認"
};

// PDFに署名
var pdf = PdfDocument.FromFile("contract.pdf");
pdf.Sign(signature);
pdf.SaveAs("signed-contract.pdf");
```

### 生成時の署名

```csharp
using IronPdf;
using IronPdf.Signing;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>契約</h1><p>条件...</p>");

// 即座に署名
pdf.Sign(new PdfSignature("certificate.pfx", "password"));
pdf.SaveAs("generated-and-signed.pdf");
```

---

## 証明書の種類

### 自己署名（開発用のみ）

```csharp
// 自己署名証明書を作成（PowerShell）
// New-SelfSignedCertificate -DnsName "dev.example.com" -CertStoreLocation "Cert:\CurrentUser\My"

// PFXにエクスポート
// $cert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object {$_.Subject -eq "CN=dev.example.com"}
// Export-PfxCertificate -Cert $cert -FilePath "dev-cert.pfx" -Password (ConvertTo-SecureString -String "password" -Force -AsPlainText)
```

### 商用証明書

本番用の信頼できる証明書機関：

| プロバイダー | タイプ | 価格 | 検証 |
|----------|------|-------|------------|
| DigiCert | 文書署名 | $449/年 | 組織 |
| GlobalSign | AATL | $329/年 | 組織 |
| SSL.com | 文書署名 | $249/年 | 組織 |
| Sectigo | 文書署名 | $299/年 | 組織 |

### Azure Key Vault統合

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using IronPdf;
using IronPdf.Signing;

// Azure Key Vaultから証明書を取得
var client = new CertificateClient(
    new Uri("https://your-vault.vault.azure.net/"),
    new DefaultAzureCredential());

var certificate = await client.DownloadCertificateAsync("pdf-signing-cert");

// IronPDFで使用
var signature = new PdfSignature(certificate.Value);
pdf.Sign(signature);
```

---

## 可視署名と不可視署名

### 不可視署名（デフォルト）

```csharp
using IronPdf;
using IronPdf.Signing;

var signature = new PdfSignature("certificate.pfx", "password");
// 視覚的な外観なし - 署名はPDFのメタデータのみに存在

var pdf = PdfDocument.FromFile("document.pdf");
pdf.Sign(signature);
pdf.SaveAs("invisibly-signed.pdf");
```

### 画像付きの可視署名

```csharp
using IronPdf;
using IronPdf.Signing;

var signature = new PdfSignature("certificate.pfx", "password")
{
    SigningContact = "ceo@company.com",
    SigningLocation = "Chicago, IL",
    SigningReason = "経営承認"
};

// 可視署名画像を追加
signature.SignatureImage = new PdfSignatureImage("signature-image.png")
{
    PageIndex = 0,  // 最初のページ
    X = 400,        // 左からの位置
    Y = 100,        // 下からの位置
    Width = 150,
    Height = 50
};

var pdf = PdfDocument.FromFile("contract.pdf");
pdf.Sign(signature);
pdf.SaveAs("visibly-signed.pdf");
```

### カスタム外観の署名

```csharp
var signature = new PdfSignature("certificate.pfx", "password");

// HTMLでカスタム外観を作成
var appearanceHtml = @"
<div style='border: 2px solid #333; padding: 10px; font-family: Arial;'>
    <p style='margin: 0;'><strong>デジタル署名者:</strong></p>
    <p style='margin: 5px 0;'>Jacob Mellor, CTO</p>
    <p style='margin: 5px 0; font-size: 10px;'>日付: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + @"</p>
    <p style='margin: 5px 0; font-size: 10px;'>理由: 契約承認</p>
</div>";

signature.SignatureImage = new PdfSignatureImage(appearanceHtml)
{
    PageIndex = pdf.PageCount - 1,  // 最後のページ
    X = 50,
    Y = 50,
    Width = 200,
    Height = 80
};
```

---

## 複数の署名

### 逐次署名（承認チェーン）

```csharp
using IronPdf;
using IronPdf.Signing;

var pdf = PdfDocument.FromFile("contract.pdf");

// 最初の署名：法務部門
var legalSignature = new PdfSignature("legal-cert.pfx", "password")
{
    SigningReason = "法務レビュー完了"
};
pdf.Sign(legalSignature);

// 二番目の署名：CFO
var cfoSignature = new PdfSignature("cfo-cert.pfx", "password")
{
    SigningReason = "財務承認"
};
pdf.Sign(cfoSignature);

// 三番目の署名：CEO
var ceoSignature = new PdfSignature("ceo-cert.pfx", "password")
{
    SigningReason = "経営承認"
};
pdf.Sign(ceoSignature);

pdf.SaveAs("fully-approved-contract.pdf");
```

### 並行署名（証人）

```csharp
// 全ての当事者が同じ元の文書に署名
var originalPdf = PdfDocument.FromFile("agreement.pdf");

// 各当事者からの署名を集める
var signatures = new[]
{
    new PdfSignature("party-a.pfx", "pass1") { SigningReason = "Party A Agreement" },
    new PdfSignature("party-b.pfx", "pass2") { SigningReason = "Party B Agreement" },
    new PdfSignature("witness.pfx", "pass3") { SigningReason = "Witness Attestation" }
};

foreach (var sig in signatures)
{
    originalPdf.Sign(sig);
}

originalPdf.SaveAs("multi-party-agreement.pdf");
```

---

## タイムスタンプ機関

タイムスタンプは、文書が署名された時点を証明するために重要です。これは以下の場合に特に重要です：
- 証明書の有効期限後の署名の有効性
- 時間要件を伴う法的手続き
- 監査のための記録

### タイムスタンプの追加

```csharp
using IronPdf;
using IronPdf.Signing;

var signature = new PdfSignature("certificate.pfx", "password")
{
    TimestampHashAlgorithm = PdfSignatureTimestampHashAlgorithms.SHA256,
    TimeStampUrl = "http://timestamp.digicert.com"
};

var pdf = PdfDocument.FromFile("document.pdf");
pdf.Sign(signature);
pdf.SaveAs("timestamped-document.pdf");
```

### 一般的なタイムスタンプ機関

| プロバイダー | URL | 費用 |
|----------|-----|------|
| DigiCert | http://timestamp.digicert.com | 無料 |
| GlobalSign | http://timestamp.globalsign.com | 無料 |
| Sectigo | http://timestamp.sectigo.com | 無料 |
| SSL.com | http://ts.ssl.com | 無料 |

---

## 検証

### 署名の検証

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("signed-document.pdf");

// 文書が署名されているか確認
bool isSigned = pdf.Signatures.Count > 0;
Console.WriteLine($"Document is signed: {isSigned}");

// 各署名を検証
foreach (var signature in pdf.Signatures)
{
    Console.WriteLine($"Signer: {signature.SigningContact}");
    Console.WriteLine($"Reason: {signature.SigningReason}");
    Console.WriteLine($"Date: {signature.SignedDate}");
    Console.WriteLine($"Valid: {signature.IsValid}");
    Console.WriteLine($"Modified after signing: {signature.IsModified}");
}
```

### 証明書チェーンの確認

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("signed-document.pdf");

foreach (var signature in pdf.Signatures)
{
    var cert = signature.SigningCertificate;

    Console.WriteLine($"Subject: {cert.Subject}");
    Console.WriteLine($"Issuer: {cert.Issuer}");
    Console.WriteLine($"Valid From: {cert.NotBefore}");
    Console.WriteLine($"Valid To: {cert.NotAfter}");
    Console.WriteLine($"Expired: {cert.NotAfter < DateTime.Now}");
}
```

---

## セキュリティのベストプラクティス

### 1. 証明書の秘密鍵を保護する

```csharp
// 本番環境ではWindows証明書ストアを使用
var cert = new X509Certificate2(
    "certificate.pfx",
    "password",
    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
);

// または、Azure Key Vault / AWS KMSで鍵の保存を行う
```

### 2. 強力なハッシュアルゴリズムを使用する

```csharp
var signature = new PdfSignature("certificate.pfx", "password")
{
    // SHA-256またはそれ以上の強度を使用
    HashAlgorithm = PdfSignatureHashAlgorithms.SHA256
};
```

### 3. 署名前に検証する

```csharp
// 文書が最終版であることを確認
var pdf = PdfDocument.FromFile("document.pdf");

// 変更可能なフォームフィールドがないかチェック
if (pdf.Form.FormFields.Any(f => !f.IsReadOnly))
{
    // 署名前にフォームをフラット化
    pdf.Form.Flatten();
}

pdf.Sign(signature);
```

### 4. 長期的な有効性のためにPAdESを使用する

```csharp
// PAdES（PDF Advanced Electronic Signatures）フォーマット
// 長期的な検証のためにタイムスタンプと証明書チェーンを含む
var signature = new PdfSignature("certificate.pfx", "password")
{
    PadesCompliance = true,
    TimeStampUrl = "http://timestamp.digicert.com"
};
```

---

## 推奨事項

### デジタル署名にIronPDFを選ぶべき時：
- ✅ シンプルな3行での署名が必要な場合
- ✅ HTMLからPDFを生成している場合
- ✅ クロスプラットフォーム展開が必要な場合
- ✅ 可視署名のサポートが必要な場合

### iText7を選ぶべき時：
- PAdESの特