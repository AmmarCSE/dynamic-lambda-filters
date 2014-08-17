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
using System.Data.Entity;

namespace LambdaFilters.FilterData
{
    public class FilterDataRetriever
    {
        private TAS_DevEntities dbContext = ContextHelper.GetContext();

        public List<FilterItem> GetFilterDataForFilter<TMainSet, TFilterSet, TKeyType>(
            Filter<TMainSet, TFilterSet, TKeyType> filter
                , List<FilterSearchItem> searchItems) 
            where TMainSet : class where TFilterSet : class 
        {
            LambdaExpressionHelper expressionHelper = new LambdaExpressionHelper();

            var test = dbContext
                 .Set<Reservation>()
                //.Where(expressionHelper.GenerateWhereClause<TMainSet>(searchItems[0]))
                 .Where(expressionHelper.TASTemplateWhereExpression<Reservation>())
                 .Join(
                     dbContext
                         .Set<Nationality>()
                         .Where(expressionHelper.TASTemplateWhereExpression<Nationality>())
                     , n=> n.NationalityID
                     , n => n.NationalityID
                     , (m, f) => f);
            return dbContext
                 .Set<TMainSet>()
                //.Where(expressionHelper.GenerateWhereClause<TMainSet>(searchItems[0]))
                 .Where(expressionHelper.TASTemplateWhereExpression<TMainSet>())
                 .Join(
                     dbContext
                         .Set<TFilterSet>()
                         .Where(expressionHelper.TASTemplateWhereExpression<TFilterSet>())
                     , expressionHelper.GetJoinPredicate<TMainSet, TKeyType>(filter.MainSetKey)
                     , expressionHelper.GetJoinPredicate<TFilterSet, TKeyType>(filter.FilterSetKey)
                     , (m, f) => f)
                 //.Select(s => new FilterItem())
                .Select(expressionHelper
                    .GetSelectClause<TFilterSet>(filter.FilterSetKey
                        , filter.FilterSetDisplayProperty)
                    )
                 .Distinct()
                 .ToList();
        }
    }
}