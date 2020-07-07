using System.Collections.Generic;

namespace PIM.Common.Models
{
    public class PagingResultModel<T> where T : class
    {
        public IList<T> Records { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
