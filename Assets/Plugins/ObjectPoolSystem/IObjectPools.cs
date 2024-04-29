using System;
using UnityEngine;

namespace Plugins.ObjectPoolSystem
{
    public interface IObjectPools<in T> where T : Enum
    {
        public event Action<GameObject> OnEnabledObject;
        public event Action<GameObject> OnDisabledObject;
        public event Action<GameObject> OnDestroyedObject;
        public event Action OnCleared;

        public bool TryGetPool(T key, out ObjectPool pool);

        public ObjectPool GetPool(T key);

        public void Clear();
    }
}