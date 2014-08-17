using LambdaFilters.LamdaFilterResources;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LambdaFilters.FilterData.LambdaHelper
{
    class LambdaExpressionHelper
    {
        public Expression<Func<TEntity, TKeyType>> GetJoinPredicate<TEntity, TKeyType>(string property)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");

            return Expression.Lambda<Func<TEntity, TKeyType>>(Expression.Convert(Expression.PropertyOrField(parameter, property), typeof(TKeyType)), parameter);
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
    }
}
