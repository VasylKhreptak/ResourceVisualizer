using System;

namespace Plugins.MinMaxProperties
{
    [Serializable]
    public class MinMaxData<T>
    {
        public T Min;
        public T Max;

        public MinMaxData(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public MinMaxData()
        {
            Min = default;
            Max = default;
        }
    }
}