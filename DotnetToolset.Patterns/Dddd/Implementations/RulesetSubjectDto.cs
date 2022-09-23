using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;

namespace DotnetToolset.Patterns.Dddd.Implementations
{
	public class RulesetSubjectDto : IRulesetSubjectDto
	{
		public string Name { get; set; }

		public List<RuleDto> Rules { get; set; }
	}
}
