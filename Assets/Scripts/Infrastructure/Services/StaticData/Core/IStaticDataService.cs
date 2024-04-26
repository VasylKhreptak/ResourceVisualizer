using Infrastructure.Data.SaveLoad.Core;
using Infrastructure.Data.Static;

namespace Infrastructure.Services.StaticData.Core
{
    public interface IStaticDataService : ILoadHandler
    {
        public Config Config { get; }
        public Balance Balance { get; }
        public Prefabs Prefabs { get; }
    }
}