using System.Collections.Generic;
using System.Linq;
using DotnetToolset.Patterns.Dddd.Enums;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// Defines a subject (object property) that has rules
	/// </summary>
	public interface IRulesetSubject
	{


		/// <summary>
		/// Gets or sets the subject name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Rules for this ruleset subject
		/// </summary>
		public List<IRule> Rules { get; set; }

        /// <summary>
        /// Gets all rules from a subject (object property)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IRule> GetRules();

        /// <summary>
        /// Validates all rules for the subject
        /// </summary>
        /// <returns>Boolean that indicates if the rules have passed and IEnumerable with rule, rule passed and optional error message.</returns>
        public (
            bool isValid,
            IEnumerable<(KeyValuePair<RuleType, object>, bool, string)> validRules,
            IEnumerable<(KeyValuePair<RuleType, object>, bool, string)> invalidRules
            ) Validate(object value, object relatedValue = null);
    }
}
