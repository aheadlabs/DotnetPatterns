using DotnetRepository.Services;
using DotnetToolset.Adapters;
using DotnetToolset.ExtensionMethods;
using DotnetToolset.Patterns.Dddd.Interfaces;
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
		where TDomainEntity : class, IDomainEntity
		where TModelEntity : class
		where TContext : DbContext


	{
		private readonly ITypeAdapter<TDomainEntity, TModelEntity> _domainToModelAdapter;
		private readonly ITypeAdapter<TModelEntity, TDomainEntity> _modelToDomainAdapter;
		private readonly ICrudService<TDomainService, TModelEntity, TContext> _crudService;
		private readonly ILogger<TDomainService> _logger;
		private readonly TContext _context;

        protected DomainServiceBase(ILogger<TDomainService> logger,
			ICrudService<TDomainService, TModelEntity, TContext> crudService,
			ITypeAdapter<TDomainEntity, TModelEntity> domainToModelAdapter,
			ITypeAdapter<TModelEntity, TDomainEntity> modelToDomainAdapter,
			TContext context)
		{
			_logger = logger;
			_crudService = crudService;
			_domainToModelAdapter = domainToModelAdapter;
			_modelToDomainAdapter = modelToDomainAdapter;
			_context = context;
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
				// Validate entity
				if (!entity.IsValid(_logger))
				{
					throw new ArgumentException(Literals.p_EntityNotValid.ParseParameter(entity.GetType().ToString()), nameof(entity));
				}

				// Adapt domain entity to model entity
				TModelEntity modelEntity = _domainToModelAdapter.Adapt(entity);

				// add to database
				Tuple<bool, TModelEntity> result = _crudService.Add(modelEntity);
				return Tuple.Create(result.Item1, _modelToDomainAdapter.Adapt(result.Item2));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Literals.p_ErrorAddingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
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
				_logger.LogError(ex, Literals.p_ErrorAddingDtoListOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
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
				Tuple<bool, TModelEntity> result = _crudService.Delete(new object[] { id });
				return result.Item1 ? id : 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Literals.p_ErrorDeletingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
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
					TModelEntity modelEntity = _domainToModelAdapter.Adapt(entity);
					Tuple<bool, TModelEntity> result = _crudService.Delete(modelEntity);
					results.Add(Tuple.Create(result.Item1, _modelToDomainAdapter.Adapt(result.Item2)));
				}

				return results;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Literals.p_ErrorDeletingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
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
				// Validate entity
				if (!entity.IsValid(_logger))
				{
					throw new ArgumentException(Literals.p_EntityNotValid.ParseParameter(entity.GetType().ToString()), nameof(entity));
				}

				// Adapt domain entity to model entity
				TModelEntity modelEntity = _domainToModelAdapter.Adapt(entity);

				// edit in the database
				Tuple<bool, TModelEntity> result = _crudService.Edit(id, modelEntity);
				return Tuple.Create(result.Item1, _modelToDomainAdapter.Adapt(result.Item2));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Literals.p_ErrorEditingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
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
				IList<TModelEntity> dbEntities = _crudService.Get(predicate, navigationProperties);

				// Adapt the results to domain entities
				IEnumerable<TDomainEntity> domainEntities = _modelToDomainAdapter.Adapt(dbEntities);

				return domainEntities;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, Literals.p_ErrorGettingDtoOfTypeX.ParseParameter(typeof(TDomainEntity).Name));
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
			where TRelatedEntity : class, IDomainEntity
		{

			IList<Tuple<bool, TRelatedEntity>> results = new List<Tuple<bool, TRelatedEntity>>();

			ICrudService<TDomainService, TRelatedEntity, TContext> crudServiceRelated = new CrudService<TDomainService, TRelatedEntity, TContext>(_context, _logger);

			foreach (TRelatedEntity entity in entities)
			{
				// Validate entity
				if (!entity.IsValid(_logger))
				{
					throw new ArgumentException(Literals.p_EntityNotValid.ParseParameter(entity.GetType().ToString()), nameof(entities));
				}

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

			ICrudService<TDomainService, TRelatedEntity, TContext> crudServiceRelated = new CrudService<TDomainService, TRelatedEntity, TContext>(_context, _logger);

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
