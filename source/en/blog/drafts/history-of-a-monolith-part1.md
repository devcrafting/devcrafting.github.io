History of a monolith: genesis
==============================

- uniquekey: history-of-a-monolith-part1
- date: 2017-06-15
- tags: legacy,architecture

-------------------------------

I would like to share an experience I started at the end of 2005 and I followed in time up to now (in 2017). In 2005, the team I was working in (3 persons) started a from-scratch rebuild of a legacy Notes application. Today, it is a huge monolith application with lots of legacy code, having several issues these kind of application have. My goal is to retrospect its history, to hightlight impacts of some choices and then try to find solutions.

-------------------------------

## Context

<img alt="Green field illustrating our context" src="" class="img-float-left"/>

For this full-rebuild, a small team of 3 developers and 1 functional team leader was set up. I was in charge of technical choices (architecture, technologies, tools...), even if I had only 1 year experience (but a great year with lots of technical insights and learning! Thanks to [Antoine]()).

On functional aspects, we were supposed to do a "iso-functional" application based on the existing Notes application. As you can expect, this does not mean anything: from Notes (silos) to Web app (hypertext), we had lots of new possibilities. At least, the existing application was a basis to foster discussion with business team. That was a first great topic: **we were easily inline between business team and development team**, finding consensus and solutions together. Note that this team was a user proxy, users were all around the world (3000 users in target, internal use only).

On organisational aspects, this lead us to consider an iterative development process, without knowing anything about Agile movement ("officially" started in 2001 with the [manifesto]()). We were doing **incremental early delivery** (live environment with continuous delivery) **every two weeks, with a demonstration of what was new**. We were more or less agile.

On technical aspects, we had **some technical constraints** by the team leader's manager...but we were **still quite free** out of these constraints.

## Technical constraints

.NET (2.0) and then WebForms
In-house "sort of" ORM

## First architecture

3-tiers layered architecture
Independance from the ORM: "Data Access Layer" and "Business Objects" abstraction
Fear of ProcStoc
Both highly decoupled (lots of interfaces) AND coupled (no dependency inversion, use of a single static abstract factory for all interfaces)

## Delivery process

Continuous delivery on a demo site
No production delivery, Big Bang strategy on a minimal subset of the project => took 6 months to deliver with few administrative users at start + 6 months more to deliver a first milestone to deliver all around the world with "big" data migration (not big data nevertheless ;)).
Production delivery was a pain, lots of manual steps, human errors prone...

## Team organisation evolution

End up with 6 developers (including 1 internship, a very productive guy BTW!).
More and more challenge on recruiting developers, and keeping homogeneous practices and code.

## First time I quit the project

The delivery was considered successful, business team was happy of their product, we were happy of that, IMHO **being Agile was a key**.

On technical side, there were some perfectible topics, but we were also quite satisfied. We mainly struggled with the in-house ORM constraint and with lack of knowledge on OOP practices (like SOLID).

I quit the project leaving the technical lead to one of the original team member.
