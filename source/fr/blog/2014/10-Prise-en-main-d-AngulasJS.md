Prise en main d'AngularJS : retour d'expérience
===============================================

- uniquekey: prise-en-main-angularjs
- date: 2014-10-23
- tags: JavaScript

------------------------

Je vous propose dans ce billet quelques réflexions sur mes premiers pas avec AngularJS (voir aussi [billet sur mes activités de l'été](/fr/2014/09-Activites-de-l-ete-autoformation-lyontechhub)).

------------------------

Prise en main
-------------

Pour commencer, j'ai trouvé la prise en main d'AngularJS très aisée, la documentation est bien faite (bien que le site soit un peu lent). Le cours CodeSchool m'a permis de mettre le pied à l'étrier de manière efficace.

Pour résumer, voici ce qu'il faut comprendre à mon avis pour commencer (retrouver ces points dans l'historique des commits sur le site du LyonTechHub) :

* Les modules et leurs dépendances à d'autres modules, utile pour modulariser l'application


    [lang=javascript]
    // 1er argument = nom du module
    // 2nd argument = tableau de dépendances
    var app = angular.module("lyontechhub", ["mgcrea.ngStrap.navbar"]);

* Les directives (standards et personnalisées), le binding et les filtres pour agrémenter le dev Web, une sorte de "super HTML" (cf. https://docs.angularjs.org/api/ng/directive et https://docs.angularjs.org/api/ng/filter), voici quelques exemples :


    [lang=html]
    <!-- ng-app permet de déclarer l'usage du module nommer sur ce noeud du DOM (pas forcément html) -->
    <html lang="fr"<span class="x"> ng-app="lyontechhub"</span>>

    <!-- ng-view permet de lier le contenu de la balise au template défini pour l'URL en cours via le routing (cf. ci-dessous) -->
    <div ng-view></div>

    <!-- ng-repeat permet de répéter une balise (ici 'li') à partir d'une liste du modèle -->
    <!-- | orderBy est un filtre, la syntaxe {{ }} permet de binder une propriété du modèle -->
    <ul class="communitiesList">
        <li ng-repeat="community in communitiesViewModel.communities | orderBy : 'name'" id="{{ community.key }}">
            <h2><a href="#/community/{{ community.key }}"><img src="/imgs/{{ community.key }}.png">{{ community.name }}</a></h2>
            <p>{{ community.shortDescription }}</p>
            <hr/>
        </li>
    </ul>

    <!-- bs-navbar est une directive personnalisée proposée par le module angular-strap -->
    <!-- Ref: http://mgcrea.github.io/angular-strap/, voir l'exemple de déclaration ci-dessous -->
    <div class="collapse navbar-collapse" id="bs-navbar-collapse-1"<span class="x"> bs-navbar</span>>
    
* Voici comment déclarer une directive personnalisée par exemple (cf. https://code.angularjs.org/1.2.24/docs/guide/directive pour plus de détails)


    [lang=javascript]
    // Les directives personnalisées permettent de créer des composants d'interfaces réutilisables
    // bien utile pour limiter la duplication de code HTML
    .directive('blocHtmlSouventRepete', function() {
        return {
            template: 'Name: {{community.name}} Address: {{community.address}}'
        };
    });

* Le routing pour gérer plusieurs pages (module ngRoute séparé depuis la version 1.2 d'AngularJS)


    [lang=javascript]
    // app est un module (ou un sous-module) pour lequel on configure le routing, 
    // c'est-à-dire quel template de vue est associé à quelle URL (utilisé par la directive ng-view)
    // on peut également spécifier un controller avec les propriétés "controller" et "controllerAs"
    app.config(["$routeProvider", function($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "views/welcome.html"
            })
    [...]
            .otherwise({
                redirectTo: "/"
            });
    }]);

* Les controlleurs (ou View Models, cf. ci-dessous)


    [lang=javascript]
    // app est un module (ou un sous-module), on peut par exemple créer un sous-module par domaine métier de l'application
    // $http est un service défini comme une dépendance à injecter (/!\ pas les mêmes dépendances que pour les modules)
    // la fonction prend en paramètre les dépendances définies juste avant et décrit la logique du controlleur
    app.controller("CommunitiesViewModel", ["$http", function($http){
        [...logique de présentation, /!\ PAS DE LOGIQUE METIER /!\...]
    }]);

* Les services (standards et personnalisés), cf. https://docs.angularjs.org/api/ng/service (les services standards sont préfixés par un $)


    [lang=javascript]
    // dans le controlleur ci-dessus, on a une dépendance sur le service $http qui permet de faire des requêtes HTTP
    $http.get("/data/communities.json").success(function(data) {
        [...traitement de 'data']
    });
   
    // et voici un exemple de service personnalisé
    // (un repository pour abstraire l'accès aux données JSON...un peu surfait, mais ça donne un exemple)
    // on définit un objet (avec 2 méthodes ici)...
    var communitiesRepository = function($http) {
        this.getOne = function(key) {
            return $http.get("/data/" + key + ".json");
        };
        this.getAll = function() {
            return $http.get("/data/communities.json");
        };
    }

    // ...on définit la factory qui permet à AngularJS d'injecté le service lorsqu'il est déclaré en dépendance...
    app.factory('communitiesRepository', ['$http', function($http) {
    return new communitiesRepository($http);
    }]);

    // ...dans un autre fichier, on peut injecter ce service
    app.controller("CommunitiesViewModel", ["communitiesRepository", function(communitiesRepository){ [...] });

Bien évidemment, AngularJS ne se résume pas à ça, mais les grandes lignes (de la v1.3) sont là si je ne me trompe pas. Avec ça, vous êtes bien armés pour attaquer tout le reste. Le tutoriel et le guide du développeur sont bien fait.

Les "controlleurs" ou View Models
---------------------------------

Avant la version 1.2 d'AngularJS, les controlleurs avaient $scope injecté systématiquement afin (notamment) de fournir les données à la vue pour le binding. Par exemple :

    [lang=javascript]
    app.controller("CommunitiesController", function($scope){
        $scope.communities = ["MUGLyon", "CARA"];
    });

Depuis la version 1.2, la syntaxe appelée "controllerAs" a été introduite. Les controlleurs n'ont plus à avoir $scope pour passer des données à la vue, ils portent les données à binder.

    [lang=javascript]
    app.controller("CommunitiesViewModel", function(){
        this.communities = ["MUGLyon", "CARA"];
    });

Vous noterez que du coup j'ai nommé mon controlleur ViewModel plutôt que Controller, car à mes yeux on est alors plus dans un pattern MVVM (Model-View-ViewModel) que dans un pattern MVC (Model-View-Controller). En effet, définir un état pour un Controller m'a paru un peu "choquant", et surtout ça ressemble tellement à un ViewModel que je ne vois pas l'intérêt de créer la confusion en l'appelant Controller. Aussi, ça se rapproche ainsi beaucoup de concepts vus dans WPF ou Knockout. Bref, je pinaille sur des concepts, d'ailleurs AngularJS a pris le parti de dire que c'est un framework MVW, où W signifie Whatever :), c'est-à-dire ce qu'on veut : Controller ou ViewModel ou autre...

Au passage, je trouve cette notation "controllerAs" plus légère et moins "je fourre tout dans $scope". C'est d'ailleurs aussi l'avis de Todd Moto, dont je vous recommande les conventions de code AngularJS : https://github.com/toddmotto/angularjs-styleguide/.

La faible intrusivité du framework dans les View Models
-------------------------------------------------------

Un dernier point qui m'a marqué est la faible intrusivité du framework au niveau des ViewModels (ou Controller, vous avez compris). En effet, je connaissais Knockout où chaque propriété pour laquelle on souhaite faire un databinding dynamique (c'est-à-dire si la valeur change en JS, la valeur affichée change aussi), il est nécessaire de le déclarer en tant que ko.observable() ou ko.observableArray(), puis d'utiliser une fonction pour définir (set) ou récupérer (get) la valeur de la propriété. Je trouve cette notation un peu lourde dans Knockout, j'ai donc bien apprécié la syntaxe AngularJS.

A noter que pour Knockout, vous pouvez rajouter [un plugin pour éviter cette notation](http://blog.stevensanderson.com/2013/05/20/knockout-es5-a-plugin-to-simplify-your-syntax/). Attention, cela nécessite un navigateur implémentant ECMAScript5, soit au moins IE9 dans la famille IE...



Voilà, jetez un oeil à AngularJS, c'est une corde à son arc qui est toujours intéressante d'avoir en ce moment.