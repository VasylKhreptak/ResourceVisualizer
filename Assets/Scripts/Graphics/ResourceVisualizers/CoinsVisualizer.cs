using Infrastructure.Data.Static.Core;
using Infrastructure.Services.PersistentData.Core;
using Infrastructure.Services.StaticData.Core;
using Plugins.ResourceVisualizer;

namespace Graphics.ResourceVisualizers
{
    public class CoinsVisualizer : PooledResourceVisualizer
    {
        public CoinsVisualizer(IPersistentDataService persistentDataService, IStaticDataService staticDataService, ResourcesRoot resourcesRoot,
            Preferences preferences)
            : base(persistentDataService.Data.PlayerData.Coins, staticDataService.Prefabs[Prefab.Coin], resourcesRoot, preferences) { }
    }
}