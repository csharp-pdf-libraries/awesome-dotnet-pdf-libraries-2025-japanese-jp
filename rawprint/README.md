---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [rawprint/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/rawprint/README.md)
🇯🇵 **日本語:** [rawprint/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/rawprint/README.md)

---

# RawPrintとIronPDFをC#で比較：PDFの印刷と作成

C#で文書の印刷や生成を扱う際、開発者は多くのライブラリから選択することがしばしば混乱を招きます。これらのライブラリの中で、RawPrintとIronPDFはそれぞれ異なるアプローチと使用例によって対照的です。RawPrintはプリンターへの生のバイトの送信に特化した低レベルのソリューションを提供する一方で、IronPDFはPDFの作成、操作、変換のための高レベルAPIを提供します。この記事では、両ライブラリの強みと弱点を分析し、C#アプリケーションに最適なソリューションを求める開発者に実用的な洞察と比較を提供することを目指します。

## RawPrintの理解

RawPrintは、生のデータを直接プリンターに送信するための実装のコレクションです。従来のプリンタードライバーをバイパスして、直接プリンターにコマンドを送信することが必要なアプリケーションにとって不可欠です。この機能は、ZPL（Zebra Programming Language）やEPL（Eltron Programming Language）を使用するラベル作成などの特殊なプリンターを使用するシナリオで特に有用です。

RawPrintの強みの一つは、データストリームを直接プリンターに送信することのシンプルさです。Windows特有の環境を対象とし、直接プリンターとの通信が必要な開発者にとって、RawPrintはドライバーやグラフィカルインターフェースといった中間層をバイパスする効率的な方法を提供します。

しかし、RawPrintには顕著な制限があります：

- **非常に低レベル**：生のバイトを直接扱うことで、開発者はプリンターのコマンド言語に深い理解が必要とされ、単純な文書印刷タスクにはあまり適していません。
  
- **PDF作成不可**：RawPrintはデータ送信にのみ焦点を当てており、PDFの作成、レンダリング、または操作の機能がありません。
  
- **プラットフォーム固有**：Windowsのプリントスプーラーに依存しており、クロスプラットフォームの適用性が限られます。

### C#でのRawPrintの使用例

より明確なイメージを提供するために、以下はRawPrintがプリンターにデータを送信するために実装されるかもしれないC#の例です：

```csharp
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RawPrintExample
{
    class Program
    {
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(string src, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter")]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA pDocInfo);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter")]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter")]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter")]
        public static.extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter")]
        public static.extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        static void Main(string[] args)
        {
            IntPtr hPrinter;
            IntPtr pBytes;
            int dwBytesWritten;

            string printerName = "YourPrinterName";
            string documentName = "RawPrint Document Example";
            string data = "Raw data to send to the printer.";

            DOCINFOA di = new DOCINFOA();
            di.pDocName = documentName;
            di.pDataType = "RAW";

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            int length = bytes.Length;
            pBytes = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(bytes, 0, pBytes, length);

            if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, ref di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        WritePrinter(hPrinter, pBytes, length, out dwBytesWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }

            Marshal.FreeCoTaskMem(pBytes);
        }
    }
}
```

この例は、DLLインポートを使用して生のデータをプリンターに送信するWindows特有のアプローチを示しています。開発者がRawPrintを実装する方法を強調し、その強力だが限定的な範囲を認識しています。

## IronPDF：より高レベルの代替手段

RawPrintとは対照的に、IronPDFはPDFを包括的に扱うための堅牢で多機能なAPIを提供します。.NET環境で確立された名前として、IronPDFは開発者がプラットフォームを問わずPDFを簡単に作成、編集、変換することを可能にします。複雑なフォーマットの文書の作成からHTMLをPDFに変換することまで、IronPDFはC#アプリケーションでのPDF処理を合理化する高レベルの機能で広く評価されています。

IronPDFの顕著な利点：

- **高レベルPDF API**：RawPrintとは異なり、IronPDFは開発者が基礎となるデータストリームやフォーマットの複雑な知識を必要とせずにPDFを操作できるようにします。この使いやすさは、効率的かつ簡単なPDF処理ソリューションを求める開発者にとって有益です。

- **完全な作成機能**：IronPDFは、PDFの作成、マージ、注釈付け、圧縮、変換のための包括的な機能範囲をサポートしています。

