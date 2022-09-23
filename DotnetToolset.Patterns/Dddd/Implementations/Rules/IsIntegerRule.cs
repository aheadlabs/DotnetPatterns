using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;
using DotnetToolset.Patterns.Dddd.Enums;
using Res = DotnetToolset.Patterns.Resources.Literals;
using DotnetToolset.ExtensionMethods;

namespace DotnetToolset.Patterns.Dddd.Implementations.Rules
{
	/// <inheritdoc />
	public class IsIntegerRule : IRule
	{
		/// <summary>
		/// Constructor to set the rule value
		/// </summary>
		public IsIntegerRule()
		{
			Rule = new KeyValuePair<RuleType, object>(RuleType.IsInteger, null);
		}

		/// <summary>
		/// Rule
		/// </summary>
		public KeyValuePair<RuleType, object> Rule { get; set; }

		/// <summary>
		/// Validates data passed using the rule logic
		/// </summary>
		/// <param name="value">Data to be validated</param>
		/// <returns>True if rule passed and an optional error message</returns>
		public (bool isValid, string errorMessage) Validate(object value)
		{
            if (value == null)
            {
                return (true, Res.b_NoValueToValidate);
            }

			if (int.TryParse(value.ToString(), out _))
			{
				return (true, null);
			}

			return (false, Res.p_RuleIsIntegerNotPassed.ParseParameters(new[] { value.ToString(), Rule.Value }));
		}
	}
}
