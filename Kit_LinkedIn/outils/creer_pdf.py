"""Create a portable carousel PDF from the JPEG pages; Python standard library only."""
from pathlib import Path

root = Path(__file__).resolve().parent.parent
pages = sorted((root / 'outils' / 'pdf_images').glob('*.jpg'))
if len(pages) != 6:
    raise SystemExit(f'Expected 6 pages, got {len(pages)}')
objects = []

def add(data):
    objects.append(data.encode('ascii') if isinstance(data, str) else data)
    return len(objects)

def stream(header, data):
    return (f'<< {header} /Length {len(data)} >>\nstream\n'.encode('ascii')
            + data + b'\nendstream')

catalog = add('')
page_tree = add('')
page_ids = []
for path in pages:
    image_id = add(stream('/Type /XObject /Subtype /Image /Width 1080 /Height 1350 '
                          '/ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode', path.read_bytes()))
    content_id = add(stream('', b'q 540 0 0 675 0 0 cm /Im0 Do Q'))
    page_ids.append(add(f'<< /Type /Page /Parent {page_tree} 0 R /MediaBox [0 0 540 675] '
                        f'/Resources << /XObject << /Im0 {image_id} 0 R >> >> /Contents {content_id} 0 R >>'))
objects[catalog-1] = f'<< /Type /Catalog /Pages {page_tree} 0 R /Lang (fr-FR) >>'.encode('ascii')
objects[page_tree-1] = ('<< /Type /Pages /Count 6 /Kids [' + ' '.join(f'{i} 0 R' for i in page_ids) + '] >>').encode('ascii')
info = add('<< /Title (Prototype de plateforme 2D - Unity et C#) /Subject (Portfolio gameplay - presentation du projet) >>')
pdf = bytearray(b'%PDF-1.4\n%\xe2\xe3\xcf\xd3\n')
offsets = [0]
for i, data in enumerate(objects, 1):
    offsets.append(len(pdf))
    pdf.extend(f'{i} 0 obj\n'.encode('ascii') + data + b'\nendobj\n')
start = len(pdf)
pdf.extend(f'xref\n0 {len(objects)+1}\n0000000000 65535 f \n'.encode('ascii'))
for offset in offsets[1:]:
    pdf.extend(f'{offset:010d} 00000 n \n'.encode('ascii'))
pdf.extend(f'trailer\n<< /Size {len(objects)+1} /Root {catalog} 0 R /Info {info} 0 R >>\nstartxref\n{start}\n%%EOF\n'.encode('ascii'))
target = root / '04_Carrousel_LinkedIn.pdf'
target.write_bytes(pdf)
print(f'PDF created: {target.name}, {len(pdf):,} bytes, 6 pages')
