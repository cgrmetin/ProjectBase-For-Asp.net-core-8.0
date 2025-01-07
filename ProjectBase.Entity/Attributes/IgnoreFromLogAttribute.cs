using System.Web.Http.Filters;

namespace ProjectBase.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class IgnoreFromLogAttribute : ActionFilterAttribute
    {
        public IgnoreFromLogAttribute() 
        {
            
        }
    }
}

