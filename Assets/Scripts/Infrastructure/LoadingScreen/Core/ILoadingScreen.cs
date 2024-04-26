using System;

namespace Infrastructure.LoadingScreen.Core
{
    public interface ILoadingScreen
    {
        public void Show();

        public void Hide(Action onComplete = null);
    }
}