//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using DotnetToolset.Adapters;
//using DotnetToolset.Patterns.Dddd.Bases;
//using DotnetToolset.Patterns.Dddd.Interfaces;
//using DotnetToolset.Patterns.Tests.Dddd.Fakes;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;

//namespace DotnetToolset.Patterns.Tests.Dddd.Bases
//{
//    public class ApplicationServiceBaseTests
//    {
//        #region Private Fields

//        private readonly Mock<ITypeAdapter<FakeBareDto, FakeDomainEntity>> _bareDtoToDomainAdapterMock;
//        private readonly Mock<ITypeAdapter<FakeDomainEntity, FakeUiDto>> _domainToUiDtoListAdapterMock;
//        private readonly Mock<IDomainService<FakeDomainEntity, FakeEntity>> _domainServiceMock;
//        private readonly Mock<ApplicationServiceBase<ApplicationServiceBaseTests, FakeDomainEntity, FakeEntity, FakeBareDto, FakeUiDto>> _sut;

//        #endregion Private Fields

//        #region Public Constructors

//        public ApplicationServiceBaseTests()
//        {
//            // services
//            _bareDtoToDomainAdapterMock = new Mock<ITypeAdapter<FakeBareDto, FakeDomainEntity>>();
//            _domainToUiDtoListAdapterMock = new Mock<ITypeAdapter<FakeDomainEntity, FakeUiDto>>();
//            Mock<ILogger<ApplicationServiceBaseTests>> loggerMock = new Mock<ILogger<ApplicationServiceBaseTests>>();
//            _domainServiceMock = new Mock<IDomainService<FakeDomainEntity, FakeEntity>>();

//            // system under test   
//            _sut = new Mock<ApplicationServiceBase<ApplicationServiceBaseTests, FakeDomainEntity, FakeEntity, FakeBareDto, FakeUiDto>>
//                (
//                    loggerMock.Object,
//                    _domainToUiDtoListAdapterMock.Object,
//                    _bareDtoToDomainAdapterMock.Object,
//                    _domainServiceMock.Object
//                )
//            { CallBase = true };

//        }

//        #endregion Public Constructors

//        #region Public Methods

//        #region Get()

//        [Fact]
//        public void Get_ReturnsIEnumerableTUiDto()
//        {
//            // arrange
//            _domainServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Returns(new List<FakeDomainEntity> { new FakeDomainEntity(1, null) });
//            _domainToUiDtoListAdapterMock.Setup(a => a.Adapt(It.IsAny<IList<FakeDomainEntity>>())).Returns(new List<FakeUiDto>());

//            // act
//            IEnumerable<FakeUiDto> result = _sut.Object.Get(null, typeof(FakeUiDto));

//            // assert
//            Assert.IsAssignableFrom<IEnumerable<FakeUiDto>>(result);
//            Assert.NotNull(result);
//        }

//        [Fact]
//        public void Get_ThrowsBackAndLogsErrorWhenAdaptThrowsException()
//        {
//            // arrange
//            _domainServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Returns(new List<FakeDomainEntity> { new FakeDomainEntity(1, null) });
//            _domainToUiDtoListAdapterMock.Setup(a => a.Adapt(It.IsAny<IList<FakeDomainEntity>>())).Throws<Exception>();

//            // act & assert
//            Assert.Throws<Exception>(() => _sut.Object.Get(null, typeof(FakeUiDto)));
//        }

//        [Fact]
//        public void Get_ThrowsBackAndLogsErrorWhenBaseGetThrowsException()
//        {
//            // arrange
//            _domainServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Throws<Exception>();
//            _domainToUiDtoListAdapterMock.Setup(a => a.Adapt(It.IsAny<IList<FakeDomainEntity>>())).Returns(new List<FakeUiDto>());
//            It.IsAny<IEnumerable<IDto>>();

//            // act & assert
//            Assert.Throws<Exception>(() => _sut.Object.Get(null, typeof(FakeUiDto)));
//        }

//        [Fact]
//        public void Get_ThrowsBackAndLogsErrorWhenGetNavigationPropertiesThrowsException()
//        {
//            // arrange
//            _domainServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<FakeEntity, bool>>>(), It.IsAny<string[]>())).Returns(new List<FakeDomainEntity> { new FakeDomainEntity(1, null) });
//            _sut.Setup(s => s.GetNavigationProperties(It.IsAny<Type>())).Throws<Exception>();
//            _domainToUiDtoListAdapterMock.Setup(a => a.Adapt(It.IsAny<IList<FakeDomainEntity>>())).Returns(new List<FakeUiDto>());
//            It.IsAny<IEnumerable<IDto>>();

//            // act & assert
//            Assert.Throws<Exception>(() => _sut.Object.Get(null, typeof(FakeUiDto)));
//        }

//        #endregion Get()

//        #region Add(TBareDto dto)

//        [Fact]
//        public void Add_BareDto_ThrowsBackWhenAdaptThrowsException()
//        {
//            // arrange
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Throws<Exception>();
//            It.IsAny<bool>();

