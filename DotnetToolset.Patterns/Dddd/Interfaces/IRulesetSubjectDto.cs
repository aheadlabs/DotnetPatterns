using System.Collections.Generic;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// Defines a subject (object property) that has rules
	/// </summary>
	public interface IRulesetSubjectDto : IDto
	{


		/// <summary>
		/// Gets or sets the subject name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Rules for this ruleset subject
		/// </summary>
		public List<IRuleDto> Rules { get; set; }
	}
}
