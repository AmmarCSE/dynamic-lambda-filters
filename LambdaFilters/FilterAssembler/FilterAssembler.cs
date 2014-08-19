using LambdaFilters.LambdaFilterResources.FilterModels;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LambdaFilters.FilterAssembler
{
    public class FilterAssembler
    {
        public string AssembleFiltersForModel(string filterModelName
            , List<IFilter> filters
            , List<FilterSearchItem> searchItems)
        {
            foreach (var filter in filters)
            {
                if (filter.FilterType != "autocomplete")
                {
                    filter.SetFilterDataForFilter(searchItems);
                }
            }

            var filterResults = AssembleFilterStructure(filterModelName, filters);

            return SerializeFilters(filterResults);
        }

        public dynamic AssembleFilterStructure(string filterModelName
            , List<IFilter> filters)
        {
            return new Dictionary<string, List<IFilter>>()
            {
                {
                    filterModelName, filters
                }
            };
        }

        public string SerializeFilters(dynamic filterResults)
        {
            return new JavaScriptSerializer().Serialize(filterResults);
        }

        public MvcHtmlString BuildRazorScriptBlock(string serializedFilters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<script>");
            sb.Append("var filters = ");
            sb.Append(serializedFilters);
            sb.Append(";");
            sb.Append("</script>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
