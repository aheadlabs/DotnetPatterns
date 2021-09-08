using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	/// <summary>
	/// This is generic entity interface.
	/// Common features to all entity types in the layer go here.
	/// </summary>
	/// <typeparam name="TDomainEntity"></typeparam>
	/// <typeparam name="TModelEntity"></typeparam>
	public interface IDomainService<TDomainEntity, TModelEntity, TRuleset>
		where TDomainEntity : class, IDomainEntity
		where TModelEntity : class
		where TRuleset: class, IRuleset<TDomainEntity>

	{
		/// <summary>
		/// Adds the entity to the persistence infrastructure layer
		/// </summary>
		/// <param name="entity">Entity to be added</param>
		/// <returns>Tuple representing success and affected dto</returns>
		Tuple<bool, TDomainEntity> Add(TDomainEntity entity);

		/// <summary>
		/// Adds the entities to the persistence infrastructure layer
		/// </summary>
		/// <param name="entities">List of entities to be added</param>
		/// <returns>List object with the individual results for each entity</returns>
		IList<Tuple<bool, TDomainEntity>> Add(IList<TDomainEntity> entities);

		/// <summary>
		/// Deletes the entity from the persistence infrastructure layer
		/// </summary>
		/// <param name="id">Id of the entity to be deleted</param>
		/// <returns>Id of the deleted entity</returns>
		int Delete(int id);

		/// <summary>
		/// Deletes a list of entities from the persistence infrastructure layer
		/// </summary>
		/// <param name="entities">Entities to be deleted</param>
		/// <returns>>Object with the individual results for each entity</returns>
		IList<Tuple<bool, TDomainEntity>> Delete(IList<TDomainEntity> entities);

		/// <summary>
		/// Edits the entity in the the persistence infrastructure layer
		/// </summary>
		/// <param name="id">Id of the entity to be edited</param>
		/// <param name="entity">Entity to be edited</param>
		/// <returns>Tuple representing success and affected dto</returns>
		Tuple<bool, TDomainEntity> Edit(int id, TDomainEntity entity);

		/// <summary>
		/// Gets the specified entities from the persistence infrastructure layer
		/// </summary>
		/// <param name="predicate">Filter for the entities</param>
		/// <param name="navigationProperties">Navigation properties used in this operation</param>
		/// <returns>List of entities</returns>
		IEnumerable<TDomainEntity> Get(Expression<Func<TModelEntity, bool>> predicate, string[] navigationProperties);

		/// <summary>
		/// Gets the validation rules for this entity
		/// </summary>
		/// <returns>Ruleset</returns>
		public TRuleset GetValidationRules();
	}
}
