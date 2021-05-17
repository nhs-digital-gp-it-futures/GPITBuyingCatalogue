using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Logging
{
    public interface ILogWrapper<T>
        where T : class
    {
        void LogError(string message, params object[] args);

        void LogError(Exception exception, string message, params object[] args);

        void LogInformation(string message, params object[] args);

        void LogWarning(string message, params object[] args);

        void LogTrace(string message, params object[] args);
    }
}
