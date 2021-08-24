using DotnetToolset.ExtensionMethods;
using DotnetToolset.Patterns.Dddd.Interfaces;
using System;
using System.Collections.Generic;
using Res = DotnetToolset.Patterns.Resources.Literals;

namespace DotnetToolset.Patterns.Dddd.Implementations.Rules
{
	/// <inheritdoc />
	public class LengthRule : IRule
	{
		/// <summary>
		/// Constructor to set the rule value
		/// </summary>
		/// <param name="length"></param>
		public LengthRule(int length)
		{
			Rule = new KeyValuePair<RuleType, object>(RuleType.Length, length);
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
			if (String.IsNullOrEmpty((string)value) || ((string)value).Length <= (int)Rule.Value)
			{
				return (true, null);
			}

			return (false, Res.p_RuleLengthNotPassed.ParseParameters(new[] { value?.ToString(), Rule.Value }));
		}
	}
}
