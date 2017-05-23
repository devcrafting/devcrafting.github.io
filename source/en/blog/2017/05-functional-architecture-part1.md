Functional architecture and hexagonal architecture
==================================================

- uniquekey: functional architecture
- date: 2017-05-23
- tags: fsharp,csharp,functional,architecture

------------------

I already mentioned the [video from Mark Seeman about Functional Architecture](https://www.youtube.com/watch?v=US8QG9I1XW0). I proposed sessions on this subject at [SocratesCH](http://www.socrates-ch.org) to discuss and to understand it better. My understanding is still far from perfect, and in this blog post, I would like to give some feedbacks on experiments I have done with an OOP language, sketching hexagonal architecture implementations, before switching to functional implementation.

------------------

The idea behind "functional architecture" is to have a pure "functional core" and an "imperative shell", pushing side effects to the border, removing them from the core (the domain, where rules are enforced) to make it more deterministic and then less error prone.

## Setup context

### The domain and a proposed model

The code is based on a (subset of) online shopping domain. For the purpose of these experiments, I only focus on the use case of adding a product to cart with the following rules:

* Check enough stock then reserve temporarily (let's imagine there is a timer service that remove outdated reservations)
* Do not allow accumulated quantity (in the cart) for each product to be more than 10 (given it is possible to add a product several times, and cumulated quantity is 0)

I propose the following model (with DDD in mind):

* ProductStock aggregate to enforce the first rule
* Cart aggregate to enforce the second one

### Common technical assumptions

In any of the examples, I will rely on these assumptions:

* (sort of MVC) controller end point
* some data store somewhere (but not implemented => it is not the subject)

## Hexagonal architecture with OOP

A.K.A. "[Onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/)" or "[Clean architecture](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)" or "[Port and Adapters](http://alistair.cockburn.us/Hexagonal+architecture)"...as [Mark Seeman explained there are all about the same thing](http://blog.ploeh.dk/2013/12/03/layers-onions-ports-adapters-its-all-the-same/).

So the first step I propose is to setup an hexagonal architecture, with different flavors. It is a first step towards a functional architecture since the idea is to only rely on abstracted input and output adapters in the core.

### "Ifs and primitives" implementation

First, I propose to [rely only on primitives return types from the core domain](https://github.com/devcrafting/FunctionalArchitecture/blob/master/CartControllerIfs.cs):

    [lang=csharp]
    public CartControllerIfs(IProductStocks productStocks, ICarts carts)
    {
        _productStocks = productStocks;
        _carts = carts;
    }

    public HttpResponseMessage AddProduct(AddProduct addProduct)
    {
        var productStock = _productStocks.Get(addProduct.ProductId);
        var temporaryReservation = productStock.MakeATemporaryReservation(addProduct.Quantity);
        if (temporaryReservation != null)
        {
            _productStocks.Save(productStock);
            var cart = _carts.Get(addProduct.CartId);
            if (cart.Add(addProduct.ProductId, addProduct.Quantity, temporaryReservation))
            {
                _carts.Save(cart);
            }
            else
            {
                return Error("Sorry, cannot add to cart");
            }
        }
        else
        {
            return Error("Sorry, not enough stock");
        }
        return new HttpResponseMessage(HttpStatusCode.Created);
    }

Drawbacks:

* Check "null" on productStock.MakeATemporaryReservation return => failure very implicit
* Check true/false on cart.Add => no information on why it fails
* Cascading "ifs"
* Pure and impure (Get/Save) functions alternate mixed with business logic 
* Duplicate conditionals between aggregates functions and "ifs"

Three first drawbacks are quite well known. The two last ones are more interesting.

If we mix business logic with calls to pure and impure functions, then we need to test and test can become quite hard to write (because of need to write fake implementations for abstracted impure functions).

About "duplicate conditionals", you can imagine that we have to write some conditions to return null or not on productStock.MakeATemporaryReservation, and the same for cart.Add. In the code using these methods, we add conditions based on their return values. Each of these conditions is a duplication of the inner conditions of called methods. That's why we talk about duplicate conditions.

Note we could also have a [slightly modified implementation with a ProductStockService and a CartService](https://github.com/devcrafting/FunctionalArchitecture/blob/master/CartControllerIfsWithServices.cs):

    [lang=csharp]
    public class ProductStockService
    {
        private readonly IProductStocks _productStocks;

        public ProductStockService(IProductStocks productStocks)
        {
            _productStocks = productStocks;
        }

        public TemporaryReservation MakeATemporaryReservation(AddProduct addProduct)
        {
            var productStock = _productStocks.Get(addProduct.ProductId);
            var temporaryReservation = productStock.MakeATemporaryReservation(addProduct.Quantity);
            if (temporaryReservation != null)
            {
                _productStocks.Save(productStock);
            }
            return temporaryReservation;
        }
    }

    public class CartService
    {
        private readonly ICarts _carts;

        public CartService(ICarts carts)
        {
            _carts = carts;
        }

        public bool Add(AddProduct addProduct, TemporaryReservation temporaryReservation)
        {
            var cart = _carts.Get(addProduct.CartId);
            if (cart.Add(addProduct.ProductId, addProduct.Quantity, temporaryReservation))
            {
                _carts.Save(cart);
                return true;
            }
            return false;
        }
    }

    public CartControllerIfsWithServices(ProductStockService productStockService, CartService cartService)
    {
        _productStockService = productStockService;
        _cartService = cartService;
    }

    public HttpResponseMessage AddProduct(AddProduct addProduct)
    {
        var temporaryReservation = _productStockService.MakeATemporaryReservation(addProduct);
        if (temporaryReservation != null)
        {
            if(!_cartService.Add(addProduct, temporaryReservation))
            {
                return Error("Sorry, cannot add to cart");
            }
        }
        else
        {
            return Error("Sorry, not enough stock");
        }
        return new HttpResponseMessage(HttpStatusCode.Created);
    }

It has the same drawbacks, and it adds two others:

* There is a bit more of ceremony around injection
* Injection of data store abstractions hides the impure function calls inside services, which is against the idea of pushing side-effects to the border

Note that I am absolutely not against injection, but sometimes abusing of injection can be worst than better.

### "Exceptions" implementation

Then I propose to [rely on exceptions for error cases](https://github.com/devcrafting/FunctionalArchitecture/blob/master/CartControllerException.cs):

    [lang=csharp]
    public HttpResponseMessage AddProduct(AddProduct addProduct)
    {
        var productStock = _productStocks.Get(addProduct.ProductId);
        try
        {
            var temporaryReservation = productStock.MakeATemporaryReservation(addProduct.Quantity);
            _productStocks.Save(productStock);
            var cart = _carts.Get(addProduct.CartId);
            cart.Add(addProduct.ProductId, addProduct.Quantity, temporaryReservation);
            _carts.Save(cart);
        }
        catch (BusinessException e)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent(e.Message)
            };
            return httpResponseMessage;
        }
        return new HttpResponseMessage(HttpStatusCode.Created);
    }

Advantages:

* No more check on null/bool => more explicit with clear exceptions derived from BusinessException
* No more cascading "ifs"
* No more stop/continue business logic, try/catch is explicit to stop the flow in case of error
* Less duplicate conditionals

But still, there is one drawback. Exceptions should not be used to handle business errors, they are expected, we usually consider exceptions are more for unexpected cases.

It seems quite cool, but the drawback is still not so great. Note I was quite surprised in fact that this implementation feels so close to the next one using "continuation" (did I miss something?).

### "Continuation" implementation

With previous implementation based on exceptions, we saw a pattern to continue execution: continues on success, stop and report on errors. Instead of exceptions, we can then rely on an Either generic type that represents success with a first type parameter (special case of void with EitherVoid) OR error with a second type parameter. Either type has a ContinueWith method. Let's see this [continuation implementation](https://github.com/devcrafting/FunctionalArchitecture/blob/master/CartControllerContinuation.cs).

    [lang=csharp]
    public class Either<T1, T2>
    {
        private T1 _left;
        private T2 _right;

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

        internal void ContinueWith(Action<T1> onT1, Action<T2> onT2)
        {
            if (_left != null)
            {
                onT1(_left);
            }
            onT2(_right);
        }
    }

    public class EitherVoid<T>
    {
        internal void ContinueWith(Action onVoid, Action<T> onT)
        {
            [...]
        }
    }

The implementation follows:

    [lang=csharp]
    public HttpResponseMessage AddProduct(AddProduct addProduct)
    {
        HttpResponseMessage response = null;
        var productStock = _productStocks.Get(addProduct.ProductId);
        var eitherTemporaryReservationErrors = productStock.MakeATemporaryReservation_(addProduct.Quantity);
        eitherTemporaryReservationErrors.ContinueWith(temporaryReservation =>
        {
            _productStocks.Save(productStock);
            var cart = _carts.Get(addProduct.CartId);
            var eitherVoidErrors = cart.Add_(addProduct.ProductId, addProduct.Quantity, temporaryReservation);
            eitherVoidErrors.ContinueWith(() =>
            {
                _carts.Save(cart);
                response = new HttpResponseMessage(HttpStatusCode.Created);
            }, x => response = new HttpResponseMessage(HttpStatusCode.Forbidden));
        }, x => response = new HttpResponseMessage(HttpStatusCode.Forbidden));
        return response;
    }

Advantages:

* Same as "exceptions" implementation
* But without using exceptions :)

The main drawback of this implementation is that syntax is a bit ugly, with some sort of cascading.

Note we could use Either type slightly modified to use it in the "if" implementation, removing the primitives drawbacks.

NB: for C# developers, perhaps, it reminds you the Task API, it is very close. Task implement some sort of continuation expression. For JS developers, it reminds use of callbacks, it is the same idea.

## Next step

I haven't though to other OOP implementations (at least in C#), if you do, please suggest me your thoughts.

The next step I propose is to implement the same logic with F#, with functional architecture is mind. Stay tuned ;) (hoping I will not take 3 months to write it...).
