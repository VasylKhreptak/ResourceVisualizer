using Infrastructure.EntryPoints.Core;
using UnityEngine;
using Zenject;

namespace Infrastructure.EntryPoints.Game
{
    public class GameRunner : MonoBehaviour, IEntryPoint
    {
        private void Awake()
        {
            Enter();
        }

        public void Enter()
        {
            if (FindObjectOfType<ProjectContext>() == null)
            {
                GameObject sceneContextObject = new GameObject("SceneContext");
                sceneContextObject.AddComponent<SceneContext>();
            }

            Destroy(gameObject);
        }
    }
}