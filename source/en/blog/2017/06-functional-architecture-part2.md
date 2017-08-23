Functional architecture and hexagonal architecture - Introducing Railway Oriented Programming
=============================================================================================

- uniquekey: functional-architecture-part2
- date: 2017-06-07
- tags: fsharp,csharp,functional,architecture

-----------------

As a followup of the [previous post](http://www.devcrafting.com/en/blog/2017/05-functional-architecture-part1/), I would like to adjust the last example (using some comments' suggestions) and introduce a first basic F# implementation, which will allow to illustrate Railway Oriented Programming.

-----------------

## Continuation implementation revisited

As suggested in the comments of the previous post, the [continuation implementation would be even nicer with a more fluent interface](https://github.com/devcrafting/FunctionalArchitecture/blob/0a05b2255dd2e8cb9b10995886735a0c940147b8/HexagonalImpl/CartControllerContinuation.cs) (you know, like Linq API). Then, the CartController implementation would be:

    [lang=csharp]
    public class CartControllerContinuation
    {
        private readonly IProductStocks _productStocks;
        private readonly ICarts _carts;

        public CartControllerContinuation(IProductStocks productStocks, ICarts carts)
        {
            _productStocks = productStocks;
            _carts = carts;
        }

        public HttpResponseMessage AddProduct(AddProduct addProduct)
        {
            var productStock = _productStocks.Get(addProduct.ProductId);
            return productStock
                .MakeATemporaryReservation_(addProduct.Quantity)
                .ContinueWith(temporaryReservation =>
                    {
                        _productStocks.Save(productStock);
                        var cart = _carts.Get(addProduct.CartId);
                        return cart.Add_(addProduct.ProductId, addProduct.Quantity, temporaryReservation);
                    })
                .ContinueWith(cart =>
                    {
                        _carts.Save(cart);
                        return Either<HttpResponseMessage, Error>.Left(new HttpResponseMessage(HttpStatusCode.Created));
                    })
                .OnError(error => new HttpResponseMessage(HttpStatusCode.BadRequest))
                .Result();
        }
    }

We could improve readability using function instead of lambdas. 

<img alt="Railway oriented programming illustration" src="/images/railway-programming.jpg" class="img-float-right"/>

To allow this fluent interface, I just modified the Either construct to return something instead of `void` for `ContinueWith` method. It returns a new Either with the same type for the second parameter. Depending on the result of the current Either, it continues with the one returned by the lambda given in parameter (the Left), else it switches to the Right. This switch mecanism can be viewed as a rail switch: you can continue "straight" (Left) OR you change to the other way (Right). For this reason, it is also known as [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) (you will find great illustration with railway switches schemas + a link to some code in F# and C# also). By the way, I forgot to mention that `Either` naming come from Haskell, it is a Monad (yes I said the M-word!). But I really like the metaphor with railway switches to explain that.

    [lang=csharp]
    public class Either<T1, T2>
    {
        private T1 _left;
        private T2 _right;
        private Func<T2, T1> _onError;

        private Either(T1 t1)
        {
            _left = t1;
        }

        private Either(T2 t2)
        {
            _right = t2;
        }

        public static Either<T1, T2> Left(T1 t1)
        {
            return new Either<T1, T2>(t1);
        }

        public static Either<T1, T2> Right(T2 t2)
        {
            return new Either<T1, T2>(t2);
        }

        public Either<T3, T2> ContinueWith<T3>(Func<T1, Either<T3, T2>> continueWith)
        {
            if (_left != null)
            {
                return continueWith(_left);
            }
            return Either<T3, T2>.Right(_right);
        }

        public Either<T1, T2> OnError(Func<T2, T1> onError)
        {
            _onError = onError;
            return this;
        }

        public T1 Result()
        {
            if (_left != null)
            {
                return _left;
            }
            return _onError(_right);
        }
    }

My feelind is that Either is still quite (too much?!) verbose in object oriented language. Also, if you have a look at the (fake) implementation of Cart or ProductStock, you will see that it is also quite verbose to declare "go left" or "go right" as return value.

That's why, now, I propose you to switch to F# to see the magic happen :).

## A first basic F# implementation

In this first F# implementation, I stick with the old continuation implementation shown at the end of the previous post (i.e without fluent interface), we will have something even nicer with F# in the next post.

The Either construct is called Result in this case:

    [lang=fsharp]
    type Result<'a> = Success of 'a | Failure of string

Yes, it is just one line! That's part of the magic with F# :). We still have to implement the `ContinueWith` logic, but you will see it is not harder. I can even put the whole code here since it is so concise. It starts with the (immutable by default!) types I use:

    [lang=fsharp]
    module FunctionalArchitecture

    open System

    type AddProduct = {
        CartId: Guid
        ProductId: Guid
        Quantity: int
    }

    type TemporaryReservation = {
        Id: Guid
    }

    type ProductStock = {
        Id: Guid
        Quantity: int
    }

    type Cart = {
        Id: Guid
        Items: CartItem list
    }
    and CartItem = {
        ProductId: Guid
        Quantity: int
        Reservations: TemporaryReservation list
    }

    type Result<'a> = Success of 'a | Failure of string

Then I can declare the domain logic (equiv. of Cart and ProductStock classes) 

    [lang=fsharp]
    let makeATemporaryReservation (addProduct:AddProduct) (productStock:ProductStock) = 
        let remainingQuantity = productStock.Quantity - addProduct.Quantity
        if remainingQuantity >= 0 then
            Success ({ TemporaryReservation.Id = Guid.NewGuid() }, { productStock with Quantity = remainingQuantity })
        else
            Failure "Not enough stock"

    let addProductToCart (addProduct:AddProduct) (temporaryReservation:TemporaryReservation) (cart:Cart) =
        // Nothing is modified here (immutable by default)
        let cartItems, cartItem = 
            match cart.Items |> List.tryFind (fun i -> i.ProductId = addProduct.ProductId) with
            | Some item -> 
                cart.Items |> List.except [item], 
                { item with Quantity = item.Quantity + addProduct.Quantity; Reservations = temporaryReservation :: item.Reservations }
            | None -> 
                cart.Items, 
                { ProductId = addProduct.ProductId; Quantity = addProduct.Quantity; Reservations = [ temporaryReservation ] }
        if cartItem.Quantity > 99 then
            Failure "Too many items of same product"
        // ... many other rules on cart content
        else
            Success { cart with Items = cartItem :: cartItems }

Then I have some fake implementations of infrastructure (equiv. of ICarts and IProductStocks implementation):

    [lang=fsharp]
    let getProductStock id = 
        // Some data access...
        { Id = id; Quantity = 10 }

    let saveProductStock productStock = 
        // Some data access...
        ()

    let getCart id =
        // Some data access...
        { Id = id; Items = [] }
    
    let saveCart cart =
        // Some data access...
        ()

And in the end, the controller orchestrates the whole:

    [lang=fsharp]
    open System.Net
    open System.Net.Http

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

The [full source code is here](https://github.com/devcrafting/FunctionalArchitecture/blob/0a05b2255dd2e8cb9b10995886735a0c940147b8/FunctionalImpl/CartController.fs).

Some cool things to note:

* I do not hide anything from you, the whole F# code is there. **It is far less verbose, without being cryptic IMHO**. If you are not used to F#, it could be a little cryptic, but I highly encourage you to learn functional languages, I feel it is much easier than the whole OOP languages, patterns and practices. [Scott Wlaschin explains more in depth the conciseness power](https://fsharpforfunandprofit.com/posts/conciseness-intro/).
* F# forces you to layer things in the order they are used, so **it enforces dependencies' direction of an onion/hexagonal architecture**: first bits of code are the core, last bits are the infrastracture and the shell of the application.
* I did not use any injection in the domain functions, on the opposite of what we are used to in OOP (like in CartService and ProductStockService in the previous post). It was already there in the previous OOP continuation implementation. Orchestration of infrastructure call (with side-effects) and domain logic is done in the shell. **Both approches are hexagonal/onion architecture, but only the one without injection is pure functional architecture, i.e functional core (domain without side-effects = inner layer of the onion), imperative shell (side-effects + glue the whole = outer layer of the onion)**.
* And last, can you see the pattern with `function | Success ... | Failure ...` (using a [shortened syntax of a pattern matching](http://www.markhneedham.com/blog/2010/02/07/f-function-keyword/))? It will help us for the next post :).

## Next step

From this first F# implementation, we [will be able to evolve towards an even nicer implementation](http://www.devcrafting.com/en/blog/2017/08-functional-architecture-part3/), removing the pattern we just spotted. It will explain by example what is a Monad. And I will also show you how to use Computation Expression in F# to have a final version I really like.
