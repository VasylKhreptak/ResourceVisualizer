using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Plugins.MinMaxProperties;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Plugins.ResourceVisualizer
{
    public abstract class ResourceVisualizer : IDisposable
    {
        private readonly ResourcesRoot _resourcesRoot;
        private readonly Camera _camera;
        private readonly Preferences _preferences;

        protected ResourceVisualizer(ResourcesRoot resourcesRoot, Camera camera, Preferences preferences)
        {
            _resourcesRoot = resourcesRoot;
            _camera = camera;
            _preferences = preferences;
        }

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Collect(Vector3 worldSpawnPosition, Transform target, Action<int> onCollected = null, Action onCollectedAll = null) =>
            Collect(worldSpawnPosition, target, onCollected, onCollectedAll, _cancellationTokenSource.Token);

        private async void Collect(Vector3 worldSpawnPosition, Transform target, Action<int> onCollected, Action onCollectedAll,
            CancellationToken cancellationToken)
        {
            GameObject resource = CreateResourceInstance();
            RectTransform resourceRectTransform = resource.GetComponent<RectTransform>();

            Vector2 anchoredPosition = WorldToAnchoredPoint(worldSpawnPosition);

            resourceRectTransform.SetParent(_resourcesRoot.RectTransform);
            resourceRectTransform.anchoredPosition = anchoredPosition;
            resourceRectTransform.localScale = Vector3.one * _preferences.StartScale.Random();
            resourceRectTransform.localRotation = Quaternion.Euler(0f, 0f, _preferences.StartRotation.Random());

            await MoveResource(resourceRectTransform, target, cancellationToken);

            onCollected?.Invoke(1);
            onCollectedAll?.Invoke();
        }

        private async UniTask MoveResource(RectTransform resourceRectTransform, Transform target, CancellationToken cancellationToken)
        {
            float moveSpeed = _preferences.StartMoveSpeed.Random();
            float aimSpeed = _preferences.StartAimSpeed.Random();

            Vector2 direction = Random.insideUnitCircle;

            while (cancellationToken.IsCancellationRequested == false)
            {
                bool canceled = await UniTask.Yield().ToUniTask().AttachExternalCancellation(cancellationToken).SuppressCancellationThrow();

                if (canceled)
                    break;

                resourceRectTransform.anchoredPosition += direction * moveSpeed * Time.deltaTime;
                resourceRectTransform.localScale =
                    Vector3.Lerp(resourceRectTransform.localScale, Vector3.one, _preferences.ScaleSpeed * Time.deltaTime);
                resourceRectTransform.localRotation =
                    Quaternion.Lerp(resourceRectTransform.localRotation, Quaternion.identity, _preferences.RotationSpeed * Time.deltaTime);

                Vector2 targetDirection = (target.position - resourceRectTransform.position).normalized;
                direction = Vector2.Lerp(direction, targetDirection, aimSpeed * Time.deltaTime);

                moveSpeed += _preferences.MoveAcceleration;
                aimSpeed += _preferences.AimAcceleration;

                if (Vector2.Distance(resourceRectTransform.position, target.position) < _preferences.DistanceThreshold)
                    break;
            }

            if (resourceRectTransform != null)
                Object.Destroy(resourceRectTransform.gameObject);
        }

        public void Dispose() => _cancellationTokenSource.Cancel();

        #region TemplateMethods

        protected abstract GameObject CreateResourceInstance();

        #endregion

        private Vector2 WorldToAnchoredPoint(Vector3 worldPoint)
        {
            Camera camera = null;

            if (_resourcesRoot.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camera = _resourcesRoot.Canvas.worldCamera;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_resourcesRoot.RectTransform, screenPoint, camera, out Vector2 anchoredPosition);

            return anchoredPosition;
        }

        [Serializable]
        public class Preferences
        {
            [SerializeField] private float _distanceThreshold = 5f;
            [SerializeField] private FloatMinMax _startScale = new FloatMinMax(0.2f, 0.7f);
            [SerializeField] private FloatMinMax _startRotation = new FloatMinMax(0f, 360f);
            [SerializeField] private FloatMinMax _startMoveSpeed = new FloatMinMax(190f, 200f);
            [SerializeField] private FloatMinMax _startAimSpeed = new FloatMinMax(20f, 30f);
            [SerializeField] private float _scaleSpeed = 5f;
            [SerializeField] private float _rotationSpeed = 5f;
            [SerializeField] private float _moveAcceleration;
            [SerializeField] private float _aimAcceleration;

            public float DistanceThreshold => _distanceThreshold;
            public FloatMinMax StartScale => _startScale;
            public FloatMinMax StartRotation => _startRotation;
            public FloatMinMax StartMoveSpeed => _startMoveSpeed;
            public FloatMinMax StartAimSpeed => _startAimSpeed;
            public float ScaleSpeed => _scaleSpeed;
            public float RotationSpeed => _rotationSpeed;
            public float MoveAcceleration => _moveAcceleration;
            public float AimAcceleration => _aimAcceleration;
        }
    }
}