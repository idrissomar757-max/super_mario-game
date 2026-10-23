# Analyse du prototype de plateforme 2D

Projet analysé : `Exam_26` — Unity **2017.2.0f3**, C#. Analyse réalisée le 22 octobre 2026.

## Positionnement pour un studio

Présenter ce travail comme un **prototype de gameplay 2D inspiré des classiques du jeu de plateforme**. Son intérêt professionnel réside dans le contrôleur, les interactions, la caméra et l’outil de génération de scènes. Le nom public conseillé est « Prototype de plateforme 2D — Unity / C# » : aucun titre original n’est établi dans les fichiers fournis.

Les jeux de plateforme traditionnels offrent un terrain concret pour travailler des fondamentaux : lisibilité des obstacles, précision des commandes, timing du saut et relation entre caméra et parcours. La référence aux classiques donne un contexte au projet ; expliquer les décisions techniques permet de montrer ce que ce travail apporte à un portfolio.

La publication proposée emploie la première personne en suivant votre présentation du projet. Les fichiers seuls ne permettent pas d’établir la contribution de chaque personne : adapter cette formulation si le projet a été réalisé en équipe ou avec un accompagnement.

## Caractéristiques vérifiées dans les sources

| Système | Ce qui est présent | Ce que cela permet de montrer |
|---|---|---|
| Déplacement | Accélération et freinage distincts au sol et en l’air, `Rigidbody2D`, interpolation | Réglage du mouvement et intégration de la physique 2D |
| Saut | Hauteur variable au relâchement, gravité de chute renforcée, coyote time et jump buffer | Attention portée à la commande de saut |
| Caméra | `SmoothDamp`, anticipation horizontale selon vitesse et direction, décalage vertical, limites | Relation entre mouvement et cadrage |
| Animation | Repos, course sur 4 images, saut, retournement horizontal | Animation par sprites pilotée par l’état du personnage |
| Collecte | Détection par trigger, score, disparition de la pièce, oscillation et pulsation | Interaction entre physique, état de partie et retour visuel |
| Ennemis | Patrouille entre deux bornes, déplacement cinématique, contact entraînant la défaite | Comportement simple et gestion des collisions |
| État de partie | Score, arrêt du temps à la défaite, redémarrage, détection de chute | Boucle de jeu et gestion de l’état |
| Fin du parcours | Trigger, chargement de la scène suivante, écran permettant de rejouer | Transition entre scènes |
| Décor | Ciel, nuages et deux couches de relief, avec défilement différencié | Organisation des plans et effet de profondeur |
| Outil Unity | Commande de menu générant les scènes et renseignant les Build Settings | Automatisation d’une tâche d’éditeur |

Le générateur définit **un niveau jouable**, **une scène de transition**, **4 bandes de sol**, **8 plateformes**, **4 blocs**, **14 pièces**, **3 ennemis** et **12 objets de parallaxe**. Les 12 objets correspondent à quatre nuages et quatre objets pour chacune des deux couches de relief. Il ne s’agit pas de 12 plans visuels distincts.

La scène générée comporte **173 SpriteRenderer**. Ce nombre décrit la composition de la scène ; il ne mesure ni son coût GPU, ni le nombre de draw calls.

## Réglages intéressants à expliquer

| Paramètre du projet | Valeur | Lecture correcte |
|---|---:|---|
| Coyote time | 0,12 s | Fenêtre de saut après la perte du contact au sol |
| Jump buffer | 0,12 s | Mémorisation brève d’un appui avant le retour au sol |
| Vitesse maximale | 6 unités Unity/s | Réglage du contrôleur, pas une mesure de performance |
| Gravité en chute | × 2,2 | Multiplicateur appliqué à la gravité de base |
| Gravité après relâchement du saut | × 1,8 | Permet un saut plus court |
| Suivi caméra dans le générateur | 0,14 s | Paramètre de lissage, pas une latence mesurée |
| Parallaxe horizontale | 0,08 / 0,18 / 0,28 | Facteurs de déplacement relatifs à la caméra |
| Pas de physique configuré | 0,02 s | Fréquence cible de 50 mises à jour physiques/s ; aucune garantie de FPS |

