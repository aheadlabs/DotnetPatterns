namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// Types of rules
	/// </summary>
	public enum RuleType
	{
		/// <summary>
		/// Filed must be a currency
		/// </summary>
		Currency,

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
		/// Filed must be an integer
		/// </summary>
		IsInteger,

		/// <summary>
		/// Filed must not be null
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
