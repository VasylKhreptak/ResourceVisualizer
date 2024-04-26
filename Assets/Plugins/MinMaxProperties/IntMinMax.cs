using System;
using Plugins.MinMaxProperties.Core;

namespace Plugins.MinMaxProperties
{
    [Serializable]
    public class IntMinMax : MinMaxProperty<int>
    {
        public IntMinMax(int min, int max) : base(min, max) { }

        public override int Random() => UnityEngine.Random.Range(Min, Max);
    }
}