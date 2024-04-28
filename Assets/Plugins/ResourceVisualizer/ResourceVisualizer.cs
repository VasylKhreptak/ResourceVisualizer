using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Plugins.MinMaxProperties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.ResourceVisualizer
{
    public abstract class ResourceVisualizer : IDisposable
    {
        private readonly ResourcesRoot _resourcesRoot;
        private readonly Preferences _preferences;

        protected ResourceVisualizer(ResourcesRoot resourcesRoot, Preferences preferences)
        {
            _resourcesRoot = resourcesRoot;
            _preferences = preferences;
        }

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Collect(Vector3 origin, Transform target, Action<int> onCollected = null, Action onCollectedAll = null) =>
            Collect(origin, target, onCollected, onCollectedAll, _cancellationTokenSource.Token);

        private async void Collect(Vector3 origin, Transform target, Action<int> onCollected, Action onCollectedAll,
            CancellationToken cancellationToken)
        {
            GameObject resourceInstance = Instantiate();
            RectTransform resource = resourceInstance.GetComponent<RectTransform>();
            PrepareResource(resource, origin);

            await MoveResource(resource, target, cancellationToken);

            onCollected?.Invoke(1);
            onCollectedAll?.Invoke();
        }

        private void PrepareResource(RectTransform resource, Vector3 worldSpawnPoint)
        {
            resource.SetParent(_resourcesRoot.RectTransform);
            resource.anchoredPosition3D = WorldToAnchoredPoint(worldSpawnPoint);
            resource.localScale = Vector3.one * _preferences.StartScale.Random();
            resource.localRotation = Quaternion.Euler(0f, 0f, _preferences.StartRotation.Random());
        }

        private async UniTask MoveResource(RectTransform resource, Transform target, CancellationToken cancellationToken)
        {
            float moveSpeed = _preferences.StartMoveSpeed.Random();
            float aimSpeed = _preferences.StartAimSpeed.Random();

            Vector2 direction = Random.insideUnitCircle;

            Vector3 lastTargetPosition = target.position;

            while (cancellationToken.IsCancellationRequested == false)
            {
                bool canceled = await UniTask.Yield().ToUniTask().AttachExternalCancellation(cancellationToken).SuppressCancellationThrow();

                if (canceled || resource == null)
                    break;

                resource.anchoredPosition += direction * moveSpeed * Time.deltaTime;
                resource.localScale = Vector3.Lerp(resource.localScale, Vector3.one, _preferences.ScaleSpeed * Time.deltaTime);
                resource.localRotation = Quaternion.Lerp(resource.localRotation, Quaternion.identity, _preferences.RotationSpeed * Time.deltaTime);

                Vector2 targetDirection = (lastTargetPosition - resource.position).normalized;
                direction = Vector2.Lerp(direction, targetDirection, aimSpeed * Time.deltaTime);

                if (target != null)
                    lastTargetPosition = target.position;

                moveSpeed += _preferences.MoveAcceleration;
                aimSpeed += _preferences.AimAcceleration;

                if (Vector2.Distance(resource.anchoredPosition, WorldToAnchoredPoint(lastTargetPosition)) < _preferences.DistanceThreshold)
                    break;
            }

            if (resource != null)
                Destroy(resource.gameObject);
        }

        public void Dispose() => _cancellationTokenSource.Cancel();

        #region TemplateMethods

        protected abstract GameObject Instantiate();

        protected abstract void Destroy(GameObject gameObject);

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