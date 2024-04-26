namespace Infrastructure.Services.Log.Core
{
    public interface ILogService
    {
        public void Log(object message);

        public void LogWarning(object message);

        public void LogError(object message);
    }
}