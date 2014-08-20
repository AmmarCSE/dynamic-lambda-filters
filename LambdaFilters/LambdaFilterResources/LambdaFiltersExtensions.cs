using LambdaFilters.FilterAssembler;
using LambdaFilters.LambdaFilterResources.FilterModels;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    public static class LambdaFiltersExtensions
    {
        public static MvcHtmlString FiltersFor<TModel, TFilter>(this HtmlHelper<TModel> htmlHelper
            , Expression<Func<TModel, TFilter>> expression
            , string filterModelName)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            FilterAssembler assembler = new FilterAssembler();
            string serializedFilters = 
                assembler.AssembleFiltersForModel(filterModelName
                , (List<IFilter>)metadata.Model
                , new List<FilterSearchItem>());

            return assembler.BuildRazorScriptBlock(serializedFilters);
        }
    }
}
