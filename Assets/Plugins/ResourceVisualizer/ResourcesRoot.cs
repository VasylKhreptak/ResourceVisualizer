using UnityEngine;

namespace Plugins.ResourceVisualizer
{
    public class ResourcesRoot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _canvas;

        public RectTransform RectTransform => _rectTransform;
        public Canvas Canvas => _canvas;

        #region MonoBehaviour

        private void OnValidate()
        {
            _rectTransform ??= GetComponent<RectTransform>();
            _canvas ??= GetComponentInParent<Canvas>(true);
        }

        #endregion
    }
}