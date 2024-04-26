using Infrastructure.Data.Static;
using Infrastructure.Services.StaticData.Core;
using UnityEngine;

namespace Infrastructure.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string GameConfigPath = "StaticData/GameConfig";
        private const string GameBalancePath = "StaticData/GameBalance";
        private const string GamePrefabsPath = "StaticData/GamePrefabs";

        public Config Config { get; private set; }
        public Balance Balance { get; private set; }
        public Prefabs Prefabs { get; private set; }

        public void Load()
        {
            Config = Resources.Load<Config>(GameConfigPath);
            Balance = Resources.Load<Balance>(GameBalancePath);
            Prefabs = Resources.Load<Prefabs>(GamePrefabsPath);
        }
    }
}