using ProjectBase.DTO.Middleware;

namespace ProjectBase.Business.Abstract
{
    public interface ILoggerService
    {
        void Info(string message);
        void Warning(string message);
        void ErrorException(Exception ex);
        void Error(string message);
        void Debug(string message);
        void Error(LogDto logDto);
        void UsageLog(LogDto logDto);
        void ServiceLog<T>(T request);
        void ServiceError<T>(T request);
    }
}
