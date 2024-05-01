using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Plugins.Banks.Core;
using Plugins.Extensions;
using Plugins.MinMaxProperties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Plugins.ResourceVisualizer
{
    public abstract class ResourceVisualizer<T> where T : IComparable<T>
    {
        private readonly ResourcesRoot _resourcesRoot;
        private readonly IBank<T> _resourceBank;
        private readonly Preferences _preferences;

        protected ResourceVisualizer(ResourcesRoot resourcesRoot, IBank<T> resourceBank, Preferences preferences)
        {
            _resourcesRoot = resourcesRoot;
            _resourceBank = resourceBank;
            _preferences = preferences;
        }

        private T _plannedSpendAmount;

        public async UniTask Collect(T amount, Vector3 fromWorld, Func<Vector3> worldPositionGetter, CancellationToken cancellationToken)
        {
            if (amount.CompareTo(default) is -1 or 0)
                return;

            List<UniTask> resourceTasks = new List<UniTask>();

            float delay = 0f;
            float delayIncrement = GetInterval(amount);

            int count = GetCount(amount);
            T maxAmountPerResource = FloorToT(ToFloat(amount) / count);
            T transferredAmount = default;

            for (int i = 0; i < count; i++)
            {
                int index = i;
                float capturedDelay = delay;

                T amountPerResource = index + 1 == count ? Subtract(amount, transferredAmount) : maxAmountPerResource;
                transferredAmount = Add(transferredAmount, amountPerResource);

                UniTask task = UniTask.Create(async () =>
                {
                    bool canceled = await UniTask
                        .Delay(TimeSpan.FromSeconds(capturedDelay), cancellationToken: cancellationToken)
                        .SuppressCancellationThrow();

                    if (canceled)
                        return;

                    GameObject resourceInstance = Instantiate();
                    RectTransform resource = resourceInstance.GetComponent<RectTransform>();
                    PrepareResource(resource, fromWorld);

                    await MoveResource(resource, worldPositionGetter, cancellationToken);

                    _resourceBank.Add(amountPerResource);
                    OnCollected(amountPerResource);
                });

                resourceTasks.Add(task);

                delay += delayIncrement;
            }

            await UniTask.WhenAll(resourceTasks);
        }

        public async UniTask Spend(T amount, Vector3 fromWorld, Func<Vector3> worldPositionGetter, CancellationToken cancellationToken)
        {
            if (amount.CompareTo(default) is -1 or 0 || _resourceBank.HasEnough(Add(amount, _plannedSpendAmount)) == false)
                return;

            List<UniTask> resourceTasks = new List<UniTask>();

            float delay = 0f;
            float delayIncrement = GetInterval(amount);

            int count = GetCount(amount);
            T maxAmountPerResource = FloorToT(ToFloat(amount) / count);
            T transferredAmount = default;

            for (int i = 0; i < count; i++)
            {
                int index = i;
                float capturedDelay = delay;

                T amountPerResource = index + 1 == count ? Subtract(amount, transferredAmount) : maxAmountPerResource;
                transferredAmount = Add(transferredAmount, amountPerResource);
                _plannedSpendAmount = Add(_plannedSpendAmount, amountPerResource);

                UniTask task = UniTask.Create(async () =>
                {
                    await UniTask
                        .Delay(TimeSpan.FromSeconds(capturedDelay), cancellationToken: cancellationToken)
                        .SuppressCancellationThrow();

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    GameObject resourceInstance = Instantiate();
                    RectTransform resource = resourceInstance.GetComponent<RectTransform>();
                    PrepareResource(resource, fromWorld);

                    _plannedSpendAmount = Subtract(_plannedSpendAmount, amountPerResource);

                    _resourceBank.Spend(amountPerResource);
                    OnSpent(amountPerResource);

                    await MoveResource(resource, worldPositionGetter, cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        _resourceBank.Add(amountPerResource);
                });

                resourceTasks.Add(task);

                delay += delayIncrement;
            }

            await UniTask.WhenAll(resourceTasks);
        }

        private void PrepareResource(RectTransform resource, Vector3 worldSpawnPoint)
        {
            resource.SetParent(_resourcesRoot.RectTransform);
            resource.anchoredPosition3D = WorldToAnchoredPoint(worldSpawnPoint);
            resource.localScale = Vector3.one * _preferences.StartScale.Random();
            resource.localRotation = Quaternion.Euler(0f, 0f, _preferences.StartRotation.Random());
            resource.SetAsLastSibling();
        }

        private async UniTask MoveResource(RectTransform resource, Func<Vector3> worldPositionGetter, CancellationToken cancellationToken)
        {
            float moveSpeed = _preferences.StartMoveSpeed.Random();
            float aimSpeed = _preferences.StartAimSpeed.Random();

            Vector2 direction = Random.insideUnitCircle;

            Vector3 lastTargetPosition = worldPositionGetter.Invoke();

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

                lastTargetPosition = worldPositionGetter.Invoke();

                moveSpeed += _preferences.MoveAcceleration;
                aimSpeed += _preferences.AimAcceleration;

                if (resource.anchoredPosition.IsCloseTo(WorldToAnchoredPoint(lastTargetPosition), _preferences.DistanceThreshold))
                    break;
            }

            if (resource != null)
                Destroy(resource.gameObject);
        }

        #region TemplateMethods

        protected abstract GameObject Instantiate();

        protected abstract void Destroy(GameObject gameObject);

        protected abstract float ToFloat(T value);

        protected abstract int ToInt(T value);

        protected abstract T Add(T a, T b);

        protected abstract T Subtract(T a, T b);

        protected abstract T FloorToT(float value);

        #endregion

        #region Callbacks

        protected virtual void OnSpent(T amount) { }

        protected virtual void OnCollected(T amount) { }

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

        private int GetCount(T amount)
        {
            float evaluatedCount = _preferences.CountCurve.Evaluate(ToFloat(amount), ToFloat(_preferences.Amount.Min),
                ToFloat(_preferences.Amount.Max), _preferences.Count.Min, _preferences.Count.Max);

            return Mathf.Min(ToInt(amount), (int)evaluatedCount);
        }

        private float GetInterval(T amount) =>
            _preferences.IntervalCurve.Evaluate(ToFloat(amount), ToFloat(_preferences.Amount.Min), ToFloat(_preferences.Amount.Max),
                _preferences.Interval.Max, _preferences.Interval.Min);

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
            [SerializeField] private MinMaxData<T> _amount;
            [SerializeField] private IntMinMax _count = new IntMinMax(1, 30);
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
            public MinMaxData<T> Amount => _amount;
            public IntMinMax Count => _count;
            public FloatMinMax Interval => _interval;
            public AnimationCurve CountCurve => _countCurve;
            public AnimationCurve IntervalCurve => _intervalCurve;
        }
    }
}