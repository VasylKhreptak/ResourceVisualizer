using Graphics.ResourceVisualizers;
using Graphics.UI;
using Plugins.ResourceVisualizer;
using UnityEngine;
using Zenject;

namespace Test
{
    public class CoinSpawner : MonoBehaviour
    {
        private Coin _coin;
        private CoinsVisualizer _coinsVisualizer;
        private ResourcesRoot _resourcesRoot;

        [Inject]
        private void Constructor(Coin coin, CoinsVisualizer coinsVisualizer, ResourcesRoot resourcesRoot)
        {
            _coin = coin;
            _coinsVisualizer = coinsVisualizer;
            _resourcesRoot = resourcesRoot;
        }

        #region MonoBehaviour

        private void Update()
        {
            if (Input.GetMouseButton(0) == false)
                return;

            Camera camera = null;

            if (_resourcesRoot.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camera = _resourcesRoot.Canvas.worldCamera;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_resourcesRoot.RectTransform, Input.mousePosition, camera, out Vector3 position);

            transform.position = position;

            _coinsVisualizer.Collect(position, _coin.transform);
        }

        #endregion
    }
}