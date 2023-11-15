using System;
using System.Collections.Generic;
using System.Linq;

namespace Laboratory.Core
{
    public class LabContext
    {
        private HashSet<Type> types;

        public void AddType(Type type)
        {
            types.Add(type);
        }

        public void AddTypes(List<Type> types)
        {
            foreach (Type type in types)
            {
                AddType(type);
            }
        }

        public List<Type> GetTypes()
        {
            return types.ToList();
        }
    }
}