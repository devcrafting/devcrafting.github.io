Refactoring de code legacy : isoler les dépendances
===================================================

- uniquekey: refactoring-legacy-code-isoler-les-dependances
- date: 2017-01-03
- tags: refactoring, software craftsmanship

----------------

Voici la suite du [billet sur l'isolation des dépendances dans le cadre d'un refactoring de code legacy](/fr/blog/2016/12-refactoring-de-code-legacy-part-1-tests-de-caracterisation/). Ces billets font suite à une journée d'atelier avec [Mickael Feathers](https://twitter.com/mfeathers), l'auteur de [Working effectively with legacy code](http://wiki.c2.com/?WorkingEffectivelyWithLegacyCode). 

Dans le premier billet, nous avons vu comment écrire des tests de caractérisation. Dans ce billet, nous allons voir comment isoler les dépendances identifiées via les tests de caractérisation.

----------------

NB: désolé pour le délai de cette suite, j'ai eu du [refactoring de plancher à faire](https://twitter.com/clem_bouillier/status/811701932951748608) (entre autres travaux et rangement)...une autre histoire ;).

## Isoler les dépendances

<img alt="Plat de nouilles représentant les fortes dépendances à isoler" class="img-float-right" src="/images/spaghetti-bolognese.jpg" />

L'objectif est de rendre explicite les dépendances en les isolant. Pour cela, voici la technique de base en trois étapes :

* Extraire les dépendances sous forme d'interfaces
* Utiliser ces interfaces dans un nouveau constructeur de la classe à tester (en gardant le constructeur existant)
* Bouchonner les dépendances dans les tests

Reprenons l'exemple du [billet précédent](/fr/blog/2016/12-refactoring-de-code-legacy-part-1-tests-de-caracterisation/), où nous avons identifié les effets de bord avec les tests de caractérisation :

    [lang=csharp]
    public class CodeBoiteNoire
    {
        public string FaireQuelqueChose() 
        { 
            ...
            // Accès à une base de données (lecture ou écriture)
            ...
            // Ou encore écriture à la console
            ...
            // Ou autres effets de bord...
            ...
        }
    }

Ajoutons donc une interface pour isoler le premier effet de bord, un accès à une base de données :

    [lang=csharp]
    // L'interface de l'effet de bord
    // A NOTER : on cherchera à donner un nom beaucoup plus explicite à la classe et à la méthode
    public interface IAccessBaseDeDonnees
    {
        public void SauvegarderDonnees();
    }

    // L'implémentation contenant le code extrait de la méthode testée
    public class AccessBaseDeDonnees
    {
        public void SauvegarderDonnees()
        {
            // A NOTER : le commentaire extrait ne sert plus à rien
            // => on a mis l'équivalent dans le nom de la classe et de la méthode 
            // Accès à une base de données (lecture ou écriture)
            ...
        }
    }

    public class CodeBoiteNoire
    {
        private IAccessBaseDeDonnees _accessBaseDeDonnees;

        // On conserve le constructeur par défaut pour ne pas casser le code existant
        public CodeBoiteNoire() : this(new AccessBaseDeDonnees()) {}

        // Nouveau constructeur qu'on peut utiliser dans les tests de caractérisation
        public CodeBoiteNoire(IAccessBaseDeDonnees accessBaseDeDonnees)
        {
            _accessBaseDeDonnees = accessBaseDeDonnees;
        }

        public string FaireQuelqueChose() 
        {
            ...
            // on remplace le code extrait par l'appel à la méthode
            _accessBaseDeDonnees.SauvegarderDonnees();
            // Ou encore écriture à la console
            ...
            // Ou autres effets de bord...
            ...
        }
    }

On répéte alors cette opération pour chaque effet de bord trouvé lors des tests de caractérisation. Pour les tests de caractérisation, il y a la possibilité d'utiliser le nouveau constructeur en passant un simuli d'implémentation. On pourra utiliser un "Stub" (implémentation écrite dans le code de test) ou un "Mock" (implémentation générée par une lib de mock en donnant le comportement attendu, via une lambda typiquement). Je ne rentre pas dans le débat Mock versus Stub, voici un [court article de Thomas Jaskula](http://blogs.developpeur.org/tja/archive/2009/09/15/tests-diff-rence-entre-les-mocks-et-les-stubs.aspx) et [l'article initiateur de ce débat, de Martin Fowler](http://martinfowler.com/articles/mocksArentStubs.html).

Si vous avez plusieurs effets de bord, vous allez avoir beaucoup de paramètres dans le constructeur. C'est très intéressant, cela explicite et documente les dépendances, en plus de révéler le fait que la classe a probablement trop de responsabilités. **Faire émerger les dépendances est aussi un moyen très efficace de découvrir un problème de conception**.

Si une méthode publique de la classe initiale ne fait que faire le passe-plat vers une méthode d'une dépendance, il est préférable dans un premier temps de garder intacte l'API publique. Le nettoyage se fait plutôt une fois qu'on y voit plus clair.

A noter que certains éditeurs de code permettent de faire cette opération automatiquement.

## Isoler un Singleton

Le cas du pattern Singleton est un peu particulier, car il porte la propriété d'unicité dans le système qu'il peut être nécessaire de conserver (mais le plus souvent vous pouvez vous en passer). Exemple :

    [lang=csharp]
    public class CodeBoiteNoire
    {
        public string FaireQuelqueChose() 
        { 
            ...
            UnSingleton.Instance.FaireAutreChose();
            ...
        }
    }

Pour conserver la propriété d'unicité, il est possible de rajouter un niveau d'indirection au travers d'une méthode virtuelle "protected" encapsulant les appels au singleton. On peut alors surcharger cette méthode dans une classe dérivant de la classe à tester et utiliser cette classe dérivée dans le test.

    [lang=csharp]
    public class CodeBoiteNoire
    {
        protected virtual void FaireAutreChose()
        {
            UnSingleton.Instance.FaireAutreChose();
        }

        public string FaireQuelqueChose()
        {
            ...
            FaireAutreChose();
            ...
        }
    }

    // Dans les tests, utiliser une classe dérivée surchargeant la classe à tester
    public class CodeBoiteNoireATester
    {
        protected override void FaireAutreChose() {
            // simuli de l'appel au singleton
        }
    }

Cela reste un artifice pour tester, mais on n'altère pas le comportement de la classe à tester en ajoutant ce niveau d'indirection (à condition de ne pas faire autre chose dans cette méthode d'indirection!). 

On évitera par contre d'utiliser un setter pour définir l'instance unique sur la classe implémentant le Singleton, car cela apporte de la confusion à l'API publique du singleton.

## Isoler une dépendance statique

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

Ces premières techniques permettent déjà d'obtenir une base solide pour des refactorings plus poussés, je dirais même qu'elles sont nécessaires. Dans le prochain billet, nous nous intéresserons donc à quelques techniques de refactoring complémentaires.
