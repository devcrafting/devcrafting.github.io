Specialization in Agile (part 2): from theory to practice 
=========================================================

--------------

In the [first part post](), I expose some refreshing visions on IT industry. It was quite rhetorical, now it’s time to illustrate with real examples from my experiences.
Stereotypical organization nowadays

--------------

First, I try to expose objectively a mainstream stereotypical organisation that illustrate use of hyperspecialization for an application development (I have already worked in a context like that):
users (they don’t know what they need exactly, don’t complain about it, you have to build adaptable solution through quick feedback)
business analyst team (produce specs not related to something that could be developed and not in phase with “real” user needs)
architecture (often in an “ivory tower”, dictates several constraints, and in worst case, it could be several different teams : security, urbanism, functional architecture, application architecture, IS architecture, technical architecture…)
X project dev team (“just” write code for new features)
maintenance dev team (“just” write code for bug fixes, far more cheaper !)
level 1 support (don’t want to eliminate “waste” their work are based on, see http://bit.ly/pE8h8N)
level 2 support (same bias as level 1)…and many more levels in worst case
test team (will not try to automate tests : same bias as support…)
dev tools organization (as complex as previous one for each tool !) : standards &amp; norms are the rules! Then impossible to customize anything according IT teams needs
many teams for operations (often too specialized themselves also…)
Note I know that there are also some constraints that could be required by regulation authorities or encouraged for responsibility segregation principles. By the way, hyperspecialization is not required.
I don’t see how you could have only one team with one member specialized on each function…how do you feed constantly each function with the same amount of work ? How do you do when one of the function is overloaded ? It seems to me completely counter-productive and against lean spirit.
My vision of specialization

I think generalizing specialist principle (http://bit.ly/9AOqRL) are a good way to benefit from existing specialities of some people and to leverage skills of others (http://en.wikipedia.org/wiki/Learning_organization).
For sure, I don’t want to install my workstation and servers supporting VMs, nor administrate network, nor install databases clusters…but hyper-specialization leads to an extreme where your are finally responsible of something so tiny, that you become responsible of nothing…and a land of everyone responsible of nothing, it becomes a nightmare!
The way we apply this in our team is : everyone is responsible from business need elicitation (with users directly, proxy of users – Marketing – exactly) to delivery for bugs and features, and supporting at level 2 (level 1 is done by). So each member work on business analysis, dev, part of tests (unit, integration &amp; functional ones automation). Giving the problems encountered with dev tools organisation, we have our own operation (not so hard and secured at best we can). With this organisation, we are often slowed by some counter-productive activities (like support level 2, manual tests, regressions due to bad tests…), so we are motivated too reduce them to improve our velocity.