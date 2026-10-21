$ErrorActionPreference='Stop'
$root=$PSScriptRoot
$out=Join-Path $root 'release'
New-Item -ItemType Directory -Path $out -Force | Out-Null
$compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
Add-Type -AssemblyName System.Drawing
$bitmap=New-Object System.Drawing.Bitmap 64,64
$g=[Drawing.Graphics]::FromImage($bitmap)
$g.SmoothingMode=[Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([Drawing.Color]::FromArgb(8,127,121))
$pen=New-Object Drawing.Pen ([Drawing.Color]::White),4
$g.DrawRectangle($pen,9,12,23,39)
$g.DrawLine($pen,38,22,54,22);$g.DrawLine($pen,49,17,54,22);$g.DrawLine($pen,49,27,54,22)
$g.DrawLine($pen,38,40,54,40);$g.DrawLine($pen,38,40,43,35);$g.DrawLine($pen,38,40,43,45)
$icon=[Drawing.Icon]::FromHandle($bitmap.GetHicon());$stream=[IO.File]::Create((Join-Path $out 'PocketBridge.ico'));$icon.Save($stream);$stream.Dispose();$g.Dispose();$pen.Dispose();$bitmap.Dispose()
$resource='/resource:"'+$root+'\Portal.html",Portal.html'
$qrResource='/resource:"'+$root+'\vendor\QRCoder.dll",QRCoder.dll'
& $compiler /nologo /target:winexe /platform:x64 /optimize+ /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Core.dll /reference:Microsoft.CSharp.dll /reference:System.Web.Extensions.dll /reference:"$root\vendor\QRCoder.dll" $resource $qrResource /win32manifest:"$root\app.manifest" /win32icon:"$out\PocketBridge.ico" /out:"$out\PocketBridge.exe" "$root\Bridge.cs" "$root\PhoneLibrary.cs" "$root\Qr.cs"
if($LASTEXITCODE -ne 0){throw 'Compilation failed'}
Copy-Item -LiteralPath "$out\PocketBridge.exe" -Destination "$out\PocketBridge-Setup.exe" -Force
Get-Item "$out\PocketBridge.exe" | Select-Object FullName,Length
