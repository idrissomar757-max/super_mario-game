"""Assemble evidence, a local review page, and a clean shareable archive."""
from pathlib import Path
import hashlib
import html
import json
import shutil
import struct
import zipfile
from html.parser import HTMLParser

root = Path(__file__).resolve().parent.parent
workspace = root.parent
project = workspace / 'Exam_26'
capture = workspace / '.capture_unity'

proof = root / 'preuves'
proof.mkdir(exist_ok=True)
raw_log = capture / 'unity_capture_original.log'
if raw_log.exists():
    selected = [line for line in raw_log.read_text(errors='replace').splitlines()
                if any(key in line for key in ('Version is', 'Renderer:', 'Starting compile',
                        'Finished compile', 'Scenes creees:', 'PORTFOLIO_CAPTURE_SUCCESS',
                        'Exiting batchmode successfully'))]
    (proof / 'unity_capture_resume.txt').write_text('\n'.join(selected)+'\n', encoding='utf-8')

records = []
for path in sorted((project / 'Assets').rglob('*.cs')):
    relative = path.relative_to(project)
    records.append({'source': relative.as_posix(),
                    'sha256': hashlib.sha256(path.read_bytes()).hexdigest(),
                    'copie_identique': path.read_bytes() == (capture / relative).read_bytes()})
(proof / 'integrite_scripts.json').write_text(json.dumps(records, ensure_ascii=False, indent=2), encoding='utf-8')
assert all(record['copie_identique'] for record in records)
playmode = (proof / 'verification_playmode.txt').read_text(encoding='utf-8')
for expected in ('Initial grounded state: True', 'Initial score: 0',
                 'Coin collection score: 1 (expected 1)', 'Coins remaining: 13 (expected 13)',
                 'Enemy contact GameOver: True (expected True)', 'GameOver timeScale: 0 (expected 0)',
                 'Reload score: 0 (expected 0)', 'Reload GameOver: False (expected False)',
                 'Falling below killY GameOver: True (expected True)',
                 'Goal transition scene: NextLevel (expected NextLevel)'):
    assert expected in playmode, expected
interactive_log = workspace / 'Communication_LinkedIn/preuves/unity_playmode.log'
if interactive_log.exists():
    filtered = [line for line in interactive_log.read_text(errors='replace').splitlines()
                if ('NullReferenceException' in line or 'PortfolioRuntimeCapture' in line
                    or 'PORTFOLIO_PLAYMODE_SUCCESS' in line)]
    (proof / 'unity_playmode_resume.txt').write_text(
        'Extraits du journal : erreurs de l’outil de capture et fin du scénario.\n'
        + '\n'.join(filtered) + '\n', encoding='utf-8')

source = root / 'sources'
frames = source / 'assets_personnage'
frames.mkdir(parents=True, exist_ok=True)
for path in (project / 'Assets/Art/OpenGameArt/PlayerFrames').glob('player_*.png'):
    shutil.copy2(path, frames / path.name)
shutil.copy2(project / 'Assets/Art/THIRD_PARTY_ASSETS.md', source / 'THIRD_PARTY_ASSETS_original.md')
lines = (project / 'Assets/Scripts/Gameplay/PlayerController.cs').read_text().splitlines()
(source / 'extrait_PlayerController.txt').write_text(
    'Source : Assets/Scripts/Gameplay/PlayerController.cs, lignes 9 à 20\n\n'
    + '\n'.join(lines[8:20]) + '\n', encoding='utf-8')
tools = root / 'outils/unity'
tools.mkdir(exist_ok=True)
for relative in ('Assets/Editor/PortfolioCapture.cs', 'Assets/Editor/PortfolioSession.cs', 'Assets/PortfolioRuntimeCapture.cs'):
    content = (capture / relative).read_text(encoding='utf-8')
    # Adapt only the export destination in the delivered utilities.
    content = content.replace('../../Communication_LinkedIn', '../../Kit_LinkedIn')
    (tools / Path(relative).name).write_text(content, encoding='utf-8')

