using DotnetRepository.Services;
using DotnetToolset.Adapters;
using DotnetToolset.ExtensionMethods;
using DotnetToolset.Patterns.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotnetToolset.Patterns.Dddd.Bases
{
	/// <summary>
	/// This is the supertype for all services in this domain layer (layer supertype pattern).
	/// Common features to all service types in the layer go here.
	/// </summary>
	public abstract class DomainServiceBase<TDomainService, TDomainEntity, TModelEntity, TContext>

		where TDomainService : class
		where TDomainEntity : class
		where TModelEntity : class
		where TContext : DbContext


	{
		protected readonly ITypeAdapter<TDomainEntity, TModelEntity> DomainToModelAdapter;
        protected readonly ITypeAdapter<TModelEntity, TDomainEntity> ModelToDomainAdapter;
        protected readonly ICrudService<TDomainService, TModelEntity, TContext> CrudService;
        protected readonly ILogger<TDomainService> Logger;
        protected readonly TContext Context;

        protected DomainServiceBase(ILogger<TDomainService> logger,
			ICrudService<TDomainService, TModelEntity, TContext> crudService,
			ITypeAdapter<TDomainEntity, TModelEntity> domainToModelAdapter,
			ITypeAdapter<TModelEntity, TDomainEntity> modelToDomainAdapter,
			TContext context)
		{
			Logger = logger;
			CrudService = crudService;
			DomainToModelAdapter = domainToModelAdapter;
			ModelToDomainAdapter = modelToDomainAdapter;
			Context = context;
		}

		#region CRUD

		/// <summary>
		/// Adds the entity to the persistence infrastructure layer
		/// </summary>
		/// <param name="entity">Entity to be added</param>
		/// <returns>Tuple representing success and affected dto</returns>
		public virtual Tuple<bool, TDomainEntity> Add(TDomainEntity entity)
		{
			try
			{
				// Adapt domain entity to model entity
				TModelEntity modelEntity = DomainToModelAdapter.Adapt(entity);

				// add to database
				Tuple<bool, TModelEntity> result = CrudService.Add(modelEntity);
				return Tuple.Create(result.Item1, ModelToDomainAdapter.Adapt(result.Item2));
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorAddingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
				throw;
			}
		}

		/// <summary>
		/// Adds the entities to the persistence infrastructure layer
		/// </summary>
		/// <param name="entities">List of entities to be added</param>
		/// <returns>List object with the individual results for each entity</returns>
		public virtual IList<Tuple<bool, TDomainEntity>> Add(IList<TDomainEntity> entities)
		{
			try
			{
				List<Tuple<bool, TDomainEntity>> results = new List<Tuple<bool, TDomainEntity>>();

				foreach (TDomainEntity domainEntity in entities)
				{
					Tuple<bool, TDomainEntity> result = Add(domainEntity);
					results.Add(result);
				}

				return results;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorAddingDtoListOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
				throw;
			}
		}

		/// <summary>
		/// Deletes the entity from the persistence infrastructure layer
		/// </summary>
		/// <param name="id">Id of the entity to be deleted</param>
		/// <returns>Id of the deleted item, 0 otherwise</returns>
		public virtual int Delete(int id)
		{
			try
			{
				// delete the entity matching from id in the database
				Tuple<bool, TModelEntity> result = CrudService.Delete(new object[] { id });
				return result.Item1 ? id : 0;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorDeletingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
				throw;
			}
		}

		/// <summary>
		/// Deletes the entity from the persistence infrastructure layer
		/// </summary>
		/// <param name="entities">Id of the entity to be deleted</param>
		/// <returns>List object with the individual results for each id</returns>
		public virtual IList<Tuple<bool, TDomainEntity>> Delete(IList<TDomainEntity> entities)
		{
			try
			{
				List<Tuple<bool, TDomainEntity>> results = new List<Tuple<bool, TDomainEntity>>();

				// delete the entity matching from id in the database
				foreach (TDomainEntity entity in entities)
				{
					TModelEntity modelEntity = DomainToModelAdapter.Adapt(entity);
					Tuple<bool, TModelEntity> result = CrudService.Delete(modelEntity);
					results.Add(Tuple.Create(result.Item1, ModelToDomainAdapter.Adapt(result.Item2)));
				}

				return results;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorDeletingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
				throw;
			}
		}

		/// <summary>
		/// Edits the entity in the the persistence infrastructure layer
		/// </summary>
		/// <param name="id">Id of the entity to be edited</param>
		/// <param name="entity">Entity to be edited</param>
		/// <returns>Tuple representing success and affected dto</returns>
		public virtual Tuple<bool, TDomainEntity> Edit(int id, TDomainEntity entity)
		{
			try
			{
				// Adapt domain entity to model entity
				TModelEntity modelEntity = DomainToModelAdapter.Adapt(entity);

				// edit in the database
				Tuple<bool, TModelEntity> result = CrudService.Edit(id, modelEntity);
				return Tuple.Create(result.Item1, ModelToDomainAdapter.Adapt(result.Item2));
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorEditingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
				throw;
			}
		}

		/// <summary>
		/// Gets the specified entities from the persistence infrastructure layer
		/// </summary>
		/// <param name="predicate">Filter for the entities</param>
		/// <param name="navigationProperties">Navigation properties used in this operation</param>
		/// <returns>List of entities</returns>
		public virtual IEnumerable<TDomainEntity> Get(Expression<Func<TModelEntity, bool>> predicate, string[] navigationProperties)
		{
			try
			{

				// get data
				IList<TModelEntity> dbEntities = CrudService.Get(predicate, navigationProperties);

				// Adapt the results to domain entities
				IEnumerable<TDomainEntity> domainEntities = ModelToDomainAdapter.Adapt(dbEntities);

				return domainEntities;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorGettingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
				throw;
			}
		}

		#endregion CRUD

		#region CRUD to related entities

		/// <summary>
		/// Adds related entities to the join table
		/// </summary>
		/// <param name="entities">Entities to be added to the join table</param>
		/// <returns>>True if operation persisted correctly</returns>
		public bool AddRelated<TRelatedEntity>(IList<TRelatedEntity> entities)
			where TRelatedEntity : class
		{

			IList<Tuple<bool, TRelatedEntity>> results = new List<Tuple<bool, TRelatedEntity>>();

			ICrudService<TDomainService, TRelatedEntity, TContext> crudServiceRelated = new CrudService<TDomainService, TRelatedEntity, TContext>(Context, Logger);

			foreach (TRelatedEntity entity in entities)
			{
				Tuple<bool, TRelatedEntity> result = crudServiceRelated.Add(entity);
				results.Add(result);
			}

			// Returning true if all results of internal operations are true
			return results.All(r => r.Item1);
		}

		/// <summary>
		/// Deletes related entities from the join table
		/// </summary>
		/// <param name="entities">Entities to be deleted to the join table</param>
		/// <returns>>True if operation persisted correctly</returns>
		public bool DeleteRelated<TRelatedEntity>(IList<TRelatedEntity> entities)
			where TRelatedEntity : class
		{

			IList<Tuple<bool, TRelatedEntity>> results = new List<Tuple<bool, TRelatedEntity>>();

			ICrudService<TDomainService, TRelatedEntity, TContext> crudServiceRelated = new CrudService<TDomainService, TRelatedEntity, TContext>(Context, Logger);

			foreach (TRelatedEntity entity in entities)
			{
				Tuple<bool, TRelatedEntity> result = crudServiceRelated.Delete(entity);
				results.Add(result);
			}

			// Returning true if all results of internal operations are true
			return results.All(r => r.Item1);
		}

		#endregion CRUD to related entities
	}
}
