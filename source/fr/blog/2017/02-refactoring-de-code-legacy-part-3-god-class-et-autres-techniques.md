Refactoring de code legacy : "God class" et quelques autres techniques
======================================================================

- uniquekey: refactoring-legacy-code-part-3
- date : 2017-02-15
- tags : refactoring, software craftsmanship

----------------

Voici la suite du [billet sur l'isolation des dépendances dans le cadre d'un refactoring de code legacy](/fr/blog/2017/01-refactoring-de-code-legacy-part-2-isoler-les-dependances/). Ces billets font suite à une journée d'atelier avec [Mickael Feathers](https://twitter.com/mfeathers), l'auteur de [Working effectively with legacy code](http://wiki.c2.com/?WorkingEffectivelyWithLegacyCode). 

Dans les deux premiers billets, nous avons vu comment écrire des tests de caractérisation et isoler les dépendances. Dans ce billet, nous allons voir quelques techniques de refactoring.

----------------

## Refactoring d'une "God class"

<img alt="Illustration d'une God class" class="img-float-left" src="/images/god.jpg" />

Une "God class" c'est une classe qui fait beaucoup trop de choses, qui a donc trop de responsabilités. C'est assez facile à reconnaître : la taille de la classe suffit généralement à caractériser une "God class". Je ne veux pas forcément donner de seuil arbitraire sur une métrique arbitraire, mais à partir de 200 à 300 lignes de code, ou 10 à 15 méthodes, ou 5 à 10 propriétés (privées ou publiques via getter/setter)...bref vous voyez l'idée.

### S'attaquer à une "God class"

Alors comment s'y prendre ? On se sent souvent démuni face à ce genre de classe, c'est d'ailleurs loin d'être simple !

On peut vérifier si certains membres semblent avoir été groupés dans la classe, avec des commentaires par exemple (ou des #region en C#). Cela indique souvent la présence de choses liées qu'on peut tenter d'isoler. Malheureusement, ce n'est pas toujours aussi simple, dans ce cas, il faut étudier un peu plus le code. Pour ce genre de refactoring, il est généralement nécessaire de faire quelques essais à blanc, c'est-à-dire qu'on tente un refactoring, puis on recommence à zéro au besoin. 

	[lang=csharp]
	public class Product
	{
		// Fields about pricing
		public decimal PriceExcludingTax { get; set; }
		public decimal VatRate { get; set; }

		// Fields about product description
		public string Name { get; set; }
		public string Description { get; set; }
		public string Manufacturer { get; set; } 
		public List<string> Comments { get; set; } 

		public decimal GetPriceIncludingTax() { ... }
		public void ChangePrice(decimal priceExcludingTax, decimal vatRate) { ... }

		public void ChangeProductDescription(string name, string description, string manufacturer) { ... }
		public void AddComment(string comment) { ... }
	}

Le refactoring consiste à créer une nouvelle classe ("Extract Class"), pour laquelle on prendra le temps de bien la nommer. On peut ensuite extraire des méthodes ("Extract Method") vers cette nouvelle classe tout en gardant l'interface publique de la classe d'origine.

Cela peut être facilité par des outils intégrés à l'IDE, c'est plus simple et plus sûr (moins de chances de faire une erreur de manipulation). Si vous n'avez pas ce genre d'outils, ce n'est pas forcément compliqué à faire manuellement non plus, c'est principalement du copier-coller. Il y a d'ailleurs des cas où ce sera plus adapté de le faire manuellement pour éviter un "big bang" trop important, c'est-à-dire changer trop de code d'un coup.

	[lang=csharp]
	public class Product
	{
		public ProductPricing ProductPricing { get; set; }

		// Fields about product description
		public string Name { get; set; }
		public string Description { get; set; }
		public string Manufacturer { get; set; } 
		public List<string> Comments { get; set; } 

		public decimal GetPriceIncludingTax() { return ProductPricing.GetPriceIncludingTax(); }
		public void ChangePrice(decimal priceExcludingTax, decimal vatRate)
		{
			ProductPricing.ChangePrice(priceExcludingTax, vatRate);
		}

		public void ChangeProductDescription(string name, string description, string manufacturer) { ... }
		public void AddComment(string comment) { ... }
	}

	public class ProductPricing
	{
		// Fields about pricing
		public decimal PriceExcludingTax { get; set; }
		public decimal VatRate { get; set; }

		public decimal GetPriceIncludingTax() { ... }
		public void ChangePrice(decimal priceExcludingTax, decimal vatRate) { ... }
	}

Dans cet exemple, on aurait par exemple pu commencer par extraire la classe ProductPricing en commençant par les propriétés uniquement, puis extraire les méthodes une à une. A noter que cela peut mener à rompre l'encapsulation, l'idée est de le faire temporairement pour faciliter la démarche de refactoring. L'exemple pris n'illustre pas ce point car le code manque cruellement d'encapsulation (getter/setter publics systématiques), c'est un raccourci intentionnel, à ne pas reproduire évidemment, mais je laisse ce sujet pour plus tard.

Autre note pour aller plus loin, dans l'exemple donné, on voit deux responsabilités distinctes : la description du produit et son pricing. Il est très courant de voir une conception autour de l'objet Product visant à modéliser la réalité, or ce genre d'abstraction trop proche de la réalité est souvent une fausse bonne idée. J'interprête cela comme un travers provenant d'exemples simplistes pour expliquer la programmation objet et notamment de l'héritage (du type Chien et Chat sont des Animaux...). Plutôt que modéliser un état des concepts issus du monde réel (Product), essayer de modéliser la dynamique/les usages de ces concepts dans les différents contextes d'utilisation, cela fait émerger des concepts plus précis (ProductPricing, ProductDescription) avec une inter-dépendance souvent très faible (ici l'id du produit).

### Utiliser une "God class"

Lorsque vous devez faire appel à une "God class" ou l'utiliser en paramètre, pour clarifier l'intention que vous aviez en l'utilisant, il est préférable d'utiliser une interface extraite ("Extract Interface").

Dans un code existant, on peut également utiliser cette technique pour clarifier les dépendances à cette "God class". Pour cela, commencer par extraire une interface vide, remplacer l'utilisation de la "God class" par l'interface, on a alors des erreurs de compilation qui vous guideront pour savoir quelles méthodes ajouter. Enfin, on renomme l'interface pour clarifier l'intention de son utilisation.

## Quelques autres techniques de refactoring

### Faites germer vos fonctionnalités

<img alt="Illustration du fait de faire germer vos fonctionnalités par une jeune pousse de plante" class="img-float-left" src="/images/sprout.jpg"/>

Quand on ajoute une fonctionnalité dans un code legacy, on a tendance à s'insérer dans l'existant, en mode "Edit and Pray". Mais alors quand est-ce qu'on améliore le code ?

Face à un code legacy, pour éviter de continuer à écrire du code "à l'ancienne", on peut utiliser la technique "Sprout method/class", c'est-à-dire littéralement "faire germer une méthode/une classe". L'idée est d'écrire 

### Enlever le "code qui pue"

<img alt="Illustration du fait d'enlever les Code Smells par le nettoyage d'une plage après une marée noire" class="img-float-left" src="/images/removeCodeSmells.jpg"/>

[Martin Fowler](https://twitter.com/martinfowler) dans son livre [Refactoring](https://martinfowler.com/books/refactoring.html) liste un grand nombre de "Code Smells", avec des refactorings associés. Un exemple type de "Code Smell" est la suppression des "switch" qui révèlent souvent un polymorphisme non implémenté. Pour corriger cela, c'est assez simple, il suffit d'extraire une méthode dans une classe, puis d'ajouter successivement les classes dérivées représentant chaque cas du "switch".

### Utiliser des "variables tests"

<img alt="Illustration des variables tests par un multimètre" class="img-float-left" src="/images/multimeter.jpg"/>

Pour les tests, ne vous limitez pas à écrire du code "beau", parfois il est préférable de passer par des étapes intermédiaires "non idéales". Notamment, on peut utiliser des variables tests ("sensing variables") qui vont permettre de mieux comprendre le comportement d'une classe/méthode ou de le tester plus facilement, on les enlèvera une fois le refactoring terminé.

## La suite...

Nous voici à la fin de cette série de billets sur le refactoring. Il y a évidemment beaucoup plus à dire, mais le plus important est de pratiquer pour s'améliorer. La suite c'est donc vous qui allez l'écrire...via du code, lors de Coding Dojo par exemple :). 

Je reviendrais probablement sur certaines pratiques de refactoring. Un point qui m'intéresse particulièrement en ce moment est le refactoring de code legacy fonctionnel...à suivre.
