using LambdaFilters.FilterData.LambdaHelper;
using LambdaFilters.LamdaFilterResources;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Travel.Agency.EntityFramework;

namespace LambdaFilters.FilterData
{
    public class FilterDataRetriever
    {
        private TAS_DevEntities dbContext = ContextHelper.GetContext();
        public List<FilterItem> GetFilterDataForFilter<TMainSet, TFilterSet>(Filter<TMainSet, TFilterSet> filter, List<FilterSearchItem> searchItems) 
            where TMainSet : class where TFilterSet : class 
        {
            LambdaExpressionHelper expressionHelper = new LambdaExpressionHelper();

            return dbContext
                .Set<TMainSet>()
                //.Where(expressionHelper.GenerateWhereClause<TMainSet>(searchItems[0]))
                .Where(expressionHelper.TASTemplateWhereExpression<TMainSet>())
                .Join(
                    dbContext
                        .Set<TFilterSet>()
                        .Where(expressionHelper.TASTemplateWhereExpression<TFilterSet>())
                    , expressionHelper.GetJoinPredicate<TMainSet>(filter.MainSetKey)
                    , expressionHelper.GetJoinPredicate<TFilterSet>(filter.FilterSetKey)
                    , (m, f) => f)
                .Select(expressionHelper
                    .GetSelectClause<TFilterSet>(filter.FilterSetKey
                        , filter.FilterSetDisplayProperty)
                    )
                .Distinct()
                .ToList();
        }
    }
}