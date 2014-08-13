using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Travel.Agency.EntityFramework;

namespace LambdaFilters
{
    public class Class1
    {
        private TAS_DevEntities dbContext = ContextHelper.GetContext();
        public void Listtesting123<TMainSet, TFilterSet>(Filter<TMainSet, TFilterSet> filter, List<FilterSearchItem> searchItems) 
            where TMainSet : class where TFilterSet : class 
        {
            var test = dbContext
                .Set<TMainSet>()
                .Where(GenerateWhereClause<TMainSet>(searchItems[0]))
                .Join( 
                    dbContext
                        .Set<TFilterSet>()
                        .Where(TASTemplateWhereExpression<TFilterSet>())
                    , GetJoinPredicate<TMainSet>(filter.MainSetKey)
                    , GetJoinPredicate<TFilterSet>(filter.FilterSetKey)
                    , (m, f) => f)
                .Select(GetSelectClause<TFilterSet>(filter.FilterSetKey, filter.FilterSetDisplayProperty))
                .Distinct();
        }

        public Expression<Func<TEntity, int>> GetJoinPredicate<TEntity>(string property)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            return Expression.Lambda<Func<TEntity, int>>(Expression.PropertyOrField(parameter, property), parameter);
        }

        public Expression<Func<TFilterSet, FilterItem>> GetSelectClause<TFilterSet>
            (string keyProperty, string valueProperty)
        {
            Type filterSetType = typeof( TFilterSet );
            Type filterItemType = typeof( FilterItem );

            ParameterExpression parameter = Expression.Parameter( filterSetType, "filterSet" );

            MemberExpression filterSetId = 
                Expression.MakeMemberAccess(parameter, filterSetType.GetProperty(keyProperty));
            MemberExpression filterSetValue = 
                Expression.MakeMemberAccess(parameter, filterSetType.GetProperty(valueProperty));

            MemberAssignment idBind = Expression.Bind( filterItemType.GetProperty("Id"), filterSetId );
            MemberAssignment valueBind = Expression.Bind( filterItemType.GetProperty( "Value" ), filterSetValue );

            NewExpression newFilterItem = Expression.New(filterItemType);
            MemberInitExpression init = Expression.MemberInit(newFilterItem, valueBind, idBind);

           return (Expression<Func<TFilterSet,FilterItem>>)Expression.Lambda(init, parameter);
        }

        public Expression<Func<TEntity, bool>> TASTemplateWhereExpression<TEntity>()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            Expression left = Expression.Property(parameter, typeof(TEntity).GetProperty("Deleted"));
            Expression right = Expression.Constant(false);

            Expression expressionDeleted = Expression.Equal(left, right);

            left = Expression.Property(parameter, typeof(TEntity).GetProperty("Active"));
            right = Expression.Constant(true);

            Expression expressionActive = Expression.Equal(left, right);

            Expression predicateBody = Expression.And(expressionDeleted, expressionActive);

            return Expression.Lambda<Func<TEntity, bool>>(predicateBody
                , new ParameterExpression[] { parameter });
        }

        public Expression<Func<TMainSet, bool>> GenerateWhereClause<TMainSet>(FilterSearchItem searchItem)
        {
            List<int> array = searchItem.SearchData.Split(new string[] { "," }, StringSplitOptions.None).Select(s => int.Parse(s)).ToList();
            ParameterExpression parameter = Expression.Parameter(typeof(TMainSet), "entity");

            MemberExpression key = Expression.Property(parameter, typeof(TMainSet), searchItem.SearchKey);
            Type searchValuesType = array.GetType().GetGenericArguments().FirstOrDefault();
            ConstantExpression searchValuesAsConstant = Expression.Constant(array, array.GetType());
            MethodCallExpression containsBody = 
                Expression.Call(typeof(Enumerable)
                    , "Contains"
                    , new[] { searchValuesType }
                    , searchValuesAsConstant
                    , key);

            return Expression.Lambda<Func<TMainSet, bool>>(containsBody, parameter);
        }
        public void LambdaTreeJoin<TMainSet, TFilterSet>() where TMainSet : class where TFilterSet : class 
        {

            IQueryable<TMainSet> queryableData = dbContext.Set<TMainSet>().AsQueryable<TMainSet>();

            // Compose the expression tree that represents the parameter to the predicate.
            ParameterExpression s = Expression.Parameter(queryableData.ElementType, "s");
            Expression<Func<TMainSet, int>> outerKeySelector =
                Expression.Lambda<Func<TMainSet, int>>(Expression.PropertyOrField(s, "HotelID"), s);

            ParameterExpression l = Expression.Parameter(dbContext.Set<TFilterSet>().AsQueryable<TFilterSet>().ElementType, "l");
            Expression<Func<TFilterSet, int>> innerKeySelector =
                Expression.Lambda<Func<TFilterSet, int>>(Expression.PropertyOrField(l, "HotelID"), l);

            var resultSelector = Expression.Lambda<Func<TMainSet, TFilterSet, TFilterSet>>(l, s, l);
            MethodCallExpression join =  Expression.Call (
            typeof (Queryable),
            "Join",
            new Type[]
            {
                typeof (TMainSet),   // TOuter,
                typeof (TFilterSet),   // TInner,
                typeof (int),       // TKey,
                typeof (TFilterSet)      // TResult
            },
            new Expression[]
            {
                Expression.PropertyOrField (Expression.Constant (dbContext), "Allotments"),
                Expression.PropertyOrField (Expression.Constant (dbContext), "Hotels"),
                outerKeySelector,
                innerKeySelector,
                resultSelector
            });
             var resutl = Expression.Lambda<Func<IQueryable<TFilterSet>>> (join).Compile()();
        }
    }
}