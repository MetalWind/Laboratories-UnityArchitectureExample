using System;
using System.Collections.Generic;

namespace Laboratory.Core
{
    public class EventBus
    {
        private Dictionary<Type, List<EventCallback>> _allCallbacksLists;

        public EventBus()
        {
            _allCallbacksLists = new Dictionary<Type, List<EventCallback>>();
        }

        public void Subscribe<T>(Action<T> callback)
        {
            Type key = typeof(T);

            if (!_allCallbacksLists.ContainsKey(key))
            {
                _allCallbacksLists[key] = new List<EventCallback>() { new EventCallback(callback) };
                return;
            }
            _allCallbacksLists[key].Add(new EventCallback(callback));
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            Type key = typeof(T);

            if (_allCallbacksLists[key] != null)
            {
                EventCallback toDeleting = _allCallbacksLists[key].Find(x => x.Callback.Equals(callback));
                if (toDeleting != null) _allCallbacksLists[key].Remove(toDeleting);
            }
        }

        public void Invoke<T>(T param)
        {
            if (!_allCallbacksLists.ContainsKey(typeof(T)))
            {
                return;
            }

            List<EventCallback> allCallbacks = _allCallbacksLists[typeof(T)];
            foreach (var callback in allCallbacks)
            {
                Action<T> actCallback = callback.Callback as Action<T>;
                actCallback.Invoke(param);
            }
        }
    }
}