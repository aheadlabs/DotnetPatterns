using Microsoft.Extensions.Logging;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// This is generic entity interface.
	/// Common features to all entity types in the layer go here.
	/// </summary>
	public interface IDomainEntity
	{
		/// <summary>
		/// Validates the domain entity from a business logic perspective
		/// </summary>
		/// <returns></returns>
		public bool IsValid(ILogger logger);
	}
}
