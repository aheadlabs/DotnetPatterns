namespace DotnetToolset.Patterns.Dddd.Enums
{
	/// <summary>
	/// Types of rules
	/// </summary>
	public enum RuleType
	{
		/// <summary>
		/// Field must be a currency
		/// </summary>
		Currency,

        /// <summary>
        /// Field must be greater than other specified subject's value
        /// </summary>
        GreaterThanSubject,

        /// <summary>
        /// Field must be lower than other specified subject's value
        /// </summary>
        LowerThanSubject,

        /// <summary>
		/// Field max length
		/// </summary>
		MaxLength,

        /// <summary>
        /// Field min length
        /// </summary>
        MinLength,

		/// <summary>
		/// Field with a minimum value
		/// </summary>
		Min,

		/// <summary>
		/// Field with a maximum value
		/// </summary>
		Max,

		/// <summary>
		/// Field must be an integer
		/// </summary>
		IsInteger,

		/// <summary>
		/// Field must not be null
		/// </summary>
		IsNotNull,

		/// <summary>
		/// Field in a range
		/// </summary>
		Range,

		/// <summary>
		/// Regular expression to match
		/// </summary>
		Regex
	}
}
