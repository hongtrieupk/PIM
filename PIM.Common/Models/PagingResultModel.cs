using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM.Common.Models
{
    public class PagingResultModel<T> where T : class
    {
        public IList<T> Records { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
