New blog and website
====================

- uniquekey: new-blog-and-website
- date: 2016-10-25
- tags: fsharp,devcrafting

--------------

I had two blogs in the past, the first one in English on Blogspot and then when I created DevCrafting company, I created a new site only in French with Office 365 (SharePoint). 

I never managed to keep writing new posts, for several reasons, mostly bad ones probably, but I would like to give another try with another approach. I want to talk about that as a feedback.

Also, I would like to be able to write sometimes in English, sometimes in French depending on the subject.

And last, I like the idea of [FsBlog](https://github.com/fsprojects/FsBlog) and since it did not support all I wanted to do, I decided to play with F# on my own and to create my website and blog with F# as [Tomas Petricek did](http://tomasp.net/).

--------------

## First (bad) reason: I was bored of online edition and of WYSIWYG editors. 

Online edition was not smooth enough in my opinion, I never managed to finish a blog post in one go, and drafting system was not enough simple. It seems to me much simpler to open a raw text file locally than having to open a browser, login into a website to search for the draft I want to edit, but perhaps it is just a dream, experience will say.

Also, the two experience I had (with Blogspot and Office 365/SharePoint) was using a WYSIWYG editor, which is quite a nightmare in some cases. I remember taking way too much time to write a blog post with embedded code on both platforms.

As a sidenote, I would add that Office 365 SharePoint is a nightmare period...at least for developers.

## Second (bad) reason: I did not manage to write something valuable

I know I am a bit too perfectionist, I don't like the idea of having partially speak about something. For my blog posts, I was then too much restrictive, and I got several draft posts waiting to be "improved".

I would like to talk about lots of things, but sometimes I read "better" blog posts elsewhere and lost faith to write my own thinking on the subject...sometimes I started to write and the subject was so large, I also lost faith to write...

In fact, now I understand that I should write smaller blog posts AND I should avoid asking myself if it will be valuable to others...I particularly like the explanation [@ouarzy](https://twitter.com/ouarzy) gave me: "be more selfish" when you write, because there are others goals: improve your written expression, share your thoughts, contribute to the community and gather feedbacks from others on your thoughts.

## Probably THE main (real) reason: I was not organized to write regularly

Writing blog posts is time consuming and having a constant pace is even more difficult. In my previous experiences, writing was clearly not a priority, then I was not regular and lost faith in the end...

Talking with [@ouarzy](https://twitter.com/ouarzy), he explained me how he tried to reserve some timeslots every week to write, without exemption to keep the pace. His experience shows that it works quite well: he manages to post regularly and he has lots of feedbacks from the community.

## A new try

Now I justified myself ;), I can expose my "plan", or rather my new wish: a new try taking all these experiences in account:

* An offline edition based on [GitHub Pages](https://pages.github.com), Markdown and [F# Formatting](https://tpetricek.github.io/FSharp.Formatting/)
* Simpler or more limited subjects, with less self-censorship ;)
* Time allocated every week to write blog posts
* Some subjects in English, some in French (or even both?!), depending on which community I would like to share with

Hope I will keep the pace of writing, let's say one or two times a month as a good start!

## Side-effects: some fun(ctional) with F#

I will probably write more on this, but I built my new website/blog with F#, using Markdown or F#, processed with [F# Formatting library](https://tpetricek.github.io/FSharp.Formatting/) to obtain a static site published on [GitHub Pages](https://pages.github.com).

Note I had a look at [FsBlog](https://github.com/fsprojects/FsBlog), but find it too difficult to add multilingual support in the code base, with lots of coupling and poor unit tests in place.

You can have a look at the [reame.md file](https://github.com/devcrafting/devcrafting.github.io/blob/dev/readme.md) to see which features I support. Most of features are configurable, F# code is located in [tools directory](https://github.com/devcrafting/devcrafting.github.io/tree/dev/tools) and [build.fsx file](https://github.com/devcrafting/devcrafting.github.io/blob/dev/build.fsx). For sure, I could improve it, and I could add some unit tests to avoid future problems (the same I got with FsBlog...).

Note I could have done TDD, but I found it quite difficult to improve both on a language and on a practice with this language (I will try to write on that). I could say I tried Type-DD (i.e write Types first instead of Tests), but it would be pretentious ;).

Hope you will enjoy reading me, [in French](http://www.devcrafting.com/fr/blog/) and/or [in English](http://www.devcrafting.com/en/blog/).