//            // act & assert
//            Assert.Throws<Exception>(() => _sut.Object.Add(It.IsAny<FakeBareDto>()));
//        }

//        [Fact]
//        public void Add_BareDto_ThrowsBackWhenWhenBaseAddThrowsException()
//        {
//            // arrange
//            FakeDomainEntity fakeDomainEntity = new FakeDomainEntity(1, null);
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Returns(fakeDomainEntity);
//            _domainServiceMock.Setup(s => s.Add(fakeDomainEntity)).Throws<Exception>();
//            It.IsAny<bool>();

//            // act
//            Assert.Throws<Exception>(() => _sut.Object.Add(It.IsAny<FakeBareDto>()));

//            // assert
//            _domainServiceMock.Verify(s => s.Add(fakeDomainEntity), Times.Once);
//        }

//        [Fact]
//        public void Add_BareDto_ReturnsBaseAdd()
//        {
//            // arrange
//            FakeDomainEntity fakeDomainEntity = new FakeDomainEntity(1, null);
//            Tuple<bool, FakeDomainEntity> domainResult = Tuple.Create(true, fakeDomainEntity);
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Returns(fakeDomainEntity);
//            _domainServiceMock.Setup(s => s.Add(fakeDomainEntity)).Returns(domainResult);

//            // act
//            _sut.Object.Add(It.IsAny<FakeBareDto>());

//            // assert
//            _domainServiceMock.Verify(s => s.Add(fakeDomainEntity), Times.Once);
//        }

//        #endregion Add(TBareDto dto)

//        #region Add(List<TBareDto> dto)

//        [Fact]
//        public void Add_ListBareDto_ReturnsListOfResultsWithIndividualAddResultsInside()
//        {
//            // arrange
//            List<FakeBareDto> dtos = new List<FakeBareDto>();
//            FakeBareDto fakeBareDtoOk = new FakeBareDto(DateTime.Now);
//            FakeBareDto fakeBareDtoKo = new FakeBareDto(DateTime.Now);
//            dtos.Add(fakeBareDtoOk);
//            dtos.Add(fakeBareDtoKo);
//            FakeUiDto fakeUiDtoOk = new FakeUiDto();
//            FakeUiDto fakeUiDtoKo = new FakeUiDto();
//            _sut.Setup(s => s.Add(fakeBareDtoOk)).Returns(fakeUiDtoOk);
//            _sut.Setup(s => s.Add(fakeBareDtoKo)).Returns(fakeUiDtoKo);

//            // act
//            IList<FakeUiDto> results = _sut.Object.Add(dtos);

//            // assert
//            Assert.Equal(results.Count, dtos.Count);
//        }

//        [Fact]
//        public void Add_ListBareDto_ReturnsEmptyListOfResultsWhenNoDtosToAdd()
//        {
//            // arrange
//            List<FakeBareDto> dtos = new List<FakeBareDto>();

//            // act
//            IList<FakeUiDto> results = _sut.Object.Add(dtos);

//            // assert
//            Assert.Empty(results);
//        }

//        [Fact]
//        public void Add_ListBareDto_ThrowsBackAndLogsErrorWhenSingleAddThrowsException()
//        {

//            // arrange
//            List<FakeBareDto> dtos = new List<FakeBareDto>
//            {
//                new FakeBareDto(DateTime.Now),
//                new FakeBareDto(DateTime.Now)
//            };
//            _sut.Setup(s => s.Add(It.IsAny<FakeBareDto>())).Throws<Exception>();

//            // act & assert
//            Assert.Throws<Exception>(() => _sut.Object.Add(dtos));
//        }

//        #endregion Add(List<IDto> dto)

//        #region Edit(int id, IDto dto)

//        [Fact]
//        public void Edit_BareDto_ThrowsBackAndLogsWhenAdaptThrowsException()
//        {
//            // arrange
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Throws<Exception>();

//            // act & assert
//            Assert.Throws<Exception>(() => _sut.Object.Edit(It.IsAny<int>(), It.IsAny<FakeBareDto>()));
//        }

//        [Fact]
//        public void Edit_BareDto_ReturnsNullWhenBaseEditThrowsException()
//        {
//            // arrange
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Returns(new FakeDomainEntity(1, null));
//            _domainServiceMock.Setup(s => s.Edit(It.IsAny<int>(), It.IsAny<FakeDomainEntity>())).Throws<Exception>();
//            FakeUiDto result = It.IsAny<FakeUiDto>();

//            // act
//            Assert.Throws<Exception>(() => result = _sut.Object.Edit(It.IsAny<int>(), It.IsAny<FakeBareDto>()));

//            // assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public void Edit_BareDto_ReturnsUiDtoIfBaseEditSucceeded()
//        {
//            // arrange
//            const int entityId = 1;
//            FakeDomainEntity fakeDomainEntity = new FakeDomainEntity(entityId, null);
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Returns(fakeDomainEntity);
//            Tuple<bool, FakeDomainEntity> domainEditResult = Tuple.Create(true, fakeDomainEntity);
//            _domainServiceMock.Setup(s => s.Edit(entityId, fakeDomainEntity)).Returns(domainEditResult);