Sources principales : `PlayerController.cs:9`, `CameraFollow2D.cs:20`, `SuperMarioDemoSceneBuilder.cs:202`, `SuperMarioDemoSceneBuilder.cs:353` et `ProjectSettings/TimeManager.asset:6`. Les valeurs spécifiques du générateur priment sur les valeurs par défaut des scripts pour cette scène.

## État du projet et limites de la démonstration

Au début de l’analyse, `Assets/Scenes` était vide et les Build Settings ne contenaient aucune scène. Le générateur du projet a été exécuté avec la version Unity installée, **dans une copie isolée**, pour produire les scènes et les visuels. Le dossier original `Exam_26` n’a pas été utilisé comme dossier de capture.

`NextLevel` est un écran de confirmation et de retour au jeu : ce n’est pas un deuxième niveau jouable. Les blocs « question » ont un sprite et un collider ; aucun script d’impact, de récompense ou de destruction n’est présent. Les ennemis patrouillent ; aucun comportement de combat, d’écrasement ou de recherche de chemin n’est implémenté dans les scripts analysés.

Aucun système de sauvegarde, checkpoint, menu d’accueil, pause, son ou commande tactile n’a été identifié dans les scripts fournis. L’interface utilise `OnGUI`. La compatibilité mobile, WebGL et console n’a pas été vérifiée. Les entrées clavier définies dans le projet constituent la démonstration actuelle.

Les cinq rendus caméra ont été produits avec succès par Unity. Les scripts du projet ont compilé dans la copie de présentation. Après un problème initial de démarrage de la méthode de capture, l’instance Unity déjà ouverte a finalement terminé le scénario automatique. Une relance supplémentaire n’a pas été autorisée et n’a pas eu lieu. **Quatre captures du jeu en Play Mode sont disponibles ; aucune capture de l’éditeur complet n’a été obtenue.**

Le fichier `preuves/verification_playmode.txt` enregistre les observations suivantes : personnage initialement au sol, score passant de 0 à 1 lors d’une collecte, 13 pièces restantes, défaite après contact avec un ennemi, arrêt du temps à 0, remise à zéro du score après rechargement, défaite sous la limite de chute et chargement de `NextLevel` au trigger d’arrivée. Ces observations correspondent aux valeurs attendues du scénario.

Le scénario **repositionne automatiquement le personnage** et désactive temporairement son contrôleur pour isoler les triggers et collisions. Il ne valide pas la qualité des commandes, le franchissement naturel du parcours, les fenêtres exactes de saut ou le redémarrage via une touche. Le journal contient également des `NullReferenceException` provenant de `PortfolioRuntimeCapture`, l’outil ajouté pour les captures, lors des chargements de scènes. Le résultat est donc une vérification ciblée des interactions, pas une session de jeu certifiée sans erreur.

Les captures `08_game_over_playmode.png` et `09_transition_playmode.png` montrent des **textes coupés** : les rectangles d’affichage `OnGUI` ne suffisent pas à certaines chaînes et tailles de police. Corriger ces écrans avant une vidéo de présentation : dimensions des labels, retour à la ligne et adaptation à la taille de la fenêtre. Les fichiers originaux sont conservés sans retouche du texte.

## Performances : ce qui peut être annoncé

Aucun benchmark représentatif n’a été effectué. Les images exportées, les réglages de physique et la compilation des scripts ne permettent pas d’annoncer un nombre de FPS, une consommation mémoire ou une compatibilité matérielle générale.

Pour ajouter ensuite des chiffres crédibles :

