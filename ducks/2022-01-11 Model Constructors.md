
## Motivation

My binder pipeline currently requires a model type with a default constructor.
This is a bit of a rude assumption. Some types (e.g. positional records) may not support a default constructor.
The consumer may also want to enforce invariants on the class through the constructor (i.e. minimum required fields).

Overall, there are too many usecases for me to enforce a default constructor.

## Requirements and Constraints
REQ: Allow mapping to types that have no default constructor
REQ: Allow mapping to types that have a default constructor without specifying a constructor/instance


## Exploration

Q: Can constru