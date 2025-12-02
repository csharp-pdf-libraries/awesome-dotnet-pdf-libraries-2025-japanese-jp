# C#でPDFフォームを埋める: AcroForms、XFA、およびフラット化ガイド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> PDFフォームは、手動でのデータ入力なしにデータ収集を可能にします。このガイドでは、AcroFormsの入力、XFAフォームの処理、および配布のためのフラット化について説明します。

---

## 目次

1. [クイックスタート](#quick-start)
2. [ライブラリ比較](#library-comparison)
3. [フォームの種類について](#form-types-explained)
4. [AcroFormsの操作](#working-with-acroforms)
5. [フォームデータの読み取り](#reading-form-data)
6. [フォームのフラット化](#flattening-forms)
7. [フォームの作成](#creating-forms)
8. [一般的な使用例](#common-use-cases)

---

## クイックスタート

### IronPDFでフォームフィールドを埋める

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("application-form.pdf");

// テキストフィールドを埋める
pdf.Form.FindFormField("FirstName").Value = "John";
pdf.Form.FindFormField("LastName").Value = "Smith";
pdf.Form.FindFormField("Email").Value = "john.smith@example.com";

// チェックボックスを埋める
pdf.Form.FindFormField("AgreeToTerms").Value = "Yes";

pdf.SaveAs("filled-application.pdf");
```

---

## ライブラリ比較

### フォーム入力機能

| ライブラリ | AcroFormsを埋める | フォームを読む | フラット化 | フォームを作成 | XFAサポート |
|---------|---------------|-----------|---------|--------------|-------------|
| **IronPDF** | ✅ シンプル | ✅ | ✅ | ✅ | ⚠️ 変換 |
| iText7 | ✅ | ✅ | ✅ | ✅ | ✅ |
| Aspose.PDF | ✅ | ✅ | ✅ | ✅ | ✅ |
| PDFSharp | ⚠️ 限定的 | ⚠️ | ⚠️ | ❌ | ❌ |
| PuppeteerSharp | ❌ | ❌ | ❌ | ❌ | ❌ |
| QuestPDF | ❌ | ❌ | ❌ | ❌ | ❌ |

### コードの複雑さ

**IronPDF — 3行:**
```csharp
var pdf = PdfDocument.FromFile("form.pdf");
pdf.Form.FindFormField("Name").Value = "John";
pdf.SaveAs("filled.pdf");
```

**iText7 — 15+行:**
```csharp
using var reader = new PdfReader("form.pdf");
using var writer = new PdfWriter("filled.pdf");
using var pdfDoc = new PdfDocument(reader, writer);
var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
form.GetField("Name").SetValue("John");
pdfDoc.Close();
```

---

## フォームの種類について

### AcroForms (標準)

- 最も一般的なPDFフォームタイプ
- Adobe Acrobat、LibreOfficeなどで作成
- PDFに埋め込まれた静的フィールド
- 広くサポートされている

### XFAフォーム (レガシー)

- XML Forms Architecture
- 動的でデータ駆動型のフォーム
- Adobe LiveCycle Designerで作成
- **Adobeによって非推奨** — 段階的に廃止されている
- 現代のリーダーでのサポートは限定的

### HTMLフォーム (IronPDFのアプローチ)

- HTMLからPDFフォームを生成
- 完全なCSSスタイリング
- AcroFormsへの現代的な代替手段

---

## AcroFormsの操作

### すべてのフォームフィールドをリストする

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("form.pdf");

Console.WriteLine("フォームフィールド:");
foreach (var field in pdf.Form.FormFields)
{
    Console.WriteLine($"  名前: {field.Name}");
    Console.WriteLine($"  タイプ: {field.Type}");
    Console.WriteLine($"  値: {field.Value}");
    Console.WriteLine($"  読み取り専用: {field.IsReadOnly}");
    Console.WriteLine();
}
```

### 異なるフィールドタイプを埋める

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("comprehensive-form.pdf");

// テキストフィールド
pdf.Form.FindFormField("FullName").Value = "Jane Doe";

// 複数行テキスト
pdf.Form.FindFormField("Address").Value = "123 Main St\nApt 4B\nNew York, NY 10001";

// チェックボックス
pdf.Form.FindFormField("Subscribe").Value = "Yes";

// ラジオボタン
pdf.Form.FindFormField("PaymentMethod").Value = "CreditCard";

// ドロップダウン/コンボボックス
pdf.Form.FindFormField("Country").Value = "United States";

// 日付フィールド
pdf.Form.FindFormField("BirthDate").Value = "1990-01-15";

pdf.SaveAs("filled-form.pdf");
```

### フィールドインデックスで埋める

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("form.pdf");

// フィールド名が不明な場合はインデックスで埋める
pdf.Form.FormFields[0].Value = "最初のフィールド値";
pdf.Form.FormFields[1].Value = "二番目のフィールド値";

pdf.SaveAs("filled.pdf");
```

### 条件付きで埋める

```csharp
using IronPdf;

public void FillFormFromData(string formPath, Dictionary<string, string> data)
{
    var pdf = PdfDocument.FromFile(formPath);

    foreach (var field in pdf.Form.FormFields)
    {
        if (data.TryGetValue(field.Name, out string value))
        {
            field.Value = value;
        }
        else
        {
            Console.WriteLine($"警告: フィールド'{field.Name}'のデータがありません");
        }
    }

    pdf.SaveAs(formPath.Replace(".pdf", "-filled.pdf"));
}

// 使用例
var formData = new Dictionary<string, string>
{
    { "FirstName", "John" },
    { "LastName", "Doe" },
    { "Email", "john@example.com" }
};

FillFormFromData("application.pdf", formData);
```

---

## フォームデータの読み取り

### すべてのフィールド値を抽出する

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("submitted-form.pdf");

var formData = new Dictionary<string, string>();

foreach (var field in pdf.Form.FormFields)
{
    formData[field.Name] = field.Value;
}

// JSONにエクスポート
string json = System.Text.Json.JsonSerializer.Serialize(formData, new JsonSerializerOptions
{
    WriteIndented = true
});
File.WriteAllText("form-data.json", json);
```

### 提出されたフォームを処理する

```csharp
using IronPdf;

public class FormSubmission
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool AgreeToTerms { get; set; }
}

public FormSubmission ExtractFormData(byte[] pdfBytes)
{
    var pdf = new PdfDocument(pdfBytes);

    return new FormSubmission
    {
        FirstName = pdf.Form.FindFormField("FirstName")?.Value,
        LastName = pdf.Form.FindFormField("LastName")?.Value,
        Email = pdf.Form.FindFormField("Email")?.Value,
        AgreeToTerms = pdf.Form.FindFormField("AgreeToTerms")?.Value == "Yes"
    };
}
```

---

## フォームのフラット化

フラット化は、フォームフィールドを静的コンテンツに変換し、これ以上の編集を防ぎます。

### すべてのフィールドをフラット化する

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("filled-form.pdf");

// フラット化 - フィールドを静的テキストに変換
pdf.Form.Flatten();

pdf.SaveAs("flattened-form.pdf");
```

### 配布のためにフラット化する

```csharp
using IronPdf;

public void PrepareForDistribution(string formPath, Dictionary<string, string> data)
{
    var pdf = PdfDocument.FromFile(formPath);

    // すべてのフィールドを埋める
    foreach (var kvp in data)
    {
        var field = pdf.Form.FindFormField(kvp.Key);
        if (field != null)
        {
            field.Value = kvp.Value;
        }
    }

    // 編集を防ぐためにフラット化
    pdf.Form.Flatten();

    // ウォーターマークを追加
    pdf.ApplyWatermark("<p style='opacity:0.3;font-size:10pt;'>最終コピー - 編集禁止</p>",
        VerticalAlignment.Bottom, HorizontalAlignment.Center);

    pdf.SaveAs(formPath.Replace(".pdf", "-final.pdf"));
}
```

### 選択的なフラット化

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("form.pdf");

// 特定のフィールドのみをフラット化
var fieldsToFlatten = new[] { "Signature", "Date", "ApproverName" };

foreach (var fieldName in fieldsToFlatten)
{
    var field = pdf.Form.FindFormField(fieldName);
    if (field != null)
    {
        field.IsReadOnly = true;  // フィールドをロック
    }
}

pdf.SaveAs("partially-locked.pdf");
```

---

## フォームの作成

### HTMLフォームからPDFへ

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

string formHtml = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; padding: 40px; }
        .form-group { margin-bottom: 15px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; }
        input[type='text'], select, textarea {
            width: 100%;
            padding: 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
        .checkbox-group { display: flex; align-items: center; gap: 10px; }
    </style>
</head>
<body>
    <h1>登録フォーム</h1>

    <div class='form-group'>
        <label for='name'>フルネーム</label>
        <input type='text' id='name' name='FullName' />
    </div>

    <div class='form-group'>
        <label for='email'>メールアドレス</label>
        <input type='text' id='email' name='Email' />
    </div>

    <div class='form-group'>
        <label for='country'>国</label>
        <select id='country' name='Country'>
            <option value=''>選択...</option>
            <option value='US'>アメリカ合衆国</option>
            <option value='UK'>イギリス</option>
            <option value='CA'>カナダ</option>
        </select>
    </div>

    <div class='form-group'>
        <label for='comments'>コメント</label>
        <textarea id='comments' name='Comments' rows='4'></textarea>
    </div>

    <div class='form-group checkbox-group'>
        <input type='checkbox' id='terms' name='AgreeToTerms' />
        <label for='terms'>利用規約に同意する</label>
    </div>
</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(formHtml);
pdf.SaveAs("registration-form.pdf");
```

---

## 一般的な使用例

### 税金フォームの入力

```csharp
using IronPdf;

public void FillTaxForm(string formPath, TaxData data)
{
    var pdf = PdfDocument.FromFile(formPath);

    // 個人情報
    pdf.Form.FindFormField("FirstName").Value = data.FirstName;
    pdf.Form.FindFormField("LastName").Value = data.LastName;
    pdf.Form.FindFormField("SSN").Value = data.SSN;

    // 収入
    pdf.Form.FindFormField("WagesLine1").Value = data.Wages.ToString("F2");
    pdf.Form.FindFormField("InterestLine2").Value = data.Interest.ToString("F2");
    pdf.Form.FindFormField("DividendsLine3").Value = data.Dividends.ToString("F2");

    // 合計を計算
    decimal total = data.Wages + data.Interest + data.Dividends;
    pdf.Form.FindFormField("TotalIncome").Value = total.ToString("F2");

    // 署名と日付
    pdf.Form.FindFormField("SignatureDate").Value = DateTime.Now.ToString("MM/dd/yyyy");

    pdf.SaveAs($"tax-return-{data.LastName}-{DateTime.Now.Year}.pdf");
}
```

### アプリケーション処理

```csharp
using IronPdf;

public class ApplicationProcessor
{
    public void ProcessApplication(byte[] submittedPdf, string applicantId)
    {
        var pdf = new PdfDocument(submittedPdf);

        // 提出されたデータを抽出
        var data = new Dictionary<string, string>();
        foreach (var field in pdf.Form.FormFields)
        {
            data[field.Name] = field.Value;
        }

        // 内部追跡フィールドを追加
        pdf.Form.FindFormField("ApplicationID").Value = applicantId;
        pdf.Form.FindFormField("ReceivedDate").Value = DateTime.Now.ToString("yyyy-MM-dd");
        pdf.Form.FindFormField("Status").Value = "Pending Review";

        // 追跡情報を保存
        pdf.SaveAs($"applications/{applicantId}.pdf");

        // データベースにログ
        SaveToDatabase(applicantId, data);
    }
}
```

### 契約書の生成

```csharp
using IronPdf;

public void GenerateContract(string templatePath, ContractData contract)
{
    var pdf = PdfDocument.FromFile(templatePath);

    // 当事者情報
    pdf.Form.FindFormField("PartyAName").Value = contract.PartyA.Name;
    pdf.Form.FindFormField("PartyAAddress").Value = contract.PartyA.Address;
    pdf.Form.FindFormField("PartyBName").Value = contract.PartyB.Name;
    pdf.Form.FindFormField("PartyBAddress").Value = contract.PartyB.Address;

    // 契約詳細
    pdf.Form.FindFormField("ContractAmount").Value = contract.Amount.ToString("C");
    pdf.Form.FindFormField("StartDate").Value = contract.StartDate.ToString("MMMM dd, yyyy");
    pdf.Form.FindFormField("EndDate").Value = contract.EndDate.ToString("MMMM dd, yyyy");
    pdf.Form.FindFormField("Terms").Value = contract.TermsDescription;

    // 署名フィールドは署名用に空白にしておく
    // pdf.Form.FindFormField("PartyASignature") - 空白のまま
    // pdf.Form.FindFormField("PartyBSignature") - 空白のまま

    pdf.SaveAs($"contracts/contract-{contract.ContractNumber}.pdf");
}
```

---

## 推奨事項

### フォームにIronPDFを選ぶ場合:
- ✅ 埋めるためのシンプルなAPI
- ✅ HTMLからPDFへの生成と組み合わせ
- ✅ 配布のためのフラット化が必要
- ✅ クロスプラットフォームのデプロイメント

### iText7を選ぶ場合:
- XFAフォームサポートが重要な場合
- 非常に複雑なフォーム操作が必要な場合
- 既にiTextエコシステムを