using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace LambdaFilters.LambdaFilterResources.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class JunctionEntityAttribute : Attribute
    {
         public Type TJunctionEntity { get; set; }
         public string JunctionSetKey { get; set; }
    }
}