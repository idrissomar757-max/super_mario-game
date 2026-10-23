# Guide des visuels et de la publication

## Provenance des images

| Fichier dans `visuels/` | Nature et légende proposée |
|---|---|
| `01_depart_camera_unity.png` | Rendu Unity en mode édition : personnage au point de départ, sol et premier obstacle. Le personnage est à sa position définie par le générateur, avant la simulation physique. |
| `02_plateformes_camera_unity.png` | Rendu Unity en mode édition : portion centrale, plateformes, pièces et ennemi. Caméra déplacée pour montrer cette zone ; personnage resté au départ. |
| `03_arrivee_camera_unity.png` | Rendu Unity en mode édition : dernière partie du parcours et zone d’arrivée. Caméra déplacée pour montrer cette zone. |
| `04_vue_ensemble_camera_unity.png` | Rendu Unity en mode édition : vue générale avec un cadrage élargi pour expliquer la disposition du parcours. |
| `05_fond_transition_camera_unity.png` | Rendu du fond de la scène `NextLevel`, sans les textes `OnGUI`. Ce fond seul ne représente pas l’interface complète de transition. |
| `06_jeu_hud_playmode.png` | Capture Play Mode réelle : personnage au départ, après stabilisation physique, et score à zéro. |
| `07_collecte_score_playmode.png` | Capture Play Mode réelle après collecte : score à un et personnage repositionné automatiquement pour tester la pièce. |
| `08_game_over_playmode.png` | Capture Play Mode réelle après contact automatique avec un ennemi. Une instruction est coupée dans l’interface actuelle ; défaut conservé dans le fichier. |
| `09_transition_playmode.png` | Capture Play Mode réelle de la transition après déclenchement automatique de l’arrivée. Des textes sont coupés dans l’interface actuelle ; défaut conservé dans le fichier. |
| `10_assets_extraits.png` | Planche des six images du personnage utilisées par les scripts : repos, quatre étapes de course et saut. Extraits agrandis au plus proche voisin, avec leurs crédits. |

Les rendus 1, 2, 3 et 5 mesurent 1920 × 1080 ; le rendu général mesure 3200 × 1000. Les captures Play Mode mesurent 1536 × 685. Les trois zones du niveau sont des cadrages du même niveau, pas trois niveaux différents. Aucun HUD, écran Game Over ni interface Play Mode n’a été ajouté artificiellement aux images.

## Ordre proposé pour le carrousel

1. **Présentation** : expliquer le projet et ses trois axes — mouvement, physique et outils.
2. **Saut et contrôle** : relier un extrait réel du contrôleur à l’utilité du coyote time et du jump buffer.
3. **Caméra et décor** : montrer le parcours et les coefficients des trois couches de parallaxe.
4. **Outil de génération** : expliquer la hiérarchie et les éléments créés automatiquement.
5. **Boucle de jeu** : relier exploration, collecte, défaite et transition.
6. **Perspectives** : proposer une suite concrète et ouvrir la discussion avec les studios.

Les pages PNG se trouvent dans `carrousel/`. Le PDF regroupe ces mêmes six pages. La présentation mélange captures Play Mode, rendus caméra en mode édition et schémas explicatifs. Les captures Play Mode proviennent d’un scénario automatisé avec repositionnement ; elles ne montrent pas un parcours continu joué au clavier.

## Textes alternatifs des pages

1. Présentation d’un prototype de plateforme 2D sous Unity et C#. Une capture Play Mode montre le personnage sur une plateforme, des pièces et un score à un après un test de collecte. Trois axes sont annoncés : mouvement, physique 2D et outils Unity.
2. Extrait du contrôleur C# montrant un coyote time de 0,12 seconde, un jump buffer de 0,12 seconde et deux multiplicateurs de gravité. Les cartes expliquent la tolérance après le bord d’une plateforme et la mémorisation d’un appui avant l’atterrissage.
3. Rendu de la fin du parcours. Trois barres indiquent les facteurs de parallaxe horizontale configurés dans le générateur : nuages 0,08, relief lointain 0,18 et relief proche 0,28.
4. Schéma de la hiérarchie générée sous World : Systems, Background, Geometry, Coins, Enemies, Goal et Player. La scène contient 14 pièces, 3 ennemis et 8 plateformes. Une vue générale montre leur disposition.
5. Capture Play Mode après un test automatique de collecte et quatre étapes de la boucle définie dans les scripts : explorer, collecter, recommencer après une défaite et atteindre la transition. Le projet contient un niveau jouable de démonstration et un écran de transition.
6. Trois prochaines priorités sont proposées : améliorer les raccords visuels, enregistrer une courte vidéo avec les commandes réelles et mesurer une version jouable. Les crédits citent Kenney et PROWNE via OpenGameArt.

## Utilisation de la publication

Utiliser le texte français avec le carrousel, ou choisir quelques rendus caméra pour une publication plus courte. Le texte anglais est une adaptation indépendante, destinée à une audience internationale.

Personnaliser la phrase sur les opportunités selon votre recherche réelle : stage, premier poste, alternance ou collaboration. Le texte actuel reste volontairement général. Si d’autres personnes ont participé, préciser leur rôle et ajuster les formulations à la première personne.

Le texte ne contient ni chiffres de performance non mesurés, ni création graphique attribuée à tort, ni annonce de sortie commerciale. Les limites et crédits sont détaillés dans l’analyse.

## Captures à ajouter lors d’une prochaine session Unity

Préparer une fenêtre `Game` avec le score visible, une fenêtre `Scene` cadrant le niveau et une sélection du personnage dans l’Inspector. Capturer séparément le contrôleur, les colliders et la hiérarchie permettrait de montrer le travail d’intégration. Une capture du Profiler peut compléter ces vues après une vraie session de mesure.

Les captures du jeu sont incluses. Les vues de l’éditeur, de l’Inspector et du Profiler restent à réaliser.
