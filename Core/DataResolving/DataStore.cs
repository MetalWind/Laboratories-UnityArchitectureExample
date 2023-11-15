using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Laboratory.Core
{
    public class DataStore
    {
        private Dictionary<Type, IDataAccessor> accessorsDict;
        private List<IDataResolver> resolvers;
        private EventBus _bus;

        [Inject]
        public DataStore(TypeObjFactory factory, EventBus bus)
        {
            accessorsDict = new Dictionary<Type, IDataAccessor>();
            resolvers = new List<IDataResolver>();

            resolvers = CreateResolvers(factory);
            accessorsDict = CreateAccessorsDict(resolvers);

            _bus = bus;
            Subscribe();
        }

        public T GetAccessor<T>(object sender) where T : IDataAccessor
        {
            if (accessorsDict.ContainsKey(typeof(T))) { return (T)accessorsDict[typeof(T)]; }

            Debug.LogError($"{sender} is trying to get accessor that does not exist");
            return default;
        }

        private void SaveAll(SaveProcessInvokeSignal signal)
        {
            _bus.Invoke<PrepareToSaveSignal>(default);
            resolvers.ForEach(res => res.SaveCurrent());
        }

        private List<IDataResolver> CreateResolvers(TypeObjFactory factory)
        {
            List<IDataResolver> resolvs = new List<IDataResolver>();

            foreach (IDataResolver res in factory.CreateAll<IDataResolver>())
            {
                resolvs.Add(res);
            }

            return resolvs;
        }

        private Dictionary<Type, IDataAccessor> CreateAccessorsDict(List<IDataResolver> resolvers)
        {
            Dictionary<Type, IDataAccessor> dict = new Dictionary<Type, IDataAccessor>();

            foreach (IDataResolver resolver in resolvers)
            {
                IDataAccessor accessor = resolver.CreateAccessor();
                dict.Add(accessor.GetType(), accessor);
            }
            return dict;
        }

        public void Subscribe()
        {
            _bus.Subscribe<SaveProcessInvokeSignal>(SaveAll);
        }
    }
}