using System;
using Plugins.Banks.Core;
using Plugins.ObjectPoolSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.ResourceVisualizer
{
    public abstract class PooledResourceVisualizer<T> : ResourceVisualizer<T> where T : IComparable<T>
    {
        private const int InitialSize = 0;
        private const int MaxSize = 999;

        private readonly IObjectPool _resourcePool;

        public PooledResourceVisualizer(IBank<T> resourceBank, GameObject resourcePrefab, ResourcesRoot resourcesRoot, Preferences preferences) :
            base(resourcesRoot, resourceBank, preferences)
        {
            _resourcePool = new ObjectPool(() => Object.Instantiate(resourcePrefab, resourcesRoot.transform), InitialSize, MaxSize);
        }

        protected sealed override GameObject Instantiate() => _resourcePool.Get();

        protected sealed override void Destroy(GameObject gameObject) => gameObject.SetActive(false);
    }
}