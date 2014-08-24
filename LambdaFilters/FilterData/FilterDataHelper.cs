using LambdaFilters.FilterData.LambdaHelper;
using LambdaFilters.LamdaFilterResources.FilterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Agency.EntityFramework;

namespace LambdaFilters.FilterData
{
    public class FilterDataHelper
    {
        private TAS_DevEntities dbContext = ContextHelper.GetContext();

        public IQueryable<TMainSet> RetreiveMainSetQueryable<TMainSet>(List<FilterSearchItem> searchItems) 
            where TMainSet : class 
        {
            LambdaExpressionHelper expressionHelper = new LambdaExpressionHelper();

            return dbContext
                 .Set<TMainSet>()
                 .Where(expressionHelper.GenerateWhereClause<TMainSet>(searchItems));
        }
    }
}
