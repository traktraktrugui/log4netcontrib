using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace log4netContrib
{
    /// <summary>
    /// In conjuction with the <see cref="RecordingErrorHandler" /> object this object
    /// is responsible for trying to append to the appender it wraps and returning whether
    /// the appending caused an error or not
    /// </summary>
    /// 
    /// <author>Michael Cromwell</author>
    public class SafeAppenderProxy
    {
        protected AppenderSkeleton innerAppender;
        private readonly object locker = new object();
        protected bool firstTimeThrough = true;

        public SafeAppenderProxy(IAppender appenderToDecorate)
        {
            var convertedAppender = appenderToDecorate as AppenderSkeleton;
            if (convertedAppender == null)
                throw new InvalidOperationException("cannot use SafeAppenderDecorator with an appender that does not inherit from AppenderSkeleton as it needs to hook into the IErrorHandler, to gather errors.");

            innerAppender = convertedAppender;
            convertedAppender.ErrorHandler = new RecordingErrorHandler(
                                                new OnlyOnceErrorHandler());
        }

        protected void SetFirstTimeThrough(bool value)
        {
            lock (locker)
            {
                firstTimeThrough = value;
            }
        }

        protected bool IsFirstTimeThrough
        {
            get
            {
                lock (locker)
                {
                    return firstTimeThrough;
                }
            }
        }

        public AppenderSkeleton Appender
        {
            get
            {
                return innerAppender;
            }
        }
        
        public bool TryAppend(LoggingEvent loggingEvent)
        {
            return DoAppend(() => innerAppender.DoAppend(loggingEvent));
        }

        public bool TryAppend(LoggingEvent[] loggingEvents)
        {
            return DoAppend(() => innerAppender.DoAppend(loggingEvents));
        }

        protected bool DoAppend(Action appendAction)
        {
            var errorHandler = (RecordingErrorHandler)innerAppender.ErrorHandler;
            if (IsFirstTimeThrough)
            {
                appendAction();
                SetFirstTimeThrough(false);
            }
            else
            {
                if (!errorHandler.HasError)
                    appendAction();
            }

            return !errorHandler.HasError;
        }
    }
}
