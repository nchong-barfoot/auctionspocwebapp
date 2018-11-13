namespace BT.Auctions.API.Common.Interfaces
{
    public interface ILogger
    {
        void LogException(string exceptionMessage);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogDebug(string message);
    }
}