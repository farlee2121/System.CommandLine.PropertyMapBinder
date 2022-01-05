

## TODO
- [ ] Add tests
- [ ] Try a builder to improve type inference
- [ ] experiment with simplified property maps using AutoMapper
- [ ] Try mixing some conventions with explicit binders (e.g. the name-based convention)
- [ ] Create a readme
- [ ] Figure out packaging

## Explorations


Q: How do I create a handler at the end of the chain of binders / property maps?
- I think I might be able to get away using AnonymousCommandHandler from the project and withing the scope passing the binding context to a pipeline of delegates
- I think this is possible, the properties only need the context, they don't need to create a handler. And the end handler only needs to know about the aggregated config type, not the property types


Add alias is part of IdentifierSymbol https://github.com/dotnet/command-line-api/blob/b805d096b6caabbabc321ff95425a6bf0de45d90/src/System.CommandLine/IdentifierSymbol.cs#L68
- All children should have an Aliases collection
- Name seems to always be listed as an alias


Possible next steps
- [ ] Try improving type inference / signatures of map 
  - idea: could use a builder
  - maybe I create an interface instead of the Func. The construction happens internally anyway where I could always construct the type
  - idea: could create an PropertyMapPipeline (interceptor), that has Add() and is itself an instance of `Func<input, content, input>` 
    - This has the added benefit that any convention-based approaches don't need the list. They can specify the PropertyMap.ConventionHere direction
- idea: could use AutoMapper behind the scenes to prove out expression-based setters as a nicer-looking api
  - probably still want to keep an explicit setter overload
- [x] See if I can get the handler argument to accept a fully-fledged function
- [ ]  See if there are tests I could base from in the main project
  - it actually shouldn't be too hard. I mostly need some spys to verify parsed values are correct, make sure alternate aliases are accepted, what happens on errors, etc
  - probably various edge cases i'm not thinking of, but those should mostly be handled by the parser. I just care that I match a parser def to a property
- [x] Alternate handler overloads (e.g. Func)


PICKUP: I needs tests. Decide if I'm going to write them in C# or go F#
- I think C# because it gives me a check that the C# experience is pleasant, and warning if something breaks
- This will mean I can't use test lists, but I think it's worth it.
- I don't think I need to worry about the F# experience. That'll be a wrapper library with it's own tests

## Improving inference

No success wrapping the `Func<input, InvocationContext,input>` with an interface. The function will only infer types from it's arguments
Maybe this would be different with constructors? I'm skeptical though.

`Func` is sealed and can't be inherited. 
While creating an interface `IPropertyBinder` makes implementation a bit more verbose, It also allows me more flexibility in what gets passed as a binder 
(i.e. I can create composites, builders, etc)
- I think it'll be enough for F# that I have a function to construct a func into an IPropertyBinder

NEXT: experiment with a builder to see if fewer types need explicitly specified
- The builder extension can infer the input contract type, but not the property type, which still leaves us with specifying all types
  - This would probably be different if we were using the expression-based approach, because it'd only need to infer one input, then it could infer the property type as the return type