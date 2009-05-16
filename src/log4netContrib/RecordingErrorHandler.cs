using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Util;
using log4net.Core;

namespace log4netContrib.Appender
{
    /// <summary>
    /// This object records whether an error has been recorded
    /// </summary>
    /// 
    /// <author>Michael Cromwell</author>
    public class RecordingErrorHandler : IErrorHandler
    {
        protected IErrorHandler innerErrorHandler;
        protected bool hasError;

        public RecordingErrorHandler(IErrorHandler errorHandler)
        {
            innerErrorHandler = errorHandler;
        }

        public bool HasError
        {
            get
            {
                return hasError;
            }
        }

        public void Error(string message)
        {
            innerErrorHandler.Error(message);
            hasError = true;
        }

        public void Error(string message, Exception exception)
        {
            innerErrorHandler.Error(message, exception);
            hasError = true;
        }

        public void Error(string message, Exception exception, ErrorCode errorCode)
        {
            innerErrorHandler.Error(message, exception, errorCode);
            hasError = true;
        }
    }
}
