Activités de l'été : auto-formation et LyonTechHub
==================================================

- uniquekey: activite-ete-autoformation-LyonTechHub
- date: 2014-09-10
- tags: Entrepreunariat, Cloud, Communautés, Outils de dev, JavaScript

---------------------------

J'avais prévu cette été une période assez large de "congés"...l'objectif était de pouvoir profiter de mes enfants, mais également d'en profiter pour avancer quelques sujets, dont me frotter de plus près à certaines technos...

Premier objectif que je m'étais fixé, aller plus loin dans le dev JavaScript. J'ai eu l'occasion de manipuler certains outils via les communautés, sur certains projets, mais je voulais tenter de les mettre en place pour m'y confronter un peu plus. Au menu initialement, j'avais prévu: AngularJS, Gulp, RequireJS, LESS, TypeScript...finalement, je ne me suis pas lancé complètement dans TypeScript, mais je me suis frotté en plus aux problématiques de SEO avec les SPA (Single Page Application), aux déploiements d'application Node.js sur Heroku, un peu de Ruby...bref, ça a été riche en apprentissages ! Ce billet reprend quelques éléments retenus de ces expérimentations.

------------------------------

LyonTechHub 
-----------

Au passage, je me suis dit que cela pourrait être mis à profit d'un petit projet communautaire...et je pensais déjà depuis un moment à faire un site pour la communauté des communautés lyonnaises : LyonTechHub, pour agrandir sa notoriété, qui à mon avis ne peut que profiter à l'ensemble de l'écosystème IT de la région : plus d'échanges entre communautés, plus de visibilité pour celles-ci...

Pour remettre un peu de contexte, LyonTechHub était jusque-là [un calendrier Google public](https://www.google.com/calendar/embed?src=ck2ruq6cqfch3t4gshbd6vdnd4%40group.calendar.google.com) référençant un grand nombre d'événements des communautés lyonnaises. Il y avait également la liste de diffusion Google Groups : [lyontechhub@googlegroups.com](mailto:lyontechhub@googlegroups.com) qui est utilisée pour annoncer quelques événements, mais il y a peu de membres pour le moment. UN GRAND MERCI à tout ceux qui ont créé et fais grandir ce groupe, je trouve cela GENIAL ! Je pense notamment à [Agnès](https://twitter.com/agnes_crepet), [Cédric](https://twitter.com/cedric_exbrayat), [Romain](https://twitter.com/romaincouturier), [Franck](https://twitter.com/franckverrot) et tous les autres leaders des communautés lyonnaises qui se sont lancés (pas d'offenses, je ne peux pas mentionner tout le monde, du coup, j'ai mentionné les managers de la liste, avec qui j'ai d'ailleurs été en lien rapidement dès mon arrivée à Lyon :P...).

Constat : on utilisait beaucoup le calendrier pour relayer les autres événements, notamment via le [MUGLyon](http://muglyon.github.io/), cependant on avait toujours l'impression que pointer vers un calendrier, ça ne parlait pas à tout le monde...et faut dire qu'un calendrier Google public quand on l'ouvre via une recherche Google, on se demande si c'est vraiment Google qui a fait ça :).

Objectif : créer un site vitrine plus facile d'accès que le calendrier Google, reprenant les informations du Calendrier et référençant les communautés faisant partie de LyonTechHub (nb: c'est très facile d'en faire partie...vous pouvez déjà aller [voir le code](https://github.com/lyontechhub/lyontechhub.github.io), le site sera officiellement lancé sous peu j'espère). Autre objectif : ouvrir un maximum la liste et en faire un point de contact pour les personnes cherchant des renseignements.

Passons à la technique (nb: je reviendrais plus en détails sur certains points). Au passage, mea culpa pour la "complexité" induite par la solution que j'ai proposé (overkill?!)...il y aurait peut-être eu des solutions beaucoup plus simples pour ce site vitrine. Il a au moins le mérite d'exister maintenant :) !

AngularJS
---------

