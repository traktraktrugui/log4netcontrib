using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace log4netContrib.Appender
{
    /// <summary>
    /// In conjuction with the <see cref="RecordingErrorHandler" /> object, this object
    /// is responsible for trying to append to the appender it wraps and returning whether
    /// the appending caused an error or not
    /// </summary>
    /// 
    /// <author>Michael Cromwell</author>
    public class SafeAppenderProxy
    {
        protected AppenderSkeleton innerAppender;
        protected bool firstTimeThrough = true;

        public SafeAppenderProxy(IAppender appenderToWrap)
        {
            var convertedAppender = appenderToWrap as AppenderSkeleton;
            if (convertedAppender == null)
                throw new InvalidOperationException("cannot use SafeAppenderDecorator with an appender that does not inherit from AppenderSkeleton as it needs to hook into the IErrorHandler, to gather errors.");

            innerAppender = convertedAppender;
            convertedAppender.ErrorHandler = new RecordingErrorHandler(
                                                new OnlyOnceErrorHandler());
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
            if (firstTimeThrough)
            {
                appendAction();
                firstTimeThrough = false;
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
