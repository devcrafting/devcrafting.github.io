Pourquoi vous devriez vraiment passer à Git
===========================================

- tags: git, outils

-----------------

J'ai mis relativement longtemps à passer à Git, j'ai démarré "seulement" début 2013. Auparavant, j'ai travaillé successivement avec Subversion et TFS Version Control. Et dernièrement, j'ai retrouvé TFS VC sur une mission. J'ai aussi croisé ClearCase. C'est l'occasion de faire un retour d'expérience et d'avancer des arguments pour ceux qui ne seraient toujours pas passés à Git (ou un autre DVCS - Distributed Version Control System - ayant le même principe de fonctionnement).

------------------

## Rapide tour d'horizon

<img alt="Logo de Git" class="img-float-left img-bg-white" src="/images/git.png"/>

[Subversion](https://subversion.apache.org/) a été la norme à la suite de CVS, notamment pour beaucoup de projets Open Source (dont de nombreux sur SourceForge), puis l'arrivée de [Git](https://git-scm.com/) (et d'autres DVCS comme Mercurial) a sonné le glas de Subversion et aujourd'hui la majorité des projets Open Source est géréé avec Git (surtout sur GitHub).

Mais qu'en est-il de son utilisation par les entreprises? J'ai clairement remarqué que beaucoup d'entreprises (en France en tout cas) avaient mis beaucoup de temps à passer à un DVCS, ou n'y étaient toujours pas passées.

Certaines étaient passées à Subversion déjà tardivement. D'autres utilisaient encore des CVS payants plus ou moins anciens (PVCS, ClearCase...). Certaines étaient passées à [TFS VC](https://www.visualstudio.com/en-us/docs/tfvc/overview), qui faisait plus récent et plus "sérieux" que Subversion (avec Microsoft comme faire-valoir). A noter que concrètement, TFS VC était très proche de Subversion et franchement mieux intégré (pour des devs sur plateforme .NET au moins).

De nombreuses raisons (plus ou moins bonnes) à cette réticence des entreprises, dont:

* "Il n'y a pas assez de personnes qui connaissent" => avec ce genre d'arguments, on taillerait encore des silex...et surtout, c'est tombé à l'eau avec de plus en plus de jeunes connaissant Git (formés dès leurs études? ou par eux-mêmes?)
* "L'Open Source ce n'est pas assez sérieux pour l'entreprise" => sans commentaires...
* "Subversion marche très bien (dans mon contexte)" => c'est possible dans certains cas, mais quand même cela ressemble souvent à la stratégie de l'autruche
* "La migration va coûter cher" => râté, il y a plusieurs outils comme [Git-SVN](https://git-scm.com/book/fr/v1/Git-et-les-autres-syst%C3%A8mes-Git-et-Subversion) ou [Git-TFS](https://github.com/git-tfs/git-tfs), qui permettent de travailler avec Git en local et Subversion ou TFS VC sur un serveur central, mais permettent aussi de réaliser une migration vers Git assez simplement...
* "Git est un marteau pour écraser une mouche, je n'ai pas de problématiques complexes de développement distribué..." => c'est un argument de façade, mais en fait, les fonctionnalités des DCVS ne servent pas que dans un contexte de développement distribué...
* Mais surtout, la raison la plus probable, mais la moins avouable, est que beaucoup de personnes craignent le changement...ils ont "peur" de passer d'un outil qu'ils maîtrisent (et encore) à un outil inconnu! Surtout que Git a toujours eu un peu cette réputation d'être ardu au début...

Vous vous sentez un peu bousculé par cette introduction (à noter que j'espère que vous n'avez pas pris personnellement mes sarcasmes...), je vous propose de reprendre quelques points cruciaux de la gestion de configuration. Dernièrement, j'ai eu des expériences avec des CVS classiques, voire un pré-historique ;), cela pourra peut-être apporter plus d'éléments.

## Expérience récente avec ClearCase

<img alt="Logo de ClearCase" class="img-float-left" src="/images/IBM-ClearCase.png"/>

J'ai participé à une migration de ClearCase vers Git, et franchement, il faut être masochiste pour utiliser ClearCase car c'est extrêmement lent et très complexe. Le plus étonnant dans cette expérience a tout de même été que, malgré que la plupart des personnes partageait une certaine aversion à ClearCase, ils étaient peu enclin à passer à Git...une manifestation de la "peur" du changement probablement?

D'un point de vue plus technique, et pour étayer un peu mon jugement à l'emporte-pièce ci-dessus, ClearCase suit les fichiers unitairement, avec un historique de version propre à chacun, sans atomicité des changements de plusieurs fichiers (comme les changesets dans TFS VC ou les commits Git). De plus, le système est basé sur un modèle à base de "Locks", ne permettant qu'à une seule personne de modifier un fichier...à noter que certains y voyaient un intérêt particulier ("si quelqu'un bosse sur un fichier alors il ne faut pas que je travaille dessus aussi"), un argument un peu désuet à mon sens avec l'utilisation de plus en plus de fichiers (on s'éloigne heureusement des applications avec des fichiers sources de milliers de lignes).

Les impacts de cela sont qu'ajouter un fichier est une opération unitaire, que pour modifier un fichier il faut d'abord l'extraire, que l'archivage (commit/checkin) se fait fichier par fichier, toutes ces opérations sont fastidieuses et ralentissent notablement le développement...

De plus, la gestion des branches (streams) est connue comme "avancée" certes, mais complexe, dans ClearCase...et complexe n'est pas un vain mot, d'ailleurs très peu de personnes géraient cela dans l'entreprise où j'effectuais la migration...sans compter la lenteur effarante de toutes les opérations sur les branches. 

Le passage à Git était l'occasion de mettre la puissance des branches dans les mains des développeurs, avec certes une peur de l'inconnue, mais globablement après un peu de pratique et une dose de bonne volonté, j'ai l'impression que c'est très bien passé (avec même certains au point sur le Feature Branching). A noter par contre que la migration de ClearCase vers Git n'est pas une mince affaire si on veut migrer son historique. 

## Expérience récente avec TFS VC

<img alt="Logo de TFS VC" class="img-float-left" src="/images/TFS-logo.png"/>

Autre expérience avec un CVS plus "récent" et plus "sérieux" : TFS VC de Microsoft. L'ayant utilisé pendant un certain temps, je n'étais pas particulièrement pro "Microsoft/TFS bashing", surtout qu'objectivement je ne trouvais pas cet outil si catastrophique, sans être extraordinaire non plus évidemment. Cependant, mon retour récent à la gestion de configuration avec TFS VC est l'occasion de revenir sur cette brique TFS un peu viellissante à mon goût.

Comme je disais en introduction, TFS VC est très proche de Subversion. En arrivant en 2005, c'était franchement (enfin) un bon rafraichissement côté Microsoft, pour ceux qui ont connu Visual Source Safe...Ces CVS ont apportés notamment l'atomicité des modifications d'un ensemble de fichiers : les changesets/révisions. En apparence, les commits Git n'apportent rien de plus. Mais en fait, en regardant Git dans son ensemble, les commits apportent notamment un niveau d'atomicité/cohérence supplémentaire sur l'ensemble du dépôt. 

Concrêtement avec TFS VC, quand vous faites un check-in, s'il n'y a pas de conflits, un nouveau changeset est créé sur le serveur sans que vous ayez à mettre à jour votre version locale. Résultat : vous avez potentiellement mis à disposition des autres un code qui ne fonctionne pas avec les changesets que vous n'avez pas récupérés, d'où un potentiel problème "d'incohérence" de votre dépôt (est-ce que l'ensemble compile même encore ?!)...certes cela n'existe pas avec Subversion qui force à mettre à jour sa version locale avant de faire un check-in, mais cela m'amène à un autre point.

En effet, j'ai vraiment eu du mal à faire de petits pas avec TFS VC (idem avec Subversion) et j'ai eu quelques sueurs froides quand je devais récupérer la dernière version alors que j'avais des modifications en cours...et là je me suis dit vive les commits locaux : 1) ils me permettent d'avancer par petits pas sans avoir forcément à les mettre à disposition de tous avant d'avoir un ensemble satisfaisant de commits et 2) la mise à jour de ma version locale est sécurisée par le fait qu'il faut avoir un espace de travail "propre" sans modifications en cours, donc sans risque de perdre ses modifications locales lors d'une mise à jour à partir du serveur.

Pour finir avec TFS, un serveur Git est intégré depuis TFS 2013, donc vous pouvez oublier TFS VC. Pensez-y.

## Pour finir : autres petits plus "pro-Git"

Je n'ai évidemment pas parlé de tous les aspects de Git, ce serait long pour un billet, l'objectif était de faire un rapide retour sur quelques aspects m'ayant marqués en réutilisant des CVS après mon adoption de Git. Pour conclure, j'ajoute tout de même quelques autres petits plus de Git.

<img alt="Branches Git" class="img-float-right" src="/images/GitBranching.jpg"/>

Premièrement les branches sont des "first-class citizen", c'est-à-dire que ce ne sont plus des sous-répertoires du dépôt, organisés de manière plus ou moins rigoureuse...le concept de branche est central dans Git, vous commencez sur une branche (master typiquement), à partir de laquelle vous allez pouvoir créer d'autres branches pour isoler vos développements, mais votre espace de travail local ne reflète qu'une seule branche à la fois, il n'y a plus de mapping d'un sous-ensemble du dépôt comme sur Subversion ou TFS VC. Vous devez basculer d'une branche à l'autre pour changer votre espace de travail. Cela peut paraîtrei contraignant, mais au final, je trouve cela plutôt sécurisant.

De plus, la manipulation des branches est nettement plus aisée qu'avec TFS VC ou Subversion. Qui peut me dire avoir utilisé intensivement les branches sur ces outils sans aucun problème ? Je connais plus de gens qui me disent les avoir évitées...et pourtant les pratiques associées aux branches comme le Feature Branching apporte une grande souplesse, c'est une partie de ce qu'on appelle les [workflows Git](https://www.atlassian.com/git/tutorials/comparing-workflows/). On peut se dire que "ça ne nous servirait à rien", mais après avoir essayé, on en comprend mieux l'utilité.

Enfin, un autre argument : c'est le travail offline et la rapidité de fonctionnement que cela apporte. En fait cela est permis par ["les tripes de Git"](https://git-scm.com/book/fr/v1/Les-tripes-de-Git), qui de part son aspect distribué, permet/nécessite que chacun ait en local l'ensemble de l'historique du dépôt (central pour simplifier). A partir de là, toutes les opérations de changement de branche, de commits, de visualisation de l'historique sont locales et très rapides, et c'est un régal ! A noter que l'aspect distribué permet aussi d'envisager des scénarios autres que le serveur centralisé avec des clients, toute copie du dépôt peut faire office de serveur pour un autre dépôt. Et au passage, toute copie du dépôt est une sauvegarde.

Voilà, j'espère que ce billet pourra convaincre quelques derniers résistants à faire le pas. Les premiers temps sont un peu "rudes", mais une fois un peu exercé, on ne peut plus s'en passer.
