using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Laboratory.Core
{
    public static class ReflectionTool
    {
        public static List<Type> GetTypesByBase(Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                           .GetTypes()
                           .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                           .ToList();
        }
    }
}