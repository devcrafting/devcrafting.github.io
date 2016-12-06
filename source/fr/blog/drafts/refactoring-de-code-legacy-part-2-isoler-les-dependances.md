Refactoring de code legacy : tests de caractérisation
=====================================================

- uniquekey: refactoring-legacy-code-tests-de-caracterisation
- tags: refactoring

----------------

Voici la suite du [billet sur l'isolation des dépendances dans le cadre d'un refactoring de code legacy](/fr/blog/2016/12-refactoring-de-code-legacy-part-2-isoler-les-dependances/). Ces billets font suite à une journée d'atelier avec [Mickael Feathers](), l'auteur de [Working effectively with legacy code](). 

Dans les deux premiers billets, nous avons vu comment écrire des tests de caractérisation et isoler les dépendances. Dans ce billet, nous allons voir quelques techniques de refactoring.

----------------

## Mais au fait, c'est quoi du code legacy?

Il y a pas mal de discussions sur ce sujet, je ne vais pas rentrer dans des explications détaillées des controverses. Je vous propose deux définitions (imparfaites probablement)

Premièrement, je dirais que tout code écrit, même très "beau" et testé, devient très rapidement du code legacy. En effet, pour écrire un code donné, vous avez fait des choix en fonction d'un contexte donné. Or le contexte peut (va) évoluer et les choix faits lors du développement de ce code vont s'avérer alors potentiellement moins adaptés. Le soucis avec cette définition est qu'il n'y a pas de discrimination du code legacy, quasiment tout le code en production l'est.

Aternativement, on pourrait dire que c'est du code qui gratte lorsqu'on essaie de le changer, voir qu'on craint de changer de peur de tout casser. Je restreint intentionnellement à du code qu'on veut changer car le code qui ne change pas fonctionne transquillement en production. Même si ce dernier est potentiellement très "moche", pas testé...il fonctionne, pourquoi le soupçonner/l'identifier comme legacy si vous n'avez pas l'intention de le changer? Avec cette définition, on peut plus facilement discriminer le code legacy. C'est d'ailleurs comment travailler (changer) ce code qui nous intéresse tout particulièrement.

## Premier pas : tests de caractérisation

<img alt="Boîte noire symbolisant le code à tester pour le refactorer" class="img-float-left" src=""/>

La première technique à utiliser face à un code legacy est l'écriture de tests de caractérisation, c'est-à-dire d'appeler le code existant sans le modifier pour voir comment il se comporte. Cela permet d'écrire des tests automatisés qui assurent que les comportements observés sont conservés lors du refactoring, car a priori ces comportements sont ceux de l'application en production. On appelle aussi cette technique le "**Golden Master**".

Ecrire des tests unitaires automatisés ne sera probablement pas une bonne idée, car cela nécessite une bonne connaissance du code existant. Il est préférable de **tester le code en mode "boîte noire"** plutôt que d'essayer d'ouvrir la boîte tout de suite.

Petit rappel: ne changer pas les fonctionnalités lors d'un refactoring, ou plutôt alterner des phases de refactoring et d'ajout de fonctionnalités. Il est important de séparer les deux activités pour éviter des effets indésirables (i.e des bugs probablement).

Pour commencer vos tests de caractérisation, ne résonner pas trop sur le code existant et notamment sur les dépendances ou la configuration nécessaire. Faites au plus simple: un test où vous appelez le code existant sans aucune configuration préalable (instanciation d'une classe + appel d'une méthode typiquement). Cela va permettre de sentir les **effets de bord** :

