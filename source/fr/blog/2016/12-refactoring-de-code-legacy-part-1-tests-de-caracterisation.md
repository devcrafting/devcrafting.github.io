Refactoring de code legacy : tests de caractérisation
=====================================================

- uniquekey: refactoring-legacy-code-tests-de-caracterisation
- date: 2016-12-06
- tags: refactoring, software craftsmanship

----------------

Après avoir suivi une journée d'atelier avec [Mickael Feathers](https://twitter.com/mfeathers), l'auteur de [Working effectively with legacy code](http://wiki.c2.com/?WorkingEffectivelyWithLegacyCode), il fallait que je fasse un billet sur ce sujet (en français car il y a déjà beaucoup de contenu en anglais). J'en aurais même plusieurs :), voici la première partie se concentrant sur la première étape : écrire des tests de caractérisation. Nous verrons dans la suite de cette série l'isolation des dépendances, puis quelques techniques de refactoring typiques.

J'avais déjà lu le livre en 2008, j'ai souvent fait des codings dojo sur du code legacy, mais cet atelier était l'occasion de s'exercer une nouvelle fois et de voir comment Mickael Feathers animait son atelier. Voici donc un retour sur les points principaux.

----------------

## Mais au fait, c'est quoi du code legacy?

<img alt="Le code legacy est comme une bombe à retardement, qui finira par vous exploser dans les mains, et qu'on craint de désamorcer" class="img-float-left" src="/images/bombe.png"/>

Il y a pas mal de discussions sur ce sujet, je ne vais pas rentrer dans des explications détaillées des controverses. Je vous propose deux définitions (imparfaites probablement).

Premièrement, je dirais que tout code écrit, même très "beau" et testé, devient rapidement du code legacy. En effet, pour écrire un code donné, vous avez fait des choix en fonction d'un contexte donné. Or le contexte peut (va) évoluer et les choix faits lors du développement de ce code vont s'avérer alors potentiellement moins adaptés. Le soucis avec cette définition est qu'il n'y a pas de discrimination du code legacy, quasiment tout le code en production l'est. Dans ce cas, sur lequel faut-il se focaliser ?

Aternativement, on pourrait dire que c'est du code qui gratte lorsqu'on essaie de le changer, voir qu'on craint de changer de peur de tout casser. Je restreins intentionnellement à du code qu'on veut changer car le code qui ne change pas fonctionne tranquillement en production. Même si ce dernier est potentiellement très "moche", pas testé...il fonctionne. Pourquoi le soupçonner/l'identifier comme legacy si vous n'avez pas l'intention de le changer? Avec cette définition, on peut plus facilement discriminer le code legacy. C'est d'ailleurs comment travailler avec (changer) ce code qui nous intéresse tout particulièrement.

## Premier pas : tests de caractérisation

La première technique à utiliser face à un code legacy est l'écriture de tests de caractérisation, c'est-à-dire d'appeler le code existant sans le modifier pour voir comment il se comporte. Cela permet d'écrire des tests automatisés qui assurent que les comportements observés sont conservés lors du refactoring, car a priori ces comportements sont ceux de l'application en production. On appelle aussi cette technique le "**Golden Master**", du nom donné aux résultats donnés par le code legacy original.

<img alt="Boîte noire symbolisant le code à tester pour le refactorer" class="img-full-width" src="/images/blackbox.png"/>

Ecrire des tests unitaires automatisés ne sera probablement pas une bonne idée, car cela nécessite une bonne connaissance du code existant. Il est préférable de **tester le code en mode "boîte noire"** plutôt que d'essayer d'ouvrir la boîte tout de suite.

Petit rappel: ne changez pas les fonctionnalités lors d'un refactoring, ou plutôt alterner des phases de refactoring et d'ajout de fonctionnalités. Il est important de séparer les deux activités pour éviter des effets indésirables (i.e des bugs probablement).

## Concrètement...

Pour commencer vos tests de caractérisation, **ne résonnez pas trop sur le code existant et notamment sur les dépendances ou la configuration nécessaire**. Faites au plus simple: un test où vous appelez le code existant sans aucune configuration préalable (instanciation d'une classe + appel d'une méthode typiquement). Cela va permettre de "sentir" les **effets de bord** :

* les exceptions (pas de connexion à la base de données, fichiers/dossiers introuvables, connexion réseau à un service...)
* les messages de sortie à la console (si vous savez qu'il y a une librairie de log, vous pouvez vous permettre de configurer une sortie sur la console...)


    [lang=csharp]
    // Classe à tester (qui cache probablement plein d'autres classes = dépendances...)
    public class CodeBoiteNoire
    {
        public string FaireQuelqueChose() { ... }
    }

    // Test de caractérisation
    public class CodeBoiteBoite
    {
        [Fact] public void DevraitFaireQuelqueChose() 
        {
            var boiteNoire = new CodeBoiteNoire();
            var resultat = boiteNoire.FaireQuelqueChose();
            // TODO : isoler les dépendances et ajouter des asserts découverts en exécutant le test
        }
    }

Votre test a de fortes chances d'être rouge en raison d'une exception (j'aurais même tendance à débrancher le câble réseau si vous suspectez des appels réseaux indésirables). Dans ce cas, votre objectif est de vous séparer de la dépendance l'ayant créée pour pouvoir utiliser un simuli de cette dépendance. C'est l'objet du [billet suivant](/fr/blog/2017/01-refactoring-de-code-legacy-part-2-isoler-les-dependances/). Faites de même avec les sorties à la console, vous devez vous en abstraire pour pouvoir écrire des assertions.

Une fois toutes les exceptions évitées (ou si vous n'avez pas d'exceptions), intéressez-vous au résultat: la valeur de retour de la méthode appellée ou les valeurs passées en paramètre des dépendances isolées, et écrivez une ou des assertions pour vérifier que ce résultat peut-être conservé.

## Détails d'implémentation

Si vous avez des types autres que les types primitifs, vous pouvez tout à fait utiliser leur sérialisation JSON ou surcharger la méthode ToString().

Pour simplifier, dans vos assertions, vous pouvez tester le fait que le résultat contient des sous-ensembles attendus, plutôt que de systématiquement tester la totalité. 

La bibliothèque [Approval Tests (plusieurs languages disponibles)](http://approvaltests.com) peut vous faciliter la vie pour gérer la conservation du résultat (pour éviter de grosses chaînes de caractères dans vos classes de tests). Elle s'appuie sur un fichier préfixé par le nom du test et suffixé par "approved" que vous conserver dans votre gestionnaire de code source. Quand vous lancez le test, elle génère un fichier suffixé par "received". Vous avez ensuite le choix de la manière de rapporter le résultat. En local, s'il y a des différences avec le fichier "approved", elle ouvre typiquement votre outil de diff (sinon elle n'interfère pas). Vous voyez ainsi les différences.

## Isoler les effets de bord et programmation fonctionnelle...

Vous aurez remarqué qu'on cherche via ces tests à isoler les effets de bord, c'est la même chose quand on écrit des tests en mode TDD. Finalement, ne serait-ce pas mieux d'avoir une architecture où les effets de bord sont isolés à la périphérie? C'est un sujet très intéressant portés par des languages fonctionnels, dont F# et Haskel. [Mark Seeman](https://twitter.com/ploeh) a donné une présentation très intéressante (et accessible/pas élitiste) à [BuildStuff](http://www.buildstuff.lt), la vidéo sera en ligne prochainement, je mettrais le lien ici...

## Suite...

Pour mener à bien cette étape de tests de caractérisation, vous allez certainement avoir besoin de la première pratique de refactoring : l'isolation des dépendances. C'est le [sujet du billet suivant](/fr/blog/2017/01-refactoring-de-code-legacy-part-2-isoler-les-dependances/).

Circonspect ? Pratiquez en allant à des "codings dojos" ou des "legacy code retreat", il y a pas mal de groupes locaux organisant ce genre d'événements.
