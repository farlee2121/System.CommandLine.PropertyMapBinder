

Q: How do I create a handler at the end of the chain of binders / property maps?
- I think I might be able to get away using AnonymousCommandHandler from the project and withing the scope passing the binding context to a pipeline of delegates
- I think this is possible, the properties only need the context, they don't need to create a handler. And the end handler only needs to know about the aggregated config type, not the property types


Add alias is part of IdentifierSymbol https://github.com/dotnet/command-line-api/blob/b805d096b6caabbabc321ff95425a6bf0de45d90/src/System.CommandLine/IdentifierSymbol.cs#L68
- All children should have an Aliases collection
- Name seems to always be listed as an alias