using LambdaFilters.FilterData;
using LambdaFilters.LamdaFilterResources;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaFilters
{
    public class FilterBuilder
    {
        public List<FilterItem> GenerateFilters<TMainSet, TFilterSet, TKeyType>(
            Filter<TMainSet, TFilterSet, TKeyType> filter
                , List<FilterSearchItem> searchItems)
            where TMainSet : class where TFilterSet : class 
        {
            FilterDataRetriever dataReteriever = new FilterDataRetriever();
            List<FilterItem> items = dataReteriever.GetFilterDataForFilter(filter, searchItems);
            return items;
        }
    }
}