Je connaissais le développement de SPA avec [Knockout](http://knockoutjs.com/), j'avais notamment co-animé [l'atelier Crafting Workshop](http://knockoutjs.com/) au Mix-IT 2014 avec [Florent](https://twitter.com/florentpellet) et [Emilien](https://twitter.com/ouarzy), pour lequel la stack utilisée comprenait Knockout. La notoriété de AngularJS ne faisant plus de doute, je me disais qu'il fallait que j'y jetes un oeil, notamment pour voir les différences avec ce que je connaissais.

J'ai commencé par suivre le [cours Shaping up with Angular.js](http://campus.codeschool.com/courses/shaping-up-with-angular-js/intro) de Code School. Vraiment bien fait, facile à ingurgiter même sur plusieurs petites périodes de temps (les siestes des enfants par exemple :P).

Je me suis ensuite mis à faire une première ébauche du site LyonTechHub...en mode à l'arrache clairement...en utilisant NPM, pour récupérer Bower, pour récupérer les paquets (Bootstrap, Angular...). J'ai posé le menu (plugin AngularStrap), mis en place le routing, puis j'ai entammé le contenu des pages, de l'accueil très simple (texte uniquement) aux communautés (données JSON avec $http, un peu plus de directives...)

J'ai créé un controlleur par page nécessitant de la logique, et pour ces controlleurs, j'ai utilisé la nouvelle syntaxe "Controller as" au lieu du $scope. Je n'étais pas fan de ce fourre-tout pour passer les données du controlleur à la vue. D'ailleurs, la nouvelle syntaxe m'a plus fait penser à du MVVM (un controlleur en MVC n'ayant pas d'état), ce pour quoi j'ai suffixé les "controlleurs" ViewModel dans le code. J'ai d'ailleurs twitté cette remarque, qui a été pas mal retwitté et mis en favoris (à mon échelle, jamais eu autant de RT et de favoris :P). J'ai ensuite sourit en voyant ce qui été affiché sur le site AngularJS : c'est du MVW en fait...le W signifiant "Whatever suits you", i.e ViewModel ou Controller...

Par ailleurs, j'ai créé un service pour isoler l'accès aux données Google Calendar dans un composant eventsRepository. J'ai aussi tenté quelques directives personnalisées, regardé pas mal de code de directives mises à disposition ici et là.

Premier ressenti après cette première utilisation : ça a l'air assez puissant et c'est plutôt agréable à utiliser. Comparativement à Knockout, ce n'est pas le même genre d'outil. Knockout n'est qu'une librairie de binding, il faut assembler plusieurs librairies pour faire une SPA (Sammy.js pour le routing...). AngularJS est plus un framework plus complet (de plus en plus modularisé, mais homogène). Je citerais une discussion que j'ai eu avec [Rui](https://twitter.com/rhwy) : "AngularJS est une surcouche au navigateur, le framework de base du Web est limité, AngularJS étend cela avec un framework plus puissant". Sur la partie binding, j'ai bien aimé l'aspect non intrusif de AngularJS (pas de ko.observable dans le ViewModel), bien qu'on puisse faire la même chose avec Knockout en ajoutant une extension. Au global, je n'ai pas forcément de préférences, les 2 outils me plaisent bien et il faut voir au cas par cas ce qui est le plus adapté.

Il y a une grande partie que je n'ai pas exploré, ce sont les tests, aïe ! Je ne recommencerais pas, c'est promis :P...

Une fois que j'avais mes premières pages, je me suis attaqué à la partie plus générique à un environnement de dev Web/Javascript.

Gulp, RequireJS, LESS
---------------------

C'est bien joli de développer à l'arrache, mais ça me pesait un peu quand même...du coup, je me suis dit que j'allais outiller un peu tout ça.

Premièrement, j'ai mis en place Gulp pour compiler une feuille de style en LESS. A propos de LESS, c'est quand même largement plus agréable que du CSS tout court, l'imbrication, les variables sont bien pratiques. Concernant Gulp, je le préfère a priori à Grunt, pour son aspect plus procédural que déclaratif, on code des tâches avec des librairies pour faire à peu près n'importe quoi (minification, compilation LESS, optimisation AMD...) plutôt que de paramétrer des tâches standards avec du JSON comme avec Grunt. J'ai rajouté la minification CSS/JS, l'optimisation AMD (cf. RequireJS ci-dessous), la copie des fichiers dans un répertoire de build...j'ai aussi mis en place des watch, qui surveillent les modifications apportées aux fichiers sources et relancent les tâches associées, pour éviter de relancer la compilation Gulp et le serveur Web à chaque changement. Ainsi, avec un terminal lançant "gulp watch" qui tourne en continue, l'expérience de développement est très agréable (le serveur Web est démarré une seule fois au début).

Ensuite, j'ai mis en place RequireJS pour éviter le référencement de toutes les librairies JS manuellement et gérer les dépendances plus proprement. Je connaissais l'outil, j'ai pu aller un peu plus loin en le mettant en place. A utiliser sans conteste à mon avis.

Bon ok, toujours pas de tests, on pourrait dire que c'est encore à l'arrache ! :o) Mais c'est déjà mieux.

