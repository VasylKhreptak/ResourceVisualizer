namespace Plugins.MinMaxProperties.Core
{
    public abstract class MinMaxProperty<T> : MinMaxData<T>
    {
        protected MinMaxProperty() { }

        protected MinMaxProperty(T min, T max) : base(min, max) { }

        public abstract T Random();
    }
}