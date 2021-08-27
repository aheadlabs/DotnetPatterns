//using System;
//using System.ComponentModel.DataAnnotations;
//using DotnetToolset.Patterns.Dddd.Interfaces;

//namespace DotnetToolset.Patterns.Tests.Dddd.Fakes
//{
//    public class FakeBareDto : IDto
//    {
//        public FakeBareDto(DateTime date)
//        {
//            Date = date;
//        }

//        public DateTime Date { get; set; }

//        [Required]
//        public int Id { get; set; }

//        public bool IsValid()
//        {
//            return Id > 0;
//        }
//    }
//}
