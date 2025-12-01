```csharp
// NuGet: Install-Package GrabzIt をインストール
using GrabzIt;
using GrabzIt.Parameters;
using System;

class Program
{
    static void Main()
    {
        var grabzIt = new GrabzItClient("YOUR_APPLICATION_KEY", "YOUR_APPLICATION_SECRET");
        var options = new ImageOptions();
        options.Format = ImageFormat.png; // フォーマットをPNGに設定
        options.Width = 800; // 幅を800に設定
        options.Height = 600; // 高さを600に設定
        
        grabzIt.HTMLToImage("<html><body><h1>Hello World</h1></body></html>", options); // HTMLを画像に変換
        grabzIt.SaveTo("output.png"); // output.pngに保存
    }
}
```