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

            return dbContext
                 .Set<TMainSet>()
                //.Where(expressionHelper.GenerateWhereClause<TMainSet>(searchItems[0]))
                 .Where(expressionHelper.TASTemplateWhereExpression<TMainSet>())
                 .Join(
                     dbContext
                         .Set<TFilterSet>()
                         //.Where(expressionHelper.TASTemplateWhereExpression<TFilterSet>())
                     , expressionHelper.GetJoinPredicate<TMainSet, TKeyType>(filter.MainSetKey)
                     , expressionHelper.GetJoinPredicate<TFilterSet, TKeyType>(filter.FilterSetKey)
                     , (m, f) => f)
                .Select(expressionHelper
                    .GetSelectClause<TFilterSet>(filter.FilterSetKey
                        , filter.FilterSetDisplayProperty)
                    )
                .Distinct()
                .ToList();
        }

        public List<FilterItem> GetFilterDataForFilter<TMainSet, TJunctionSet, TFilterSet, TKeyType>(
            Filter<TMainSet, TJunctionSet, TFilterSet, TKeyType> filter
                , List<FilterSearchItem> searchItems) 
            where TMainSet : class where TJunctionSet : class where TFilterSet : class 
        {
            LambdaExpressionHelper expressionHelper = new LambdaExpressionHelper();

            return dbContext
                 .Set<TMainSet>()
                 .Where(expressionHelper.TASTemplateWhereExpression<TMainSet>())
                 .Join(
                     dbContext
                         .Set<TJunctionSet>()
                         //.Where(expressionHelper.TASTemplateWhereExpression<TJunctionSet>())
                     , expressionHelper.GetJoinPredicate<TMainSet, TKeyType>(filter.MainSetKey)
                     , expressionHelper.GetJoinPredicate<TJunctionSet, TKeyType>(filter.JunctionSetLeftKey)
                     , (m, j) => j)
                 .Join(
                     dbContext
                         .Set<TFilterSet>()
                         //.Where(expressionHelper.TASTemplateWhereExpression<TFilterSet>())
                     , expressionHelper.GetJoinPredicate<TJunctionSet, TKeyType>(filter.JunctionSetRightKey)
                     , expressionHelper.GetJoinPredicate<TFilterSet, TKeyType>(filter.FilterSetKey)
                     , expressionHelper
                        .GetJoinResultSelector<TJunctionSet, TFilterSet>(filter.JunctionSetLeftKey, filter.FilterSetDisplayProperty))
                .Distinct()
                .ToList();
        }

        public List<FilterItem> GetFilterDataForFilter<TParentSet, TMainSet, TJunctionSet, TFilterSet, TKeyType>(
            Filter<TParentSet, TMainSet, TJunctionSet, TFilterSet, TKeyType> filter
                , List<FilterSearchItem> searchItems) 
            where TParentSet : class where TMainSet : class where TJunctionSet : class where TFilterSet : class 
        {
            LambdaExpressionHelper expressionHelper = new LambdaExpressionHelper();

            return dbContext
                 .Set<TParentSet>()
                 .Where(expressionHelper.TASTemplateWhereExpression<TParentSet>())
                 .Join(
                     dbContext
                         .Set<TMainSet>()
                         .Where(expressionHelper.TASTemplateWhereExpression<TMainSet>())
                     , expressionHelper.GetJoinPredicate<TParentSet, TKeyType>(filter.ParentSetKey)
                     , expressionHelper.GetJoinPredicate<TMainSet, TKeyType>(filter.MainSetLeftKey)
                     , (p, m) => m)
                 .Join(
                     dbContext
                         .Set<TJunctionSet>()
                         //.Where(expressionHelper.TASTemplateWhereExpression<TJunctionSet>())
                     , expressionHelper.GetJoinPredicate<TMainSet, TKeyType>(filter.MainSetRightKey)
                     , expressionHelper.GetJoinPredicate<TJunctionSet, TKeyType>(filter.JunctionSetLeftKey)
                     , (m, j) => j)
                 .Join(
                     dbContext
                         .Set<TFilterSet>()
                         //.Where(expressionHelper.TASTemplateWhereExpression<TFilterSet>())
                     , expressionHelper.GetJoinPredicate<TJunctionSet, TKeyType>(filter.JunctionSetRightKey)
                     , expressionHelper.GetJoinPredicate<TFilterSet, TKeyType>(filter.FilterSetKey)
                     , expressionHelper
                        .GetJoinResultSelector<TJunctionSet, TFilterSet>(filter.JunctionSetLeftKey, filter.FilterSetDisplayProperty))
                .Distinct()
                .ToList();
        }
    }
}