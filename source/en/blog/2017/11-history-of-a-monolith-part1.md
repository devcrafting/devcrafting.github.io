History of a monolith: genesis
==============================

- uniquekey: history-of-a-monolith-part1
- date: 2017-11-07
- tags: legacy,architecture,history-of-a-monolith

-------------------------------

I would like to share an experience I started at the end of 2005 and I followed in time up to now (in 2017). In 2005, the team I was working in (3 persons) started a from-scratch rebuild of a legacy Notes application. Today, it is a huge monolith application with lots of legacy code, having several issues these kind of application have. My goal is to retrospect its history, to hightlight impacts of some choices and then try to find solutions.

-------------------------------

## Context

<img alt="Green field illustrating our context - everything to be done in a fertile world" src="/images/greenfield.jpg" class="img-float-right"/>

For this full-rebuild, a small team of 3 developers and 1 functional team leader was set up. I was in charge of technical choices (architecture, technologies, tools...), even if I had only 1 year experience (but a great year with lots of technical insights and learning! Many thanks to [Antoine](https://twitter.com/lamouetterieuse)).

On functional aspects, we were supposed to do an "iso-functional" application based on the existing Notes application. As you can expect, this does not mean anything: from Notes (silos) to Web app (hypertext), we had lots of new possibilities. At least, the existing application was a basis to foster discussions with business team. That was a first great topic: **we were easily inline between business team and development team**, finding consensus and solutions together. Note that this team was a user proxy, users were all around the world (3000 users in target, internal use only).

On organisational aspects, we considered an iterative development process, without knowing anything about Agile movement ("officially" started in 2001 with the [manifesto](http://agilemanifesto.org/)). We were doing **incremental early delivery** (live environment with continuous delivery) **every two weeks, with a demonstration of what was new**. We were more or less agile.

On technical aspects, we had **some technical constraints** by the team leader's manager...but we were **still quite free** out of these constraints.

## Technical constraints

<img alt="Ball and chain representing constraints" src="/images/ball-and-chain.jpg" class="img-float-left"/>

.NET 2.0 was just out (with VS2005). The mainstream way of doing Web was ASP.NET WebForms
.NET (2.0) and then WebForms. We were not enough mature to start with alternatives like first MVC implementation in open source library. By the way, managers would not had let us use it :/ (you know "we can't trust open source"...).

But it was "nice" constraints compared to the in-house "sort of" ORM we were forced to use. First, we fought for weeks to avoid an even worst code generator. It was generating sort of 3-layers code, reading the dictionary of known keywords to search them in the input file! Layers were resumed to CRUD WebForm pages, with one Biz and one Db classes, completely coupled. In brief, a nightmare.

Then, another project came with its in-house sort of ORM, the manager wanted to use this library. We thought we were safe...but not really. There was also a code generator producing one class per SQL table, with weird methods to navigate through relations. And don't ask why not NHibernate (open source...).

## First architecture

<img alt="Classic 3-tiers architecture: UI/Business/Data Access" src="/images/3-tier-arch.png" class="img-float-right"/>

I learned in my first year that we had to do **3-tiers layered architecture**: one layer for presentation, one for business logic and one for data access. I was reading dotnetguru.org (a french web site focused on architecture), this architecture seemed to be "state of the art" back in 2005.

I especially focused abstractions between layers (with interfaces), especially on independance of Business Logic Layer (BLL) from the ORM with the Data Access Layer (DAL). I messed up a bit using interfaces on Business Objects also, everything had an interface, a bit too much. I did not know about DI/IoC at this time (don"t really know when was released first container for .NET), and the choice I made was terrible: an abstract factory with implementation defined as a singleton, defining an accessor for each interfaces. It was both highly decoupled (lots of interfaces) AND coupled (no dependency inversion, use of this singleton abstract factory for all interfaces). It was merely not testable.

I knew that we should have done unit testing (not talking of TDD...), I tested NUnit, but I really struggled to find a way to use it "properly". With all the issue to solve to set up the architecture, I have to rue the day I gave up on this point...

I did not mention an implementation detail of Data Access Layer, I was scared by stored procedures. First, I had very bad (one year) experience with them, everything was coded in stored procedures in applications I worked on (with hundreds of SQL code in one stored proc). Second, it was not compatible with my understanding of ORM. At this time, I thought we had to load objects in memory, and I systematically avoided premature optimization, the main argument people were choosing stored procedure. In the end, I was a bit too much extreme not using stored procedures at all.

## Delivery process

<img alt="Cruise control tray logo" src="/images/cruisecontrol.net.png" class="img-float-left"/>

I remember this week I set up continuous integration server (CruiseControl.NET). Yes, there was no tests to run, but we used it to compile and deliver on a demo site constantly available, sort of "continuous delivery". We could demonstrate new features to users.

For production delivery, we choosed Big Bang strategy on a minimal subset of the project, with a 6 months delay to deliver a first version, used by few administrative users at start. Then, it took 6 months more to deliver a first milestone to deliver all around the world with huge data migration from Notes. This production delivery was a bit painful, with lots of manual steps and human errors prone, but we delivered on time.

After this first release, the goal was to release more frequently, once a month. It was a revolution in the company. Sometimes we heard that our software was crappy to deliver on this pace. I just precise that there were new features in addition of (quicker) bug fixes. We needed to automate some steps to secure the delivery process, even if we were far from automated release.

## Team organisation evolution

<img alt="Growing team" src="/images/growing-team.png" class="img-float-right"/>

To start the project, we were 4 developpers including 1 internship (a very productive guy BTW!). We hired 2 other developers during the first year. I then faced the challenges of recruiting developers and keeping homogeneous practices and code.

Our recruting process was quite simple based on interviews, mostly based on how people react in some situations, and few technical questions. Our biggest issue was to evaluate technical skills and how people were practicing.

## First time I quit the project

The delivery was considered successful, business team was happy of their product, and we were happy of that, I think **being Agile was a key**.

On technical side, there were some perfectible topics, but we were also quite satisfied. We mainly struggled with the in-house ORM constraint and with lack of knowledge on OOP practices (like SOLID).

I quit the project 3 months after the first "release to the world", leaving the technical lead to one of the original team member. My next contract was with the same functional leader around quality and methodology, but at the whole IT scale.
