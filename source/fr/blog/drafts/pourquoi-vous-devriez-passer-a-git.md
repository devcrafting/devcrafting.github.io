Pourquoi vous devriez vraiment passer à Git
===========================================

- tags: git, outils

-----------------

J'ai mis relativement longtemps à passer à Git, j'ai démarré "seulement" début 2013. Auparavant, j'ai travaillé successivement avec Subversion et TFS Version Control. Et dernièrement, j'ai retrouvé TFS VC sur une mission, l'occasion de faire un retour d'expérience et d'avancer des arguments pour ceux qui ne seraient toujours pas passés à Git (ou un autre DVCS - Distributed Version Control System - ayant le même principe de fonctionnement).

------------------

## Rapide tour d'horizon

Subversion a été la norme à la suite de CVS, notamment pour beaucoup de projets Open Source (dont de nombreux sur SourceForge), puis l'arrivée de Git (et d'autres DVCS comme Mercurial) a sonné le glas de Subversion et aujourd'hui la majorité des projets Open Source est géréé avec Git (surtout sur GitHub).

![Logo de Git]()

Mais qu'en est-il de son utilisation par les entreprises? J'ai clairement remarqué que beaucoup d'entreprises (en France en tout cas) avaient mis beaucoup de temps à passer à un DVCS, voire n'y étaient toujours pas passées.

Certaines étaient passées à Subversion déjà tardivement. D'autres utilisaient encore des CVS payants plus ou moins anciens (PVCS, ClearCase...). Certaines étaient passées à TFS VC, qui faisait plus récent et plus "sérieux" que Subversion (avec Microsoft comme faire-valoir). A noter que concrètement, TFS VC était très proche de Subversion et franchement mieux intégré pour des devs sur plateforme .NET.

De nombreuses raisons (plus ou moins bonnes) à la réticence des entreprises, dont:

* "Il n'y a pas assez de personnes qui connaissent" => avec ce genre d'arguments, on taillerait encore des silex...et surtout, c'est tombé à l'eau avec de plus en plus de jeunes (formés dès leurs études? ou par eux-mêmes?)
* "L'Open Source ce n'est pas assez sérieux pour l'entreprise" => sans commentaires...
* "Subversion marche très bien (dans mon contexte)" => c'est possible dans certains cas, mais quand même ça ressemble souvent à la stratégie de l'autruche
* Mais surtout, la raison la plus probable mais la moins avouable est que beaucoup de personnes craignent le changement...ils ont "peur" de passer d'un outil qu'ils maîtrisent à un outil inconnu! Surtout que Git a toujours eu un peu cette réputation d'être ardu au début...

Vous vous sentez un peu bousculé par cette introduction (à noter que j'espère que vous n'avez pas pris personnellement mes sarcasmes...), je vous propose de reprendre quelques points cruciaux de la gestion de configuration. Dernièrement, j'ai eu des expériences avec des CVS classiques, voire un pré-historique ;), cela pourra peut-être apporter plus d'éléments.

## Expérience récente avec ClearCase

J'ai participé à une migration de ClearCase vers Git, et franchement, il faut être masochiste pour utiliser ClearCase car c'est extrêmement lent et très complexe. Le plus étonnant dans cette expérience a tout de même été que, malgré que la plupart des personnes partageait une certaine aversion à ClearCase, ils étaient peu enclin à passer à Git...une manifestation de la "peur" du changement probablement?

![Logo de ClearCase]()

D'un point de vue plus technique, et pour étayer un peu mon jugement à l'emporte-pièce ci-dessus, ClearCase suit les fichiers unitairement, avec un historique de version propre à chacun, sans atomicité des changements de plusieurs fichiers (comme les changesets dans TFS VC ou les commits Git). De plus, le système est basé sur un modèle à base de "Locks", ne permettant qu'une seule personne à modifier un fichier...à noter que certains y voyaient un intérêt particulier ("si quelqu'un bosse sur un fichier alors il ne faut pas que je travaille dessus aussi"), un argument un peu désuet à mon sens avec l'utilisation de plus en plus de fichiers (on s'éloigne heureusement des applications avec fichiers sources de milliers de lignes).

Les impacts de cela sont qu'ajouter un fichier est une opération unitaire, que pour modifier un fichier il faut d'abord l'extraire, que l'archivage (commit/checkin) se fait fichier par fichier, toutes ces opérations sont fastidieuses et ralentissent notablement le développement...

De plus, la gestion des branches (streams) est connue comme "avancée" certes, mais complexe, dans ClearCase...et complexe n'est pas un vain mot, d'ailleurs très peu de personnes géraient cela dans l'entreprise où j'effectuais la migration...sans compter la lenteur effarante de toutes les opérations sur les branches. 

Le passage à Git était l'occasion de mettre la puissance des branches dans les mains des développeurs, avec certes une peur de l'inconnue, mais globablement après un peu de pratique et une dose de bonne volonté, j'ai l'impression que c'est très bien passé (avec même certains au point sur le Feature Branching).

# Expérience récente avec TFS VC

![Logo de TFS VC]()
Consistance après check-in

## Arguments "pro-Git"

Atomicité
Offline
Git dans TFS
