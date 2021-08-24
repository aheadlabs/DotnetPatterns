using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// Defines a ruleset
	/// </summary>
	public interface IRuleset<out T>
	{
		/// <summary>
		/// Instance of the type to be validated
		/// </summary>
		public T Instance { get; }

		/// <summary>
		/// Ruleset subjects for this ruleset
		/// </summary>
		public List<IRulesetSubject> RulesetSubjects { get; }

		/// <summary>
		/// Gets all subjects (object properties) from a ruleset
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IRulesetSubject> GetRulesetSubjects()
		{
			return RulesetSubjects.AsEnumerable();
		}

		/// <summary>
		/// Validates ruleset (all rules for every subject)
		/// </summary>
		/// <returns>Validation result and validation result for every subject.</returns>
		public (
			bool result,
			IEnumerable<(
				string name,
				bool isValid,
				IEnumerable<(KeyValuePair<RuleType, object> rule, bool isValid, string errorMessageg)> validRules,
				IEnumerable<(KeyValuePair<RuleType, object> rule, bool isValid, string errorMessage)> invalidRules
				)> rulesetSubjects
			) Validate(ILogger logger)
		{
			bool result = true;
			List<(
				string name,
				bool isValid,
				IEnumerable<(KeyValuePair<RuleType, object> rule, bool isValid, string errorMessage)> validRules,
				IEnumerable<(KeyValuePair<RuleType, object> rule, bool isValid, string errorMessage)> invalidRules
				)> subjects =
				new List<(string name, bool isValid,
					IEnumerable<(KeyValuePair<RuleType, object> rule, bool isValid, string errorMessage)> validRules,
					IEnumerable<(KeyValuePair<RuleType, object> rule, bool isValid, string errorMessage)> invalidRules
					)>();

			PropertyInfo[] properties = Instance.GetType().GetProperties();

			// Validate each subject of this ruleset
			foreach (IRulesetSubject rulesetSubject in GetRulesetSubjects())
			{
				// Validate subject
				object valueToValidate = properties.Single(p => p.Name == rulesetSubject.Name).GetValue(Instance);
				(
					bool isValid,
					IEnumerable<(KeyValuePair<RuleType, object> rule, bool result, string errorMessage)> subjectValidRules,
					IEnumerable<(KeyValuePair<RuleType, object> rule, bool result, string errorMessage)> subjectInvalidRules
				) subjectValidation = rulesetSubject.Validate(valueToValidate);

				// Add validation result to the list as valid or invalid
				subjects.Add((rulesetSubject.Name, subjectValidation.isValid, subjectValidation.subjectValidRules, subjectValidation.subjectInvalidRules));

				if (!subjectValidation.isValid)
				{
					result = false;

					// Log errors
					foreach ((KeyValuePair<RuleType, object> rule, bool result, string errorMessage) rule in subjectValidation.subjectInvalidRules)
					{
						logger.LogError(rule.errorMessage);
					}

					// Break after first subject with validation errors
					break;
				}
			}

			return (result, subjects);
		}
	}
}
