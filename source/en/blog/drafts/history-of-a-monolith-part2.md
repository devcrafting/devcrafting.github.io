History of a monolith: the rise of keywords
===========================================

- uniquekey: history-of-a-monolith-part2
- date: 2017-07-15
- tags: legacy,architecture,DDD

-------------------------------

In the [first part of this series](/en/blog/2017/11-history-of-a-monolith-part1/), I focused on the genesis of the monolith we started at the end of 2005. I left the project in early 2007. It was planned to grow quickly, then the team would have to adapt (in size and more...). I came back on the project in 2009, this post highlights new challenges we faced, evolution we made (lots of experiments) and consequences until 2012 when I left again.

-------------------------------

## Back on board

<img alt="Arche de Noé???" src="" class="img-float-left"/>

In 2009, I came back in a team of 12 developers, whereas 2 years before, I left a team of 6 developers. It doubled in 2 years!

The recruiting had been quite hard, with some issues to find people with the "right enough" technical skills. The team added a new interview process: one hour prepared questions and then discussions with a team member on their answers. Questions were based on code taken from "bad examples" found in the application source code. It allowed to better qualify technical skills, but it was also harder to find people, we had to limit our expectations to be able to onboard new developers.

## Far less agile...

<img alt="" src="" class="img-float-left"/>

The team leader had not changed, and he was doing the interface/proxy for every developers with the business team. Given the team size and heterogeneousness of people communication skills, he thought it was simpler to proxy, even if developers were still allowed to contact the business team. By the way, it was not natural to developers, people tend to prefer to hide behind someone and just keep developing what the manager ask for. He was under high pressure. He could have hired business analysts, but it was not the model he has in mind. By the way, he was tough for his mental and physical integrity, it was time to release pressure.

Since I started to read agile books in 2008 (and realized we were "sort of" agile back in 2005-2007), I proposed to experiment it. A key thing was developers had to face business people, to be much more involved in building the right product, and not just "building it right". Then, the team leader had to take a position of coach to help developers in their new position. Practicing all of this was also new to me.

We mainly struggled to find the right practices for a team of 12 people. It was a bit too big to make planning pokers, daily meetings or retrospectives correctly. We adapted some practices, feeling it was agile to adapt to context. Managers were saying that maximum of 8 was written in the Scrum book, but we did not care. Even if we rejected the "by the book" approach, we should have considered issues we had as "weak signals".

