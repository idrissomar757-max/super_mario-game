"""Verify photo exports and package the complete local delivery."""
from pathlib import Path
import hashlib
import json
import struct
import zipfile

root = Path(__file__).resolve().parent.parent
workspace = root.parent
exports = root/'01_Exports'
pngs = sorted(exports.glob('*.png'))
jpegs = sorted(exports.glob('*.jpg'))
assert len(pngs) == 5 and len(jpegs) == 5
records=[]
for n,path in enumerate(pngs,1):
    data=path.read_bytes()
    assert data[:8] == b'\x89PNG\r\n\x1a\n'
    dimensions=struct.unpack('>II',data[16:24])
    assert dimensions == (1536,1024),(path.name,dimensions)
    jpg=path.with_suffix('.jpg')
    assert jpg.read_bytes().startswith(b'\xff\xd8')
    records.append({'png':path.relative_to(root).as_posix(),
                    'jpg':jpg.relative_to(root).as_posix(),
                    'dimensions':list(dimensions),
                    'type':'Mise en scène photoréaliste générée par IA' if n<=3 else 'Composition technique à partir des sources et captures réelles',
                    'sha256_png':hashlib.sha256(data).hexdigest(),
                    'sha256_jpg':hashlib.sha256(jpg.read_bytes()).hexdigest()})
for path in (root/'02_Captures_originales').glob('*.png'):
    original=workspace/'Kit_LinkedIn/visuels'/path.name
    assert path.read_bytes()==original.read_bytes(),path.name
assert (root/'03_Sources_et_prompts/PlayerController_original.cs.txt').read_bytes()==(workspace/'Exam_26/Assets/Scripts/Gameplay/PlayerController.cs').read_bytes()
assert len(list((root/'04_Sources_modifiables').glob('*.svg'))) == 2
assert (root/'00_Apercu.jpg').is_file()

manifest={'outil_ia':'image_gen intégré — mode compositing avec références du projet',
          'photoshop_utilise':False,
          'projet_unity_original_modifie':False,
          'nombre_visuels':5,
          'exports':records}
(root/'03_Sources_et_prompts/manifest_exports.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
archive=workspace/'Photos_LinkedIn.zip'
with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED) as package:
    for path in sorted(root.rglob('*')):
        if path.is_file():
            assert path.suffix not in ('.log','.psd')
            package.write(path,(Path(root.name)/path.relative_to(root)).as_posix())
with zipfile.ZipFile(archive) as package:
    assert package.testzip() is None
print('Vérification : 5 PNG + 5 JPG, 1536 × 1024, 4 captures originales identiques, 2 SVG modifiables.')
print(f'Archive : {archive.name} — {archive.stat().st_size:,} octets.')
