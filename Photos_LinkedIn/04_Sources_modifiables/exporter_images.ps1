$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$photoRoot = Split-Path -Parent $PSScriptRoot
$exportRoot = Join-Path $photoRoot '01_Exports'
$layouts = Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $PSScriptRoot 'layouts.json') | ConvertFrom-Json

function Get-Brush([string]$Color) { return [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Color)) }
function Write-Text($Graphic,[string]$Value,$X,$Y,$W,$H,$Size,[string]$Color,[bool]$Bold,[string]$Family) {
    $style = [System.Drawing.FontStyle]::Regular
    if ($Bold) { $style = [System.Drawing.FontStyle]::Bold }
    $font = [System.Drawing.Font]::new($Family,[single]$Size,$style,[System.Drawing.GraphicsUnit]::Pixel)
    $format = [System.Drawing.StringFormat]::new()
    $format.FormatFlags = [System.Drawing.StringFormatFlags]::LineLimit
    $measured = $Graphic.MeasureString($Value,$font,[int]$W,$format)
    while ($measured.Height -gt ($H - 2) -and $Size -gt 14) {
        $font.Dispose(); $Size -= 1
        $font = [System.Drawing.Font]::new($Family,[single]$Size,$style,[System.Drawing.GraphicsUnit]::Pixel)
        $measured = $Graphic.MeasureString($Value,$font,[int]$W,$format)
    }
    $brush = Get-Brush $Color
    $Graphic.DrawString($Value,$font,$brush,[System.Drawing.RectangleF]::new([single]$X,[single]$Y,[single]$W,[single]$H),$format)
    $brush.Dispose(); $format.Dispose(); $font.Dispose()
}
function Place-Image($Graphic,[string]$Path,$X,$Y,$W,$H) {
    $source = [System.Drawing.Image]::FromFile($Path)
    $ratio = [Math]::Min($W/$source.Width,$H/$source.Height)
    $dw=$source.Width*$ratio; $dh=$source.Height*$ratio
    $Graphic.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $Graphic.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $Graphic.DrawImage($source,[System.Drawing.RectangleF]::new([single]($X+($W-$dw)/2),[single]($Y+($H-$dh)/2),[single]$dw,[single]$dh))
    $source.Dispose()
}
function Save-Jpeg($Bitmap,[string]$Path) {
    $codec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
    $parameters = [System.Drawing.Imaging.EncoderParameters]::new(1)
    $parameters.Param[0] = [System.Drawing.Imaging.EncoderParameter]::new([System.Drawing.Imaging.Encoder]::Quality,[long]95)
    $Bitmap.Save($Path,$codec,$parameters)
    $parameters.Dispose()
}
foreach ($layout in $layouts) {
    $bitmap = [System.Drawing.Bitmap]::new([int]$layout.width,[int]$layout.height)
    $graphic = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphic.Clear([System.Drawing.ColorTranslator]::FromHtml($layout.background))
    $graphic.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
    foreach ($item in $layout.items) {
        switch ($item.type) {
            'rect' {
                $brush=Get-Brush $item.fill
                $graphic.FillRectangle($brush,[single]$item.x,[single]$item.y,[single]$item.width,[single]$item.height)
                $brush.Dispose()
            }
            'text' { Write-Text $graphic $item.value $item.x $item.y $item.width $item.height $item.size $item.color $item.bold $item.family }
            'image' { Place-Image $graphic (Join-Path $photoRoot $item.file) $item.x $item.y $item.width $item.height }
        }
    }
    $bitmap.Save((Join-Path $exportRoot ($layout.name+'.png')),[System.Drawing.Imaging.ImageFormat]::Png)
    $graphic.Dispose(); $bitmap.Dispose()
}

$exports = Get-ChildItem -LiteralPath $exportRoot -Filter *.png | Sort-Object Name
$records = @()
foreach ($file in $exports) {
    $image = [System.Drawing.Bitmap]::new($file.FullName)
    Save-Jpeg $image (Join-Path $exportRoot ($file.BaseName+'.jpg'))
    $records += [PSCustomObject]@{Fichier=$file.Name;Largeur=$image.Width;Hauteur=$image.Height}
    $image.Dispose()
}
$records | ConvertTo-Json | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $photoRoot '03_Sources_et_prompts\dimensions_exports.json')

# Contact sheet only: main deliverable photographs are not retouched here.
$sheet = [System.Drawing.Bitmap]::new(1600,1950)
$graphics = [System.Drawing.Graphics]::FromImage($sheet)
$graphics.Clear([System.Drawing.ColorTranslator]::FromHtml('#F6F3EB'))
$graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
Write-Text $graphics 'PHOTOS LINKEDIN / APERÇU' 50 30 1500 85 58 '#152C35' $true 'Segoe UI'
Write-Text $graphics '3 mises en scène IA + 2 visuels techniques issus du projet' 50 120 1500 60 29 '#50636B' $false 'Segoe UI'
$labels=@('01 / Workspace — illustration IA','02 / Code et gameplay — illustration IA','03 / Parcours — illustration IA','04 / Contrôleur C# — source réelle','05 / Organisation du niveau — source réelle')
for ($i=0; $i -lt $exports.Count; $i++) {
    $column = $i % 2
    $row = [Math]::Floor($i/2)
    $x = 50 + $column*770
    $y = 215 + $row*555
    Place-Image $graphics $exports[$i].FullName $x $y 730 487
    Write-Text $graphics $labels[$i] $x ($y+495) 730 48 24 '#152C35' $true 'Segoe UI'
}
Write-Text $graphics 'PNG et JPG en 1536 × 1024 • Captures originales et prompts conservés dans le dossier' 50 1890 1500 45 25 '#50636B' $false 'Segoe UI'
Save-Jpeg $sheet (Join-Path $photoRoot '00_Apercu.jpg')
$graphics.Dispose(); $sheet.Dispose()
Write-Output '5 images préparées en PNG et JPG, avec aperçu global.'
