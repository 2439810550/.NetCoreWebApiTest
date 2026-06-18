# 生成自签名证书脚本（在服务器上以管理员身份运行）
# 用于 Kestrel HTTPS，让手机浏览器允许摄像头访问

param(
    [string]$IpAddress = "47.99.133.151",
    [string]$CertPassword = ""
)

$ErrorActionPreference = "Stop"

Write-Host "=== 为 $IpAddress 生成自签名 HTTPS 证书 ===" -ForegroundColor Cyan
Write-Host ""

# 1. 创建证书
$cert = New-SelfSignedCertificate `
    -DnsName $IpAddress, "localhost" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -KeyExportPolicy Exportable `
    -NotAfter (Get-Date).AddYears(10) `
    -KeyLength 2048 `
    -KeyAlgorithm RSA `
    -HashAlgorithm SHA256 `
    -Type SSLServerAuthentication

Write-Host "✅ 证书已创建: $($cert.Thumbprint)" -ForegroundColor Green

# 2. 确保 certs 目录存在
$certsDir = Join-Path $PSScriptRoot "certs"
if (-not (Test-Path $certsDir)) {
    New-Item -ItemType Directory -Path $certsDir | Out-Null
}

# 3. 导出为 PFX
$pfxPath = Join-Path $certsDir "pigeon-shake.pfx"
$securePassword = if ($CertPassword) {
    ConvertTo-SecureString -String $CertPassword -Force -AsPlainText
} else {
    $null
}

Export-PfxCertificate `
    -Cert $cert `
    -FilePath $pfxPath `
    -Password $securePassword

Write-Host "✅ PFX 已导出: $pfxPath" -ForegroundColor Green

# 4. 绑定到 HTTPS 端口（可选，如果不用 Kestrel 直接处理）
# netsh http add sslcert ipport=0.0.0.0:5273 certhash=$($cert.Thumbprint) appid="{214124cd-d05b-4309-9c44-9b4d3f8d2e2a}"

Write-Host ""
Write-Host "=== 手机端操作 ===" -ForegroundColor Yellow
Write-Host "1. 将此脚本所在目录下的 certs/pigeon-shake.pfx 留在服务器上"
Write-Host "2. 手机访问 https://$($IpAddress):5273/pigeon-shake/"
Write-Host "3. 首次访问会提示「此连接非私人连接」→ 点击「高级」→「继续前往」"
Write-Host "4. Chrome: 点击屏幕任意位置，输入 'thisisunsafe' 即可跳过"
Write-Host ""
Write-Host "⚠️  如果想彻底消除警告，需要："
Write-Host "   a) 购买域名 + Let's Encrypt 免费 SSL 证书"
Write-Host "   b) 或使用 Cloudflare Tunnel / ngrok 等隧道服务"
