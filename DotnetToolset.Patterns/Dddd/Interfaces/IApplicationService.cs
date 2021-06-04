using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotnetToolset.Patterns.Dddd.Interfaces
{
	public interface IApplicationService<TModelEntity, TBareDto, TUiDto> 
        where TModelEntity : class
        where TBareDto : class, IDto
        where TUiDto : class, IDto
    {
        /// <summary>
        /// Adds the entity DTO to the persistence infrastructure layer
        /// </summary>
        /// <param name="dto">Entity DTO to be added</param>
        /// <returns>Affected dto</returns>
        TUiDto Add(TBareDto dto);

        /// <summary>
        /// Adds DTOs to the persistence infrastructure layer
        /// </summary>
        /// <param name="dtos">List of DTOs to be added</param>
        /// <returns>List with the individual results for each dto</returns>
        IList<TUiDto> Add(IList<TBareDto> dtos);

        /// <summary>
        /// Deletes the entity DTO from the persistence infrastructure layer
        /// </summary>
        /// <param name="id">Id of the entity DTO to be deleted</param>
        /// <returns>Id of the entity deleted</returns>
        int Delete(int id);

        /// <summary>
        /// Deletes the DTOs from the persistence infrastructure layer
        /// </summary>
        /// <param name="dtos">DTOs to be deleted</param>
        /// <returns>List object with the individual results for each entity</returns>
        IList<TUiDto> Delete(IList<TBareDto> dtos);

        /// <summary>
        /// Edits the entity DTO in the the persistence infrastructure layer
        /// </summary>
        /// <param name="id">Id of the entity DTO to be edited</param>
        /// <param name="dto">Entity DTO to be edited</param>
        /// <returns>Affected dto</returns>
        TUiDto Edit(int id, TBareDto dto);

        /// <summary>
        /// Gets the specified entity DTOs from the persistence infrastructure layer
        /// </summary>
        /// <param name="predicate">Filter for the entity DTOs</param>
        /// <param name="dtoType">DTO type to be used to parse navigation properties</param>
        /// <returns>List of entity DTOs</returns>
        IEnumerable<TUiDto> Get(Expression<Func<TModelEntity, bool>> predicate, Type dtoType);

        /// <summary>
        /// Gets the navigation properties array for the specified DTO type
        /// </summary>
        /// <param name="dtoType">DTO type for getting the navigation properties</param>
        /// <returns>String array of navigation properties</returns>
        string[] GetNavigationProperties(Type dtoType);

    }
}
