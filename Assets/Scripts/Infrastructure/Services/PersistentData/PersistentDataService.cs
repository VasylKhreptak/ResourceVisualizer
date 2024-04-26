using Infrastructure.Services.PersistentData.Core;

namespace Infrastructure.Services.PersistentData
{
    public class PersistentDataService : IPersistentDataService
    {
        public Data.Persistent.PersistentData Data { get; set; }
    }
}