using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DbObjectAttribute : Attribute
    {
    }
}
