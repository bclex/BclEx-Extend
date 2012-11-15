//using System.Patterns.Reporting;
//namespace Sample
//{
//    public class SimpleModel
//    {
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public int Age { get; set; }
//    }

//    public class Test
//    {
//        public void Do()
//        {
//            var typeMap = new TypeMap();
//            var x = new MappingExpression<SimpleModel>(typeMap);
//                .ForMember(c => c.FirstName, o => o.Ignore())
//               .ForMember(c => c.FirstName, o => o.DoThis(sdfjsdkjfsd))
//                .ForMember(c => c.FirstName, value);
//        }
//    }
//}