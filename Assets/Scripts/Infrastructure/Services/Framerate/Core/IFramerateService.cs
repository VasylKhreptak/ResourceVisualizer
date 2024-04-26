using System;
using UniRx;
using Zenject;

namespace Infrastructure.Services.Framerate.Core
{
    public interface IFramerateService : IInitializable, IDisposable
    {
        public IReadOnlyReactiveProperty<float> AverageFramerate { get; }

        public void SetTargetFramerate(int framerate);
    }
}