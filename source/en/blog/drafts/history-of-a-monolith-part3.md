History of a monolith: a new try ?
==================================

- uniquekey: history-of-a-monolith-part3
- date: 2017-07-15
- tags: legacy,architecture,DDD,CQRS

-------------------------------

In the [previous parts of this series](), I focused on the genesis of the monolith we started at the end of 2005. I left the project in early 2007. It was then planned to grow quickly, then the team would have to adapt (in size and more...). I came back on the project in 2009, this post highlights new challenges we faced, evolution we made (lots of experiments) and consequences until 2012 when I left again.

-------------------------------

## Back on board

<img alt="Arche de Noé???" src="" class="img-float-left"/>

In 2009, I came back in a team of 12 developers, whereas 2 years before, I left a team of 6 developers. It doubled in 2 years!

Technically, it was a continuation of the architecture set up in 2005.

## Far less agile...

Due to the team size?

## Slow and expensive delivery

Size of the team => need more synchronisation
Very few branches (just for "long-term"/isolated subjects)

## Legacy technical choices revisited

Lots of refactoring with dedicated refactoring budget

Dependency inversion

Very complex and unpredictible Data Access Layer: lots of SELECT N+1, too many findByXXX methods...=> introduce a sort of Query pattern abstracting database querying (called SearchCriteria) including graph loading depth management => too generic, leaking in upper layers, end up with something very complex (but still more predictible).

## Introduction of some "sort of" DDD

Command, events, but no CQRS, no Event Sourcing.
Use of 

## The ultimate error: distributed & transactional cache of object graph

## I quit again...

Leaving Paris to Lyon, I quit the project in March 2012. The team was still around 12 developers at this time.