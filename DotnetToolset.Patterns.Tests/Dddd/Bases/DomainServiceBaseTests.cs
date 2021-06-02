using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DotnetRepository.Services;
using DotnetToolset.Adapters;
using DotnetToolset.Patterns.Dddd.Bases;
using DotnetToolset.Patterns.Tests.Dddd.Fakes;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotnetToolset.Patterns.Tests.Dddd.Bases
{
	public class DomainServiceBaseTests
	{
		#region Private Fields

		private readonly Mock<ITypeAdapter<FakeDomainEntity, FakeEntity>> _domainToModelAdapterMock;
		private readonly Mock<ITypeAdapter<FakeEntity, FakeDomainEntity>> _modelToDomainAdapterMock;
		private readonly Mock<DomainServiceBase<DomainServiceBaseTests, FakeDomainEntity, FakeEntity, DbContextMock>> _sut;
		private readonly Mock<ICrudService<DomainServiceBaseTests, FakeEntity, DbContextMock>> _crudServiceMock;

		#endregion Private Fields

		#region Public Constructors

		public DomainServiceBaseTests()
		{
			// services
			_domainToModelAdapterMock = new Mock<ITypeAdapter<FakeDomainEntity, FakeEntity>>();
			_modelToDomainAdapterMock = new Mock<ITypeAdapter<FakeEntity, FakeDomainEntity>>();
			_crudServiceMock = new Mock<ICrudService<DomainServiceBaseTests, FakeEntity, DbContextMock>>();
			Mock<ILogger<DomainServiceBaseTests>> loggerMock = new Mock<ILogger<DomainServiceBaseTests>>();
			 Mock<DbContextMock> dbContextMock = new Mock<DbContextMock>();

			// system under test   
			_sut = new Mock<DomainServiceBase<DomainServiceBaseTests, FakeDomainEntity, FakeEntity, DbContextMock>>
				(
					loggerMock.Object,
					_crudServiceMock.Object,
					_domainToModelAdapterMock.Object,
					_modelToDomainAdapterMock.Object,
					dbContextMock.Object
				)
			{ CallBase = true };

		}

		#endregion Public Constructors

		#region Public Methods

		#region Get()

		[Fact]
		public void Get_ReturnsIEnumerableDomainEntities()
		{
			// arrange
			_crudServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Returns(new List<FakeEntity> { new(1, null) });
			_modelToDomainAdapterMock.Setup(a => a.Adapt(It.IsAny<FakeEntity>())).Returns(new FakeDomainEntity(1, null));

			// act
			IEnumerable<FakeDomainEntity> result = _sut.Object.Get(null, null);

			// assert
			Assert.IsAssignableFrom<IEnumerable<FakeDomainEntity>>(result);
			Assert.NotNull(result);
		}

		[Fact]
		public void Get_ThrowsBackAndLogsErrorWhenAdaptThrowsException()
		{
			// arrange
			_crudServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Returns(new List<FakeEntity> { new(1, null) });
			_modelToDomainAdapterMock.Setup(a => a.Adapt(It.IsAny<IList<FakeEntity>>())).Throws<Exception>();

			// act & assert
			Assert.Throws<Exception>(() => _sut.Object.Get(null, null));
		}

		[Fact]
		public void Get_ThrowsBackAndLogsErrorWhenBaseGetThrowsException()
		{
			// arrange
			_crudServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Throws<Exception>();
			_modelToDomainAdapterMock.Setup(a => a.Adapt(It.IsAny<IList<FakeEntity>>())).Returns(new List<FakeDomainEntity>());
			It.IsAny<IEnumerable<FakeDomainEntity>>();

			// act & assert
			Assert.Throws<Exception>(() => _sut.Object.Get(null, null));
		}



		#endregion Get()

		#region Add(TDomainEntity entity)

		[Fact]
		public void Add_DomainEntity_ThrowsBackAndLogsWhenAdaptThrowsException()
		{
			// arrange
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Throws<Exception>();
			It.IsAny<bool>();

			// act & assert
			Assert.Throws<Exception>(() => _sut.Object.Add(It.IsAny<FakeDomainEntity>()));
		}

		[Fact]
		public void Add_DomainEntity_ReturnsFalseWhenBaseAddThrowsException()
		{
			// arrange
			FakeEntity fakeEntity = new FakeEntity(1, null);
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Returns(fakeEntity);
			_crudServiceMock.Setup(s => s.Add(It.IsAny<FakeEntity>())).Throws<Exception>();
			Tuple<bool, FakeDomainEntity> result = It.IsAny<Tuple<bool, FakeDomainEntity>>();

			// act
			Assert.Throws<Exception>(() => result = _sut.Object.Add(It.IsAny<FakeDomainEntity>()));

			// assert
			Assert.Null(result);
			_crudServiceMock.Verify(s => s.Add(fakeEntity), Times.Once);
		}

		[Fact]
		public void Add_DomainEntity_ReturnsTrueIfBaseAddSucceeded()
		{
			// arrange
			FakeEntity fakeEntity = new FakeEntity(1, null);
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Returns(fakeEntity);
			Tuple<bool, FakeEntity> expectedResult = Tuple.Create(true, fakeEntity);
			_crudServiceMock.Setup(s => s.Add(fakeEntity)).Returns(expectedResult);

			// act
			Tuple<bool, FakeDomainEntity> result = _sut.Object.Add(It.IsAny<FakeDomainEntity>());

			// assert
			Assert.True(result.Item1);
			_crudServiceMock.Verify(s => s.Add(fakeEntity), Times.Once);
		}

		[Fact]
		public void Add_DomainEntity_ReturnsFalseIfBaseAddDoesNotAddAnyEntity()
		{
			// arrange
			FakeEntity fakeEntity = new FakeEntity(1, null);
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Returns(fakeEntity);
			Tuple<bool, FakeEntity> expectedResult = Tuple.Create(false, fakeEntity);
			_crudServiceMock.Setup(s => s.Add(fakeEntity)).Returns(expectedResult);

			// act
			Tuple<bool, FakeDomainEntity> result = _sut.Object.Add(It.IsAny<FakeDomainEntity>());

			// assert
			Assert.False(result.Item1);
			_crudServiceMock.Verify(s => s.Add(fakeEntity), Times.Once);
		}

		#endregion Add(TDomainEntity dto)

		#region Add(List<TDomainEntity> entity)

		[Fact]
		public void Add_ListDomainEntity_ReturnsDictionaryOfResultsWithEqualNumberOfElements()
		{
			// arrange
			List<FakeDomainEntity> dtos = new List<FakeDomainEntity>
			{
				new(1, new List<FakeEntityRelationed>()),
				new(2, new List<FakeEntityRelationed>())
			};
			Tuple<bool, FakeDomainEntity> result = Tuple.Create(true, It.IsAny<FakeDomainEntity>());
			_sut.Setup(s => s.Add(It.IsAny<FakeDomainEntity>())).Returns(result);

			// act
			IList<Tuple<bool, FakeDomainEntity>> results = _sut.Object.Add(dtos);

			// assert
			Assert.IsAssignableFrom<IEnumerable<Tuple<bool, FakeDomainEntity>>>(results);
			Assert.True(results.Count == dtos.Count);
		}

		[Fact]
		public void Add_ListDomainEntity_ReturnsListOfResultsWithIndividualAddResultsInside()
		{
			// arrange
			List<FakeDomainEntity> entities = new List<FakeDomainEntity>();
			FakeDomainEntity fakeDomainOk = new FakeDomainEntity(1, new List<FakeEntityRelationed>());
			FakeDomainEntity fakeDomainKo = new FakeDomainEntity(2, new List<FakeEntityRelationed>());
			entities.Add(fakeDomainOk);
			entities.Add(fakeDomainKo);
			Tuple<bool, FakeDomainEntity> resultOk = Tuple.Create(true, fakeDomainOk);
			Tuple<bool, FakeDomainEntity> resultKo = Tuple.Create(false, fakeDomainKo);
			_sut.Setup(s => s.Add(fakeDomainOk)).Returns(resultOk);
			_sut.Setup(s => s.Add(fakeDomainKo)).Returns(resultKo);

			// act
			IList<Tuple<bool, FakeDomainEntity>> results = _sut.Object.Add(entities);

			// assert
			Assert.IsAssignableFrom<IEnumerable<Tuple<bool, FakeDomainEntity>>>(results);
			Assert.True(results[0].Item1);
			Assert.False(results[1].Item1);
		}

		[Fact]
		public void Add_ListDomainEntity_ReturnsEmptyListOfResultsWhenNoDtosToAdd()
		{
			// arrange
			List<FakeDomainEntity> dtos = new List<FakeDomainEntity>();

			// act
			IList<Tuple<bool, FakeDomainEntity>> results = _sut.Object.Add(dtos);

			// assert
			Assert.IsAssignableFrom<IEnumerable<Tuple<bool, FakeDomainEntity>>>(results);
			Assert.Empty(results);
		}

		[Fact]
		public void Add_ListDomainEntity_ThrowsBackAndLogsErrorWhenSingleAddThrowsException()
		{

			// arrange
			List<FakeDomainEntity> dtos = new List<FakeDomainEntity>
			{
				new(1, new List<FakeEntityRelationed>()),
				new(2, new List<FakeEntityRelationed>())
			};
			_sut.Setup(s => s.Add(It.IsAny<FakeDomainEntity>())).Throws<Exception>();

			// act & assert
			Assert.Throws<Exception>(() => _sut.Object.Add(dtos));
		}

		#endregion Add(List<TDomainEntity> entity)

		#region Edit(int id, TDomainEntity entity)

		[Fact]
		public void Edit_DomainEntity_ThrowsBackAndLogsWhenAdaptThrowsException()
		{
			// arrange
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Throws<Exception>();

			// act & assert
			Assert.Throws<Exception>(() => _sut.Object.Edit(It.IsAny<int>(), It.IsAny<FakeDomainEntity>()));
		}

		[Fact]
		public void Edit_DomainEntity_ReturnsFalseWhenBaseEditThrowsException()
		{
			// arrange
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Returns(new FakeEntity(1, null));
			_crudServiceMock.Setup(s => s.Edit(It.IsAny<int>(), It.IsAny<FakeEntity>())).Throws<Exception>();
			Tuple<bool, FakeDomainEntity> result = It.IsAny<Tuple<bool, FakeDomainEntity>>();

			// act
			Assert.Throws<Exception>(() => result = _sut.Object.Edit(It.IsAny<int>(), It.IsAny<FakeDomainEntity>()));

			// assert
			Assert.Null(result);
		}

		[Fact]
		public void Edit_DomainEntity_ReturnsTrueIfBaseEditSucceeded()
		{
			// arrange
			const int entityId = 1;
			FakeEntity fakeEntity = new FakeEntity(entityId, null);
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Returns(fakeEntity);
			Tuple<bool, FakeEntity> expectedResult = Tuple.Create(true, fakeEntity);
			_crudServiceMock.Setup(s => s.Edit(entityId, fakeEntity)).Returns(expectedResult);

			// act
			Tuple<bool, FakeDomainEntity> result = _sut.Object.Edit(entityId, It.IsAny<FakeDomainEntity>());

			// assert
			Assert.True(result.Item1);
			_crudServiceMock.Verify(s => s.Edit(entityId, fakeEntity), Times.Once);

		}

		[Fact]
		public void Edit_DomainEntity_ReturnsFalseIfBaseEditDoesNotEditAnyEntity()
		{
			// arrange
			const int entityId = 1;
			FakeEntity fakeEntity = new FakeEntity(entityId, null);
			_domainToModelAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeDomainEntity>())).Returns(fakeEntity);
			Tuple<bool, FakeEntity> expectedResult = Tuple.Create(false, fakeEntity);
			_crudServiceMock.Setup(s => s.Edit(entityId, fakeEntity)).Returns(expectedResult);

			// act
			Tuple<bool, FakeDomainEntity> result = _sut.Object.Edit(entityId, It.IsAny<FakeDomainEntity>());

			// assert
			Assert.False(result.Item1);
			_crudServiceMock.Verify(s => s.Edit(entityId, fakeEntity), Times.Once);
		}

		#endregion Edit(int id, TDomainEntity entity)

		#region Delete(int id)

		[Fact]
		public void Delete_DomainEntity_ReturnsNullWhenBaseDeleteThrowsException()
		{
			// arrange
			_crudServiceMock.Setup(s => s.Delete(It.IsAny<object[]>())).Throws<Exception>();
			int? expectedResult = It.IsAny<int?>();

			// act
			Assert.Throws<Exception>(() => expectedResult = _sut.Object.Delete(It.IsAny<int>()));

			// assert
			Assert.Null(expectedResult);
		}

		[Fact]
		public void Delete_DomainEntity_ReturnsTrueIfBaseDeleteSucceeded()
		{
			// arrange
			const int entityId = 1;
			Tuple<bool, FakeEntity> expectedResult = Tuple.Create(true, It.IsAny<FakeEntity>());
			_crudServiceMock.Setup(s => s.Delete(new object[] { entityId })).Returns(expectedResult);

			// act
			int? result = _sut.Object.Delete(entityId);

			// assert
			Assert.Equal(result, entityId);
			_crudServiceMock.Verify(s => s.Delete(new object[] { entityId }), Times.Once);

		}

		[Fact]
		public void Delete_DomainEntity_ReturnsNullIfBaseDeleteDoesNotDeleteAnyEntity()
		{
			// arrange
			const int entityId = 1;
			Tuple<bool, FakeEntity> expectedResult = Tuple.Create(false, It.IsAny<FakeEntity>());
			_crudServiceMock.Setup(s => s.Delete(new object[] { entityId })).Returns(expectedResult);

			// act
			int? result = _sut.Object.Delete(entityId);

			// assert
            _crudServiceMock.Verify(s => s.Delete(new object[] { entityId }), Times.Once);
			Assert.Equal( 0, result);

		}

		#endregion Delete(int id)

		#region Delete(IList<TDomainEntity> entities)

		[Fact]
		public void Delete_ListDomainEntity_ReturnsDictionaryOfResultsWithEqualNumberOfElements()
		{
			// arrange
			FakeDomainEntity entity1 = new FakeDomainEntity(1, null);
			FakeDomainEntity entity2 = new FakeDomainEntity(2, null);
			List<FakeDomainEntity> entities = new List<FakeDomainEntity>
			{
				entity1,
				entity2
			};
			Tuple<bool, FakeEntity> result = Tuple.Create(true, new FakeEntity(1, null));
			_crudServiceMock.Setup(s => s.Delete(It.IsAny<FakeEntity>())).Returns(result);

			// act
			IList<Tuple<bool, FakeDomainEntity>> results = _sut.Object.Delete(entities);

			// assert
			Assert.IsAssignableFrom<IEnumerable<Tuple<bool, FakeDomainEntity>>>(results);
			Assert.True(results.Count == entities.Count);
		}

		[Fact]
		public void Delete_ListDomainEntity_ReturnsEmptyListOfResultsWhenNoDtosToDelete()
		{
			// arrange
			List<FakeDomainEntity> dtos = new List<FakeDomainEntity>();

			// act
			IList<Tuple<bool, FakeDomainEntity>> results = _sut.Object.Delete(dtos);

			// assert
			Assert.IsAssignableFrom<IEnumerable<Tuple<bool, FakeDomainEntity>>>(results);
			Assert.Empty(results);
		}

		[Fact]
		public void Delete_ListDomainEntity_ThrowsBackAndLogsErrorWhenSingleDeleteThrowsException()
		{

			// arrange
			List<FakeDomainEntity> entities = new List<FakeDomainEntity>
			{
				new(1, new List<FakeEntityRelationed>()),
				new(2, new List<FakeEntityRelationed>())
			};
			_sut.Setup(s => s.Delete(entities)).Throws<Exception>();

			// act & assert
			Assert.Throws<Exception>(() => _sut.Object.Delete(entities));
		}

		#endregion Delete(IList<TDomainEntity> entities)

		#region AddRelated(List<TDomainEntity> entities)

		[Fact]
		public void AddRelated_ListRelatedEntity_ReturnsFalseWhenAtLeastOneOperationIsFalse()
		{
			// arrange
			FakeEntity entity1 = new FakeEntity(1, null);
			FakeEntity entity2 = new FakeEntity(2, null);
			List<FakeEntity> entities = new List<FakeEntity>
			{
				entity1,
				entity2
			};
			Tuple<bool, FakeEntity> result1 = Tuple.Create(true, entity1);
			Tuple<bool, FakeEntity> result2 = Tuple.Create(false, entity2);

			// Setup the crudservice to return false with the entity2
			Mock<ICrudService<DomainServiceBaseTests, FakeEntity, DbContextMock>> crudServiceRelatedMock = new Mock<ICrudService<DomainServiceBaseTests, FakeEntity, DbContextMock>>();
			crudServiceRelatedMock.Setup(c => c.Add(entity1)).Returns(result1);
			crudServiceRelatedMock.Setup(c => c.Add(entity2)).Returns(result2);

			// act
			bool result = _sut.Object.AddRelated(entities);

			// assert
			Assert.False(result);
		}

		#endregion Add(List<TDomainEntity> entity)

		#region DeleteRelated(List<TDomainEntity> entities)

		[Fact]
		public void DeleteRelated_ListRelatedEntity_ReturnsFalseWhenAtLeastOneOperationIsFalse()
		{
			// arrange
			FakeEntity entity1 = new FakeEntity(1, null);
			FakeEntity entity2 = new FakeEntity(2, null);
			Tuple<bool, FakeEntity> result1 = Tuple.Create(true, entity1);
			Tuple<bool, FakeEntity> result2 = Tuple.Create(false, entity2);
			List<FakeEntity> entities = new List<FakeEntity>
			{
				entity1,
				entity2
			};

			// Setup the crudservice to return false with the entity2
			Mock<ICrudService<DomainServiceBaseTests, FakeEntity, DbContextMock>> crudServiceRelatedMock = new Mock<ICrudService<DomainServiceBaseTests, FakeEntity, DbContextMock>>();
			crudServiceRelatedMock.Setup(c => c.Delete(entity1)).Returns(result1);
			crudServiceRelatedMock.Setup(c => c.Delete(entity2)).Returns(result2);

			// act
			bool result = _sut.Object.DeleteRelated(entities);

			// assert
			Assert.False(result);
		}

		#endregion Delete(List<TDomainEntity> entity)

		#endregion Public Methods
	}
}
