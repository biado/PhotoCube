using System;

namespace ObjectCubeServer.Models.PublicClasses
{
    public class Page
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int FileCount { get; set; }

        public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, FileCount);
    }
}
