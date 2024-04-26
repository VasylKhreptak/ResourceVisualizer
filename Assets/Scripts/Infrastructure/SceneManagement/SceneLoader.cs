using System;
using System.Collections;
using Infrastructure.Coroutines.Runner.Core;
using Infrastructure.SceneManagement.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private ICoroutineRunner _coroutineRunner;

        [Inject]
        private void Constructor(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void LoadAsync(string name, Action onComplete = null)
        {
            _coroutineRunner.StartCoroutine(LoadSceneRoutine(name, onComplete));
        }

        public void LoadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadCurrentSceneAsync(Action onComplete = null)
        {
            _coroutineRunner.StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name, onComplete));
        }

        private IEnumerator LoadSceneRoutine(string name, Action onComplete = null)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);

            yield return new WaitUntil(() => asyncOperation.isDone);

            onComplete?.Invoke();
        }
    }
}