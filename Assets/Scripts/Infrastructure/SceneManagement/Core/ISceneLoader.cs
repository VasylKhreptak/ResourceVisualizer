using System;

namespace Infrastructure.SceneManagement.Core
{
    public interface ISceneLoader
    {
        public void Load(string name);

        public void LoadAsync(string name, Action onComplete = null);

        public void LoadCurrentScene();

        public void LoadCurrentSceneAsync(Action onComplete = null);
    }
}