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

        public Expression<Func<TJunctionSet, TFilterSet, FilterItem>> GetJoinResultSelector<TJunctionSet, TFilterSet>
            (string keyProperty, string valueProperty)
        {
            Type junctionSetType = typeof( TJunctionSet );
            Type filterSetType = typeof( TFilterSet );
            Type filterItemType = typeof( FilterItem );

            //ParameterExpression parentParameter = Expression.Parameter( typeof(object), "parent" );
            ParameterExpression junctionParameter = Expression.Parameter( junctionSetType, "junctionSet" );
            ParameterExpression filterParameter = Expression.Parameter( filterSetType, "filterSet" );

            MemberExpression junctionSetId = 
                Expression.MakeMemberAccess(junctionParameter, junctionSetType.GetProperty(keyProperty));
            MemberExpression filterSetValue = 
                Expression.MakeMemberAccess(filterParameter, filterSetType.GetProperty(valueProperty));

            MemberAssignment idBind = Expression.Bind( filterItemType.GetProperty("Id"), junctionSetId );
            MemberAssignment valueBind = Expression.Bind( filterItemType.GetProperty( "Value" ), filterSetValue );

            NewExpression newFilterItem = Expression.New(filterItemType);
            MemberInitExpression init = Expression.MemberInit(newFilterItem, valueBind, idBind);

           return (Expression<Func<TJunctionSet, TFilterSet, FilterItem>>)Expression.Lambda(init
               , new ParameterExpression[]{ junctionParameter,filterParameter} );
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

        public Expression<Func<TMainSet, bool>> GenerateWhereClause<TMainSet>(List<FilterSearchItem> searchItems)
        {
            Expression<Func<TMainSet, bool>>  whereExpression = TASTemplateWhereExpression<TMainSet>();
            foreach (var searchItem in searchItems)
            {
                whereExpression = GenerateSubWhereClause<TMainSet>(whereExpression, searchItem);
            }

            return whereExpression;
        }

        public Expression<Func<TMainSet, bool>> GenerateSubWhereClause<TMainSet>(Expression<Func<TMainSet, bool>>  whereExpression, FilterSearchItem searchItem)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TMainSet), "entity");

            if (searchItem.SearchType == "List<int>")
            {
                List<int> array = searchItem.SearchData.Split(new string[] { "," }, StringSplitOptions.None).Select(s => int.Parse(s)).ToList();

                MemberExpression key = Expression.Property(parameter, typeof(TMainSet), searchItem.SearchKey);

                if (key.Type == typeof(int?))
                {
                    key = Expression.Property(key, "Value");
                }
                Type searchValuesType = array.GetType().GetGenericArguments().FirstOrDefault();
                ConstantExpression searchValuesAsConstant = Expression.Constant(array, array.GetType());
                MethodCallExpression containsBody =
                    Expression.Call(typeof(Enumerable)
                        , "Contains"
                        , new[] { searchValuesType }
                        , searchValuesAsConstant
                        , key);

                //Expression appendedExpression = Expression.And(whereExpression, Expression.Lambda<Func<TMainSet, bool>>(containsBody, parameter));
                return Expression.Lambda<Func<TMainSet, bool>>(containsBody
                    , new ParameterExpression[] { parameter });
            }

            return null;
        }
    }
}
