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
            //var test = dbContext
            //    .Set<Reservation>()
            //    .Join(
            //        dbContext
            //        .Set<OperatorCustomer>()
            //        .Join(
            //            dbContext
            //            .Set<Customer>()
            //            , oc => oc.CustomerID
            //            , c => c.CustomerID
            //            , (oc, c) => new { oc, c })
            //       , r => r.CompanyID
            //       , oc => oc.oc.OperatorCustomerID
            //       , (r, oc) => new { oc.c.CustomerName});
            var test = dbContext
                .Set<Reservation>()
                .Join(
                    dbContext
                    .Set<OperatorCustomer>()
                   , r => r.CompanyID
                   , oc => oc.OperatorCustomerID
                   , (r, oc) => oc)
                 .Join(dbContext.Set<Customer>()
                 , oc => oc.CustomerID
                 , c => c.CustomerID
                 , (oc, c) => new  { oc, c})
                 .Select(s => new FilterItem { Id = s.oc.OperatorCustomerID, Value = s.c.AccountName });

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
                         .Where(expressionHelper.TASTemplateWhereExpression<TJunctionSet>())
                     , expressionHelper.GetJoinPredicate<TMainSet, TKeyType>(filter.MainSetKey)
                     , expressionHelper.GetJoinPredicate<TJunctionSet, TKeyType>(filter.JunctionSetLeftKey)
                     , (m, j) => j)
                 .Join(
                     dbContext
                         .Set<TFilterSet>()
                         .Where(expressionHelper.TASTemplateWhereExpression<TFilterSet>())
                     , expressionHelper.GetJoinPredicate<TJunctionSet, TKeyType>(filter.JunctionSetRightKey)
                     , expressionHelper.GetJoinPredicate<TFilterSet, TKeyType>(filter.FilterSetKey)
                     , expressionHelper
                        .GetJoinResultSelector<TJunctionSet, TFilterSet>(filter.JunctionSetLeftKey, filter.FilterSetDisplayProperty))
                       //, (f, m) => f)
                //.Select(expressionHelper
                //    .GetSelectClause<TJunctionSet, TFilterSet>(filter.JunctionSetLeftKey
                //        , filter.FilterSetDisplayProperty)
                //    )
                .Distinct()
                
                .ToList();
        }
    }
}