1. Produire une version Windows et décrire le processeur, le GPU, la RAM, la résolution, la qualité graphique et la VSync.
2. Parcourir le même trajet pendant 60 secondes, après chargement, puis répéter trois fois.
3. Relever le temps de frame médian et au 95e percentile, ainsi que les mesures CPU, GPU, mémoire et allocations pertinentes. Distinguer la capture Profiler d’une mesure sans instrumentation.
4. Publier les conditions et l’étendue du test avec les résultats. Un objectif de 60 FPS correspond à un budget d’environ 16,7 ms par image ; cela reste un objectif tant qu’il n’est pas mesuré.

Le [Profiler Unity 2017.2](https://docs.unity3d.com/2017.2/Documentation/Manual/Profiler.html) permet d’examiner les coûts CPU, GPU, mémoire, rendu et audio. Le protocole ci-dessus est une proposition adaptée à ce projet, pas une mesure déjà réalisée.

## Améliorations proposées, par ordre d’intérêt

| Priorité | Proposition | Intérêt pour le portfolio | Vérification utile |
|---|---|---|---|
| 1 | Corriger les textes coupés dans Game Over et l’écran de transition | Rendre les instructions lisibles et montrer une finition cohérente | Vérifier les captures aux résolutions retenues et dans une petite fenêtre |
| 1 | Lisser les raccords des tuiles et retravailler les grands aplats rectangulaires du fond | Donner plus de lisibilité au niveau et une présentation visuelle plus aboutie | Comparer les mêmes cadrages avant/après |
| 1 | Enregistrer 20 à 30 secondes de jeu avec les commandes normales | Montrer la qualité réelle du mouvement et la continuité du parcours | Saut court, saut long, collecte et arrivée dans une même séquence |
| 1 | Donner au prototype un titre et une identité visuelle propres | Faciliter sa mémorisation par un recruteur | Titre cohérent dans le jeu, la publication et le dépôt |
| 2 | Ajouter du son, un retour à la collecte et une réaction à l’atterrissage | Renforcer le retour des actions du joueur | Observer la compréhension des actions avec quelques testeurs |
| 2 | Ajouter un checkpoint et une courte introduction jouable | Réduire la répétition après un échec et expliquer les commandes par le parcours | Faire essayer le niveau à une personne qui ne le connaît pas |
| 2 | Construire un vrai deuxième niveau avec une nouvelle contrainte | Démontrer la progression du level design | Vérifier que la nouvelle règle s’apprend puis se combine aux précédentes |
| 3 | Regrouper les éléments répétitifs et envisager Tilemap / atlas selon le résultat du profiling | Simplifier l’édition et traiter les coûts effectivement observés | Comparer temps de frame, batches et temps de modification du niveau |
| 3 | Étudier une migration vers une version Unity maintenue dans une branche dédiée | Montrer la capacité à faire évoluer un projet ancien | Revalider compilation, collisions, entrées, caméra et transitions |

Ces éléments sont des **propositions**, pas des fonctionnalités déjà livrées. Pour la prochaine itération, privilégier la lisibilité du décor, une courte vidéo et un retour audiovisuel plutôt qu’un grand nombre de nouvelles mécaniques.

## Crédits et sources

Les graphismes proviennent d’assets tiers intégrés au projet. Leur création ne doit pas être présentée comme un travail graphique personnel.

- **Kenney — Pixel Platformer** : tuiles, décors et autres éléments. La [page officielle du pack](https://kenney.nl/assets/pixel-platformer) indique CC0, comme le fichier `License.txt` fourni.
- **PROWNE — Grass/Dead Grass Platformer Tileset**, via OpenGameArt : source du personnage selon le fichier d’attribution du projet. La [page de l’auteur](https://opengameart.org/content/grassdead-grass-platformer-tileset) indique CC0 et mentionne des animations de personnage.
- **Unity — Camera.Render** : [documentation 2017.2](https://docs.unity3d.com/2017.2/Documentation/ScriptReference/Camera.Render.html), méthode utilisée pour les cinq premiers rendus. Ce sont des images caméra en mode édition ; l’interface `OnGUI` n’y est pas incluse.

Les fichiers de provenance et les scripts de capture conservés dans ce dossier permettent de distinguer le projet original, les rendus de présentation et les contrôles effectués.
