History of a monolith: the rise of keywords
===========================================

- uniquekey: history-of-a-monolith-part2
- date: 2017-07-15
- tags: legacy,architecture,DDD

-------------------------------

In the [first part of this series](), I focused on the genesis of the monolith we started at the end of 2005. I left the project in early 2007. It was planned to grow quickly, then the team would have to adapt (in size and more...). I came back on the project in 2009, this post highlights new challenges we faced, evolution we made (lots of experiments) and consequences until 2012 when I left again.

-------------------------------

## Back on board

<img alt="Arche de Noé???" src="" class="img-float-left"/>

In 2009, I came back in a team of 12 developers, whereas 2 years before, I left a team of 6 developers. It doubled in 2 years!

The recruiting had been quite hard, with some issues to avoid people with too limited technical skills. The team added a new interview process: one hour prepared questions and then discussions with a team member on their answers. Questions were based on code taken from "bad examples" found in the application source code. It allowed to better qualify technical skills, but it was also harder to find people, we had to limit our expectations to be able to onboard new developers.

## Far less agile...

The team leader had not changed, and he was doing the interface/proxy for every developers with the business team. Given the team size and Heterogeneousness of people communication skills, he tought it was simpler to proxy, even if developers were still allowed to contact the business team. By the way, it was not natural to developers, people tend to prefer to hide behind someone and just keep developing what the manager ask for. He was under high pressure. He could have hired business analysts, but it was not the model he has in mind.

Since I started to read agile books in 2008 (and realized we were "sort of" agile back in 2005-2007), I proposed to experiment it. A key thing was developers had to face business people, to be much more involved in building the right product, and not just "building it right". The team leader had to take a position of coach to help developers in their new position. Note practicing all of this was also new to me.

We mainly struggled to find the right practices for a team of 12 people. It was a bit too big to make planning pokers, daily meetings or retrospectives correctly. We adapted some practices, feeling it was agile to adapt to context. Managers were saying that maximum of 8 was written in the Scrum book, but we did not care. Even if we rejected the "by the book" approach, we should have considered issues we had as "weak signals".

Retrospectively, I would say that we experienced some drawbacks of a monolith (more to come below). Also, we were "victim" of the [Conway lay](): as a single big team, we tried to build one big application, i.e a monolith. We could have broken the team and the application in several pieces, it would have avoided the monolith and simplified agile practices. I precise it does not mean that each sub-team would have been completely separated, we could have kept some synchronisation meeting to learn from each other.

## Slow and expensive delivery


Size of the team => need more synchronisation
Very few branches (just for "long-term"/isolated subjects)

## Legacy technical choices revisited

Technically, it was a continuation of the architecture set up in 2005. I was even surprised that it was so religiously applied.

Lots of refactoring with dedicated refactoring budget

Dependency inversion

Very complex and unpredictible Data Access Layer: lots of SELECT N+1, too many findByXXX methods...=> introduce a sort of Query pattern abstracting database querying (called SearchCriteria) including graph loading depth management => too generic, leaking in upper layers, end up with something very complex (but still more predictible).

## Introduction of some "sort of" DDD

Command, events, but no CQRS, no Event Sourcing.
Use of 

## The ultimate error: distributed & transactional cache of object graph

## I quit again...

Leaving Paris to Lyon, I quit the project in March 2012. The team was around 15 developers at this time.