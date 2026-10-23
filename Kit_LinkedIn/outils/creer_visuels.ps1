param([string]$DeliveryRoot = (Split-Path -Parent $PSScriptRoot))
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$script:Ink = '#152C35'
$script:Muted = '#50636B'
$script:Paper = '#F6F3EB'
$script:Accent = '#DC5A36'
$script:Teal = '#14786A'
$pngRoot = Join-Path $DeliveryRoot 'carrousel'
$jpegRoot = Join-Path $DeliveryRoot 'outils\pdf_images'
New-Item -ItemType Directory -Path $pngRoot,$jpegRoot -Force | Out-Null

function Brush([string]$Color) { return [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Color)) }
function Fill($X,$Y,$W,$H,[string]$Color) {
    $b = Brush $Color
    $script:G.FillRectangle($b,[single]$X,[single]$Y,[single]$W,[single]$H)
    $b.Dispose()
}
function TextBlock([string]$Text,$X,$Y,$W,$H,$Size,[string]$Color = $script:Ink,[bool]$Bold = $false,[string]$Family = 'Segoe UI') {
    $style = [System.Drawing.FontStyle]::Regular
    if ($Bold) { $style = [System.Drawing.FontStyle]::Bold }
    $font = [System.Drawing.Font]::new($Family,[single]$Size,$style,[System.Drawing.GraphicsUnit]::Pixel)
    $brush = Brush $Color
    $format = [System.Drawing.StringFormat]::new()
    $format.Trimming = [System.Drawing.StringTrimming]::None
    $format.FormatFlags = [System.Drawing.StringFormatFlags]::LineLimit
    $measured = $script:G.MeasureString($Text,$font,[int]$W,$format)
    while ($measured.Height -gt ($H - 2) -and $Size -gt 14) {
        $font.Dispose()
        $Size -= 1
        $font = [System.Drawing.Font]::new($Family,[single]$Size,$style,[System.Drawing.GraphicsUnit]::Pixel)
        $measured = $script:G.MeasureString($Text,$font,[int]$W,$format)
    }
    $rect = [System.Drawing.RectangleF]::new([single]$X,[single]$Y,[single]$W,[single]$H)
    $script:G.DrawString($Text,$font,$brush,$rect,$format)
    $format.Dispose(); $brush.Dispose(); $font.Dispose()
}
function Picture([string]$Path,$X,$Y,$W,$H) {
    $image = [System.Drawing.Image]::FromFile($Path)
    $ratio = [Math]::Min($W / $image.Width,$H / $image.Height)
    $drawW = $image.Width * $ratio
    $drawH = $image.Height * $ratio
    $script:G.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
    $script:G.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
    $script:G.DrawImage($image,[System.Drawing.RectangleF]::new([single]($X + ($W-$drawW)/2),[single]($Y + ($H-$drawH)/2),[single]$drawW,[single]$drawH),[System.Drawing.RectangleF]::new(0,0,$image.Width,$image.Height),[System.Drawing.GraphicsUnit]::Pixel)
    $image.Dispose()
}
function StartSlide([string]$Tag,[int]$Number) {
    $script:Bitmap = [System.Drawing.Bitmap]::new(1080,1350)
    $script:G = [System.Drawing.Graphics]::FromImage($script:Bitmap)
    $script:G.Clear([System.Drawing.ColorTranslator]::FromHtml($script:Paper))
    $script:G.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
    $script:G.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Fill 60 58 14 14 $script:Accent
    TextBlock $Tag 92 47 850 42 24 $script:Muted $true
    Fill 60 1280 960 2 '#CFD5CF'
    TextBlock 'UNITY / C#     •     PROTOTYPE DE PLATEFORME 2D' 60 1298 880 32 18 $script:Muted
    TextBlock ('{0:00} / 06' -f $Number) 938 1295 100 35 20 $script:Muted $true
}
function FinishSlide([int]$Number,[string]$Slug) {
    $name = '{0:00}_{1}' -f $Number,$Slug
    $script:Bitmap.Save((Join-Path $pngRoot ($name + '.png')),[System.Drawing.Imaging.ImageFormat]::Png)
    $codec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
    $parameters = [System.Drawing.Imaging.EncoderParameters]::new(1)
    $parameters.Param[0] = [System.Drawing.Imaging.EncoderParameter]::new([System.Drawing.Imaging.Encoder]::Quality,[long]95)
    $script:Bitmap.Save((Join-Path $jpegRoot ($name + '.jpg')),$codec,$parameters)
    $parameters.Dispose(); $script:G.Dispose(); $script:Bitmap.Dispose()
}
$visualRoot = Join-Path $DeliveryRoot 'visuels'
StartSlide 'PORTFOLIO  /  GAMEPLAY PROGRAMMING' 1
TextBlock "Revisiter un`nclassique en 2D." 54 135 980 250 86 $script:Ink $true
TextBlock 'Des règles simples. Des choix techniques concrets.' 60 355 960 60 33 $script:Muted
Fill 60 445 960 430 '#DFF4F2'
Picture (Join-Path $visualRoot '07_collecte_score_playmode.png') 60 445 960 430
TextBlock 'Capture Play Mode • Scénario automatisé avec repositionnement' 60 895 960 42 22 $script:Muted
TextBlock 'Contrôleur, caméra et interactions réunis dans un prototype.' 60 960 960 90 32 $script:Ink
Fill 60 1070 280 62 $script:Ink
Fill 360 1070 300 62 $script:Teal
Fill 680 1070 340 62 $script:Accent
TextBlock 'MOUVEMENT' 83 1083 250 45 25 '#FFFFFF' $true
TextBlock 'PHYSIQUE 2D' 387 1083 260 45 25 '#FFFFFF' $true
TextBlock 'OUTILS UNITY' 720 1083 295 45 25 '#FFFFFF' $true
TextBlock "Un prototype pour relier programmation,`ncaméra et conception du parcours." 60 1172 960 96 31 $script:Ink
FinishSlide 1 'presentation'

