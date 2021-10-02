using DotnetToolset.ExtensionMethods;
using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DotnetToolset.Patterns.Dddd.Enums;
using Res = DotnetToolset.Patterns.Resources.Literals;

namespace DotnetToolset.Patterns.Dddd.Implementations.Rules
{
	/// <inheritdoc />
	public class RegexRule : IRule
	{
		private readonly bool _skipWhenEmpty;

		/// <summary>
		/// Constructor to set the rule value
		/// </summary>
		/// <param name="regex">Regular expression to be evaluated.</param>
		/// <param name="skipWhenEmpty">If true, regular expression is not evaluated when the value is an empty string.</param>
		public RegexRule(string regex, bool skipWhenEmpty = false)
		{
			Rule = new KeyValuePair<RuleType, object>(RuleType.Regex, regex);
			_skipWhenEmpty = skipWhenEmpty;
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
            if (_skipWhenEmpty &&  string.IsNullOrEmpty((string)value))
            {
                return (true, Res.p_RuleRegexSkipped.ParseParameter("null"));
            }

			Regex regex = new Regex((string)Rule.Value);
			Match match = regex.Match((string)value);

            if (match.Success)
			{
				return (true, null);
			}

			return (false, Res.p_RuleRegexNotPassed.ParseParameters(new[] { value.ToString(), Rule.Value }));
		}
	}
}
