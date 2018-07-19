History of a monolith: split it ?
==================================

- uniquekey: history-of-a-monolith-part3bis
- date: 2018-06-05
- tags: legacy,architecture,DDD,CQRS,history-of-a-monolith

-------------------------------

In [the previous post](/en/blog/2018/04-history-of-a-monolith-part3/), I talked about organizational changes I observed on my third contract in 12 years on a monolith that grew up quickly with a team of 3 up to 20+ people today. In this post, I focus on design and technical changes we started one year ago.

-------------------------------

## About monoliths, modules, applications and microservices

<img alt="Monolith versus Microservices, with modular monolith and distributed monolith" src="/images/Monolith-Microservices.png" class="img-float-left"/>

The difference between modules and applications/products is tight. Sometimes, because the domains seem different, people split in different applications/products. What we call modules in the team I am working in are in fact parts of the monolith, that could be also a separate application. We could also say that modules are then DDD bounded contexts, a model for a given sub-domain, a well defined separation of business concerns of the domain, with its own ubiquituous language ([see this article for more details on bounded contexts/sub-domains](http://gorodinski.com/blog/2013/04/29/sub-domains-and-bounded-contexts-in-domain-driven-design-ddd/). Then I would define the monolith as a single application with too many modules which are too tighly coupled.

When modules are splitted, some would say that they are microservices. I mostly agree with that even if I am not confortable with the hype of this keyword, leading to too technical point of view. Being (trying to be) a DDD practioner, I constantly fight the fact that microservices are part of DDD strategic design, but there are too often reduced to technical choices. Note that we could split modules keeping them in the same application, a "well-done" monolith, and also that we could endup with a distributed monolith (probably the worst) if we jump too quickly in the microservices hype!

So please, before speaking about modules or microservices, explore your domain in-depth to let emerge bounded contexts that are adapted to your domain. You can do that with [Event Storming](https://www.slideshare.net/ziobrando/event-storming-recipes) for example.

## From legacy monolith to...

<img alt="Cells to represent contexts" src="/images/cells.jpg" class="img-float-left"/>

As I explained in some of the [previous posts of the series](/en/tag/history-of-a-monolith), we had some issues with our monolith, especially around team organization and technical constraints. It is difficult to be (I insist on "be", not "do"!) agile with more than 20 people dependent on a single release workflow. From technical point of view, it becomes more and more difficult to use new things, we are constrained by legacy choices and it can be challenging to isolate things.

We wanted to avoid a big bang migration to a new solution, we prefered to split few parts one by one with smaller sub-team. We initiated the change one year ago with new requirements. We wanted to avoid to put these new requirements in the monolith. Then, we were able to design with a lot more options, we felt more freedom to design the right model for the target sub-domain. It was not a simple new start, we made lots of experiments, mostly around implementation that was far from easy at first. It changed a lot from what people are used to in terms of design and implementation. Some would consider this experiments as wasted/unproductive time, I prefer to talk about learning effort.

Note also that it created a more focused sub-team. I would say that sort of [Conway's law](http://melconway.com/Home/Conways_Law.html) applied here again, not sure if it was the autonomous team organization that created a more isolated application or the reverse. We decided to split it "upfront" from the monolith, then the sub-team organized itself to be more and more autonomous from the rest of the team. I would say that it was an "on-purpose use of Conway's law", I recommend to do that.

One year later, we have our first isolated modules in production and some people in the team are able to maintain them. It is a very small part of the whole monolith, but we maintain the pace of migration with new requirements. We are extending a bit the migration to other modules and more people are involved, yet another challenge. The main takeover of this experience is to do small things, instead of planning/launching big ones. The main strategy used here is called "[Bubble Context](http://domainlanguage.com/wp-content/uploads/2016/04/GettingStartedWithDDDWhenSurroundedByLegacySystemsV1.pdf)" : implement new things in an isolated "context" which grows as a bubble and replace existing legacy step by step. Be careful to not build the next monolith as the context grows.

## Practical technical split

<img alt="Hitting an egg with a hammer could look like what we are doing using always the same tools" src="/images/egg-and-hammer.jpg" class="img-float-left"/>

The team tried to split the monolith before I came back in the team. From these experiments, I saw different patterns that are probably not really good practices to split a monolith. They are also patterns criticized by microservices gurus.

The first main pattern is to use the "same database" for several modules. I don't mean physically, you can still use the same database server, but data must be isolated from one module to another. For example, using two SQL databases on the same server is ok, but do not link them or access one from another module than its owner (i.e the module that uses it as its datastore). Using schema in the same database is more likely to be perversed, since creating cross-schema constraints, views or stored procedures is very easy. Using the same Event Store (like [EventStore.org](https://eventstore.org/)) for several sounds ok, since streams are mostly isolated and it can be viewed as a database with event bus features used to communicate between modules. Note that using a shared infrastructure can still be a bad dependency from microservices perspective. It can appear like strict rules, it should rather be guidelines, with few exceptions. Violating these "rules" creates undesirable dependencies, or if they are desirable, then you should assume they are! 

Another pattern is to systematically use the same database (SQL by default since many years...). The worst is to start modelling the database before modelling the domain, it adds lots of (useless?!) contraints. It is hard since most of developers were born in the "SQL golden age" (1970/2010?), it was one of first lessons learned at school. I hope it will be easier in next years, with people learning more NoSQL databases, but it will take some times to change. In our team, we used ElasticSearch for full-text indexing of internet content, Neo4J to request graphs which were too slow in SQL, and EventStore to store events with event sourced code. 

We often say "when all you have is a hammer, everything looks like a nail", that's how we end up with stereotypical systems mostly built around CRUD (Create/Read/Update/Delete) with an popular ORM, and many other source of accidental complexity...instead of building systems with a high focus on domain language and with simpler technical constructs. Note I used "simpler": they can seem complicated because it is not in our confort zone, that's why we can't say there are inherently complex. Once you get used to it, it is far simpler than huge frameworks from my point of view. I think to Event Sourcing and CQRS for example.

## Next ?

I cannot foresee the future, but I hope it will continue on the initiated path. I will try to give more feedbacks later.
