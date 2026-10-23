# Dossier LinkedIn — prototype de plateforme 2D

Ouvrir `index.html` pour parcourir la présentation, lire les textes et consulter les visuels. Tout fonctionne localement, sans compte ni connexion.

## Livrables

- `02_Publication_LinkedIn.txt` : publication en français prête à copier et à personnaliser selon votre rôle exact.
- `03_Publication_EN.txt` : adaptation anglaise pour des échanges internationaux.
- `04_Carrousel_LinkedIn.pdf` : présentation de six pages, avec versions PNG dans `carrousel/` (1080 × 1350).
- `visuels/` : cinq rendus caméra Unity, quatre captures du jeu en Play Mode et une planche d’animations extraite des assets.
- `01_Analyse_et_recommandations.md` : caractéristiques, limites, réglages, priorités d’amélioration et protocole de mesure.
- `05_Guide_visuels_et_publication.md` : provenance, légendes, textes alternatifs et ordre conseillé.
- `06_Plan_video_et_mesures.md` : séquence vidéo proposée et tableau de mesures à remplir.
- `preuves/` et `sources/` : inventaire de scène, vérifications et extraits de code.
- `outils/` : sources des mises en page et outils de capture préparés.

## Résultat de la vérification

Les scripts du projet ont compilé et les deux scènes ont été générées dans une copie isolée avec Unity 2017.2.0f3. Les cinq images caméra proviennent de ce moteur. Les scènes originales étaient absentes avant cette génération.

L’instance Unity déjà ouverte a terminé le scénario automatique après un problème initial de démarrage ; la relance demandée n’a pas été autorisée et n’a pas eu lieu. La collecte, la défaite par contact, la chute et la transition ont donné les valeurs attendues dans ce scénario avec repositionnement du personnage. Le journal signale aussi des exceptions dans l’outil de capture lors des chargements ; l’analyse précise la portée de la vérification.

**Aucune capture de l’éditeur ou mesure de FPS n’est fournie.** Les quatre captures Play Mode sont réelles. Les dessins de hiérarchie et les extraits de code du carrousel sont des mises en page explicatives, pas des captures d’écran de Unity ou de Visual Studio. Certains textes d’interface sont coupés : les captures d’origine conservent ce défaut, décrit dans l’analyse.

Le projet original reste dans `../Exam_26`. La copie de génération est dans `../.capture_unity`. La préparation de ce dossier n’a pas modifié les scripts du jeu original. Aucune publication LinkedIn n’a été effectuée.

Les éléments créés pour la communication ne constituent pas de nouvelles fonctionnalités du jeu.
