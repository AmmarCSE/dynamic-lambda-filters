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
        List<FilterItem> GetFilterDataForFilter(List<FilterSearchItem> searchItems); 
    }
}
