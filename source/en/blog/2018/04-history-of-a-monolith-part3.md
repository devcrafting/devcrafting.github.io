History of a monolith: getting bigger and bigger...
===================================================

- uniquekey: history-of-a-monolith-part3
- date: 2018-04-30
- tags: legacy,agile,Organization,history-of-a-monolith

-------------------------------

In the [previous parts of this series](/en/tag/history-of-a-monolith), I focused on my two first (quite long) takes on a project we started at the end of 2005. I left the project in early 2007, I came back in 2009, and left again in 2012. and this post is about a new come back (for a bit more than a year now) on this 12 years old project. I will try to expose lessons learned from this experience, from organizational and technical points of view.

-------------------------------

## Back on "this project"

<img alt="Getting bigger project illustrated with a kind of big muscles man" src="/images/getBigger.png" class="img-float-left"/>

In 2007, the team had 6 members, then 15 members in 2012, and then 30 members in 2017. It doubles every 5 years! You could wonder how is it possible? In fact, more than one project, it is several projects, more and more diversified, but still around the core business of customer relationship management. So why do I speak about one project? Because it (at least tries to) works like a single team, sharing practices (technical and more), common tools, collectively owning code of few different aplications (mainly one big monolith around the core). In fact, around 8 persons are already autonomous, less dependents on the rest of the team, because part of the code has been splitted and isolated in separate modules. Still there is a quite cohesive team around the main monolith.

How did we end up with a team of 20+ members working on the same application? Projects from different sponsors came to the table because it was somewhat related to customer relationship, and then it was evidence that it should be in THE application, I would say the main argument was to avoid consistency issues (remember that in 2005, we migrated from Lotus Notes, a silos application). The application was well-known as efficient and well designed, people were attracted and it was considered as a referential for some customers related data. We probably confused application and people organization. The efficiency was not in the application but in the people building it and their organization, practices and tools.

## Limits of this model

<img alt="Reaching limits illustrated with a steeper and steeper graphs" src="/images/reachingLimits.jpg" class="img-float-left"/>

Retrospectively, I would say that even in 2012, we were already reaching the limit of a too big team working on one single application built as a monolith. When I came back at the beginning of 2017, part of the team was more autonomous as I said, and some experiments were in progress to split some parts from the monolith. Still, there was absolutely no splitting in team organization: same practices, people sharing time between monolith and new splitted modules. The split was only in code.

From this experience, my point of view is that enforcing collective ownership of several applications/modules with a too big team is not efficient. I don't want to write in stone some numbers, the more you are the more it is complex to keep everyone on the same level of information, because numbers of interactions increase exponentially. For example, with 3 persons, there are 3 communication one-to-one channels (we can consider that it is part of the equation, even if we could consider 3 and more people channels). With 4 persons, there are 6. With 5, there are 10. With 8, there are 23...how to exchange common knowledge and practices with increasing size? I would say that it is easier to adapt our organization to limit this.

## Other side-effects

<img alt="Reaching limits illustrated with a steeper and steeper graphs" src="/images/sideEffects.jpg" class="img-float-left"/>

When you have too big teams, you get other side-effects. It is harder to have people aligned on the same way of working. Even if I don't say that I want one single brain in a team, I experienced that it is simpler to work efficiently with people working mostly the same way. Then, you have to be careful of they don't forget to get insights from outside their team: be sure they get out of their confort zone.

There was another split of the team in two different locations (30km away). Distance is another bottleneck of communication, we often talk about collocated teams in agile approaches, I experienced the side-effects of the opposite. It is not impossible, but it is harder for sure, thus it requires extra effort to stay synchronized.

## Potential target

<img alt="Target" src="/images/target.jpg" class="img-float-left"/>

I would like to experiment smaller teams with dedicated functional scope, with communities of practices across teams to get some insights. We tried a bit on some sub-projects, but still with too much friction with existing practices and efforts from team members to stay in the existing "monolithic organization". I think that we can call it like that, that's no mystery if we built a monolith, as the [Conway's law](http://melconway.com/Home/Conways_Law.html) says. It can be frightening to get out of its confort zone, but I would say that it is necessary at some point, avoiding to fight against normal communication issues at any cost.

In the next post, I will focus more on design and technical aspects.
