
## Motivation

My binder pipeline currently requires a model type with a default constructor.
This is a bit of a rude assumption. Some types (e.g. positional records) may not support a default constructor.
The consumer may also want to enforce invariants on the class through the constructor (i.e. minimum required fields).

Overall, there are too many usecases for me to enforce a default constructor.

## Requirements and Constraints
REQ: Allow mapping to types that have no default constructor
REQ: Allow mapping to types that have a default constructor without specifying a constructor/instance


## Exploration

Q: Can constructors impose different generic type constraints?
- Not finding definitive documentation, but it seems constructors don't accept generic type arguments. They have to be on the class itself.
  - This would make sense because the constructor should only be initializing, and it can't return anything, so there's not much it could accomplish with it's own generic parameters

What options do I have?
- OPT: Require a default constructor
- OPT: Force users to construct from a factory (which could have separate type constraints and handle differences before constructing)
  - This again pushes me out of idiomatic C#
  - no less awkward than just providing an instance
- OPT: Force users to specify a model instance or factory
  - pro: consistent
  - con: adds some awkwardness to what seems like the main path 
  - Q: Would overloaded pipeline constructors make it hard to extend the pipeline?
    - `IPropertyBinder`s would be unaffected
    - Derivatives of the pipeline don't have to forward every constructor if they don't want to
    - They do need to forward every constructor if they want to keep the functionality. 
      - !!! the `IModelFactory` approach doesn't have this problem
- OPT: have some `IModelFactory` and push construction issues into a different type
  - makes the construction more flexible and trimmable, but doesn't simplify the API. Something still needs to be passed to either the pipeline constructor
  or some special extension method
  - Q: are there other construction strategies than default or from parser values?
    - maybe from a DI container... but that's a bit sketchy. They could always use the `unit -> model` overload for such a strategy
  - Q: Do I really need this flexibility and trimability?
    - it's easy to add now, but hard to add later
    - I think the reuse of factories by derivatives seals the deal
  - PRO: derivatives of BinderPipeline can reuse constructor factories defined for BinderPipeline


Q: In general, what overloads do I need?
- a plain instance
- `context -> model`
- `unit -> model` is effectively taken care of by `context -> model`
- `handler -> symbol ref [] -> model`
- There's a notable divide here, most everything can be built on top of `context -> model`

!!! I think I should force the construction strategy. The pipeline won't work without a defined model initialization strategy (even if that's requiring a default constructor)

Moving to implement, I remembered that BinderPipeline is also an `IPropertyBinder` which expects an instance of the model passed. The place where the model really gets decided is `ToHandler` or `CommandHandler.FromPropertyMap`

## Plan

Create an `IModelFactory` interface that's `context->model`

Have two overloads
- an instance
- an `IModelFactory`

TODO: Consider division of factories
- can't really mix instance reference and name reference strategies nicely. It'd take a union type
- probably `SymbolInstanceModelFactory` that's `handler, Symbol[] -> model` and `SymbolNameModelFactory` that's `handler, string[] -> model`

Problem: the model factories won't really work as classes. I run into the constructor overload issue again where I can't have additional generic arguments to the constructor, which I need for arguments to the factory func

TODO: Should probably be consistent about error behavior between modelFactory and property binding. I.e. throw an error if they map a symbol that isn't registered