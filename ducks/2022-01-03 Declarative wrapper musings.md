---
date: 2022-01-03
---

NOTE: these thoughts were what led to me working on this library. I was trying to use the library in Notedown, and didn't like the readability of the cli structure

## Declarative cli structure

My goal is to define elements of cli (arguments, options, commands, handlers, etc) as complete declarative chunks that can be composed into a full cli.

almost have it, the sticking point is handlers and multi-arity overloads

attempts
- expressions to defer evaluation
  - taking in an untyped expression and templating it into an untyped expression almost works
- inline
  - it still tries to determine a type.
  - I think it doesn't work like addition. Addition can at least determine a member constraint like supporting addition.
- Base class or interface?
  - Func and Action overloads have no base class

Part of the problem is that the overloads take the function's types as separate generic parameters rather than the whole function signature as a single generic

Other ideas?
- require a custom binder and a single input data structure
- Investigate an overload that takes the whole function/delegate type as a single generic parameter

Q: How is the underlying binder implemented?
- Possible lead, it looks like `SetHandler` is actually part of an `ICommandHandler`
  - https://github.com/dotnet/command-line-api/commit/c31ce2797305a4e084736c1d43a078d7e90f9b60
- The underlying handler implementation is nasty redundant.
  - Each overload depends on the exact number of arguments and explicitly uses each generic type argument to extract a parameter then invoke the delegate
  - Problems with using a single structure
    - unlikely all arguments will be different types. If we don't have different types, then there has to be some implicit convention for matching arguments to properties 

IDEA: Could we require a "config type" of the command, then each nested argument and option has to specify a value path in that config type?
- This collapses all arity issue.
- Easier to configure your own convention-based bindings (name-based, type-based, annotation-based) 
- ALT: the config type could have its own configuration where each property is mapped to an argument name or an actual symbol reference
  - still easy to understand, but does have some duplication of argument declaration
  - even better for swapping in convention-based setup
  - remove the distribution of handler configuration
  - enables multiple handler configurations more cleanly
  - con: can only detect incomplete configuration at runtime
    - this is also true of the current paradigm though
- idea: I could probably use a type provider to provide compile-time checking of completeness (early detection of mismatch between handler and argument list). This would require
- TODO: I should be able to make this map-based paradigm within the current customization framework
  - 2 overloads
    - one taking a symbol name
    - one taking a symbol reference
- Looks like AutoMapper uses reflection to manage the LINQ-based path maps
  - https://github.com/AutoMapper/AutoMapper/blob/bdc0120497d192a2741183415543f6119f50a982/src/AutoMapper/Configuration/MappingExpression.cs#L173
  - This means I should either require full set functions or expressions
  - assignment functions would be easier up-front, though expressions would probably be easier for consumers (because I can take care of easily forgotten cases like enumerables)
- IDEA: I could use pipes to allow each input mapper direct access to the value type information. 
  - con: this is not really a self-contained declaration 
  - pro: avoid putting all the handlers in a shared list that requires loss of non-shared type information
- ALT: IDEA: I could construct mappers like `Input.fromName "-t" (fun input value -> expr)` or `Input.fromReference symbol ...`
  - This allows me to wrap the setter with code that shares a signature like `bindingContext -> config -> config`
  - all the work of casting and parsing is baked in at this time, which allows for the setters to all share a signature
    - probably need to pattern match between `bindingContext.ParseResult.GetValueForOption`, `bindingContext.ParseResult.GetValueForArgument`, and error for other
  - i think i should split off some handler sub-type so there is only one complete value passed to represent all the handler needs
    - It looks like I could still create an ICommandHandler that gets passed, this allows compatibility with techniques like the CommandHandler.Create
    - I could also mostly copy-paste the ordered parameter set methods into a class and return ICommandHandler instead of mutating a command internally
      - i.e. `command.Handler = ParameterBased.Create((p1, p2) => ..., arg1, arg2)`
    - I notice the NamingConventionBinder has it's own handler `ModelBindingCommandHandler` that is based on `delegate.DynamicInvoke`
  - Hander binding models so far
    - Naming Convention -> an existing package
    - Primitive delegate params -> copy of SetHandler overloads
    - PropertyMap -> as described above
  - TODO: comment on the awkward favoritism of SetHandler. It'd feel more consistent if the SetHandler overloads were a ICommandHandler factory like the NamingConventionBinder uses. It's just a different creation strategy.
```fs
//NOTE: the bindings could be a dictionary (either <string, Expr> or <IValueDescriptor,Expr>)
// I know it should be possible to specify setters/paths in a type-agnostic way, but not sure how
let handler = Cli.handler<ConfigType>
[
  binding<'a> "-t"  (fun (config) (value:'a) -> config.Tags = value)
  //ALT: then I add assignment to the expression, I can also pattern match on the type of the field to decide assignment strategy (i.e. append to list)
  binding "-t" <@ (fun config -> config.Tags) @>
]

command "name" handler [
  Cli.argument ["-t"] "description"
]


```
