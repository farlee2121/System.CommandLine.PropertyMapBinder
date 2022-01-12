
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