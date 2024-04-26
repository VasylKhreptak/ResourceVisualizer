namespace Infrastructure.Services.PersistentData.Core
{
    public interface IPersistentDataService
    {
        public Data.Persistent.PersistentData Data { get; set; }
    }
}