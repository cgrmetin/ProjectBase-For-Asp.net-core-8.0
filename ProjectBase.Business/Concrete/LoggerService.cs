using ProjectBase.Business.Abstract;
using ProjectBase.DTO.Middleware;
using log4net;
using Newtonsoft.Json;
using System.Reflection;

namespace ProjectBase.Business.Concrete
{
    public class LoggerService : ILoggerService
    {
        private readonly ILog _logger;


        public LoggerService()
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void Debug(string message)
        {
            _logger?.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void ErrorException(Exception ex)
        {
            _logger.Error(JsonConvert.SerializeObject(ex));
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }
        public void UsageLog(LogDto logDto)
        {
            _logger.Info(JsonConvert.SerializeObject(logDto));
        }
        public void Error(LogDto logDto)
        {
            _logger.Error(JsonConvert.SerializeObject(logDto));
        }
        public void ServiceLog<T>(T request)
        {
            _logger.Info(JsonConvert.SerializeObject(request));
        }
        public void ServiceError<T>(T request)
        {
            _logger.Error(JsonConvert.SerializeObject(request));
        }
    }
}
