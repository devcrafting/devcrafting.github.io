Functional code refactoring: towards immutability and pure functions 
====================================================================

- uniquekey: functional-code-refactoring-part-1 
- date: 2017-02-28
- tags: fsharp, refactoring, functional

------------------

In september 2015, I tried a refactoring kata to practice my F# skills. In this attemp, I focused on immutability and pure functions, keeping in mind what I know about refactoring in other language.

Then, in november 2016, I assisted to the refactoring workshop from [Michael Feather](https://twitter.com/mfeathers) at [BuildStuff](http://www.buildstuff.lt). And talk a bit with him about the idea of identifying functional code smells and refactoring patterns.

So I like to start to write a bit on this subject, from my humble newbie perspective :).

-------------

I started with the [Trivia kata](https://github.com/caradojo/trivia) in F#, since I know a bit more on this language since a workshop with [Scott Wlaschin](https://twitter.com/scottwlaschin) at [NCrafts.io](http://www.ncrafts.io) 2015. By the way, this kata is really great, I recommend you to do it several times, in your preferred language or others to improve.

## First goals: mutability and impure functions code smells

<img alt="Lambda caracter as functional programming symbol" src="/images/functional-programming-lambda.png" class="img-float-left"/>

My first goals were to practice immutability and pure functions, two well-known "good practices" in functional programming. I think from this perspective, we could say that mutability and impure functions are code smells in functional code. At this time, I did not focus on any other code smells, I let this point for another blog post. Let's dive a bit why it would be interesting to fix these code smells.

Mutability is about changing variables, or more exactly state of the program. What it means is that functions using a mutable variable will not have a deterministic behavior, i.e if call it twice with the same instance, it could give different output/side-effects, which can be quite disturbing to reproduce bugs/behaviors. Also when you change the state, you loose the previous state, then it is for example quite difficult to debug. Mutability also introduces time dependency, since at one time, your instance can be in state S1 and later in state S2. The most basic mutable variable used is DateTime.Now, and lots of people know that it is very difficult to test and debug (if you don't, [read this for example](http://stackoverflow.com/questions/2425721/unit-testing-datetime-now)). We could summarize that it is a matter of reliability.

Impure functions have somewhat the same drawbacks. An impure function is function that rely on reading external datas (other than the ones passed as arguments, read from a database for example) and/or that have side-effects (typically create/update/delete datas in files, call network). In fact, it is mostly any function using I/O. It means that it also adds time dependency. An impure function is also mostly not deterministic, making your program harder to reason about.

Note that mutability and side-effects are necessary in the end for a program to be useful, that's why we often talk about [functional core/imperative shell](https://www.destroyallsoftware.com/screencasts/catalog/functional-core-imperative-shell). It means that the core implementing domain logic can be pure and immutable, and that side-effects and mutability can be segregated in the shell of your program.

## Refactoring basic: caracterisation tests

<img alt="Rosetta Stone illustrating the fact that Golden Master is something that will help you recognize that refactored program is still the same, as a translation is." title="Licensed under CC-SA. © Hans Hillewaert - https://commons.wikimedia.org/wiki/File:Rosetta_Stone.JPG" src="/images/Rosetta-Stone.jpg" class="img-float-left"/>

I did not invent anything new for caracterisation tests, I used the Golden Master technique. It is usually quite easy to setup. In Trivia kata it is event easier since there are lots of Console.WriteLine, so we just need to capture program output.

I made it in several ways: 
* in 2015 [as a F# script](https://github.com/devcrafting/trivia/blob/8d20d7c460d923a571abbba6396532f2e327c1e5/fsharp/Trivia/GameRunner.fsx), 
* lately [as compiled program output capture with a FAKE script](https://github.com/devcrafting/trivia/tree/fsharp_goldenmaster).

In both cases, simple tricks:
* since there is some randomness, fix the seed
* run several games to test several game combination, changing seed each time
* redirect the program output (running the program itself or the script through fsi) in a goldenmaster.txt, given you commit it at first, then if it changes because of a bug, Git says that goldenmaster.txt file has changed.

So, now let focus on refactoring goals.

## My very first attempt

<img alt="Nuclear explosion illustrating what I did when trying a big one shot refactoring!" src="/images/nuclear-explosion.jpg" class="img-float-left"/>

My first attempt was really not baby-steps oriented. I first tried to remove state too quickly writing public methods (roll & answers) working on a new immutable type (F# record). I ended with a big refactor spending several hours without being able to check the golden master. 

In the end, I got a [quite "satisfying" implementation](https://github.com/devcrafting/trivia/tree/immutableBigRefacto/fsharp), fixing "bugs", but unable to verify the golden master, so I would say I "loose" this refactoring challenge.

## Another attempt

So I tried again with baby/small steps (commits every 5 to 30 min max). I started creating a type and moving behavior from existing methods to "pure functions" operating on this type. I added some union cases types and some pattern matching, that were revealed useless in the end, but it was a good step toward removing mutability iteratively. It let you [have a look at commit history (screenshot below)](https://github.com/devcrafting/trivia/commits/immutabilityBabySteps/fsharp), I think it is quite descriptive.

<img alt="Commits history of baby step refactoring attempt" src="/images/triviaRefactoringFirstAttemptsCommitsHistory.png" class="img-full-width"/>

Note that I talk about pure functions, but they are NOT since there are still the Console.Write/printfn calls that are side-effects on the console. I will try to remove this in another refactoring session.

In the end, it takes me far less time to refactoring with a much more reliable refactoring since I ran Golden Master before every commits. Sure there is a bias of doing it a second time, but you can see that I did not end up with the same model. I can assure you that I was often stuck at what to do next when I was in my big refactoring attempt, whereas it was much more fluent when I used baby steps. I trashed several times what I tried since previous commit to try something else.

## Next step...

As a feedback of these two experiences, first I cannot recommend more than ever baby steps refactoring (something I knew about, but...no comments). From functional programming points of view, I understood that starting with refactoring inner functions to pure functions, removing mutability step by step was the key of success. It was in fact quite the same approach than in OO refactoring: let emerge some types, but create pure functions on these immutable types instead of encapsulate things in mutable types. I don't know if it is a good conclusion from a functional perspective, but I feel it like that.

With two colleagues ([Alois](https://www.reddit.com/user/aloisdg) and [Jeff](https://twitter.com/jfsaguin)), we are trying to do this refactoring kata together. We first tried to find some code smells and we tried another way to refactor with baby steps. I let this for another blog post ;).

I can just encourage you to give a try to functional refactoring to practice your functional programming skills. It was a really good exercise for me.
