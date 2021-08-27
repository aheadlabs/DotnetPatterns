using System.Collections.Generic;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// Defines a rule
	/// </summary>
	public interface IRuleDto : IDto

	{
		/// <summary>
		/// Rule key-value pair
		/// </summary>
		/// <returns></returns>
		public KeyValuePair<string, string> Rule { get; set; }
	}
}