//            // act
//            _sut.Object.Edit(entityId, It.IsAny<FakeBareDto>());

//            // assert
//            _domainServiceMock.Verify(s => s.Edit(entityId, fakeDomainEntity), Times.Once);

//        }

//        [Fact]
//        public void Edit_BareDto_ReturnsNullIfBaseEditDoesNotEditAnyEntity()
//        {
//            // arrange
//            const int entityId = 1;
//            FakeDomainEntity fakeDomainEntity = new FakeDomainEntity(entityId, null);
//            _bareDtoToDomainAdapterMock.Setup(s => s.Adapt(It.IsAny<FakeBareDto>())).Returns(fakeDomainEntity);
//            Tuple<bool, FakeDomainEntity> domainEditResult = Tuple.Create(false, fakeDomainEntity);
//            _domainServiceMock.Setup(s => s.Edit(entityId, fakeDomainEntity)).Returns(domainEditResult);

//            // act
//            FakeUiDto result = _sut.Object.Edit(entityId, It.IsAny<FakeBareDto>());

//            // assert
//            Assert.Null(result);
//            _domainServiceMock.Verify(s => s.Edit(entityId, fakeDomainEntity), Times.Once);
//        }

//        #endregion Edit(int id, IDto dto)

//        #region Delete(int id)

//        [Fact]
//        public void Delete_BareDto_ReturnsNullWhenBaseDeleteThrowsException()
//        {
//            // arrange
//            _domainServiceMock.Setup(s => s.Delete(It.IsAny<int>())).Throws<Exception>();
//            int? result = It.IsAny<int?>();

//            // act
//            Assert.Throws<Exception>(() => result = _sut.Object.Delete(It.IsAny<int>()));

//            // assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public void Delete_BareDto_ReturnsIdIfBaseDeleteSucceeded()
//        {
//            // arrange
//            const int entityId = 1;
//            _domainServiceMock.Setup(s => s.Delete(entityId)).Returns(entityId);

//            // act
//            int? result = _sut.Object.Delete(entityId);

//            // assert
//            Assert.Equal(result, entityId);
//            _domainServiceMock.Verify(s => s.Delete(entityId), Times.Once);

//        }

//        [Fact]
//        public void Delete_BareDto_ReturnsZeroIfBaseDeleteDoesNotDeleteAnyEntity()
//        {
//            // arrange
//            const int entityId = 1;
//            _domainServiceMock.Setup(s => s.Delete(entityId)).Returns(entityId);

//            // act
//            int? result = _sut.Object.Delete(2);

//            // assert
//            Assert.Equal(0, result);
//        }

//        #endregion Delete(int id)

//        #region Delete (dtos)

//        [Fact]
//        public void Delete_GivenDtosWhenCallsDomainsDeleteThenReturnsListOfUiDtos()
//        {
//            // Arrange
//            IList<FakeBareDto> dtos = new List<FakeBareDto>
//            {
//                new FakeBareDto(DateTime.Now),
//                new FakeBareDto(DateTime.Now)
//            };
//            FakeDomainEntity entity1 = new FakeDomainEntity(1, null);
//            FakeDomainEntity entity2 = new FakeDomainEntity(2, null);
//            IList<FakeDomainEntity> domainEntities = new List<FakeDomainEntity> {
//                entity1,
//                entity2
//            };

//            IList<Tuple<bool, FakeDomainEntity>> expectedResults = new List<Tuple<bool, FakeDomainEntity>>
//            {
//                Tuple.Create(true, entity1),
//                Tuple.Create(true, entity2)
//            };

//            _bareDtoToDomainAdapterMock.Setup(b => b.Adapt(dtos)).Returns(domainEntities);
//            _domainServiceMock.Setup(d => d.Delete(domainEntities)).Returns(expectedResults);

//            // Act
//            IList<FakeUiDto> result = _sut.Object.Delete(dtos);

//            // Assert
//            Assert.Equal(result.Count, dtos.Count);
//        }

//        [Fact]
//        public void Delete_GivenDtosWhenExceptionThenLogsAndThrows()
//        {
//            // Arrange
//            IList<FakeBareDto> dtos = new List<FakeBareDto>
//            {
//                new FakeBareDto(DateTime.Now),
//                new FakeBareDto(DateTime.Now)
//            };
//            FakeDomainEntity entity1 = new FakeDomainEntity(1, null);
//            FakeDomainEntity entity2 = new FakeDomainEntity(2, null);
//            IList<FakeDomainEntity> domainEntities = new List<FakeDomainEntity> {
//                entity1,
//                entity2
//            };

//            _bareDtoToDomainAdapterMock.Setup(b => b.Adapt(dtos)).Returns(domainEntities);
//            _domainServiceMock.Setup(d => d.Delete(domainEntities)).Throws<Exception>();

//            // Act && Assert 
//            Assert.Throws<Exception>(() => _sut.Object.Delete(dtos));
//        }


//        #endregion Delete (dtos)


//        #endregion Public Methods
//    }
//}
