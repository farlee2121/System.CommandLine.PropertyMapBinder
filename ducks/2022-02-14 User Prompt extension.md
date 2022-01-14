---
date: 2022-01-14
---

## Motivation

I want to demonstrate extension of the binding pipeline with cases users are likely to want.
A fairly common input scenario is to ask users to input values for missing arguments instead of just erroring

The goal isn't to provide a comprehensive solution, just enough to demonstrate the idea.

Common behaviors of user prompts
- keep asking until valid input is provided
- exit on key combo
- have some default answer if none provided

## Exploration

Q: Can I lean on the existing parser?
- I don't want to duplicate any custom parsers or deal with the many edge cases of converting to complex types
- The parser uses a visitor, and it's not clear how to leverage the parsing at a level lower than command.Parse.
I may as well create a one argument command. It's simple and should work
- `ParseArgument` seems to be the type for custom parsing, but doesn't appear to be used internally...
  - https://github.com/dotnet/command-line-api/blob/72e86ec7615c0c8ecb6a13e34a5c0a97e9309909/src/System.CommandLine/Parsing/ParseArgument%7BT%7D.cs
- There's a bit  of a thread to pull on here, where the input gets put into nodes
  - https://github.com/dotnet/command-line-api/blob/72e86ec7615c0c8ecb6a13e34a5c0a97e9309909/src/System.CommandLine/Parsing/ParseOperation.cs#L132
- !!! Symbols surface a `.Parse`
  - I can create a symbol or easily reuse from an existing instance


This is more complicated than expected.
- I need to consider what it means to be missing
  - do empty lists count as missing?
  - Do users need to specify nullable types without defaults enable "missing"? 
    - Or do I need to hook into the parse result and check if it found a value?
- There is also ambiguity on ownership of "missing"
  - It seems to make sense that missing would be based on the state of the model. Values can come from many sources. However, then do I have any association with the input symbol? If I don't, do I require the user to specify the prompt? In general, how will users want to specify the user prompt message?


There are a lot of unknowns. I'm going to table this pending more feedback.



