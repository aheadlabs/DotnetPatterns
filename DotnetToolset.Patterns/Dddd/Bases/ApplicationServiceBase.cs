using DotnetToolset.Adapters;
using DotnetToolset.ExtensionMethods;
using DotnetToolset.Patterns.Dddd.Interfaces;
using DotnetToolset.Patterns.Resources;
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
	/// <typeparam name="TApplicationService"></typeparam>
	/// <typeparam name="TDomainEntity"></typeparam>
	/// <typeparam name="TModelEntity"></typeparam>
	/// <typeparam name="TBareDto"></typeparam>
	/// <typeparam name="TUiDto"></typeparam>
	public abstract class ApplicationServiceBase<TApplicationService, TDomainEntity, TModelEntity, TBareDto, TUiDto>
		where TApplicationService : class
		where TDomainEntity : class, IDomainEntity
		where TModelEntity : class
		where TBareDto : class, IDto
		where TUiDto : class, IDto
	{
		protected readonly ITypeAdapter<TBareDto, TDomainEntity> BareDtoToDomainAdapter;
		protected readonly IDomainService<TDomainEntity, TModelEntity> DomainService;
		protected readonly ITypeAdapter<TDomainEntity, TUiDto> DomainToUiDtoAdapter;
		protected readonly ILogger<TApplicationService> Logger;

		protected ApplicationServiceBase(ILogger<TApplicationService> logger,
			ITypeAdapter<TDomainEntity, TUiDto> domainToUiDtoAdapter,
			ITypeAdapter<TBareDto, TDomainEntity> bareDtoToDomainAdapter,
			IDomainService<TDomainEntity, TModelEntity> domainService)
		{
			Logger = logger;
			BareDtoToDomainAdapter = bareDtoToDomainAdapter;
			DomainToUiDtoAdapter = domainToUiDtoAdapter;
			DomainService = domainService;
		}

		#region CRUD

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public virtual TUiDto Add(TBareDto dto)
		{
			try
			{
				// convert to a domain entity
				TDomainEntity domainEntity = BareDtoToDomainAdapter.Adapt(dto);

				// add to database
				Tuple<bool, TDomainEntity> addResult = DomainService.Add(domainEntity);
				return addResult.Item1 ? DomainToUiDtoAdapter.Adapt(addResult.Item2) : null;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorAddingDtoOfTypeX.ParseParameter(typeof(TBareDto).Name));
				throw;
			}
		}

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public virtual IList<TUiDto> Add(IList<TBareDto> dtos)
		{
			try
			{
				List<TUiDto> results = new List<TUiDto>();

				foreach (TBareDto dto in dtos)
				{
					TUiDto result = Add(dto);
					results.Add(result);
				}

				return results;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorAddingDtoListOfTypeX.ParseParameter(typeof(TBareDto).Name));
				throw;
			}
		}

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public virtual IList<TUiDto> Delete(IList<TBareDto> dtos)
		{
			try
			{
				// Adapt dtos to domain entities
				IList<TDomainEntity> domainEntities = BareDtoToDomainAdapter.Adapt(dtos).ToList();
				// delete the entity matching from id in the database
				IList<Tuple<bool, TDomainEntity>> results = DomainService.Delete(domainEntities);

				return results.Select(r => DomainToUiDtoAdapter.Adapt(r.Item2)).ToList();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorDeletingDtoOfTypeX.ParseParameter(typeof(TBareDto).Name));
				throw;
			}
		}

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public virtual int? Delete(int id)
		{
			try
			{
				// delete the entity matching from id in the database
				return DomainService.Delete(id);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorDeletingDtoOfTypeX.ParseParameter(typeof(TBareDto).Name));
				throw;
			}
		}

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public virtual TUiDto Edit(int id, TBareDto dto)
		{
			try
			{
				// convert to a domain entity
				TDomainEntity domainEntity = BareDtoToDomainAdapter.Adapt(dto);

				// edit in the database
				Tuple<bool, TDomainEntity> editResult = DomainService.Edit(id, domainEntity);

				return editResult.Item1 ? DomainToUiDtoAdapter.Adapt(editResult.Item2) : null;

			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorEditingDtoOfTypeX.ParseParameter(typeof(TBareDto).Name));
				throw;
			}
		}

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public virtual IEnumerable<TUiDto> Get(Expression<Func<TModelEntity, bool>> predicate, Type dtoType)
		{
			try
			{
				string[] navigationProperties = GetNavigationProperties(dtoType);
				// get data
				IEnumerable<TDomainEntity> domainEntities = DomainService.Get(predicate, navigationProperties).ToList();

				// convert to DTOs
				IEnumerable<TUiDto> dtos = DomainToUiDtoAdapter.Adapt(domainEntities);

				// convert to DTO list and return
				return dtos;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, Literals.p_ErrorGettingDtoOfTypeX.ParseParameter(typeof(TUiDto).Name));
				throw;
			}
		}

		#endregion CRUD

		///<inheritdoc cref="IApplicationService{TModelEntity,TBareDto,TUiDto}"/>
		public abstract string[] GetNavigationProperties(Type dtoType);
	}
}
