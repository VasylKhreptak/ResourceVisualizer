namespace Infrastructure.Data.Persistent
{
    public class AnalyticsData
    {
        public int SessionsCount;

        public bool IsFirstSession => SessionsCount == 1;
    }
}