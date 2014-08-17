using LambdaFilters.LambdaFilterResources.FilterModels;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaFilters.FilterAssembler
{
    public class FilterAssembler
    {
        public void AssembleFiltersForModel(List<IFilter> filters, List<FilterSearchItem> searchItems)
        {
            foreach (var filter in filters)
            {
                filter.SetFilterDataForFilter(searchItems);
            }

            string test = filters.First().FilterItems.ToString();
        }
    }
}
