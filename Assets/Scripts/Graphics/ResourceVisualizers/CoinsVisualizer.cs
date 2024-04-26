using Infrastructure.Data.Static.Core;
using Infrastructure.Services.StaticData.Core;
using Plugins.ResourceVisualizer;
using UnityEngine;

namespace Graphics.ResourceVisualizers
{
    public class CoinsVisualizer : ResourceVisualizer
    {
        private readonly IStaticDataService _staticDataService;

        public CoinsVisualizer(ResourcesRoot resourcesRoot, Camera camera, Preferences preferences, IStaticDataService staticDataService) :
            base(resourcesRoot, camera, preferences)
        {
            _staticDataService = staticDataService;
        }

        protected override GameObject CreateResourceInstance() => Object.Instantiate(_staticDataService.Prefabs[Prefab.Coin]);
    }
}