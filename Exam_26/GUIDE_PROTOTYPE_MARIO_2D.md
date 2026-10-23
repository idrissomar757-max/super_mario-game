# Prototype Super Mario 2D

1. Ouvre le projet `Exam_26` avec **Unity 2017.2.0f3**.
2. Attends la fin de l'import des assets et de la compilation des scripts.
3. Dans Unity, clique sur `Tools > Super Mario 2D > Create Demo Scene`.
4. Le script cree `Assets/Scenes/MarioDemo.unity` et `Assets/Scenes/NextLevel.unity`.
5. Ouvre `Assets/Scenes/MarioDemo.unity` si elle n'est pas deja active.
6. Lance le mode Play.

Controles:

- `A / D` ou `Fleche gauche / Fleche droite` pour avancer et reculer
- `Espace` pour sauter
- Relache `Espace` plus tot pour faire un petit saut

Ce qui est deja prepare:

- un decor 2D avec nuages, montagnes et collines en parallax
- un sol principal avec un trou, des plateformes et des blocs
- un `Player` avec `Rigidbody2D`, `BoxCollider2D` et script `PlayerController`
- des objets `Coin` avec `CircleCollider2D` en trigger
- des ennemis avec `Rigidbody2D`, `Collider2D` et script `EnemyController`
- un trigger de fin de niveau qui charge la scene suivante
- une camera qui suit le joueur avec un petit look-ahead
- un `Game Over` si le joueur touche un ennemi ou tombe
- une seconde scene simple `NextLevel` pour valider le passage au niveau suivant

Structure logique de la scene:

- `Background` : sprites visuels derriere le niveau, avec `SpriteRenderer` et `BoxCollider2D` desactive
- `Geometry` : sol, plateformes et blocs avec collisions
- `Coins` : pieces en trigger a recuperer
- `Enemies` : ennemis qui bougent automatiquement
- `Goal` : drapeau/trigger de fin du niveau
- `Player` : personnage controllable
- `Systems` : gestion du score et de l'etat de partie

Scripts principaux:

- `Assets/Scripts/Gameplay/PlayerController.cs`
- `Assets/Scripts/Gameplay/GameManager.cs`
- `Assets/Scripts/Gameplay/Coin.cs`
- `Assets/Scripts/Gameplay/EnemyController.cs`
- `Assets/Scripts/Gameplay/PlayerAnimation2D.cs`
- `Assets/Scripts/Camera/CameraFollow2D.cs`
- `Assets/Scripts/Environment/ParallaxLayer2D.cs`
- `Assets/Scripts/Environment/EndLevelTrigger.cs`
- `Assets/Scripts/Environment/NextLevelScreen.cs`
- `Assets/Editor/SuperMarioDemoSceneBuilder.cs`

Si tu veux aller plus loin ensuite, on peut ajouter:

- ennemis Goomba-like
- pieces avec score
- checkpoints
- animation d'impact sur les blocs
- menu de depart et ecran de victoire
