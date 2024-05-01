using Graphics.ResourceVisualizers;
using Graphics.UI;
using Plugins.ResourceVisualizer;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelInstaller : MonoInstaller
    {
        [Header("References")]
        [SerializeField] private ResourcesRoot _resourcesRoot;
        [SerializeField] private Coin _coin;
        [SerializeField] private Camera _camera;

        [Header("Preferences")]
        [SerializeField] private ResourceVisualizer<int>.Preferences _coinsVisualizerPreferences;

        #region MonoBehaviour

        private void OnValidate()
        {
            _resourcesRoot ??= FindObjectOfType<ResourcesRoot>(true);
            _coin ??= FindObjectOfType<Coin>(true);
            _camera ??= FindObjectOfType<Camera>(true);
        }

        #endregion

        public override void InstallBindings()
        {
            Container.BindInstance(_resourcesRoot).AsSingle();
            Container.BindInstance(_coin).AsSingle();
            Container.BindInstance(_camera).AsSingle();
            Container.BindInterfacesAndSelfTo<CoinsVisualizer>().AsSingle().WithArguments(_coinsVisualizerPreferences);
        }
    }
}