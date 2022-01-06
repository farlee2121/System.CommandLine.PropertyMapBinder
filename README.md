

## Motivation / what is this
This library is an experiment. The goal is to create an intuitive handler binding experience for [System.CommandLine](https://github.com/dotnet/command-line-api).
A few goals
- intuitive binding of complex types
- support handler declaraction as a self-contained expression (no reference to symbol instances)
- blending multiple binding rules for a customizable and consistent binding experience
- easy extension of the binding pipeline



## Examples

### Pipeline List
Interceptor pattern. Think a list of strategies

### Builder

### Blended Conventions

Show pipeline as list
Show pipeline as builder
show a convention/explicit mix
Show name vs reference
WARN: run in order, later binders will override earlier binders if they operate on the same property(ies)

## How to extend
Section in progress
- IPropertyBinder is the root
- Pipeline builder extensions
- PropetyMap static factories

## Possible extensions to the pipeline
Here are some cases I haven't implemented, but would be fairly easy to add
- map default values from configuration
- Ask a user for any missing inputs 
  - can be done with the existing setter overload, but prompts could be automated with a signature like `.PromptIfMissing(name, selector)`
- match properties based on type
- Set a value directly 
  - can be done with the existing setter overload, but could be simpler `.MapFromValue(c => c.Frequency, 5)`



## Status of project

A successful experiment. Usable, but not production-tested. No guarantees of support


<!-- ## How to Contribute -->