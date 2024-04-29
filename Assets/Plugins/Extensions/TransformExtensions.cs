using UnityEngine;

namespace Plugins.Extensions
{
    public static class TransformExtensions
    {
        public static Transform[] GetChildren(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];

            for (var i = 0; i < transform.childCount; i++)
                children[i] = transform.GetChild(i);

            return children;
        }

        public static Transform CreateParent(this Transform transform, string name)
        {
            Transform newParent = new GameObject(name).transform;
            Transform oldParent = transform.parent;
            newParent.SetParent(oldParent);
            newParent.localPosition = Vector3.zero;
            newParent.localRotation = Quaternion.identity;
            newParent.localScale = Vector3.one;
            transform.SetParent(newParent, true);

            return newParent;
        }
    }
}