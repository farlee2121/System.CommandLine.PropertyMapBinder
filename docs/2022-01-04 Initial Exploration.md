

## TODO
- [ ] Add tests
- [x] Try a builder to improve type inference
- [x] experiment with simplified property maps using AutoMapper
- [x] Try mixing some conventions with explicit binders (e.g. the name-based convention)
- [x] Ensure collection inputs work
- [x] Create a readme
- [ ] Figure out packaging
  - maybe wait for more feedback / if they want to pull it into core?

Later 
- [ ] Consider possible errors getting values
  - `GetValueForHandlerParameter` considers more cases than me
  - https://github.com/dotnet/command-line-api/blob/446e832ecc07dbd7a7183c55793b27cf81e26f0d/src/System.CommandLine/Handler.cs
- [ ] See if I can collapse Argument / Option overloads (can still get type information from the property expression)
- [ ] model initialization (handle constructors at least)
- [ ] Improve naming convention binder
- [ ] some of the extension cases

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


What am I trying to achieve right now?
- I want to set property based on a path expression, not a setter function
- I think I could get away with just ripping the ReflectionHelper from AutoMapper
  - https://github.com/AutoMapper/AutoMapper/blob/bdc0120497d192a2741183415543f6119f50a982/src/AutoMapper/Internal/ReflectionHelper.cs#L77
  - Q: is this OK license-wise
    - A: yes, it's MIT licensed
  - Does `FindProperty` also handle fields?


## Alternative/convention Binding Strategies

It doesn't look like using portions of System.CommandLine.NamingConventionBinder or AutoMapper will be straight forward.

For this proof of concept, it's probably best I just make something from scratch. All I should need to do is 
- get the list of arguments and options from `context.ParseResult.CommandResult.Symbol.Children` (needs filtered because sub-commands)
- get properties and fields with `contract.GetType().GetProperties()` and `contract.GetType().GetFields()`
- run some kind of normalization process on names and find matches
  - there's certain to be case transformers out there that can handle kebab case to camel, pascal, or snake case
  - finding fewer and less commonly used than expected
  - https://github.com/vad3x/case-extensions should work
  - Would be pretty easy to give a list of allowed styles, then loop through each transform against the symbol name (to compair against the unaltered member name). If any case match, then just convert both names to some consistent format  
    - Case.Pascal, Case.Train, Case.Snake, Case.Camel, Case.Any (kebab isn't allowed in C# symbols)

Hmm. I'm not currently considering nesting, but that's a much more advanced scenario not necessary for the proof of concept

!!! This shouldn't end up in core. It should get it's own package if I even want to publish it

This could be much more general in a production case. Instead of passing a casing, pass an IConventionTest or some predicate `symbolInfo -> propertyInfo -> bool`
Then each casing gets its own predicate. This would probably be easier to understand and extend. It doesn't cost us anything performance-wise.
It also makes it much easier to add things like prefixes.

`.MapFromNamingConvention(NameConvention.PascalCase)`
`.MapFromNamingConvention(NameConvention.And(NameConvention.WithPrefix("Input_"), NameConvention.Pascal))`
- hmm. probably need some way to specify and versus or

!!! I can't extend the static `PropertyMap` class. That means it'd apply for `CommandHandler` too
- Partial seems to only work within an assembly
- F# does it somehow with modules...



## Dependency Injection 

I could see dependency injection as a common ask in the binding pipeline.
It might be interesting to metion or demonstrate it's possible.
However, I think the input model and the dependencies should be separate. 
If anything, input might get bound in the DI process, not dependencies in the input process.
Thus I think it's more appropriate that the handler internally manages dependency containers.


## Input Model Construction

Records and other immutable data types won't want to play well with this mutation pipeline.
If input models are being kept cleanly separate, then this shouldn't be an issue, but it'll probably come up.
The user may also want to do validation in the constructor, which isn't possible currently.

It's good to support construction in any case, it solves many potential limitations and scenarios
- building on existing objects
- using immutable types
- 

Some thoughts on implementing
- the constructor can't be part of the pipeline, it has to come before or we'd still require a default constructor


## Collapsing Action/Option overloads
- pro: fewer overloads to propagate
- pro: pushes consolidation of my value retrieval strategy and creates one place for handling possible strategy changes, like inherited options
- con: allows users to pass symbols that aren't valid (e.g. commands)
- pro: follows the precedent set by other handlers (which take a list of symbols)


## Tests I should write

TEST: Given an argument A of type T WHEN I invoke the handler with A value V and A mapped to property P THEN P equals V
TEST: Given an option O of type T WHEN I invoke the handler with O value V and O mapped to property P THEN P equals V
TEST: Given an option O of type T with alias ALIAS WHEN I invoke the handler with O value V and O mapped to property P by alias ALIAS THEN P equals V
- test multiple aliases too. Seems like a good property test

TEST: Same tests as for property, but with fields
TEST: Make sure many arity options map
TEST: Verify exception when no such input symbol (both name and reference)
TEST: Verify that later registrations override earlier ones
TEST: Verify constructor instantiation equals initializer instantiation
 

