Nouveau blog et nouveau site web
================================

- uniquekey: new-blog-and-website
- date: 2016-10-25
- tags: fsharp,devcrafting,Communautés

--------------

J'ai eu 2 blogs, le premier en anglais sur Blogspot et ensuite quand j'ai créé ma société DevCrafting, j'ai créé un autre site avec un blog en français avec Office365 (SharePoint). 

Je n'ai jamais réussi à écrire de nouveaux billets, pour plusieurs raisons, plus ou moins bonnes, mais je voudrais à nouveau essayer avec une autre approche. Je voulais écrire ce billet pour donner un retour d'expérience sur mes 2 premières tentatives. 

A noter que je voulais également pouvoir écrire parfois en anglais, parfois en français en fonction du sujet.

Enfin, j'aime l'idée de [FsBlog](https://github.com/fsprojects/FsBlog), mais comme tout ce que je voulais faire n'était pas supporté, j'ai décidé de jouer avec F# pour recréer mon site web et un blog à l'image de ce qu'a fait [Tomas Petricek](http://tomasp.net/).

--------------

## Première (mauvaise) raison: j'ai du mal avec l'édition en ligne et les éditeurs WYSIWYG

L'édition en ligne n'est assez naturelle à mon avis, je ne terminais jamais un billet de blog en une fois, et le système de brouillons n'était pas assez simple. Il me semble beaucoup plus simple d'ouvrir un fichier texte local que d'ouvrir un navigateur web, se connecter au site, et rechercher le brouillon à éditer...mais ce n'est peut-être qu'un doux rêve, l'expérience me le dira.

Aussi, lors de mes deux expériences (avec Blogspot et Office365/SharePoint), j'utilisais un éditeur WYSIWYG, ce qui a été un cauchemar dans certains cas. Je me rappelle notamment d'avoir passé beaucoup trop de temps à écrire un billet contenant du code, et cela sur les deux plateformes de blog.

En apparté, j'ajoute que Office 365 SharePoint est tout simplement un cauchemar...au moins pour les développeurs (qui peuvent utiliser d'autres outils).

## Seconde (mauvaise) raison: je n'arrivais pas à écrire quelquechose d'intéressant

Je sais que je suis un peu trop perfectionniste, je n'aime pas l'idée d'avoir couvert partiellement un sujet. Pour mes billets de blog, j'étais trop restrictif, et j'ai eu à chaque fois des brouillons en attente d'améliorations.

J'aimerais parler de pleins de sujets, mais parfois en lisant de "meilleurs" billets sur le web que ce que je pourrais faire, je perdais la foi d'exprimer ma propre pensée...parfois je commençais à écrire et le sujet était tellement vaste que je perdais également la foi de continuer...

En fait, je me dis que je devrais écrire des billets plus courts et moins complets, ET que je devrais éviter de me demander si mes billets intéresseront les autres...j'aime particulièrement l'explication de [@ouarzy](https://twitter.com/ouarzy): "il faut être plus égoïste" quand on écrit, parce qu'on peut avoir d'autres objectifs: améliorer son expression écrite, partager ses réflexions, contribuer à la communauté et obtenir des retours d'autres personnes.

## LA raison principale (plus réaliste): je n'étais pas organisé pour écrire régulièrement

Ecrire des billets de blog prend beaucoup de temps, et avoir un rythme constant est encore plus difficile. Dans mes expériences précédentes, écrire n'était pas une priorité (quand j'avais le temps), je n'était donc pas constant et j'ai perdy la foi de continuer au final...

En parlant avec [@ouarzy](https://twitter.com/ouarzy), j'ai compris comment il essayait de réserver du temps chaque semaine pour écrire, sans y déroger pour garder le rythme. Son expérience montre que ça marche plutôt bien: il réussit à écrire régulièrement et reçoit de nombreux retour de la communauté.

## Un nouvel essai

Voilà, maintenant que je me suis justifié ;), voici mon "plan", ou plutôt mon souhait: essayer à nouveau en prenant en compte toutes ces expériences:

* Edition hors ligne basée sur [GitHub Pages](https://pages.github.com), la syntaxe Markdown and [F# Formatting](https://tpetricek.github.io/FSharp.Formatting/)
* Des sujets plus simples et plus restreints, avec moins d'auto-censure ;)
* Du temps alloué chaque semaine pour écrire des billets de blog, disons 4 fois 1h par semaine, le matin ou le midi ou le soir en fonction de mes contraintes
* Certains sujets en anglais, d'autres en français (voire les deux?!), en fonction de la communauté avec laquelle je souhaite partager le sujet

J'espère garder le rythme pour écrire régulièrement, disons un à deux billets par mois dans chaque langue serait un bon début!

## Effets de bord: je me suis amusé avec F#

J'écrirais probablement plus à ce sujet. J'ai construit mon nouveau site web/blog avec F#, en utilisant la syntaxe Markdown ou le F# comme base, mouliné avec [F# Formatting library](https://tpetricek.github.io/FSharp.Formatting/) pour obtenir un site statique publié sur [GitHub Pages](https://pages.github.com).

A noter que j'avais jeté un oeil à [FsBlog](https://github.com/fsprojects/FsBlog), mais j'ai trouvé trop difficile d'y ajouter le support du multilingue dans le code existant, en raison de pas mal de couplage et très peu de tests unitaires pour éviter les régressions.

Vous pouvez voir le [fichier reame.md](https://github.com/devcrafting/devcrafting.github.io/blob/dev/readme.md) pour avoir une idée des fonctionnalités que j'ai implémenté. La plupart sont configurables, le code F# est dans le [répertoire tools](https://github.com/devcrafting/devcrafting.github.io/tree/dev/tools) et le [fichier build.fsx](https://github.com/devcrafting/devcrafting.github.io/blob/dev/build.fsx). Evidémment, je peux largement améliorer le code, et notamment y ajouter des tests unitaires pour m'éviter des désagréments futurs (du type de ceux que j'ai eu avec FsBlog...).

A noter que j'aurais pu le faire en TDD, mais j'ai trouvé assez difficile de m'améliorer à la fois dans un langage (F#) et sur une pratique à utiliser avec ce language (j'essaierai d'écrire sur ce sujet). J'ai par contre expérimenté le fait qu'avoir un REPL (exécution immédiate du code dans F# interactive) permet une boucle de feedback rapide. Je pourrais dire que j'ai essayé le Type-DD (i.e écrire du code fortement typés en premier, au lieu des tests), mais ce serait prétentieux ;).

J'espère que vous aurez plaisir à me lire, [en français](http://www.devcrafting.com/fr/blog/) et/ou [en anglais](http://www.devcrafting.com/en/blog/).