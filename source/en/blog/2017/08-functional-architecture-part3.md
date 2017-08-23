Functional architecture and hexagonal architecture - Introducing bind, Monad and Continuation Expressions 
=============================================================================================

- uniquekey: functional-architecture-part3
- date: 2017-08-23
- tags: fsharp,functional,architecture

-----------------

In the [previous post](http://www.devcrafting.com/en/blog/2017/06-functional-architecture-part2/), I started a first F# implementation, showing "beauty" of F# code ;). Now it is time to improve a bit the controller function to be more fluent.

-----------------

## Introduce the `bind` function

As spotted in the previous post, we could do something with the recurring pattern around Success/Failure pattern matching in the following code:

    [lang=fsharp]
    let addProductToCartController (addProduct:AddProduct) =
        getProductStock addProduct.ProductId
        |> makeATemporaryReservation addProduct
        |> function 
            | Success (tempReservation, productStock) -> 
                saveProductStock productStock
                getCart addProduct.CartId
                |> addProductToCart addProduct tempReservation
                |> function
                    | Success cart -> 
                        saveCart cart
                        new HttpResponseMessage(HttpStatusCode.OK)
                    | Failure message -> new HttpResponseMessage(HttpStatusCode.BadRequest)
            | Failure message -> new HttpResponseMessage(HttpStatusCode.BadRequest)

We could define a function with two parameters

* the function to call in case of Success on the embedded value
* the previous Result (Success or Failure)

It returns a new Result (Success or Failure).

    [lang=fsharp]
    let bind func result =
        match result with
        | Success p -> func p
        | Failure m -> Failure m

With this function, we can rewrite the `addProductToCartController`:

    [lang=fsharp]
    let addProductToCartController (addProduct:AddProduct) =
        getProductStock addProduct.ProductId
        |> makeATemporaryReservation addProduct
        |> bind (fun (tempReservation, productStock) -> 
            saveProductStock productStock
            getCart addProduct.CartId
            |> addProductToCart addProduct tempReservation
            |> bind (fun cart -> 
                saveCart cart
                Success ()))
        |> toHttpResponse

It reduces the indentation, but it not really more readable for now. Note `toHttpResponse` function pushes at the end the decision of transforming the last Result to an HttpResponseMessage, that's quite nice. For example:

    [lang=fsharp]
    let toHttpResponse result =
        match result with
        | Success _ -> new HttpResponseMessage(HttpStatusCode.OK)
        | Failure _ -> new HttpResponseMessage(HttpStatusCode.BadRequest)

There is still this weird `bind` call in the middle of our code. In functional language, we like custom operators, but in fact they are quite "standard"...and there is one for `bind`: `>>=`. So let's define it in F#:

    [lang=fsharp]
    let (>>=) f1 f2 = f1 >> bind f2

A bit confusing? It uses composition (>>), let's explain step by step:

    [lang=fsharp]
    // When you have this:
    let someFunction x =
        f1 x
        |> bind (fun y -> f2 y)
    
    // You can rewrite it:
    let someFunction x =
        f1 x
        |> bind f2

    // Or:
    let someFunction x =
        bind f2 (f1 x)

    // And it is equivalent to composition of "f1" with "bind f2"
    // Remember maths: f(g(x)) is the same as "f composed with g" applied to x (f.g(x))
    // In F#, you write f.g with g >> f (it is like apply g then f), so:
    let someFunction = f1 >> bind f2

Then, our controller can be rewritten:

    [lang=fsharp]
    let addProductToCartController (addProduct:AddProduct) =
    getProductStock addProduct.ProductId
    |> (makeATemporaryReservation addProduct
        >>= (fun (tempReservation, productStock) -> 
            saveProductStock productStock
            getCart addProduct.CartId
            |> (addProductToCart addProduct tempReservation
                >>= (fun cart -> 
                    saveCart cart
                    Success ()))))
    |> toHttpResponse

It removes the bind word, but it requires more brackets :/ (and for some readers it adds cryptic/not usual functional operators...).

## Introduce Monad by example

So let's try to talk a bit of theory around Monad from this example (please tell me if I make mistakes in my explanation, I am far from an expert).

The `bind` function transforms a "world-crossing function" between a "normal world" and an "elevated world" ([see Scott Wlaschin explanation](https://fsharpforfunandprofit.com/posts/elevated-world/#series-toc)) into a function that manipulate only "elevated world" values. For example, it can transform a function with singature `a -> Result<b>`, into a function with signature `Result<a> -> Result<b>`.

If we have a look at our controller, we want to chain several functions depending on the previous function Result. Each function uses the "normal world" value embedded in case of Success by the previous one, that's why we need this `bind` function.

Chaining operations is in fact the goal of a Monad. To define a Monad, we need a data type (`Result<a>` for example), 2 functions `bind` and `return` (which is just a function that allows to elevate a "normal world" value, i.e `a -> Result<a>` for example), and some properties that must be enforced (not covered here). That's why `bind` is also called a **monadic function**.

## Use Computation Expressions in F#

I think it is enough for Monad theory for now :). Let's switch to F# computation expression feature, which allows to simplify our controller, removing the need for bind or "esoteric" functional operator.

F# computation expression lets you define your own F# construct in the following form (async and seq constructs are built this way):

    [lang=fsharp]
    let myCustomConstruct = myCustomConstruct {
        [...statements...]
    }

Inside this construct, you can use several keywords like `return` or `let!` (said "let bang"). To use them, you must define a builder for your construct with some specific functions, and instantiate it. `let!` is defined implementing the `Bind` function, and `return` is defined with the `Return` function. Sounds good, no? So let's define and instantiate a `result` computation expression using our previous `bind` function:

    [lang=fsharp]
    type ResultBuilder() =
        member this.Bind(m, f) = bind f m
        member this.Return(x) = Success x

    let result = new ResultBuilder()

And now we can use it to declare our controller:

    [lang=fsharp]
    let addProductToCartController (addProduct:AddProduct) =
        result {
            let! (tempReservation, productStock) = 
                getProductStock addProduct.ProductId
                |> makeATemporaryReservation addProduct
            saveProductStock productStock
            let! cart = 
                getCart addProduct.CartId
                |> addProductToCart addProduct tempReservation
            return saveCart cart
        }
        |> toHttpResponse

`let!` calls the `bind` function, switching to the end on Failure, ignoring the rest like exception does, but without any exception.
I find it far more readable! And you?

## Want to learn more?

I have just shown one use case of the bind function and F# computation expressions. I encourage you to have a look to [this very detailed series written by Scott Wlaschin](https://fsharpforfunandprofit.com/posts/elevated-world/#series-toc), and in particular, there are more examples in [this one comparing applicative and monadic styles for validation](https://fsharpforfunandprofit.com/posts/elevated-world-3/).
