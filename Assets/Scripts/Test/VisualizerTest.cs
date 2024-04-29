using System.Threading;
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

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        #region MonoBehaviour

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Collect();
            else if (Input.GetKeyDown(KeyCode.C))
                ClearCoins();
            else if (Input.GetKeyDown(KeyCode.D))
                Dispose();
            else if (Input.GetMouseButtonDown(1))
                Spend();
        }

        #endregion

        private async void Collect()
        {
            await _coinsVisualizer.Collect(_amount, GetMouseWorldPosition(), () => _coin.transform.position, _cts.Token);

            Debug.Log("Collected all");
        }

        private void ClearCoins() => _persistentDataService.Data.PlayerData.Coins.Clear();

        private void Dispose() => _cts.Cancel();

        private Vector3 GetMouseWorldPosition()
        {
            Camera camera = null;

            if (_resourcesRoot.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camera = _resourcesRoot.Canvas.worldCamera;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_resourcesRoot.RectTransform, Input.mousePosition, camera, out Vector3 position);

            return position;
        }

        private async void Spend()
        {
            await _coinsVisualizer.Spend(_amount, _coin.transform.position, GetMouseWorldPosition, _cts.Token);

            Debug.Log("Spent all");
        }
    }
}