StartSlide 'GAMEPLAY  /  CONTRÔLE DU PERSONNAGE' 2
TextBlock "Le ressenti`ncommence au saut." 54 135 990 230 79 $script:Ink $true
TextBlock "Accélération au sol et en lʼair, freinage`net hauteur de saut variable." 60 378 960 115 35 $script:Muted
Fill 60 530 960 300 $script:Ink
TextBlock 'PlayerController.cs  /  réglages du contrôleur' 88 550 900 45 25 '#A7D4CE'
TextBlock "public float coyoteTime = 0.12f;`npublic float jumpBufferTime = 0.12f;`npublic float fallGravityMultiplier = 2.2f;`npublic float lowJumpGravityMultiplier = 1.8f;" 88 618 920 180 28 '#F6F3EB' $false 'Consolas'
Fill 60 880 465 262 '#E3E8DF'
Fill 550 880 470 262 '#E3E8DF'
TextBlock '0,12 s' 82 895 420 90 65 $script:Teal $true
TextBlock 'COYOTE TIME' 85 991 410 45 26 $script:Ink $true
TextBlock "Une tolérance après`nle bord dʼune plateforme." 85 1040 405 89 29 $script:Muted
TextBlock '0,12 s' 574 895 420 90 65 $script:Teal $true
TextBlock 'JUMP BUFFER' 577 991 410 45 26 $script:Ink $true
TextBlock "Un appui mémorisé juste`navant lʼatterrissage." 577 1040 410 89 29 $script:Muted
TextBlock 'Valeurs du code • La qualité du ressenti reste à évaluer en jeu.' 60 1200 960 55 25 $script:Muted
FinishSlide 2 'saut_et_controle'

StartSlide 'ENVIRONNEMENT  /  CAMÉRA & PARALLAXE' 3
TextBlock "Un décor qui`nsuit le mouvement." 54 135 990 230 79 $script:Ink $true
Picture (Join-Path $visualRoot '03_arrivee_camera_unity.png') 60 380 960 540
TextBlock 'Rendu caméra Unity • Les facteurs ci-dessous proviennent du générateur.' 60 929 960 50 22 $script:Muted
$layers = @(@('NUAGES','0,08',120),@('RELIEF LOINTAIN','0,18',270),@('RELIEF PROCHE','0,28',420))
$row = 1000
foreach ($layer in $layers) {
    TextBlock $layer[0] 60 $row 370 47 25 $script:Ink $true
    Fill 445 ($row+10) 430 18 '#D5DED4'
    Fill 445 ($row+10) $layer[2] 18 $script:Teal
    TextBlock $layer[1] 912 ($row-4) 105 50 31 $script:Teal $true
    $row += 72
}
TextBlock '12 objets répartis sur 3 couches de parallaxe.' 60 1230 960 42 26 $script:Muted
FinishSlide 3 'camera_et_decor'

StartSlide 'WORKFLOW  /  OUTIL DʼÉDITEUR' 4
TextBlock "Du code`nau niveau." 54 130 990 225 88 $script:Ink $true
TextBlock "Un outil Unity génère le parcours, enregistre les scènes`net configure leur ordre dans les Build Settings." 60 370 960 110 33 $script:Muted
Fill 60 530 540 475 $script:Ink
TextBlock 'HIÉRARCHIE GÉNÉRÉE' 84 550 490 45 24 '#A7D4CE' $true
TextBlock "World`n  Systems`n  Background`n  Geometry`n  Coins`n  Enemies`n  Goal`n  Player" 92 615 465 370 34 '#F6F3EB' $false 'Consolas'
TextBlock '14' 660 522 350 100 80 $script:Teal $true
TextBlock 'PIÈCES À COLLECTER' 666 620 360 55 24 $script:Muted $true
TextBlock '3' 660 685 350 100 80 $script:Accent $true
TextBlock 'ENNEMIS EN PATROUILLE' 666 784 360 62 23 $script:Muted $true
TextBlock '8' 660 850 350 100 80 $script:Teal $true
TextBlock 'PLATEFORMES' 666 950 360 50 24 $script:Muted $true
Picture (Join-Path $visualRoot '04_vue_ensemble_camera_unity.png') 60 1040 960 210
TextBlock 'Schéma de hiérarchie et vue générale • Niveau créé par le générateur du projet' 60 1250 960 30 18 $script:Muted
FinishSlide 4 'outil_de_generation'