* les exceptions (pas de connexion à la base de données, fichiers/dossiers introuvables, connexion réseau à un service...)
* les messages de sortie à la console (si vous savez qu'il y a une librairie de log, vous pouvez vous permettre de configurer une sortie sur la console...)


    [lang=csharp]
    // Classe à tester
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

Votre test a de fortes chances d'être rouge en raison d'une exception (j'aurais même tendance à débrancher le câble réseau si vous suspectez des appels réseaux indésirables). Dans ce cas, votre objectif est de vous séparer de la dépendance l'ayant créée pour pouvoir utiliser un simuli de cette dépendance. C'est l'objet de la section suivante. Faites de même avec les sorties à la console, vous devez vous en abstraire pour pouvoir écrire des assertions.

Une fois toutes les exceptions évitées (ou si vous n'avez pas d'exceptions), intéressez-vous au résultat: la valeur de retour de la méthode appellée ou les valeurs passées en paramètre des dépendances isolées, et écrivez une ou des assertions pour vérifier que ce résultat peut-être conservé.

## Isoler les dépendances

<img alt="Plat de nouilles représentant les fortes dépendances à isoler" class="img-float-left" src="" />

L'objectif est de rendre explicite les dépendances en les isolant. Pour cela, voici la technique de base en trois étapes :

* Extraire les dépendances sous forme d'interfaces
* Utiliser ces interfaces dans un nouveau constructeur de la classe à tester
* Bouchonner les dépendances dans les tests


    [lang=csharp]
    public class CodeBoiteNoire
    {
        public string FaireQuelqueChose() 
        { 
            ...
            
        }
    }

A noter que le fait d'avoir beaucoup de paramètres dans un constructeur est très intéressant, cela explicite et documente les dépendances, en plus de révéler le fait que la classe a probablement trop de responsabilités. **Faire émerger les dépendances est aussi un moyen très efficace de découvrir un problème de conception**.

Si une méthode publique de la classe initiale ne fait que faire le passe-plat vers une méthode d'une dépendance, il est préférable dans un premier temps de garder intacte l'API publique. Le nettoyage se fait plutôt une fois qu'on y voit plus clair.

### Isoler un Singleton

Le cas du pattern Singleton est un peu particulier, car il porte la propriété d'unicité dans le système qu'il peut être nécessaire de conserver (mais le plus souvent vous pouvez vous en passer). Dans ce cas, il est aussi possible de rajouter un niveau d'indirection au travers d'une méthode virtuelle "protected" encapsulant les appels au singleton. On peut alors surcharger cette méthode dans une classe dérivant de la classe à tester et utiliser cette classe dérivée. Cela est un artifice pour tester, mais on n'altère pas la classe à tester même en ajoutant un niveau d'indirection. On évitera par contre d'utiliser un setter pour définir l'instance unique sur la classe implémentant le Singleton, car cela apporte de la confusion à l'API publique de cette classe.

### Isoler une dépendance statique

Enfin, il y a également le cas des dépendances statiques via des appels de méthodes statiques. Pour les isoler, il est possible de créer une méthode non statique utilisant cette méthode statique. Vous pouvez alors surcharger la méthode utilisant la méthode statique avec une méthode prenant en plus une instance de la classe définissant la nouvelle méthode non statique.

    [lang=csharp]
    public class CodeBoiteNoire
    {
        public string FaireQuelqueChose()
        { 
            ...
            DependanceStatique.FaireAutreChose();
            ...
        }
    }

    public class DependanceStatique
    {
        public static void FaireAutreChose() { ... } 
    }

Se transforme en :

    [lang=csharp]
    public class CodeBoiteNoire
    {
        [Obsolete("...mais on conserve l'API publique")]
        public string FaireQuelqueChose()
        {
            FaireQuelqueChose(new DependanceStatique());
        }

        // On ajoute une surcharge de la méthode à tester
        public string FaireQuelqueChose(DependanceStatique dependance)
        { 
            ...
            dependance.FaireAutreChose();
            ...
        }
    }

    public class DependanceStatique
    {
        // On ajoute une méthode non statique déléguant à la méthode statique
        public void FaireAutreChose() 
        {
            FaireAutreChose();
        }

        [Obsolete("...mais on ne l'enlève pas pour faire petit à petit")]
        public static void FaireAutreChose() { ... } 
    }

Un cas typique peut-être l'utilisation du DateTime.Now à isoler dans une classe Time.

## La suite...

Ces premières techniques permettent déjà d'obtenir une base solide pour des refactorings plus poussés, je dirais même qu'elles sont nécessaires. Dans la seconde partie à venir, nous nous intéresserons donc quelques techniques de refactoring.