fr = (root / '02_Publication_LinkedIn.txt').read_text(encoding='utf-8')
en = (root / '03_Publication_EN.txt').read_text(encoding='utf-8')
slide_paths = sorted((root / 'carrousel').glob('*.png'))
image_paths = sorted((root / 'visuels').glob('*.png'))
manifest = []
for path in slide_paths + image_paths:
    data = path.read_bytes()
    assert data[:8] == b'\x89PNG\r\n\x1a\n'
    width, height = struct.unpack('>II', data[16:24])
    manifest.append({'fichier': path.relative_to(root).as_posix(), 'largeur': width, 'hauteur': height,
                     'sha256': hashlib.sha256(data).hexdigest()})
(proof / 'manifest_visuels.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding='utf-8')

slide_names = ['Présentation', 'Saut et contrôle', 'Caméra et décor', 'Outil de génération', 'Boucle de jeu', 'Perspectives']
slide_cards = ''.join(f'<a class="slide" href="{p.relative_to(root).as_posix()}" target="_blank" rel="noopener">'
                      f'<img loading="lazy" src="{p.relative_to(root).as_posix()}" alt="Page {i+1} : {slide_names[i]}">'
                      f'<span>{i+1:02d} / {slide_names[i]}</span></a>' for i,p in enumerate(slide_paths))
image_names = ['Départ du niveau — rendu caméra', 'Plateformes et pièces — rendu caméra', 'Zone d’arrivée — rendu caméra', 'Vue générale — rendu caméra', 'Fond de transition — rendu caméra', 'Départ avec le score — Play Mode', 'Collecte et score — Play Mode', 'Game Over — texte à corriger', 'Transition — textes à corriger', 'Animations du personnage — assets extraits']
image_cards = ''.join(f'<a class="asset" href="{p.relative_to(root).as_posix()}" target="_blank" rel="noopener">'
                      f'<img loading="lazy" src="{p.relative_to(root).as_posix()}" alt="{image_names[i]}">'
                      f'<span>{html.escape(image_names[i])}</span></a>' for i,p in enumerate(image_paths))
document = '''<!doctype html>
<html lang="fr"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>Prototype de plateforme 2D — Dossier LinkedIn</title>
<style>
:root{color-scheme:light;--ink:#152c35;--muted:#50636b;--paper:#f6f3eb;--teal:#14786a;--line:#d6ddd5}
*{box-sizing:border-box}body{margin:0;background:var(--paper);color:var(--ink);font:17px/1.65 "Segoe UI",Arial,sans-serif}
main{max-width:1280px;margin:auto;padding:60px 32px 80px}header{padding-bottom:36px;border-bottom:1px solid var(--line)}
.eyebrow{font-size:13px;letter-spacing:2px;font-weight:700;color:var(--teal)}h1{font-size:clamp(38px,6vw,72px);line-height:1.08;max-width:1000px;letter-spacing:-2px;margin:20px 0}h2{font-size:30px;margin:0 0 14px}
.intro{max-width:850px;color:var(--muted);font-size:21px}nav,.actions{display:flex;flex-wrap:wrap;gap:12px;margin:26px 0}
nav a,.button,button{display:inline-block;border:1px solid var(--line);background:white;color:var(--ink);padding:11px 18px;border-radius:4px;text-decoration:none;font:inherit;cursor:pointer}
.primary{background:var(--ink);color:white;border-color:var(--ink)}section{margin-top:50px}.status{padding:20px 24px;background:#e3e8df;border-left:4px solid var(--teal);font-size:15px}.status p{margin:5px 0}
.slides{display:grid;grid-template-columns:repeat(3,1fr);gap:20px}.slide,.asset{display:block;color:var(--ink);text-decoration:none;border:1px solid var(--line);background:white}.slide img{width:100%;height:auto;display:block}.slide span,.asset span{display:block;padding:12px 16px;font-size:14px;font-weight:600}
.gallery{display:grid;grid-template-columns:repeat(2,1fr);gap:20px}.asset img{width:100%;height:245px;object-fit:contain;background:#e7f2ee;display:block;image-rendering:pixelated}
.copy{background:#fff;border:1px solid var(--line);padding:25px}.copy textarea{width:100%;height:570px;resize:vertical;border:0;font:17px/1.8 "Segoe UI",Arial,sans-serif;color:var(--ink);background:white}.tabs{display:flex;gap:10px;align-items:center;margin-bottom:15px}.tabs button[aria-pressed=true]{background:var(--ink);color:white}.notes{display:grid;grid-template-columns:1fr 1fr;gap:20px}.note{padding:25px;background:#fff;border:1px solid var(--line)}a{color:var(--teal)}.note p{margin-bottom:0;color:var(--muted)}footer{margin-top:60px;color:var(--muted);font-size:14px}#copyStatus{color:var(--teal);font-size:14px}
@media(max-width:800px){main{padding:30px 18px}.slides{grid-template-columns:repeat(2,1fr)}.gallery,.notes{grid-template-columns:1fr}.asset img{height:auto}}
@media(max-width:460px){.slides{grid-template-columns:1fr}h1{letter-spacing:-1px}.copy{padding:15px}}
</style>
<main><header><div class="eyebrow">UNITY / C# · PORTFOLIO · COMMUNICATION</div>
<h1>Revisiter un classique.<br>Expliquer les choix techniques.</h1>
<p class="intro">Une présentation professionnelle du prototype de plateforme 2D : son contrôleur, sa caméra, sa boucle de jeu et son outil de génération de scènes.</p>
<nav><a class="primary" href="04_Carrousel_LinkedIn.pdf">Ouvrir le carrousel PDF</a><a href="#publication">Lire la publication</a><a href="01_Analyse_et_recommandations.md">Analyse détaillée</a><a href="#visuels">Voir les images</a></nav></header>
<section class="status"><p><strong>Vérifié :</strong> compilation des scripts, génération des scènes et cinq rendus caméra avec Unity 2017.2.0f3, dans une copie isolée.</p>
<p><strong>Vérification ciblée :</strong> l’instance Unity déjà ouverte a produit quatre captures Play Mode et confirmé collecte, défaite et transition dans un scénario avec repositionnement automatique. Les captures de l’éditeur, les essais au clavier et les mesures de performance restent à compléter.</p></section>
<section><h2>Le carrousel</h2><p>Six pages exportées en PNG, également réunies dans le PDF. Cliquer sur une page pour l’ouvrir en grand.</p><div class="slides">__SLIDES__</div></section>
<section id="publication"><h2>La publication</h2><p>Version française et adaptation anglaise. Personnaliser la phrase sur les opportunités et le rôle exact dans le projet avant publication.</p>
<div class="copy"><div class="tabs"><button type="button" id="frTab" aria-pressed="true">Français</button><button type="button" id="enTab" aria-pressed="false">English</button><button type="button" id="copyBtn">Copier le texte</button><span id="copyStatus" role="status"></span></div>
<textarea id="post" aria-label="Texte de la publication">__FR__</textarea></div></section>
<section id="visuels"><h2>Les visuels du projet</h2><p>Cinq rendus caméra en mode édition, quatre captures Play Mode et une planche de sprites. Les textes coupés de Game Over et de la transition sont conservés et décrits dans l’analyse.</p><div class="gallery">__IMAGES__</div></section>
<section><h2>Préparer la suite</h2><div class="notes"><article class="note"><strong>Affiner la présentation</strong><p>Améliorer les raccords des tuiles et la lisibilité du décor, puis montrer le mouvement dans une courte vidéo.</p><a href="06_Plan_video_et_mesures.md">Plan vidéo et tableau de mesures</a></article><article class="note"><strong>Garder les preuves lisibles</strong><p>Le dossier précise les fonctions présentes, les limites du prototype et la provenance de chaque image.</p><a href="05_Guide_visuels_et_publication.md">Légendes, textes alternatifs et provenance</a></article></div></section>
<footer>Projet de portfolio en développement · Assets visuels : Kenney / PROWNE via OpenGameArt (CC0).<br>Préparation locale : aucune publication LinkedIn effectuée. Le projet original est conservé dans Exam_26.</footer></main>
<script>const posts=__POSTS__;const field=document.getElementById('post');function choose(lang){field.value=posts[lang];document.getElementById('frTab').setAttribute('aria-pressed',lang==='fr');document.getElementById('enTab').setAttribute('aria-pressed',lang==='en');document.getElementById('copyStatus').textContent='';}document.getElementById('frTab').onclick=()=>choose('fr');document.getElementById('enTab').onclick=()=>choose('en');document.getElementById('copyBtn').onclick=async()=>{try{await navigator.clipboard.writeText(field.value);document.getElementById('copyStatus').textContent='Texte copié.';}catch(error){field.focus();field.select();document.getElementById('copyStatus').textContent='Texte sélectionné : utilisez Ctrl+C.';}};</script></html>'''
document = (document.replace('__SLIDES__', slide_cards).replace('__IMAGES__', image_cards)
            .replace('__FR__', html.escape(fr)).replace('__POSTS__', json.dumps({'fr':fr,'en':en}, ensure_ascii=False).replace('</','<\\/')))
(root / 'index.html').write_text(document, encoding='utf-8')
class LinkAudit(HTMLParser):
    def handle_starttag(self, tag, attrs):
        for key, value in attrs:
            if key in ('src', 'href') and value and not value.startswith(('#', 'https:', 'http:')):
                assert (root / value).is_file(), f'Missing local link: {value}'
LinkAudit().feed(document)
(capture / 'index_inline_check.js').write_text(
    document.split('<script>',1)[1].split('</script>',1)[0], encoding='utf-8')

pdf_path = root / '04_Carrousel_LinkedIn.pdf'
from pypdf import PdfReader
reader = PdfReader(str(pdf_path))
assert len(reader.pages) == 6
assert all(float(page.mediabox.width) == 540 and float(page.mediabox.height) == 675 for page in reader.pages)
assert len(slide_paths) == 6 and len(image_paths) == 10
assert all(item['largeur']==1080 and item['hauteur']==1350 for item in manifest[:6])

summary = ('VÉRIFICATIONS DU DOSSIER\n\n'
           f'{len(records)} scripts originaux identiques dans la copie de capture (SHA-256 conservés).\n'
           'Compilation Unity et génération des scènes : confirmées par le journal filtré.\n'
           '5 rendus caméra Unity, 4 captures Play Mode, 1 planche d’assets, 6 pages PNG 1080 × 1350.\n'
           'PDF vérifié : 6 pages, ratio 4:5.\n'
           'Pages vérifiées visuellement après correction des zones de texte.\n'
           'Scénario Play Mode avec repositionnement : collecte, défaite, chute et transition observées.\n'
           'Exceptions dans l’outil de capture pendant les chargements : portée limitée, voir analyse.\n'
           'Parcours au clavier, captures de l’éditeur et mesures de FPS : non réalisés.\n'
           'Les journaux de licence bruts ne sont pas inclus dans ce kit.\n'
           'Aucun envoi ou publication sur LinkedIn.\n')
(proof / 'verification_livrables.txt').write_text(summary, encoding='utf-8')

archive = workspace / 'Kit_LinkedIn.zip'
with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED) as output:
    for path in sorted(root.rglob('*')):
        if path.is_file() and path.suffix != '.flag':
            assert path.suffix != '.log', 'Raw log excluded from shareable kit'
            output.write(path, (Path(root.name)/path.relative_to(root)).as_posix())
with zipfile.ZipFile(archive) as output:
    assert output.testzip() is None
print(f'Kit final : {len(slide_paths)} pages PNG, {len(image_paths)} visuels, {len(reader.pages)} pages PDF.')
print(f'Publication FR : {len(fr)} caractères. Publication EN : {len(en)} caractères.')
print(f'Archive vérifiée : {archive.name}, {archive.stat().st_size:,} octets.')