StartSlide 'SYSTÈMES  /  BOUCLE DE JEU' 5
TextBlock "Des interactions`nqui se répondent." 54 135 990 230 79 $script:Ink $true
Picture (Join-Path $visualRoot '07_collecte_score_playmode.png') 60 385 960 430
$cards = @(
    @(60,850,'01','EXPLORER',"Se déplacer et sauter`nentre les plateformes."),
    @(550,850,'02','COLLECTER',"Récupérer les pièces`net augmenter le score."),
    @(60,1040,'03','RECOMMENCER',"Éviter les ennemis ;`nrejouer après une défaite."),
    @(550,1040,'04','PROGRESSER',"Atteindre la fin et`ncharger la transition.")
)
foreach ($card in $cards) {
    $x = [int]$card[0]; $y = [int]$card[1]
    Fill $x $y 470 174 '#E3E8DF'
    TextBlock $card[2] ($x+20) ($y+14) 65 55 32 $script:Teal $true
    TextBlock $card[3] ($x+90) ($y+19) 365 48 26 $script:Ink $true
    TextBlock $card[4] ($x+22) ($y+75) 433 90 27 $script:Muted
}
TextBlock '1 niveau jouable + 1 écran de transition • Interactions vérifiées par scénario automatisé' 60 1238 960 39 20 $script:Muted
FinishSlide 5 'boucle_de_jeu'

StartSlide 'PROGRESSION  /  PROCHAINES ITÉRATIONS' 6
TextBlock "Construire.`nObserver.`nAméliorer." 54 130 990 350 88 $script:Ink $true
TextBlock "Revisiter un classique, cʼest travailler`nles fondamentaux du gameplay :`ncommandes, lisibilité et progression." 60 495 960 165 38 $script:Muted
$priorities = @(
    @('01','AFFINER LE VISUEL','Raccords des tuiles et lisibilité du décor.'),
    @('02','MONTRER LE JEU','Une vidéo courte avec les commandes réelles.'),
    @('03','MESURER ET ITÉRER','Tester le ressenti, puis profiler une version jouable.')
)
$y = 715
foreach ($priority in $priorities) {
    Fill 60 $y 8 112 $script:Accent
    TextBlock $priority[0] 90 ($y+4) 90 63 38 $script:Teal $true
    TextBlock $priority[1] 185 ($y+1) 830 49 29 $script:Ink $true
    TextBlock $priority[2] 185 ($y+52) 830 65 28 $script:Muted
    $y += 145
}
TextBlock 'Ouvert aux échanges avec les équipes de développement.' 60 1170 960 50 29 $script:Ink $true
TextBlock 'Assets : Kenney / PROWNE via OpenGameArt (CC0) • Prototype en développement' 60 1240 960 36 19 $script:Muted
FinishSlide 6 'perspectives'

# Extracted project assets, enlarged without generating new artwork.
$script:Bitmap = [System.Drawing.Bitmap]::new(1600,700)
$script:G = [System.Drawing.Graphics]::FromImage($script:Bitmap)
$script:G.Clear([System.Drawing.ColorTranslator]::FromHtml($script:Paper))
$script:G.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
TextBlock 'ASSETS DU PROJET  /  sélection extraite' 50 30 1500 70 42 $script:Ink $true
TextBlock 'Personnage : PROWNE via OpenGameArt • Décor et objets : Kenney • CC0' 50 105 1500 55 28 $script:Muted
$projectRoot = Join-Path (Split-Path -Parent $DeliveryRoot) 'Exam_26'
$frames = @('player_idle','player_run_1','player_run_2','player_run_3','player_run_4','player_jump')
for ($i = 0; $i -lt $frames.Count; $i++) {
    $x = 50 + $i*250
    Fill $x 210 230 325 '#E3E8DF'
    Picture (Join-Path $projectRoot ('Assets\Art\OpenGameArt\PlayerFrames\'+$frames[$i]+'.png')) ($x+25) 235 180 255
    TextBlock $frames[$i] ($x+8) 545 240 50 22 $script:Ink $false 'Consolas'
}
TextBlock 'Extraits du projet agrandis au plus proche voisin • Aucun nouvel asset graphique créé' 50 630 1500 45 25 $script:Muted
$script:Bitmap.Save((Join-Path $visualRoot '10_assets_extraits.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$script:G.Dispose(); $script:Bitmap.Dispose()
Write-Output '6 pages PNG et une planche de sprites exportées.'