Heroku (Ruby, Node.js), prerender.io & déploiement continu
----------------------------------------------------------

Au début, tout était beau, j'utilisais GitHub pages...et soudain, [Florent](https://twitter.com/florentpellet) m'a rappelé une petite contrainte : ce serait mieux que le site puisse être référencé ! En effet, c'est mieux pour un site vitrine !!! Merci Florent...

Au passage, GitHub pages c'est bien pratique pour faire des sites statiques facilement modifiables par plusieurs personnes, on peut faire un site entier, ou même un blog, avec du Markdown...le tout est géré par Jekyll, le serveur Web light utilisé par GitHub pages (et c'est référencé puisque statique !) A chaque push, le site est mis à jour. On aurait pu ainsi envisager de faire plus simple pour le LyonTechHub, mais ayant avancé sur le site en AngularJS et le sujet du SEO avec une SPA me titillant, j'ai continué sur ma lancée...

Mais revenons à notre problème de référencement, les SPA faisant un chargement dynamique des pages (i.e sans recharger une nouvelle page à chaque clic), il fallait trouver quelque chose pour rendre le site SEO compatible (i.e crawlable par des moteurs de recherche cherchant à le référencer). Il y a 2 modes possibles avec AngularJS : le mode hashbang (#!, compatible sur d'anciens navigateurs) et le mode HTML5. Les 2 modes peuvent être référencés par les moteurs de recherche suivant la [spécification Ajax Crawling de Google](https://developers.google.com/webmasters/ajax-crawling/docs/specification).

Pour répondre à cette spécification, le serveur doit pouvoir servir des URLs modifiées par le moteur de recherche, ainsi pour une url http://monsite/#!/mapage, il faut lui envoyer la page en HTML pur lorsqu'il appelle la page http://monsite/?_escaped_fragment_=/mapage. Sur GitHub pages, je n'ai pas trouvé de solutions, voilà pourquoi je me suis tourné vers Heroku pour héberger une application Node.js.

En effet, en Node.js, [prerender.io](https://prerender.io/) m'a donné une solution simple et rapide à mettre en place (disponible pour plein d'autres plateformes...). Le principe est d'héberger une partie serveur encapsulant l'appel d'une SPA avec [Phantom.js](http://phantomjs.org/), un navigateur headless (pas d'interface) gérant le Javascript, qui permet ainsi de générer le HTML statique d'une page d'une SPA. Côté SPA, il suffit d'encapsuler l'application dans un serveur Web Node.js Express avec le plugin traitant les requêtes spécifiques des moteurs de recherche. Ce dernier interroge la partie serveur encapsulant Phantom.js lorsque l'URL l'impose. Et voilà, le site est SEO compliant !

La partie server de prerender.io peut être utilisée en mode cloud sur leur site (limitations ou coûts mensuels) ou déployé soi-même. C'est cette seconde option que j'ai choisie, et l'explication proposant un déploiement sur heroku, je me suis orienté vers [Heroku](http://www.heroku.com/). Très simple à mettre en oeuvre, j'ai donc créé une application pour la partie serveur de prerender.io. Etant sur heroku, j'ai alors créé une seconde application pour héberger le site LyonTechHub en SPA avec le serveur Web Node.js Express.

Dernier point un peu bloquant, il fallait alors déployer manuellement les modifications sur [Heroku](http://www.heroku.com/). J'ai donc cherché à faire un déploiement continu de GitHub vers heroku. J'ai trouvé la petite application Ruby [heroku-deployer](https://github.com/himynameisjonas/heroku-deployer), déployée toujours sans encombre sur une autre application heroku. A noter que développant sur la branche dev sur GitHub, j'ai du modifier le code pour gérer le déploiement d'une autre branche que master (cf. [fork ici](https://github.com/lyontechhub/heroku-deployer)), c'est là que je me suis frotté un peu au Ruby :o)...

Epilogue (ou pas)
-----------------

Bien content d'avoir pu me frotter à tout ça durant l'été, ainsi que de l'engouement des communautés lyonnaises pour ce site LyonTechHub...en espérant que LyonTechHub va prendre de l'ampleur ! Vive les communautés de pratiques ! A suivre donc...

Je mettrais dans des billets séparés quelques détails de ces différentes expérimentations (avec du code ! :P).