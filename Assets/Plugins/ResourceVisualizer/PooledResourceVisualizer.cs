using Plugins.Banks.Core;
using Plugins.ObjectPoolSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.ResourceVisualizer
{
    public abstract class PooledResourceVisualizer : ResourceVisualizer
    {
        private const int InitialSize = 0;
        private const int MaxSize = 999;

        private readonly IObjectPool _resourcePool;

        public PooledResourceVisualizer(IBank<int> resourceBank, GameObject resourcePrefab, ResourcesRoot resourcesRoot, Preferences preferences) :
            base(resourcesRoot, resourceBank, preferences)
        {
            _resourcePool = new ObjectPool(() => Object.Instantiate(resourcePrefab, resourcesRoot.transform), InitialSize, MaxSize);
        }

        protected sealed override GameObject Instantiate() => _resourcePool.Get();

        protected sealed override void Destroy(GameObject gameObject) => gameObject.SetActive(false);
    }
}