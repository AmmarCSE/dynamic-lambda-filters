using LambdaFilters.FilterAssembler;
using LambdaFilters.LambdaFilterResources.FilterModels;
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
        public static MvcHtmlString FiltersFor<TModel, TFilter>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TFilter>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            FilterAssembler assembler = new FilterAssembler();
            assembler.AssembleFiltersForModel((List<IFilter>)metadata.Model, null);
            return null;
        }
    }
}
