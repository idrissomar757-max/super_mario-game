# Vidéo de démonstration et mesures à compléter

Ce document propose une prochaine étape. Aucune vidéo ni mesure de performance n’a été produite pendant cette préparation.

## Une vidéo d’environ 30 secondes

| Durée indicative | Séquence | Compétence illustrée |
|---|---|---|
| 0–3 s | Titre discret et personnage au départ, HUD visible | Présenter le prototype et le contexte |
| 3–11 s | Déplacement, saut court puis saut plus long, en utilisant les commandes normales | Mouvement et hauteur de saut variable |
| 11–17 s | Collecte de pièces avec le score visible | Interaction et retour d’interface |
| 17–23 s | Passage près d’un ennemi et franchissement d’un obstacle | Lecture du parcours et collisions |
| 23–27 s | Arrivée puis écran de transition, si le parcours permet ce montage | Boucle et gestion des scènes |
| 27–30 s | Vue du niveau dans Unity et courte mention « Unity / C# — prototype » | Montrer l’environnement de développement |

Si la vidéo utilise plusieurs prises, la présenter comme un montage de démonstration. Garder les gestes lisibles et éviter d’accélérer les séquences utilisées pour illustrer la précision du mouvement. Le coyote time et le jump buffer gagnent à être expliqués par un plan spécifique ou une annotation ; ils sont difficiles à apprécier dans une seule course rapide.

## Tableau de benchmark

Remplir ce tableau après une mesure sur une version jouable, en précisant les conditions. Les cellules vides ne sont pas des résultats.

| Condition ou résultat | Valeur à renseigner |
|---|---|
| Version Unity / version du projet | |
| Plateforme et type de build | |
| CPU / GPU / RAM | |
| Résolution / qualité / VSync / limite de FPS | |
| Scénario et durée, après chargement | |
| Nombre de répétitions | |
| Temps de frame médian et P95 | |
| Temps CPU et GPU, selon les outils disponibles | |
| Mémoire et allocations observées | |
| Batches / draw calls et contexte de la mesure | |
| Profiler activé ou désactivé | |

Le budget de 16,7 ms par image correspond à un objectif de 60 FPS, pas à une performance constatée. Le pas de physique de 0,02 s configuré dans le projet ne prouve pas un débit de 50 FPS.

## Quelques questions pour une séance de test

- Les commandes sont-elles compréhensibles sans explication orale ?
- Les plateformes et les ennemis se distinguent-ils rapidement du décor ?
- Le joueur comprend-il la cause d’une défaite et sait-il comment recommencer ?
- La caméra montre-t-elle suffisamment tôt la zone du prochain saut ?
- Le temps passé à rejouer le début après une chute reste-t-il acceptable ?

Noter les hésitations et les échecs observés, puis choisir une modification à la fois. Ces observations permettent d’expliquer une véritable itération dans une publication suivante.
