Architecture et conception
===========================

- uniquekey: architecture-and-conception
- layout: default

Les nouvelles technologies offrent de plus en plus de solutions pour répondre aux besoins de vos clients, parfois de façon plus ou moins adaptées en fonction de votre contexte. Comment choisir entre des solutions très génériques/standards et des solutions sur mesure, ou comment mixer les deux, tel est le challenge de l'informatique d'aujourd'hui.

A noter également que la tendance est plutôt à la sur-standardisation des solutions et à la normalisation restrictive des solutions utilisables dans un contexte, ce qui peut mener à de sérieux problèmes d'adéquation avec les besoins. Il est donc important de bien comprendre les tenants et les aboutissants de chaque solution pour éviter les solutions stéréotypées et tendre ainsi vers un système informatique le plus adapté aux besoins.

Domain Driven Design
--------------------

Le Domain Driven Design (DDD) est trop souvent vu principalement par la **perspective tactique**, comportant certes certains patterns très courants, ceux-ci sont surtout des détails d'implémentation. L'intérêt repose plutôt sur la **perspective stratégique**, en complément de la notion de **langage ubiquitaire**.

La perspective stratégique amène des pistes de réflexion sur l'organisation et le découpage des applications, en prenant en compte des considérations organisationnelles autant que techniques. Les notions clés sont les *Bounded Contexts* et les *Context Maps*.

Le DDD permet avant tout à mon sens de choisir la bonne stratégie pour ensuite trouver les bonnes implémentations à utiliser dans un contexte donné.

Architectures/patterns
----------------------

De nombreux patterns et différents styles d'architecture sont à notre disposition, je n'aurais d'ailleurs pas la prétention de tous les connaître. La difficulté d'utilisation de ceux-ci réside dans leur diversité certes mais surtout dans la tendance à les considérer trop facilement comme des solutions universelles. Le(s) contexte(s) identifié(s) grâce au DDD stratégique sont très important, et savoir choisir la bonne implémentation dans les différents contextes est crucial pour éviter les solutions stéréotypées.

Voici un panel des éléments que j'ai pratiqué le plus:
* Architecture hexagonale/en onion/Clean
* CQRS (Command Query Responsability Segregation)
* Event Sourcing
* Architectures N-Tiers "classiques"
* ORM type Hibernate/Entity Framework/Doctrine

Conception
----------



Plateformes
-----------

Infrastructure
--------------
