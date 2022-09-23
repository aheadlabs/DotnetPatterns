using System.Collections.Generic;
using DotnetToolset.Patterns.Dddd.Interfaces;

namespace DotnetToolset.Patterns.Dddd.Implementations
{
    public class RuleDto : IRuleDto
    {
        public KeyValuePair<string, string> Rule { get; set; }
    }
}
