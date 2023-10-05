using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bwr.Exchange.Reflection.Extensions
{
    public static class ObjectExtension
    {
        public static Dictionary<string,object> ToDictionary(this object obj)
        {
            return obj.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
        }
    }
}
