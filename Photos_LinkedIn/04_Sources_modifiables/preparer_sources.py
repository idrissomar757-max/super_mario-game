"""Create editable, fact-based source layouts. No generated UI or game imagery."""
from pathlib import Path
import base64
import html
import json

root = Path(__file__).resolve().parent.parent
ink, muted, teal, paper = '#152C35', '#50636B', '#14786A', '#F6F3EB'

def rect(x,y,w,h,fill):
    return dict(type='rect',x=x,y=y,width=w,height=h,fill=fill)

def text(value,x,y,w,h,size=30,color=ink,bold=False,family='Segoe UI'):
    return dict(type='text',value=value,x=x,y=y,width=w,height=h,size=size,color=color,bold=bold,family=family)

def image(name,x,y,w,h):
    return dict(type='image',file='02_Captures_originales/'+name,x=x,y=y,width=w,height=h)

common=[rect(64,55,12,12,teal),text('UNITY / C#   —   PROTOTYPE DE PLATEFORME 2D',94,43,1360,48,23,muted,True)]
footer=[rect(64,950,1408,2,'#D4DCD4'),text('Code et captures réels du projet • Mise en page de présentation',64,971,1050,37,22,muted),text('Kenney / PROWNE',1208,971,264,37,20,muted)]

code=common+[
 text('Du mouvement au ressenti.',59,120,1420,110,72,ink,True),
 text('Un contrôleur C# avec deux fenêtres de tolérance pour le saut.',64,235,1408,60,31,muted),
 image('07_collecte_score_playmode.png',64,319,880,393),
 rect(984,319,488,393,ink),
 text('PlayerController.cs',1010,345,435,47,27,'#A7D4CE',True,'Consolas'),
 text('public float coyoteTime = 0.12f;',1010,431,435,40,21,'#F6F3EB',False,'Consolas'),
 text('public float jumpBufferTime = 0.12f;',1010,476,435,40,21,'#F6F3EB',False,'Consolas'),
 text('Extrait des lignes 17 et 18.',1010,625,435,42,23,'#A7D4CE'),
 rect(64,757,680,151,'#E3E8DF'),rect(768,757,704,151,'#E3E8DF'),
 text('0,12 s  /  COYOTE TIME',88,775,630,55,37,teal,True),
 text('Tolérance après le bord d’une plateforme.',88,843,630,50,27,muted),
 text('0,12 s  /  JUMP BUFFER',792,775,654,55,37,teal,True),
 text('Appui mémorisé avant l’atterrissage.',792,843,654,50,27,muted)
]+footer

hierarchy=common+[
 text('Du code à la construction du niveau.',59,120,1420,110,65,ink,True),
 text('Organisation de la scène produite par le générateur du projet.',64,235,1408,55,31,muted),
 rect(64,319,460,570,ink),
 text('HIÉRARCHIE / EXTRAIT',88,341,410,45,25,'#A7D4CE',True),
 text('World\n  Systems\n  Background\n  Geometry\n  Coins\n  Enemies\n  Goal\n  Player',96,407,395,454,34,'#F6F3EB',False,'Consolas'),
 image('04_vue_ensemble_camera_unity.png',564,319,908,285),
 rect(564,643,278,130,'#E3E8DF'),rect(879,643,278,130,'#E3E8DF'),rect(1194,643,278,130,'#E3E8DF'),
 text('14',586,648,230,78,58,teal,True),text('PIÈCES',586,733,230,36,23,muted,True),
 text('3',901,648,230,78,58,teal,True),text('ENNEMIS',901,733,230,36,23,muted,True),
 text('8',1216,648,230,78,58,teal,True),text('PLATEFORMES',1216,733,230,36,23,muted,True),
 text('1 niveau de démonstration',564,810,908,47,31,ink,True),
 text('+ 1 écran de transition',564,858,908,45,31,muted)
]+footer

layouts=[dict(name='04_controle_du_personnage',width=1536,height=1024,background=paper,items=code),
         dict(name='05_organisation_du_niveau',width=1536,height=1024,background=paper,items=hierarchy)]
(root/'04_Sources_modifiables/layouts.json').write_text(json.dumps(layouts,ensure_ascii=False,indent=2),encoding='utf-8')
for layout in layouts:
    body=[f'<rect width="1536" height="1024" fill="{paper}"/>']
    for item in layout['items']:
        x,y,w,h=(item[key] for key in ('x','y','width','height'))
        if item['type']=='rect':
            body.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{item["fill"]}"/>')
        elif item['type']=='image':
            content=base64.b64encode((root/item['file']).read_bytes()).decode('ascii')
            body.append(f'<image x="{x}" y="{y}" width="{w}" height="{h}" preserveAspectRatio="xMidYMid meet" image-rendering="pixelated" href="data:image/png;base64,{content}"/>')
        else:
            size=item['size']; weight='700' if item['bold'] else '400'
            lines=item['value'].splitlines()
            body.append(f'<text x="{x}" y="{y+size}" font-family="{item["family"]},sans-serif" font-size="{size}" fill="{item["color"]}" font-weight="{weight}">')
            for n,line in enumerate(lines):
                body.append(f'<tspan x="{x}" dy="{0 if n==0 else size*1.32}">{html.escape(line)}</tspan>')
            body.append('</text>')
    svg='<svg xmlns="http://www.w3.org/2000/svg" width="1536" height="1024" viewBox="0 0 1536 1024">'+''.join(body)+'</svg>'
    (root/'04_Sources_modifiables'/f'{layout["name"]}.svg').write_text(svg,encoding='utf-8')
print('2 sources SVG modifiables et un fichier de mise en page préparés.')
