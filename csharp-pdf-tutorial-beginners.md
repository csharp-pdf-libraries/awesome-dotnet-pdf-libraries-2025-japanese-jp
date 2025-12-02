# C# PDFチュートリアル入門編：最初のPDFを作成する

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![初心者向け](https://img.shields.io/badge/Level-Beginner-brightgreen)]()
[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![最終更新](https://img.shields.io/badge/Updated-November%202025-green)]()

> 5分以内にC#で最初のPDFを作成します。この初心者向けチュートリアルでは、ステップバイステップで全てを説明し、コピーして実行できる実用例を提供します。

---

## 目次

1. [学べること](#学べること)
2. [前提条件](#前提条件)
3. [ステップ1: プロジェクトを作成する](#ステップ1-プロジェクトを作成する)
4. [ステップ2: IronPDFをインストールする](#ステップ2-ironpdfをインストールする)
5. [ステップ3: 最初のPDF](#ステップ3-最初のpdf)
6. [ステップ4: スタイリングを追加する](#ステップ4-スタイリングを追加する)
7. [ステップ5: 保存して開く](#ステップ5-保存して開く)
8. [次のステップ](#次のステップ)
9. [初心者がよくある質問](#初心者がよくある質問)

---

## 学べること

このチュートリアルを終えると、以下ができるようになります：

- ✅ HTMLからPDFを作成する
- ✅ CSSでスタイリングを追加する
- ✅ 画像を含める
- ✅ PDFを保存して表示する
- ✅ C#におけるPDF生成の基礎を理解する

**所要時間：** 5-10分

---

## 前提条件

必要なもの：

1. **Visual Studio 2022**（無料のCommunity版で問題ありません）
   - ダウンロード：https://visualstudio.microsoft.com/

2. **.NET 6, 7, または 8 SDK**（Visual Studioに含まれています）

3. **基本的なC#知識**（変数、メソッド）

以上です！以前のPDF経験は必要ありません。

---

## ステップ1: プロジェクトを作成する

### Visual Studioを使用する場合

1. Visual Studioを開く
2. **新しいプロジェクトを作成**をクリック
3. **コンソールアプリ**を選択（.NET Frameworkのコンソールアプリではない）
4. **次へ**をクリック
5. 名前を`MyFirstPdf`とする
6. **次へ**をクリック
7. **.NET 8.0**を選択（またはインストールされているバージョン）
8. **作成**をクリック

### コマンドラインを使用する場合

```bash
dotnet new console -n MyFirstPdf
cd MyFirstPdf
```

---

## ステップ2: IronPDFをインストールする

### Visual Studioを使用する場合

1. **ソリューションエクスプローラー**でプロジェクトを右クリック
2. **NuGetパッケージの管理**をクリック
3. **参照**タブをクリック
4. `IronPdf`を検索
5. Iron Softwareの**IronPdf**をクリック
6. **インストール**をクリック
7. ライセンスダイアログで**承諾**をクリック

### パッケージマネージャーコンソールを使用する場合

```powershell
Install-Package IronPdf
```

### コマンドラインを使用する場合

```bash
dotnet add package IronPdf
```

---

## ステップ3: 最初のPDF

`Program.cs`の全てのコードを以下で置き換える：

```csharp
using IronPdf;

// HTMLからPDFを作成
var pdf = ChromePdfRenderer.RenderHtmlAsPdf("<h1>Hello, World!</h1>");

// 保存する
pdf.SaveAs("hello.pdf");

Console.WriteLine("PDFが正常に作成されました！");
```

### 実行する

Visual Studioで**F5**を押すか、以下を実行：

```bash
dotnet run
```

### PDFを探す

以下を確認：
- `bin/Debug/net8.0/hello.pdf`（Visual Studio）
- または、`dotnet run`を実行した同じフォルダ

**おめでとうございます！** 最初のPDFを作成しました！🎉

---

## ステップ4: スタイリングを追加する

プロフェッショナルに見えるものを作りましょう。コードを以下で置き換えます：

```csharp
using IronPdf;

// CSSスタイリング付きのHTML
string html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
            padding: 40px;
            color: #333;
        }
        .header {
            background-color: #2c3e50;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 8px;
        }
        .content {
            margin-top: 30px;
            line-height: 1.6;
        }
        .highlight {
            background-color: #f39c12;
            padding: 5px 10px;
            border-radius: 4px;
            color: white;
        }
        .footer {
            margin-top: 50px;
            text-align: center;
            color: #999;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class='header'>
        <h1>私の最初のスタイル付きPDF</h1>
        <p>C#とIronPDFで作成</p>
    </div>

    <div class='content'>
        <h2>ようこそ！</h2>
        <p>このPDFは<span class='highlight'>HTMLとCSS</span>から生成されました。</p>
        <p>知っているHTMLを使って美しいドキュメントを作成できます：</p>
        <ul>
            <li>見出しと段落</li>
            <li>リスト（このようなもの！）</li>
            <li>テーブル</li>
            <li>画像</li>
            <li>CSSスタイリング</li>
        </ul>
    </div>

    <div class='footer'>
        <p>" + DateTime.Now.ToString("MMMM dd, yyyy") + @"に生成されました</p>
    </div>
</body>
</html>";

// PDFを作成
var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
pdf.SaveAs("styled-document.pdf");

Console.WriteLine("スタイル付きPDFが作成されました！");
```

実行して`styled-document.pdf`を開くと、ずっと良くなります！

---

## ステップ5: 保存して開く

PDFを作成した後に自動的に開くようにしましょう：

```csharp
using IronPdf;
using System.Diagnostics;

// PDFを作成
var pdf = ChromePdfRenderer.RenderHtmlAsPdf(@"
    <h1 style='color: blue;'>自動で開くPDF</h1>
    <p>このPDFは自動的に開きます！</p>
");

// 保存する
string filePath = Path.Combine(Directory.GetCurrentDirectory(), "auto-open.pdf");
pdf.SaveAs(filePath);

// デフォルトのPDFビューアで開く
Process.Start(new ProcessStartInfo
{
    FileName = filePath,
    UseShellExecute = true
});

Console.WriteLine("PDFが作成され、開かれました！");
```

---

## 何が起こったかを理解する

主要な概念を分解しましょう：

### 1. ライブラリをインポートする
```csharp
using IronPdf;
```
これにより、IronPDFのクラスにアクセスできます。

### 2. レンダラーを作成する
```csharp
ChromePdfRenderer.RenderHtmlAsPdf(html)
```
これは、実際のChromeブラウザエンジンを使用してHTMLをPDFに変換します。そのため、CSSが完璧に機能します！

### 3. 結果を保存する
```csharp
pdf.SaveAs("filename.pdf")
```
ファイルにPDFを保存します。`pdf.BinaryData`でバイトを取得することもできます。

---