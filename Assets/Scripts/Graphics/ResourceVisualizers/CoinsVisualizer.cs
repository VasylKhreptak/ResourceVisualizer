using Infrastructure.Data.Static.Core;
using Infrastructure.Services.PersistentData.Core;
using Infrastructure.Services.StaticData.Core;
using Plugins.ResourceVisualizer;
using UnityEngine;

namespace Graphics.ResourceVisualizers
{
    public sealed class CoinsVisualizer : PooledResourceVisualizer<int>
    {
        public CoinsVisualizer(IPersistentDataService persistentDataService, IStaticDataService staticDataService, ResourcesRoot resourcesRoot,
            Preferences preferences)
            : base(persistentDataService.Data.PlayerData.Coins, staticDataService.Prefabs[Prefab.Coin], resourcesRoot, preferences) { }

        protected override float ToFloat(int value) => value;

        protected override int ToInt(int value) => value;

        protected override int Add(int a, int b) => a + b;

        protected override int Subtract(int a, int b) => a - b;

        protected override int FloorToT(float value) => Mathf.FloorToInt(value);

        protected override void OnSpent(int amount) => Debug.Log("Spent: " + amount);

        protected override void OnCollected(int amount) => Debug.Log("Collected: " + amount);
    }
}