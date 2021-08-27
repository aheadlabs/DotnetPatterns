//using DotnetToolset.Patterns.Dddd.Interfaces;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using DotnetToolset.Patterns.Dddd.Bases;

//namespace DotnetToolset.Patterns.Tests.Dddd.Fakes
//{
//    public sealed class FakeDomainEntity : DomainEntityBase, IDomainEntity
//    {
//        #region Public Constructors

//        public FakeDomainEntity(int id, List<FakeEntityRelationed> listOfRelatedEntities)
//        {
//            Id = id;
//            FakeEntityRelationed = listOfRelatedEntities;
//        }

//        #endregion Public Constructors

//        #region Public Properties

//        public DateTime Date { get; set; }
//        public ICollection<FakeEntityRelationed> FakeEntityRelationed { get; set; }
//        public int Id { get; set; }

//        public bool IsValid(ILogger logger) => true;

//        #endregion Public Properties
//    }
//}