History of a monolith: split it ?
==================================

- uniquekey: history-of-a-monolith-part3
- date: 2017-07-15
- tags: legacy,architecture,DDD,CQRS,history-of-a-monolith

-------------------------------

In [the previous post of this series](), I focused on my two first (quite long) takes on a project we started at the end of 2005. I left the project in early 2007, I came back in 2009, and left again in 2012. and this post is about a new come back (for a bit more than a year now) on this 12 years old project. I will try to expose lessons learned from this experience, from organizational and technical points of view.

-------------------------------

## About monoliths, modules, applications and microservices

The difference between modules and applications is tight. Sometimes, because the domains seem different, people split in different applications. What we call modules in this experience are in fact parts of the monolith, that could be also a separate application. We could also say that modules are then DDD bounded contexts, a well defined separation of business concerns of the domain. Then I would define the monolith as a single application with too many modules which are too tighly coupled.

When modules are splitted, some would say that they are microservices. I agree with that even if I am not confortable with the hype of this keyword, leading to too technical point of view. Being (trying to be) a DDD practioner, I constantly fight the fact that microservices are part of DDD strategic design, but there are too often reduced to technical choices.

So please, before speaking about modules or microservices, explore your domain in-depth to let emerge bounded contexts that are adapted to your domain. You can do that with [Event Storming workshops]() for example.

## Practical technical split

First let's talk about why we would like to split the monolith. We focused a lot on team organization, that is definitely a 

From the last experiments, I saw different patterns that are probably not good practices to split a monolith. Most are also patterns criticized by microservices gurus. 