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
        public void Listtesting123<TMainSet, TFilterSet>() 
        {
            GetLookupSource<Allotment, Hotel, int>(dbContext.Allotments,  dbContext.Hotels);
            IQueryable<Allotment> queryableData = dbContext.Allotments.AsQueryable<Allotment>();

            // Compose the expression tree that represents the parameter to the predicate.
            ParameterExpression pe = Expression.Parameter(typeof(Allotment), "allotment");

            // Create an expression tree that represents the expression 'company.ToLower() == "coho winery"'.
            Expression left = Expression.Property(pe, typeof(Allotment).GetProperty("Deleted"));
            Expression right = Expression.Constant(false);
			
            Expression e1 = Expression.Equal(left, right);

            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { queryableData.ElementType },
                queryableData.Expression,
                Expression.Lambda<Func<Allotment, bool>>(e1, new ParameterExpression[] { pe }));
            IQueryable<Allotment> results = queryableData.Provider.CreateQuery<Allotment>(whereCallExpression);

            //var test = dbContext.Set<TMainSet>()
            //    .Where(a => a.Deleted == false && a.Active == true);
                //.Join(
                //        dbContext
                //            .Hotels
                //            .Where(h => h.Active == true && h.Deleted == false)
                //    , a => a.HotelID
                //    , h => h.HotelID
                //    , (a, h) => new { Hotel = h })
                //.Distinct();
        }
public  IQueryable<TLookup> GetLookupSource<T, TLookup, TKey>(IQueryable<T> source, IQueryable<TLookup> lookup){
   ParameterExpression s = Expression.Parameter(source.ElementType, "s");
   Expression<Func<T, TKey>> outerKeySelector = Expression.Lambda<Func<T, TKey>>(Expression.PropertyOrField(s, "HotelID"), s);

   ParameterExpression l = Expression.Parameter(lookup.ElementType, "l");
   Expression<Func<TLookup, TKey>> innerKeySelector = Expression.Lambda<Func<TLookup, TKey>>(Expression.PropertyOrField(l, "HotelID"), l);

   var resultSelector = Expression.Lambda<Func<T, TLookup, TLookup>>(l, s, l);

   MethodInfo joinMethod = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "Join" && m.GetParameters().Length == 5).First();
   var genericJoinMethod = joinMethod.MakeGenericMethod(typeof(T), typeof(TLookup), typeof(TKey), typeof(IQueryable<TLookup>));
   var result = genericJoinMethod.Invoke(source, new object[] { source, lookup, outerKeySelector, innerKeySelector, resultSelector });
    IQueryable<Allotment> results = source.Provider.CreateQuery<TLookup>(genericJoinMethod);
   return (IQueryable<TLookup>)result;
}
    }
}