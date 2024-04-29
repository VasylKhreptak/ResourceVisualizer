using System;
using UnityEngine;

namespace Plugins.ObjectPoolSystem
{
    public interface IObjectPool
    {
        public event Action<GameObject> OnEnabledObject;
        public event Action<GameObject> OnDisabledObject;
        public event Action<GameObject> OnDestroyedObject;
        public event Action OnCleared;

        public GameObject Get();

        public void Clear();
    }
}