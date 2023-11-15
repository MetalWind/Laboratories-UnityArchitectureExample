using System;
using System.Collections.Generic;
using Zenject;

namespace Laboratory.Core
{
    public class TypeObjFactory
    {
        private DiContainer _container;

        [Inject]
        public TypeObjFactory(DiContainer container)
        {
            _container = container;
        }

        public List<T> CreateAll<T>()
        {
            List<Type> types = ReflectionTool.GetTypesByBase(typeof(T));
            List<T> objs = new List<T>();
            foreach (Type type in types)
            {
                T obj = (T)_container.Instantiate(type);
                objs.Add(obj);
            }
            return objs;
        }
    }
}
