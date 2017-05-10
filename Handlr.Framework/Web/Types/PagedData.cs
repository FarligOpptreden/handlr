using System.Collections.Generic;

namespace Handlr.Framework.Web.Types
{
    public class PagedData<T>
    {
        public PagedData(int pageNo, int pageCount, int pageSize, int itemCount, List<T> data)
        {
            PageNo = pageNo;
            PageCount = pageCount;
            PageSize = pageSize;
            ItemCount = itemCount;
            Data = data;
        }

        public int PageNo { get; private set; }
        public int PageCount { get; private set; }
        public int PageSize { get; private set; }
        public int ItemCount { get; private set; }
        public List<T> Data { get; private set; }
    }
}
