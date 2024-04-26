using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.Editor
{
    public class InfrastructureMenu
    {
        private const string GameRunnerPrefabPath = "Infrastructure/GameRunner";

        [MenuItem("GameObject/Infrastructure/GameRunner", false)]
        public static void CreateGameRunner()
        {
            Object gameRunnerPrefab = Resources.Load(GameRunnerPrefabPath);
            GameObject root = PrefabUtility.InstantiatePrefab(gameRunnerPrefab) as GameObject;
            Selection.activeGameObject = root;

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}