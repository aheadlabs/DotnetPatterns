using System.Collections.Generic;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// Defines a rule
	/// </summary>
	public interface IRule
	{
		/// <summary>
		/// Rule key-value pair
		/// </summary>
		/// <returns></returns>
		public KeyValuePair<RuleType, object> Rule { get; set; }
		
		/// <summary>
		/// Validates the rule
		/// </summary>
		/// <returns>Boolean that indicates if the rule has passed and an optional error message.</returns>
		public (bool isValid, string errorMessage) Validate(object value);
	}
}
