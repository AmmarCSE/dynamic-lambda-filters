using LambdaFilters.FilterData;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaFilters.LambdaFilterResources.FilterModels
{
    public abstract class BaseDataFilter : IFilter
    {
        public string FilterTitle { get; set; }
        public string FilterType { get; set; }
        public List<FilterItem> FilterItems { get; set; }

        abstract public void SetFilterDataForFilter(List<FilterSearchItem> searchItems);
        abstract public string GetFilterSearchKey();

        public dynamic ConstructReturnObject()
        {
            return 
                new {
                    FilterTitle = this.FilterTitle
                    , FilterType = this.FilterType
                    , FilterItems = this.FilterItems
                    , FilterSearchKey = this.GetFilterSearchKey()
                };
        }
    }
}
