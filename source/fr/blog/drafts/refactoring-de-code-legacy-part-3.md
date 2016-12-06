Refactoring de code legacy - deuxième partie
============================================

- uniquekey: refactoring-legacy-code-part-2
- tags : refactoring

----------------

Voici la suite du [billet sur l'isolation des dépendances dans le cadre d'un refactoring de code legacy](/fr/blog/2016/12-refactoring-de-code-legacy-part-2-isoler-les-dependances/). Ces billets font suite à une journée d'atelier avec [Mickael Feathers](), l'auteur de [Working effectively with legacy code](). 

Dans les deux premiers billets, nous avons vu comment écrire des tests de caractérisation et isoler les dépendances. Dans ce billet, nous allons voir quelques techniques de refactoring.

----------------

## Refactoring d'une "God class"

	• Check for grouped members declaration (with comment), it indicates some related things => try to isolate
		○ New class with public members
		○ Extract method to keep public interface
	• When using a big class as parameter, extract empty interface and replace this class by the interface in method parameter => compiler shows errors => add methods to the interface and rename it accordingly

## Quelques autres techniques de refactoring

	• Implement: sprout method/class => be careful of granularity AND naming things matter
Code Smells, cf. Refactoring book de Martin Fowler
	• Switch => write tests, then move method, then create derived types and add the discriminator method, then finalize refactoring in base class
	• For tests, you can use sensing variables that you would remove when refactoring is done
