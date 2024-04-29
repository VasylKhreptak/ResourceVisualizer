using UnityEngine;

namespace Plugins.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool ContainsLayer(this LayerMask layerMask, int layerID) => (layerMask.value & (1 << layerID)) > 0;
    }
}