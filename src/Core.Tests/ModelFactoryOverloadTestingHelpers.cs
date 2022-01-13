using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace Core.Tests;

public static class CommandExtensions 
{
    public static RootCommand AddOptions(this RootCommand command, IEnumerable<Option> options){
        options.ToList().ForEach(opt => command.AddOption(opt));
        return command;
    }
}