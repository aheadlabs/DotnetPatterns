using DotnetToolset.Patterns.Dddd.Interfaces;
using System.Collections.Generic;

namespace DotnetToolset.Patterns.Dddd.Implementations.Rules
{
    /// <inheritdoc />
    /// <inheritdoc />
    public class LowerThanSubjectRule : IRule
    {
        private readonly bool _skipWhenEmpty;

        /// <summary>
        /// Constructor to set the rule value
        /// </summary>
        /// <param name="subjectName">Subject name which value will be evaluated.</param>
        /// <param name="skipWhenEmpty">If true, regular expression is not evaluated when the value is an empty string.</param>
        public LowerThanSubjectRule(string subjectName, bool skipWhenEmpty = false)
        {
            Rule = new KeyValuePair<RuleType, object>(RuleType.Regex, subjectName);
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
            // Here, the value must be the subject value to be comparated with.
            MaxRule maxRule = new MaxRule((int)value);
            return maxRule.Validate(value);
        }
    }
}
