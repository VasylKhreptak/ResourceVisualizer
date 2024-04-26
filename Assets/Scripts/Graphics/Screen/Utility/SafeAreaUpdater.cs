using System;
using Infrastructure.Observers.Screen;
using UniRx;
using UnityEngine;
using Zenject;

namespace Graphics.Screen.Utility
{
    public class SafeAreaUpdater : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;

        private IDisposable _subscription;

        private ScreenObserver _screeService;

        [Inject]
        private void Constructor(ScreenObserver screenObserver)
        {
            _screeService = screenObserver;
        }

        #region MonoBehaiour

        private void OnValidate()
        {
            _rectTransform ??= GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            StartObserving();
        }

        private void OnDisable()
        {
            StopObserving();
        }

        #endregion

        private void StartObserving()
        {
            StopObserving();
            _subscription = _screeService.ScreenOrientation
                .CombineLatest(_screeService.ScreenResolution, (orientation, resolution) => (orientation, resolution))
                .DoOnSubscribe(UpdateArea)
                .Subscribe(tuple => UpdateArea());
        }

        private void StopObserving()
        {
            _subscription?.Dispose();
        }

        private void UpdateArea()
        {
            Rect safeArea = UnityEngine.Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= UnityEngine.Screen.width;
            anchorMin.y /= UnityEngine.Screen.height;
            anchorMax.x /= UnityEngine.Screen.width;
            anchorMax.y /= UnityEngine.Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}