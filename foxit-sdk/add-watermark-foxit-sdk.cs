```csharp
// NuGet: Install-Package Foxit.SDK をインストール
using Foxit.SDK;
using Foxit.SDK.Common;
using Foxit.SDK.PDFDoc;
using System;

class Program
{
    static void Main()
    {
        Library.Initialize("sn", "key");
        
        using (PDFDoc doc = new PDFDoc("input.pdf"))
        {
            doc.Load("");
            
            Watermark watermark = new Watermark(doc, "Confidential", 
                new Font(Font.StandardID.e_StdIDHelvetica), 48.0f, 0xFF0000FF);
            
            WatermarkSettings settings = new WatermarkSettings();
            settings.flags = Watermark.e_WatermarkFlagASPageContents; // ページ内容としてのウォーターマークフラグ
            settings.position = Watermark.Position.e_PosCenter; // 中央位置
            settings.rotation = -45.0f; // 回転角度
            settings.opacity = 0.5f; // 不透明度
            
            watermark.SetSettings(settings);
            watermark.InsertToAllPages();
            
            doc.SaveAs("output.pdf", PDFDoc.SaveFlags.e_SaveFlagNoOriginal); // オリジナルを保存しない
        }
        
        Library.Release();
    }
}
```