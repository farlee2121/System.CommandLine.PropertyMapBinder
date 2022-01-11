
## TODO 

- [ ] Add tests
- [ ] model initialization (handle constructors at least)
- [ ] some of the extension cases
- [ ] Map from value
- [ ] Prompt user if missing
- [ ] Improve naming convention binder
  - [ ] make a separate package
  - [ ] can generalize to convention (not just name) by taking any function `(symbol, MemberInfo) -> bool`
  - [ ] i'll want combinators for the conventions. At least And/Or. 
    - [ ] hmm, this could be tricky. Consider prefix + pascal. Prefix will pass but pascal will fail because the prefix is still there
- [ ] Conside Removing static classes (not very idiomatic to C#, maybe use constructors instead)
  - [ ] Hmm, this could improve operations on the pipeline because different bindings would have different types to be matched on instead of all being FuncPropertyBinder
  - [ ] No harm to the F# experience. I can wrap constructors just as well as static members
- [ ] Consider possible errors getting values
  - `GetValueForHandlerParameter` considers more cases than me
  - https://github.com/dotnet/command-line-api/blob/446e832ecc07dbd7a7183c55793b27cf81e26f0d/src/System.CommandLine/Handler.cs
- [ ] See if I can collapse Argument / Option overloads (can still get type information from the property expression)