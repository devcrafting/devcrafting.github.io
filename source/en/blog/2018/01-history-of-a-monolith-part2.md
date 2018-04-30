History of a monolith: organisation evolution over the years
============================================================

- uniquekey: history-of-a-monolith-part2
- date: 2018-01-31
- tags: legacy,agile,history-of-a-monolith

-------------------------------

In the [first part of this series](/en/blog/2017/11-history-of-a-monolith-part1/), I focused on the genesis of the monolith we started at the end of 2005. I left the project in early 2007. It was planned to grow quickly, then the team would have to adapt (in size and more...). I came back on the project in 2009, this post highlights new challenges we faced from organisational point of view, evolution we made (lots of experiments) and consequences until 2012 when I left again.

-------------------------------

## Back on board

<img alt="Noah's ark illustrating re-integration of the team" src="/images/noah-ark.png" class="img-float-left"/>

In 2009, I came back in a team of 12 developers, whereas 2 years before, I left a team of 6 developers. It doubled in 2 years!

The recruiting had been quite hard, with some issues to find people with the "right enough" technical skills. The team added a new interview process: one hour prepared questions and then discussions with a team member on their answers. Questions were based on code taken from "bad examples" found in the application source code. It allowed to better qualify technical skills, but it was also harder to find people, we had to limit our expectations to be able to onboard new developers.

We could wonder if we were not recruiting too many developers. Still there was lots of expectations on new features to add and to expand and integrate with other applications of the company. 

## Far less agile...

<img alt="Illustrate an agile team working together" src="/images/together.jpg" class="img-float-left"/>

The team leader was doing the interface/proxy for every developers with the business team. Given the team size and heterogeneousness of people communication skills, he thought it was simpler to proxy, even if developers were still allowed/encouraged to contact the business team. By the way, it was not natural to developers, I remark people tend to prefer to hide behind someone and just keep developing what the manager ask for. I even already heard that from several developers! He was under high pressure. He could have hired business analysts, but it was not the model he has in mind. He was convinced that developers closer to business is better. By the way, it was time to release pressure and bottleneck.

Since I started to read agile books in 2008 (and realized we were "sort of" agile back in 2005-2007), I proposed to experiment it. A key thing was developers had to face business people, to be much more involved in **building the right product**, and **not just "building it right"**. Then, the team leader had to take a position of coach to help developers in their new position. Practicing all of this was also new to me.

We mainly struggled to find the right practices for a team of 12 persons. It was a bit too big to make planning pokers, daily meetings or retrospectives correctly. We adapted some practices, feeling it was agile to adapt to context. Managers were saying that maximum of 8 was written in the Scrum book, but we did not care. Even if we rejected the "by the book" approach, we should have considered issues we had as "weak signals".

Retrospectively, I would say that we experienced some drawbacks of a monolith (more to come below). Also, we were "victim" of the [Conway lay](http://melconway.com/Home/Conways_Law.html): as a single big team, we tried to build one big application, i.e a monolith. We could have broken the team and the application in several pieces, it would have probably avoided the monolith and simplified agile practices. It does not mean that each sub-team would have been completely separated. We could have kept some synchronisation meetings to learn from each others.

By the way, with our new organization and adaptative approach, we managed to deliver value to a very statisfied business team, so it was not so dark.

## Slow and expensive delivery

<img alt="Slow and expensive delivery" src="/images/slow-and-complex.png" class="img-float-left"/>

Delivery was more and more painful, because it needed lots of synchronisation between team members to gather what has to be delivered. We had no practices like Feature Flipping, but I am not sure it would have really simplified the delivery process (at this time).

We were using TFS Version Control, and we avoided branches as much as possible. Branching was reserved for some "long-term"/isolated subjects. We tried to avoid such subjects also since it is really difficult to release them and it creates stocks of undelivered features. But even with few branches we had to do some merge and it was really painful. 

The monolith was hurting here. We worked a lot on automation of different tasks, it helped, but did not removed main issues due to monolith. Note we were not really aware of this potential root cause at this time.

## Next: technical point of view...

I just described some issues we faced and tried to solve. I mostly linked it to the monolith we were growing years after years without knowing what was a monolith at this time. In the next post, I will expose some technical issues we had and solutions we found.
