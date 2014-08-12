using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaFilters
{
    public class Filter<TMainSet, TFilterSet> 
    {
        public string MainSetKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }
        public string FilterTitle { get; set; }
    }
}
