---
** ** | [English Version](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/cross-platform-pdf-dotnet.md)

---
# .NETでのクロスプラットフォームPDF生成：Windows、Linux、Docker、およびクラウド

**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)による** — Iron SoftwareのCTO、IronPDFのクリエーター

[![.NET](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209-512BD4)](https://dotnet.microsoft.com/)
[![Platforms](https://img.shields.io/badge/Platforms-Windows%20%7C%20Linux%20%7C%20macOS%20%7C%20Docker-blue)]()
[![Last Updated](https://img.shields.io/badge/Updated-November%202025-green)]()

> 「クロスプラットフォーム」とは、PDFライブラリのマーケティングで最も乱用されている主張です。このガイドは、Windows、Linux、macOS、Docker、Azure、およびAWSで実際に何が機能し、何が失敗するかをお伝えします。

---

## 目次

1. [クロスプラットフォームの現実](#クロスプラットフォームの現実)
2. [プラットフォームサポートマトリックス](#プラットフォームサポートマトリックス)
3. [Windowsデプロイメント](#windowsデプロイメント)
4. [Linuxデプロイメント](#linuxデプロイメント)
5. [Dockerコンテナ](#dockerコンテナ)
6. [macOSデプロイメント](#macosデプロイメント)
7. [Azureデプロイメント](#azureデプロイメント)
8. [AWSデプロイメント](#awsデプロイメント)
9. [.NET MAUIとBlazor](#net-mauiとblazor)
10. [一般的なクロスプラットフォームの問題](#一般的なクロスプラットフォームの問題)

---

## クロスプラットフォームの現実

10以上のLinuxディストリビューション、Windowsのバリアント、およびクラウド環境で真にクロスプラットフォームで機能するようにIronPDFを構築した後、私はあなたに言えます：**ほとんどの「クロスプラットフォーム」の主張は嘘です。**

### 実際にサポートしているライブラリ

| ライブラリ | Windows | Linux | macOS | Docker | Azure | AWS Lambda |
|---------|---------|-------|-------|--------|-------|------------|
| **IronPDF** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| PuppeteerSharp | ✅ | ✅ | ✅ | ⚠️ サイズ | ⚠️ コールドスタート | ⚠️ サイズ |
| Playwright | ✅ | ✅ | ✅ | ⚠️ サイズ | ⚠️ コールドスタート | ⚠️ サイズ |
| **SelectPdf** | ✅ | ❌ | ❌ | ❌ | ⚠️ Windowsのみ | ❌ |
| **WebView2** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| PDFSharp | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| QuestPDF | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| wkhtmltopdf | ✅ | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |

**重要な洞察：** 最新のCSSを使用したHTMLからPDFへの変換が必要な場合、実際の選択肢はIronPDF、PuppeteerSharp、またはPlaywrightです。

---

## プラットフォームサポートマトリックス

### IronPDF特有のサポート

| プラットフォーム | バージョン | アーキテクチャ | ステータス |
|----------|---------|--------------|--------|
| Windows 10/11 | すべて | x64, x86, ARM64 | ✅ 完全 |
| Windows Server | 2016+ | x64 | ✅ 完全 |
| Ubuntu | 20.04, 22.04, 24.04 | x64, ARM64 | ✅ 完全 |
| Debian | 10, 11, 12 | x64 | ✅ 完全 |
| CentOS | 7, 8 Stream | x64 | ✅ 完全 |
| RHEL | 8, 9 | x64 | ✅ 完全 |
| Alpine | 3.17+ | x64 | ✅ 完全 |
| Amazon Linux | 2, 2023 | x64 | ✅ 完全 |
| macOS | 12+ (Monterey) | Intel, Apple Silicon | ✅ 完全 |
| Docker | すべてのベースイメージ | x64, ARM64 | ✅ 完全 |
| Azure App Service | Windows, Linux | - | ✅ 完全 |
| Azure Functions | v4 | - | ✅ 完全 |
| AWS Lambda | .NET 6/8 | x64, ARM64 | ✅ 完全 |
| AWS ECS/Fargate | - | x64 | ✅ 完全 |

---

## Windowsデプロイメント

Windowsデプロイメントは最も簡単です。

### インストール

```bash
dotnet add package IronPdf
```

### 基本的な使用方法

```csharp
using IronPdf;

var pdf = ChromePdfRenderer.RenderHtmlAsPdf("<h1>Hello Windows</h1>");
pdf.SaveAs("output.pdf");
```

### Windows Serverの考慮事項

デスクトップエクスペリエンスがないWindows Serverの場合：

```csharp
// IronPDFは自動的にヘッドレスレンダリングを処理します
// 追加の設定は必要ありません
```

### IISデプロイメント

```xml
<!-- web.config -->
<configuration>
  <system.webServer>
    <security>
      <requestFiltering>
        <requestLimits maxAllowedContentLength="52428800" />
      </requestFiltering>
    </security>
  </system.webServer>
</configuration>
```

アプリプールのアイデンティティがtempディレクトリに書き込みアクセス権を持っていることを確認してください。

---

## Linuxデプロイメント

Linuxでは、Chromiumの依存関係をインストールする必要があります。

### Ubuntu/Debian

```bash
# 依存関係をインストール
apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    libx11-xcb1 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxi6 \
    libxtst6 \
    libnss3 \
    libcups2 \
    libxss1 \
    libxrandr2 \
    libasound2 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libpangocairo-1.0-0 \
    libgtk-3-0 \
    libgbm1 \
    fonts-liberation
```

### CentOS/RHEL

```bash
# 最初にEPELをインストール
yum install -y epel-release

# 依存関係をインストール
yum install -y \
    libgdiplus \
    libX11-xcb \
    libXcomposite \
    libXcursor \
    libXdamage \
    libXi \
    libXtst \
    nss \
    cups-libs \
    libXScrnSaver \
    libXrandr \
    alsa-lib \
    atk \
    at-spi2-atk \
    pango \
    gtk3 \
    libgbm \
    liberation-fonts
```

### Alpine Linux

```bash
# Alpineはmuslを使用しているため、追加のパッケージが必要です
apk add --no-cache \
    libgdiplus \
    chromium \
    nss \
    freetype \
    harfbuzz \
    ca-certificates \
    ttf-freefont
```

### 自動依存関係設定

```csharp
// Linux依存関係の自動検出を有効にする
Installation.LinuxAndDockerDependenciesAutoConfig = true;
```

---

## Dockerコンテナ

### 推奨されるDockerfile

```dockerfile
# ビルドステージ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

# ランタイムステージ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Chromiumの依存関係をインストール
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    libx11-xcb1 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxi6 \
    libxtst6 \
    libnss3 \
    libcups2 \
    libxss1 \
    libxrandr2 \
    libasound2 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libpangocairo-1.0-0 \
    libgtk-3-0 \
    libgbm1 \
    fonts-liberation \
    fonts-noto-cjk \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

### Alpineベース（小さいイメージ）

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

RUN apk add --no-cache \
    libgdiplus \
    icu-libs \
    chromium \
    nss \
    freetype \
    harfbuzz \
    ca-certificates \
    ttf-freefont

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app .
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - pdf-output:/app/output

volumes:
  pdf-output:
```

### Kubernetesデプロイメント

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: pdf-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: pdf-service
  template:
    metadata:
      labels:
        app: pdf-service
    spec:
      containers:
      - name: pdf-service
        image: your-registry/pdf-service:latest
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "2Gi"
            cpu: "2000m"
        securityContext:
          allowPrivilegeEscalation: false
```

---

## macOSデプロイメント

### Intel Macs

```bash
dotnet add package IronPdf
```

そのまま動作します。

### Apple Silicon (M1/M2/M3)

```bash
dotnet add package IronPdf
```

IronPDFにはネイティブのARM64バイナリが含まれています。Rosettaは必要ありません。

### 開発セットアップ

```csharp
using IronPdf;

// macOSでも同様に機能します
var pdf = ChromePdfRenderer.RenderHtmlAsPdf("<h1>Hello macOS</h1>");
pdf.SaveAs("output.pdf");
```

---

## Azureデプロイメント

### Azure App Service (Linux)

```yaml
# azure-pipelines.yml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: 'publish'
      publishWebProjects: true
      arguments: '--configuration Release --output $(Build.ArtifactStagingDirectory)'

  - task: AzureWebApp@1
    inputs:
      azureSubscription: 'Your-Subscription'
      appType: 'webAppLinux'
      appName: 'your-app-name'
      package: '$(Build.ArtifactStagingDirectory)/**/*.zip'
```

**推奨されるティア：** P1v2以上（Chromiumに十分なメモリ）

### Azure App Service (Windows)

そのまま動作します。特別な設定は必要ありません。

### Azure Functions

```csharp
public class PdfFunction
{
    [FunctionName("GeneratePdf")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string html = await new StreamReader(req.Body).ReadToEndAsync();

        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);

        return new FileContentResult(pdf.BinaryData, "application/pdf")
        {
            FileDownloadName = "document.pdf"
        };
    }
}
```

**設定：**
- Consumption PlanまたはPremium Planを使用
- `FUNCTIONS_WORKER_RUNTIME`を`dotnet-isolated`に設定
- 十分なメモリを確保（256MB+推奨）

### Azure Container Apps

```yaml
# containerapp.yaml
properties:
  configuration:
    ingress:
      external: true
      targetPort: 80
  template:
    containers:
      - image: your-registry.azurecr.io/pdf-service:latest
        name: pdf-service
        resources:
          cpu: 1
          memory: 2Gi
```

---

## AWSデプロイメント

### AWS Lambda

```csharp
public class Function
{
    public async Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest request, ILambdaContext context)
    {
        // Linux依存関係を有効にする
        Installation.LinuxAndDockerDependenciesAutoConfig = true;

        var html = request.Body;
        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/pdf" },
                { "Content-Disposition", "attachment; filename=document.pdf" }
            },
            Body = Convert.ToBase64String(pdf.BinaryData),
            IsBase64Encoded = true
        };
    }
}
```

**設定：**
- メモリ：最小512MB、推奨1024MB
- タイムアウト：最小30秒
- アーキテクチャ：x86_64またはarm64

### AWS Lambdaコンテナイメージ

```dockerfile
FROM public.ecr.aws/lambda/dotnet:8

# 依存関係をインストール
RUN yum install -y \
    libgdiplus \
    libX11-xcb \
    nss \
    cups-libs \
    libXScrnSaver \
    alsa-lib \
    atk \
    gtk3 \
    && yum clean all

COPY bin/Release/net8.0/linux-x64/publish/ ${LAMBDA_TASK_ROOT}

CMD ["YourAssembly::YourNamespace.Function::FunctionHandler"]
```

### AWS ECS/Fargate

```json
{
  "family": "pdf-service",
  "containerDefinitions": [
    {
      "name": "pdf-service",
      "image": "your-account.dkr.ecr.region.amazonaws.com/pdf-service:latest",
      "memory": 2048,
      "cpu": 1024,
      "essential": true,
      "portMappings": [
        {
          "containerPort": 80,
          "protocol": "tcp"
        }
      ]
    }
  ],
  "requiresCompatibilities": ["FARGATE"],
  "networkMode": "awsvpc",
  "cpu": "1024",
  "memory": "2048"
}
```

---

## .NET MAUIとBlazor

### Blazor Server

```csharp
// サーバーサイドレンダリングで直接動作します
@inject IJSRuntime JS

<button @onclick="GeneratePdf">PDFをダウンロード</button>

@code {
    private async Task GeneratePdf()
    {
        var html = "<h1>Blazor PDF</h1>";
        var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);

        await JS.InvokeVoidAsync("downloadFile", "document.pdf", pdf.BinaryData);
    }