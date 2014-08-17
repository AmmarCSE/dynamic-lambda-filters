using LambdaFilters.FilterData.LambdaHelper;
using LambdaFilters.LambdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Travel.Agency.EntityFramework;

namespace LambdaFilters.LamdaFilterResources.FilterModels
{
    public class Filter<TMainSet, TFilterSet, TKeyType> : IFilter
            where TMainSet : class where TFilterSet : class 
    {
        //public FilterKey<TMainSetKeyType> MainSetKey { get; set; }
        //public FilterKey<TFilterSetKeyType> FilterSetKey { get; set; }
        public string MainSetKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }
        public string FilterTitle { get; set; }
        public string FilterType { get; set; }
        public List<FilterItem> FilterItems { get; set; }

        public void SetFilterDataForFilter(List<FilterSearchItem> searchItems)
        {
            FilterItems =
                new FilterBuilder()
                .GenerateFilters(this, null);
        }
    }
}
