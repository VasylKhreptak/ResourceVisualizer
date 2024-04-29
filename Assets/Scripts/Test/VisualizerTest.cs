using Graphics.ResourceVisualizers;
using Graphics.UI;
using Infrastructure.Services.PersistentData.Core;
using Plugins.ResourceVisualizer;
using UnityEngine;
using Zenject;

namespace Test
{
    public class VisualizerTest : MonoBehaviour
    {
        [Header("Preferences")]
        [SerializeField] private int _amount = 100;

        private Coin _coin;
        private CoinsVisualizer _coinsVisualizer;
        private ResourcesRoot _resourcesRoot;
        private IPersistentDataService _persistentDataService;

        [Inject]
        private void Constructor(Coin coin, CoinsVisualizer coinsVisualizer, ResourcesRoot resourcesRoot,
            IPersistentDataService persistentDataService)
        {
            _coin = coin;
            _coinsVisualizer = coinsVisualizer;
            _resourcesRoot = resourcesRoot;
            _persistentDataService = persistentDataService;
        }

        #region MonoBehaviour

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Spawn();
            else if (Input.GetKeyDown(KeyCode.C))
                ClearCoins();
            else if (Input.GetKeyDown(KeyCode.D))
                Dispose();
        }

        #endregion

        private async void Spawn()
        {
            Camera camera = null;

            if (_resourcesRoot.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camera = _resourcesRoot.Canvas.worldCamera;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_resourcesRoot.RectTransform, Input.mousePosition, camera, out Vector3 position);

            await _coinsVisualizer.Collect(_amount, position, _coin.transform);
            
            Debug.Log("Completed");
        }

        private void ClearCoins() => _persistentDataService.Data.PlayerData.Coins.Clear();

        private void Dispose() => _coinsVisualizer.Dispose();
    }
}