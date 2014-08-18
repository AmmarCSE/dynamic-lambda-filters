using LambdaFilters.FilterData;
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
        public string MainSetKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }
        public string FilterTitle { get; set; }
        public string FilterType { get; set; }
        public List<FilterItem> FilterItems { get; set; }

        public void SetFilterDataForFilter(List<FilterSearchItem> searchItems)
        {
            FilterItems =
                new FilterDataRetriever()
                .GetFilterDataForFilter(this, null);
        }
    }

    public class Filter<TMainSet, TJunctionSet, TFilterSet, TKeyType> : IFilter
            where TMainSet : class where TJunctionSet : class where TFilterSet : class 
    {
        public string MainSetKey { get; set; }
        public string JunctionSetLeftKey { get; set; }
        public string JunctionSetRightKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }
        public string FilterTitle { get; set; }
        public string FilterType { get; set; }
        public List<FilterItem> FilterItems { get; set; }

        public void SetFilterDataForFilter(List<FilterSearchItem> searchItems)
        {
            FilterItems =
                new FilterDataRetriever()
                .GetFilterDataForFilter(this, null);
        }
    }
}
