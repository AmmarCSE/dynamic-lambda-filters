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
    public class Filter : IFilter
    {
        public string MainSetKey { get; set; }
        public string FilterTitle { get; set; }
        public string FilterType { get; set; }

        public dynamic ConstructReturnObject()
        {
            return 
                new {
                    FilterTitle = this.FilterTitle
                    , FilterType = this.FilterType
                    , FilterSearchKey = this.MainSetKey
                };
        }
    }
    
    public class Filter<TMainSet, TFilterSet, TKeyType> : BaseDataFilter
            where TMainSet : class where TFilterSet : class 
    {
        public string MainSetKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }

        public override void SetFilterDataForFilter(List<FilterSearchItem> searchItems)
        {
            FilterItems =
                new FilterDataRetriever()
                .GetFilterDataForFilter(this, searchItems);
        }

        public override string GetFilterSearchKey()
        {
            return MainSetKey;
        }
    }

    public class Filter<TMainSet, TJunctionSet, TFilterSet, TKeyType> : BaseDataFilter
            where TMainSet : class where TJunctionSet : class where TFilterSet : class 
    {
        public string MainSetKey { get; set; }
        public string JunctionSetLeftKey { get; set; }
        public string JunctionSetRightKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }

        public override void SetFilterDataForFilter(List<FilterSearchItem> searchItems)
        {
            FilterItems =
                new FilterDataRetriever()
                .GetFilterDataForFilter(this, searchItems);
        }

        public override string GetFilterSearchKey()
        {
            return MainSetKey;
        }
    }

    public class Filter<TParentSet, TChildSet, TJunctionSet, TFilterSet, TKeyType> : BaseDataFilter
            where TParentSet : class where TChildSet : class where TJunctionSet : class where TFilterSet : class 
    {
        public string MainSetKey { get; set; }
        public string ChildSetLeftKey { get; set; }
        public string ChildSetRightKey { get; set; }
        public string JunctionSetLeftKey { get; set; }
        public string JunctionSetRightKey { get; set; }
        public string FilterSetKey { get; set; }
        public string FilterSetDisplayProperty { get; set; }
        public string ChildSetPropertyName { get; set; }

        public override void SetFilterDataForFilter(List<FilterSearchItem> searchItems)
        {
            FilterItems =
                new FilterDataRetriever()
                .GetFilterDataForFilter(this, searchItems);
        }

        public override string GetFilterSearchKey()
        {
            return ChildSetPropertyName + "." + ChildSetRightKey;
        }
    }
}
