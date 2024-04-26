using System;
using Plugins.MinMaxProperties.Core;

namespace Plugins.MinMaxProperties
{
    [Serializable]
    public class FloatMinMax : MinMaxProperty<float>
    {
        public FloatMinMax(float min, float max) : base(min, max) { }

        public override float Random() => UnityEngine.Random.Range(Min, Max);
    }
}