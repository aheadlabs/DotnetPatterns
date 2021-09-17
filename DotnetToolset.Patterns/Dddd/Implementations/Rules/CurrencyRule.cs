using DotnetToolset.ExtensionMethods;
using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;
using DotnetToolset.Patterns.Dddd.Enums;
using Res = DotnetToolset.Patterns.Resources.Literals;

namespace DotnetToolset.Patterns.Dddd.Implementations.Rules
{
	/// <inheritdoc />
	public class CurrencyRule : IRule
	{
		/// <summary>
		/// Constructor to set the rule value
		/// </summary>
        public CurrencyRule()
		{
			Rule = new KeyValuePair<RuleType, object>(RuleType.Currency, null);
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
			if (value != null && (decimal)value >= 0)
			{
				return (true, null);
			}

			return (false, Res.p_RuleCurrencyNotPassed.ParseParameters(new object[] { value?.ToString(), (decimal)value }));
		}
	}
}
