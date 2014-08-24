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

            var test =Expression.Lambda<Func<TEntity, bool>>(predicateBody
                , new ParameterExpression[] { parameter }); 
            return Expression.Lambda<Func<TEntity, bool>>(predicateBody
                , new ParameterExpression[] { parameter });
        }

        public Expression<Func<TMainSet, bool>> GenerateWhereClause<TMainSet>(List<FilterSearchItem> searchItems)
        {
            Expression<Func<TMainSet, bool>> defaultExpression = TASTemplateWhereExpression<TMainSet>();

            Expression whereBody = defaultExpression.Body;
            ParameterExpression whereParameter = defaultExpression.Parameters[0];

            foreach (var searchItem in searchItems)
            {
                if(searchItem.SearchKey.Split(new char[] {'.'}).ToList().Count == 1)
                whereBody = GenerateSubWhereClause<TMainSet>(whereParameter, whereBody, searchItem);
            }

            return Expression.Lambda<Func<TMainSet, bool>>(whereBody
                , whereParameter);
        }

        public Expression GenerateSubWhereClause<TMainSet>(ParameterExpression appendantParameter, Expression appendantExpression, FilterSearchItem searchItem)
        {
            ConstantExpression filterValues;
            Expression expressionBody = null;

            List<string> propertyPath = searchItem.SearchKey.Split(new char[] {'.'}).ToList();
            MemberExpression property = Expression.Property(appendantParameter, typeof(TMainSet), propertyPath[0]);


            if (searchItem.SearchType == "List<int>")
            {
                List<int> array = searchItem.SearchData.Split(new string[] { "," }, StringSplitOptions.None).Select(s => int.Parse(s)).ToList();

                if (propertyPath.Count > 1)
                {
                    return Expression.And(Expression.LessThanOrEqual(property, property), appendantExpression);
                    //names.Any(x => subnames.Contains(x))
                    //array1.Intersect(array2).Any()

                    //propertyPath.RemoveAt(0);
                    //property = Expression.Property(property, propertyPath[0]);
                }

                if (property.Type == typeof(int?))
                {
                    property = Expression.Property(property, "Value");
                }

                Type searchValuesType = array.GetType().GetGenericArguments().FirstOrDefault();
                filterValues = Expression.Constant(array, array.GetType());

                expressionBody =
                    Expression.Call(typeof(Enumerable)
                        , "Contains"
                        , new[] { searchValuesType }
                        , filterValues
                        , property);
            }
            else if (searchItem.SearchType == "DateTime")
            {
                string[] subArgs = searchItem.SearchData.Split(new char[] { '.' });

                filterValues = Expression.Constant(DateTime.Parse(subArgs[0]));

                if (subArgs[1] == "GreaterThan")
                {
                    expressionBody = Expression.GreaterThanOrEqual(property, filterValues);
                }
                else
                {
                    expressionBody = Expression.LessThanOrEqual(property, filterValues);
                }
            }

            return Expression.And(expressionBody, appendantExpression);
        }
    }
}
