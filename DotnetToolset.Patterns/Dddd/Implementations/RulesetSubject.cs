using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;

namespace DotnetToolset.Patterns.Dddd.Implementations
{
	public class RulesetSubject : IRulesetSubject
	{
		public string Name { get; set; }

		public List<IRule> Rules { get; set; }
	}
}
