using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Votyra.Core.Utils
{
    public static class ReflectionUtils
    {
        public static string GetTypeDisplayName<T>(this T instance)
        {
            var type = instance?.GetType() ?? typeof(T);
            return type.GetDisplayName();
        }
        
        public static string GetDisplayName(this Type type)
        {
            var attr = type.GetCustomAttributes<DisplayNameAttribute>();
            return attr.FirstOrDefault()
                ?.DisplayName ?? type.Name;
        }
    }
}