using System;
using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;
using DotnetToolset.Patterns.Dddd.Enums;
using Res = DotnetToolset.Patterns.Resources.Literals;

namespace DotnetToolset.Patterns.Dddd.Implementations.Rules
{
	/// <inheritdoc />
	public class GreaterThanSubjectRule : IRule
	{
		private readonly bool _skipWhenEmpty;

		/// <summary>
		/// Constructor to set the rule value
		/// </summary>
		/// <param name="subjectName">Subject name which value will be evaluated.</param>
		/// <param name="skipWhenEmpty">If true, regular expression is not evaluated when the value is an empty string.</param>
		public GreaterThanSubjectRule(string subjectName, bool skipWhenEmpty = false)
		{
			Rule = new KeyValuePair<RuleType, object>(RuleType.GreaterThanSubject, subjectName);
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
            int castedValue;

            if (value == null)
            {
                return (true, Res.b_TargetSubjectNotPresent);
            }

            // Here, the value must be the subject value to be comparated with.
            if (value.GetType().Name.Contains("Single"))
            {
                float floatValue = (float)value;
                castedValue = Convert.ToInt32(floatValue);
            }
            else
            {
                castedValue = (int)value;
            }

            MinRule minRule = new MinRule(castedValue);
            return minRule.Validate(value);
        }
	}
}
