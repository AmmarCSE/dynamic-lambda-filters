using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaFilters.LambdaFilterResources.FilterModels
{
    public interface IFilter
    {
        string FilterTitle { get; set; }
        string FilterType { get; set; }
        List<FilterItem> FilterItems { get; set; }
        //object[] FilterAttributes { get; set; }

        void SetFilterDataForFilter(List<FilterSearchItem> searchItems); 
    }
}
