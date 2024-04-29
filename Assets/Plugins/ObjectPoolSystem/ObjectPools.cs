using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.ObjectPoolSystem
{
    public class ObjectPools<T> : IObjectPools<T>, IDisposable where T : Enum
    {
        private readonly Dictionary<T, ObjectPool> _pools = new Dictionary<T, ObjectPool>();

        public event Action<GameObject> OnEnabledObject;
        public event Action<GameObject> OnDisabledObject;
        public event Action<GameObject> OnDestroyedObject;
        public event Action OnCleared;

        public ObjectPools(ObjectPoolPreference<T>[] objectPoolPreferences)
        {
            foreach (ObjectPoolPreference<T> preference in objectPoolPreferences)
            {
                ObjectPool objectPool = new ObjectPool(preference.CreateFunc, preference.InitialSize, preference.MaxSize);
                _pools.Add(preference.Key, objectPool);

                objectPool.OnEnabledObject += InvokeOnEnabledObject;
                objectPool.OnDisabledObject += InvokeOnDisabledObject;
                objectPool.OnDestroyedObject += InvokeOnDestroyedObject;
            }
        }

        public bool TryGetPool(T key, out ObjectPool pool) => _pools.TryGetValue(key, out pool);

        public ObjectPool GetPool(T key) => _pools[key];

        public void Clear()
        {
            foreach (ObjectPool pool in _pools.Values)
            {
                pool.Clear();
            }

            OnCleared?.Invoke();
        }

        public void Dispose() => Clear();

        private void InvokeOnEnabledObject(GameObject gameObject) => OnEnabledObject?.Invoke(gameObject);

        private void InvokeOnDisabledObject(GameObject gameObject) => OnDisabledObject?.Invoke(gameObject);

        private void InvokeOnDestroyedObject(GameObject gameObject) => OnDestroyedObject?.Invoke(gameObject);
    }
}