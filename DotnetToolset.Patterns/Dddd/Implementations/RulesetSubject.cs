using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DotnetToolset.Patterns.Dddd.Enums;

namespace DotnetToolset.Patterns.Dddd.Implementations
{
	public class RulesetSubject : IRulesetSubject
    {
		public string Name { get; set; }

		public List<IRule> Rules { get; set; }

        public IEnumerable<IRule> GetRules()
        {
            return Rules.AsEnumerable();
        }

        public (
            bool isValid,
            IEnumerable<(KeyValuePair<RuleType, object>, bool, string)> validRules,
            IEnumerable<(KeyValuePair<RuleType, object>, bool, string)> invalidRules
            ) Validate(object value, object relatedValue = null)
        {
            bool result = true;
            List<(KeyValuePair<RuleType, object> rule, bool result, string errorMessage)> validRules =
                new List<(KeyValuePair<RuleType, object> rule, bool result, string errorMessage)>();
            List<(KeyValuePair<RuleType, object> rule, bool result, string errorMessage)> invalidRules =
                new List<(KeyValuePair<RuleType, object> rule, bool result, string errorMessage)>();

            // Validate each rule for this subject
            foreach (IRule rule in GetRules())
            {
                (bool isValid, string errorMessage) validation = rule.Rule.Key == RuleType.LowerThanSubject || rule.Rule.Key == RuleType.GreaterThanSubject ? rule.Validate(relatedValue) : rule.Validate(value);

                // Add validation result to the list as valid or invalid
                if (validation.isValid)
                {
                    validRules.Add((rule.Rule, true, validation.errorMessage));
                }
                else
                {
                    invalidRules.Add((rule.Rule, false, validation.errorMessage));

                    // Break after first error
                    result = false;
                    break;
                }
            }

            return (result, validRules.AsEnumerable(), invalidRules.AsEnumerable());
        }
	}
}
