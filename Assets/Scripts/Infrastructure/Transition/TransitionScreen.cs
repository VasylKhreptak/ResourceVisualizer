using System;
using DG.Tweening;
using Infrastructure.Transition.Core;
using UnityEngine;

namespace Infrastructure.Transition
{
    public class TransitionScreen : MonoBehaviour, ITransitionScreen
    {
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Preferences")]
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private AnimationCurve _curve;

        private Tween _tween;

        #region MonoBehaviour

        private void OnValidate() => _canvasGroup ??= GetComponent<CanvasGroup>();

        private void Awake() => HideImmediately();

        private void OnDestroy() => _tween?.Kill();

        #endregion

        public void Show(Action onComplete = null)
        {
            gameObject.SetActive(true);
            SetAlphaSmooth(1, onComplete);
        }

        public void Hide(Action onComplete = null)
        {
            SetAlphaSmooth(0, () =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        public void ShowImmediately()
        {
            _tween?.Kill();
            _canvasGroup.alpha = 1;
            gameObject.SetActive(true);
        }

        public void HideImmediately()
        {
            _tween?.Kill();
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        private void SetAlphaSmooth(float alpha, Action onComplete)
        {
            _tween?.Kill();
            _tween = _canvasGroup
                .DOFade(alpha, _duration)
                .SetEase(_curve)
                .OnComplete(() => onComplete?.Invoke())
                .Play();
        }
    }
}