using Infrastructure.Data.Static.Core;
using Infrastructure.Services.StaticData.Core;
using Plugins.ResourceVisualizer;
using UnityEngine;

namespace Graphics.ResourceVisualizers
{
    public class CoinsVisualizer : ResourceVisualizer
    {
        private readonly IStaticDataService _staticDataService;

        public CoinsVisualizer(ResourcesRoot resourcesRoot, Preferences preferences, IStaticDataService staticDataService) :
            base(resourcesRoot, preferences)
        {
            _staticDataService = staticDataService;
        }

        protected override GameObject Instantiate() => Object.Instantiate(_staticDataService.Prefabs[Prefab.Coin]);

        protected override void Destroy(GameObject gameObject) => Object.Destroy(gameObject);
    }
}