- **クロスプラットフォーム互換性**：Windowsに制限されるRawPrintとは異なり、IronPDFはさまざまなオペレーティングシステムでシームレスに動作するソリューションを提供します。これは、より広いサポートと柔軟性を求める開発環境にとって理想的です。

もちろん、強みには潜在的な制限も伴います。特に、RawPrintの利点に厳密に依存する高度に特化した環境ではそうです。

### IronPDFのリンクとリソースの探索

IronPDFを活用することに興味のある開発者は、公式ページで提供されている詳細なチュートリアルとガイドを探索することを奨励されます：

- [HTMLファイルをPDFに変換するIronPDF](https://ironpdf.com/how-to/html-file-to-pdf/)
- [IronPDFチュートリアル](https://ironpdf.com/tutorials/)

これらのリソースには、ライブラリの使用を最適化する方法に関する豊富な情報が含まれており、開発者が包括的な理解と実装を確保することを保証します。

## 包括的な比較

RawPrintとIronPDFの本質的な違いと役割を要約するために、以下の比較表は主要な特徴、強み、弱点を概説しています：

| 特徴                              | RawPrint                                         | IronPDF                                          |
|--------------------------------------|--------------------------------------------------|--------------------------------------------------|
| **機能**                            | プリンターに直接生の印刷データを送信する           | 包括的なPDFの作成と操作                          |
| **使用例**                           | ラベルなどの特殊な印刷                           | 一般的な文書管理と作成                           |
| **プラットフォーム依存性**           | Windows固有                                      | クロスプラットフォーム                           |
| **複雑さ**                           | 低レベル、プリンターコマンドの知識が必要          | 高レベル、ユーザーフレンドリーなAPI              |
| **PDF作成**                           | なし                                             | あり                                             |
| **最適な使用例**                      | 直接プリンターアクセスが必要な場合                | WebおよびデスクトップアプリでのPDF関連タスク     |
| **柔軟性**                           | 生のバイト処理により限定される                    | 複数の機能で広範                                 |                               
| **ライセンス**                        | 変動                                             | 商用                                             |

---

## 文書のフォーマットをどのように印刷しますか？

**RawPrint**では、このように処理します：

```csharp
// NuGet: Install-Package System.Drawing.Common
using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;

class RawPrinterHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static.extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static.extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

    [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static.extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static.extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public.static.extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public.static.extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

    public static bool SendBytesToPrinter(string szPrinterName, byte[] pBytes)
    {
        IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(pBytes.Length);
        Marshal.Copy(pBytes, 0, pUnmanagedBytes, pBytes.Length);
        IntPtr hPrinter;
        if (OpenPrinter(szPrinterName, out hPrinter, IntPtr.Zero))
        {
            DOCINFOA di = new DOCINFOA();
            di.pDocName = "Raw Document";
            di.pDataType = "RAW";
            if (StartDocPrinter(hPrinter, 1, di))
            {
                if (StartPagePrinter(hPrinter))
                {
                    Int32 dwWritten;
                    WritePrinter(hPrinter, pUnmanagedBytes, pBytes.Length, out dwWritten);
                    EndPagePrinter(hPrinter);
                }
                EndDocPrinter(hPrinter);
            }
            ClosePrinter(hPrinter);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return true;
        }
        return false;
    }
}

class Program
{
    static void Main()
    {
        // RawPrintはPCL/PostScriptコマンドを手動で必要とします
        string pclCommands = "\x1B&l0O\x1B(s0p16.66h8.5v0s0b3T";
        string text = "Plain text document - limited formatting";
        byte[] data = Encoding.ASCII.GetBytes(pclCommands + text);
        RawPrinterHelper.SendBytesToPrinter("HP LaserJet", data);
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的になります：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        string html = @"
            <html>
            <head>
                <style>
                    body { font-family: Arial; margin: 40px; }
                    h1 { color: #2c3e50; font-size: 24px; }
                    p { line-height: 1.6; color: #34495e; }
                    .highlight { background-color: yellow; font-weight: bold; }
                </style>
            </head>
            <body>
                <h1>Formatted Document</h1>
                <p>This is a <span class='highlight'>beautifully formatted</span> document with CSS styling.</p>
                <p>Complex layouts, fonts, colors, and images are fully supported.</p>
            </body>
            </html>";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("formatted.pdf");
        Console.WriteLine("Formatted PDF created successfully");
    }
}
```

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合と、PDF生成ワークフローの維持と拡張を容易にする、よりクリーンな構文を提供します。

---