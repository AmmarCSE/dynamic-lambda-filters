using System;
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
        public void Listtesting123<TMainSet, TFilterSet>(Filter<TMainSet, TFilterSet> filter) where TMainSet : class where TFilterSet : class 
        {
            var test = dbContext
                .Set<TMainSet>()
                .Where(TASTemplateWhereExpression<TMainSet>())
                .Join( 
                    dbContext
                        .Set<TFilterSet>()
                        .Where(TASTemplateWhereExpression<TFilterSet>())
                    , GetJoinPredicate<TMainSet>(filter.MainSetKey)
                    , GetJoinPredicate<TFilterSet>(filter.FilterSetKey)
                    , (m, f) => new { TFilterSet = f })
                .Distinct();
        }

        public Expression<Func<TEntity, int>> GetJoinPredicate<TEntity>(string property) where TEntity : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            return Expression.Lambda<Func<TEntity, int>>(Expression.PropertyOrField(parameter, property), parameter);
        }

        public Expression<Func<TEntity, bool>> TASTemplateWhereExpression<TEntity>() where TEntity : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            Expression left = Expression.Property(parameter, typeof(TEntity).GetProperty("Deleted"));
            Expression right = Expression.Constant(false);

            Expression expressionDeleted = Expression.Equal(left, right);

            left = Expression.Property(parameter, typeof(TEntity).GetProperty("Active"));
            right = Expression.Constant(true);

            Expression expressionActive = Expression.Equal(left, right);

            Expression predicateBody = Expression.And(expressionDeleted, expressionActive);

            //MethodCallExpression whereCallExpression = Expression.Call(
            //    typeof(Queryable)
            //    , "WHERE"
            //    , new Type[] { queryableData.ElementType }
            //    , queryableData.Expression
            //    , 
            //IQueryable<TEntity> results = queryableData.Provider.CreateQuery<TEntity>(whereCallExpression);
            return Expression.Lambda<Func<TEntity, bool>>(predicateBody
                , new ParameterExpression[] { parameter });
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