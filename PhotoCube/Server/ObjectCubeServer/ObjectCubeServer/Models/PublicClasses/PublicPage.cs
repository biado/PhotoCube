//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ObjectCubeServer.Models.PublicClasses
//{
//    public class PublicPage
//    {
//        public int CurrentPage { get; set; }
//        public int PageCount { get; set; }
//        public int PageSize { get; set; }
//        public int TotalFileCount { get; set; }

//        public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
//        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, TotalFileCount);
//        public IList<PublicCell> Results { get; set; }

//        public PublicPage()
//        {
//            Results = new List<PublicCell>();
//        }
//    }
//}