Retrospectively, I would say that we experienced some drawbacks of a monolith (more to come below). Also, we were "victim" of the [Conway lay](http://melconway.com/Home/Conways_Law.html): as a single big team, we tried to build one big application, i.e a monolith. We could have broken the team and the application in several pieces, it would have probably avoided the monolith and simplified agile practices. I precise it does not mean that each sub-team would have been completely separated, we could have kept some synchronisation meetings to learn from each other. Let's dig other drawbacks of one big team working on one big monolith.

By the way, with our reorganization and adaptative approach, we managed to deliver value to statisfied business people, so it was not so dark.

## Slow and expensive delivery

<img alt="Changer une ampoule" src="" class="img-float-left"/>

Delivery was more and more painful, because it needed lots of synchronisation between team members to gather what has to be delivered. We did not have practices like Feature Flipping, but I am not sure it would have really simplified the delivery process.

We were using TFS Version Control, and we avoided branches as much as possible. Branching was reserved for some "long-term"/isolated subjects. We tried to avoid such subjects also since it is really difficult to release them and it creates stocks of undelivered features. But even with few branches we add to do some merge and it was really painful.

## Legacy technical choices in question, first refactoring

<img alt="Questions" src="" class="img-float-left"/>

Technically, when I came back, it was still the architecture set up in 2005. I was even surprised that it was so religiously applied. But it suffered from lots of code smells: God classes, lots of manager/services long classes, anemic model...We also had performance issues. Web Forms was reaching obsolescence with the rise of MVC models.

The performance problem was first tackled trying to review every SQL queries, we removed lots of "SELECT N+1" smells (you make a query with N results and one additional request for each result), we removed lots of duplication (huge data access with lots of similar findXXX methods) and optimized queries' execution plans with indexes. As we considered that "SELECT N+1" and duplication was caused by the legacy ORM we had and misuse of its poor abstraction of queries, we tried NHibernate. But without experience and fear of impacts, we gave up and we started our own abstraction on top of the legacy ORM. 

We called it "Search Criteria" and we added the concept of "Fetching Depth" to load object graph eagerly. It was really efficient to reduce performance issue and make our Data Access more predictable, we were quite satisfied even if it was a bit complex to grasp for newcomers. Today, after years of feedbacks, it seems it is far too generic, leaking in upper layers, and it ends up with something very complex (but still quite predictable).

I repeated this on another contract in the next years abstracting and encapsulating Entity Framework, with the same conclusion. In fact, I would even say that there is no good solution for complex querying needs when you use the same model for reading and writing things. I prefer to segregate read and write models, that's what [CQRS]() is about. At this time, even if I started early to discuss with [Jérémie Chassaing]() at "CQRS beers" in Paris (around 2009) and saw Greg Young starting to talk about CQRS, I was unconfortable with the idea of "potentially desynchronized" (eventually consistent) models. I was thinking it was very complicated, but the solution we found was probably even more complicated.

## Huge refactoring started

<img alt="Nuclear explosion to illustrate the huge refactoring" src="" class="img-float-left"/>

Moreover business was asking for a major rework on the core model: clients were modelled with 2 hard coded levels, company organisation was hierarchical, the new model has to allow clients hierarchy (to represent real subdivision in entities of big companies lie Microsoft for example) and matrix form of organisation (internal entities can have several management entities).

We decided to start a huge refactoring with a dedicated budget negociated with the business to allow to make the rework they asked for. We changed way too much things and ended in a very long-term refactoring, but we managed to deliver it.

Instead of hard coded dependencies relying on Singleton pattern, we added Dependency Inversion and Unity as dependency inversion/inversion of control (DI/IoC) container. Other huge refactoring were mainly around "some sort" of DDD.

## Introduction of some "sort of" DDD

<img alt="Blue Book on Domain Driven Design by Eric Evens" src="" class="img-float-left"/>

As I said, I started talking about DDD and CQRS in 2008/2009 in meetups in Paris, it was totatlly new for me, the [Blue Book]() reading was quite mysterious and I grasped only few bits, mostly tactical bits...and completely missing the strategic view of DDD as most of  beginners. On CQRS, I was sceptical about two separated models. [Udi Dahan published its article on Domain Events](), I really liked it and made it a central piece in the architecture refactoring. But we used them only as a way to decouple persistance from business logic. 

So basically, we had SearchCriteria for reading object graphs and Commands and Events in the same object graph model. We totally missed the point of Aggregate boundaries, meaning we had a huge object graph with inter-related non-anemic domain objects (i.e data + behaviors together). CQRS and even more Event Sourcing were gave up given our low maturity and fears on these concepts.

## The ultimate error: distributed & transactional cache of object graph

<img alt="'Gas factory' representing complexity of the cache solution" src="" class="img-float-left"/>

Even if we worked a lot on performance, we were still experiencing some issues, so we decided to try the "ultimate solution": build a cache...As we were very conservative about the single object graph model, we started building a distributed and transactional cache of the entire object graph...

Basically, cache was object graph loaded in memory of each server, managing an optimistic concurrency model both on each node in memory and distributed across nodes. Synchronisation of nodes was based on events we serialized in SQL database. Initially, the idea was to be able to load it incrementally, but very quickly, we gave up and we ended up with a full loading taking 5 to 10 minutes at application startup. It took some time to tweek it to an almost stable state, with some remaining odd behaviors (then we add to restart to reload cache, some times a week). The performance gain was really good.

With time passing and team turnover, it became more and more complex and no one really mastered how it works. Today, it is still "working", but it is the thing to avoid to modify and it fosters lots of critiques (quite justified indeed). It makes difficult to modify business model since it is very tightly coupled to this cache.

Stepping back, I would say that we made the "ultimate error" with this very complex cache. Cache must be something light and decoupled mostly only to query read models. We often say that CQRS read model is the "perfect cache", since you must model your reading need with the most efficient model for reading, it is dedicated to read and you can focus on performance if it is essential. I think it would have been a much better solution for the recurring performance problems we had with our complex querying needs. Performance must not be an argument for CQRS, but rather a bonus side-effect.

## I quit again...

Leaving Paris to Lyon, I quit the project in March 2012. The team was around 15 developers at this time, and it was promised to grow quickly again...
