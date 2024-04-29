using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Plugins.Banks.Core;
using Plugins.Banks.Integer;
using Plugins.MinMaxProperties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.ResourceVisualizer
{
    public abstract class ResourceVisualizer : IDisposable
    {
        private readonly ResourcesRoot _resourcesRoot;
        private readonly IBank<int> _resourceBank;
        private readonly Preferences _preferences;
        private readonly IBank<int> _spendBank;

        protected ResourceVisualizer(ResourcesRoot resourcesRoot, IBank<int> resourceBank, Preferences preferences)
        {
            _resourcesRoot = resourcesRoot;
            _resourceBank = resourceBank;
            _preferences = preferences;
            _spendBank = new IntegerBank();
        }

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Collect(int amount, Vector3 fromWorld, Transform to, Action onCompleted = null) =>
            Collect(amount, fromWorld, to, _cancellationTokenSource.Token, onCompleted);

        private async void Collect(int amount, Vector3 fromWorld, Transform to, CancellationToken cancellationToken, Action onCompleted = null)
        {
            if (amount <= 0)
                return;

            List<UniTask> resourceTasks = new List<UniTask>();

            float delay = 0f;
            float delayIncrement = GetInterval(amount);

            int count = GetCount(amount);
            int maxAmountPerResource = amount % count == 0 ? amount / count : amount / count + 1;

            for (int i = 0; i < count; i++)
            {
                int index = i;

                UniTask task = UniTask.Create(async () =>
                {
                    bool canceled = await UniTask
                        .Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken)
                        .SuppressCancellationThrow();

                    if (canceled)
                        return;

                    GameObject resourceInstance = Instantiate();
                    RectTransform resource = resourceInstance.GetComponent<RectTransform>();
                    PrepareResource(resource, fromWorld);

                    await MoveResource(resource, to, cancellationToken);

                    int amountPerResource = index != count - 1 ? maxAmountPerResource : amount - maxAmountPerResource * index;

                    _resourceBank.Add(amountPerResource);
                });

                resourceTasks.Add(task);

                delay += delayIncrement;
            }

            await UniTask.WhenAll(resourceTasks);

            onCompleted?.Invoke();
        }

        private void PrepareResource(RectTransform resource, Vector3 worldSpawnPoint)
        {
            resource.SetParent(_resourcesRoot.RectTransform);
            resource.anchoredPosition3D = WorldToAnchoredPoint(worldSpawnPoint);
            resource.localScale = Vector3.one * _preferences.StartScale.Random();
            resource.localRotation = Quaternion.Euler(0f, 0f, _preferences.StartRotation.Random());
            resource.SetAsLastSibling();
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

        private int GetCount(int amount)
        {
            float t = (float)amount / _preferences.MaxAmount;
            return Mathf.CeilToInt(_preferences.CountCurve.Evaluate(t) * _preferences.MaxCount);
        }

        private float GetInterval(int amount)
        {
            float t = (float)amount / _preferences.MaxAmount;
            return _preferences.IntervalCurve.Evaluate(t) * _preferences.Interval.Max;
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
            [SerializeField] private int _maxAmount = 100;
            [SerializeField] private int _maxCount = 10;
            [SerializeField] private FloatMinMax _interval = new FloatMinMax(0.001f, 0.1f);
            [SerializeField] private AnimationCurve _countCurve;
            [SerializeField] private AnimationCurve _intervalCurve;
            public float DistanceThreshold => _distanceThreshold;

            public FloatMinMax StartScale => _startScale;
            public FloatMinMax StartRotation => _startRotation;
            public FloatMinMax StartMoveSpeed => _startMoveSpeed;
            public FloatMinMax StartAimSpeed => _startAimSpeed;
            public float ScaleSpeed => _scaleSpeed;
            public float RotationSpeed => _rotationSpeed;
            public float MoveAcceleration => _moveAcceleration;
            public float AimAcceleration => _aimAcceleration;
            public int MaxAmount => _maxAmount;
            public int MaxCount => _maxCount;
            public FloatMinMax Interval => _interval;
            public AnimationCurve CountCurve => _countCurve;
            public AnimationCurve IntervalCurve => _intervalCurve;
        }
    }
}