History of a monolith: technical evolution with the rise of keywords
====================================================================

- uniquekey: history-of-a-monolith-part2bis
- date: 2018-02-26
- tags: legacy,architecture,DDD,CQRS,refactoring,history-of-a-monolith

-------------------------------

In the [previous part of this series](/en/blog/2018/01-history-of-a-monolith-part2/), I focused on organisational issues we had and solutions we found when I cameback in 2009 on the monolith project we started at the end of 2005. In this post, I focus on technical issues and solutions.

-------------------------------

## Legacy technical choices, first refactoring

<img alt="Questions about technical choices" src="/images/wondering.jpg" class="img-float-left"/>

Technically, when I came back, it was still the architecture set up in 2005. I was even surprised that it was so religiously applied. It suffered from lots of code smells: God classes, lots of manager/services long classes, anemic model...we also had performance issues. Web Forms was reaching obsolescence with the rise of MVC pattern.

The performance problem was first tackled trying to review every SQL queries. We removed lots of "SELECT N+1" smells (you make a query with N results and one additional request for each result). We removed lots of code duplication (huge data access with lots of similar findXXX methods). We optimized queries' execution plans with indexes. We considered "SELECT N+1" and duplication was caused by the legacy home-made ORM, we tried NHibernate. But without experience and fear of impacts, we gave up and we started our own abstraction on top of the legacy ORM. 

We called it "Search Criteria" and we added the concept of "Fetching Depth" to load object graph eagerly. It was really efficient to reduce performance issue and make our Data Access more predictable, we were quite satisfied even if it was a bit complex to grasp for newcomers. Today, after years of feedbacks, it seems it is far too generic, leaking in upper layers, and it ends up with something very complex.

I repeated this on another contract in the next years abstracting and encapsulating Entity Framework, with the same conclusion. In fact, I would even say that there is no good solution for complex querying needs when you use the same model for reading and writing things. I prefer to segregate read and write models, that's what [CQRS](https://cqrs.wordpress.com/documents/cqrs-introduction/) is about. At this time, even if I started early to discuss with [Jérémie Chassaing](https://twitter.com/thinkb4coding) at "CQRS beers" in Paris (around 2009) and saw [Greg Young](https://twitter.com/gregyoung) starting to talk about CQRS, I was unconfortable with the idea of "potentially desynchronized" (eventually consistent) models. I was thinking it was very complicated, but the solution we found was probably even more complicated.

## Huge refactoring started

<img alt="Nuclear explosion to illustrate the huge refactoring" src="/images/nuclear-explosion.png" class="img-float-left"/>

Moreover business was asking for a major rework on the core model: clients were modelled with 2 hard coded levels and company organization was modelled with a hierarchy. The new model has to allow clients' hierarchy to represent real subdivision in entities of big companies (like Microsoft for example) and matrix form of internal organisation (internal entities can have several management entities).

We decided to start a huge refactoring with a dedicated budget negociated with the business to allow to make the rework they asked for. We changed way too much things and ended in a very long-term refactoring. Even if we managed to deliver it, I would not recommend it anymore.

Instead of hard coded (static) dependencies relying on Singleton pattern, we also added Dependency Inversion and Unity as dependency inversion/inversion of control (DI/IoC) container. Other huge refactorings were mainly around "some sort of" DDD (see below).

## Introduction of some "sort of" DDD

<img alt="Blue Book on Domain Driven Design by Eric Evens" src="/images/ddd.jpg" class="img-float-left"/>

As I said, I started chatting about DDD and CQRS in 2008/2009 in meetups in Paris, it was totally new for me. The [Blue Book from Eric Evans]() was quite mysterious and I grasped only few bits, mostly tactical bits...and I completely missed the strategic view of DDD as most of  beginners. On CQRS, I was sceptical about two separated models. [Udi Dahan published its article on Domain Events](), I really liked it and made it a central piece in the architecture refactoring. But we used them only as a way to decouple persistance from business logic. 

So basically, we had SearchCriteria for reading objects graphs and Commands and Events in the same objects graph model. We totally missed the point of Aggregate and context boundaries, then we had a huge object graph with inter-related non-anemic domain objects (i.e data + behaviors together). CQRS and even more Event Sourcing were gave up given our low maturity and fears on these concepts.

Again the monolith was hurting with a way too big single domain model. We could discuss what if we splitted things keeping a single monolith. I never saw a (big enough) monolith staying well structured in the long term. I agree we can build a well structured monolith in one-shot, still what about its evolution over everal years and team turnover.

## The "ultimate" error: distributed & transactional cache of an objects graph

<img alt="'Gas factory' representing complexity of the cache solution" src="/images/gas-factory.jpg" class="img-float-left"/>

Even if we worked a lot on performance, we were still experiencing some issues, so we decided to try the "ultimate solution": build a cache...As we were very conservative about the single object graph model, we started building a distributed and transactional cache of the entire object graph...

Basically, cache was objects graph loaded in memory of each server, managing an optimistic concurrency model both on each node in memory and distributed across nodes. Synchronisation of nodes was based on events we serialized in SQL database. Initially, the idea was to be able to load it incrementally, but very quickly, we gave up and we ended up with a full loading taking 5 to 10 minutes at application startup. It took some time to tweak it to an almost stable state, with some remaining odd behaviors (then we add to restart to reload cache, some times a week). By the way, the performance gain was really good.

With time passing and team turnover, it became more and more complex and no one really mastered how it works. Today, it is still "working", but it is the thing to avoid to modify and it fosters lots of critiques (quite justified indeed). It makes difficult to modify business model since it is tightly coupled to this cache.

Stepping back, I would say that we made the "ultimate error" with this very complex cache. Cache must be something light and decoupled mostly only to query read models. We often say that CQRS read model is the "perfect cache", since you must model your reading need with the most efficient model for reading, it is dedicated to read and you can focus on performance if it is essential. I think it would have been a much better solution for the recurring performance problems we had with our complex querying needs. Performance must not be an argument for CQRS, rather a bonus side-effect.

Cache is a typical solution we find in lots of monolithic applications. Again, it is possible to avoid it in monolith, still I think temptation of such global solution is bigger.

## Then I quit again...

Leaving Paris to Lyon, I quit the project in March 2012. The team was around 15 developers at this time, and it was promised to grow quickly again with new features to add...still, it was not my last comeback. Did the monolith keep growing? Story to be continued in [another post](history-of-a-monolith-part3)...
