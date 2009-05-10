using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;

namespace log4netContrib.Tests
{
    public class StubbingAppender : AppenderSkeleton
    {
        protected int appendCalledCounter = 0;

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            appendCalledCounter++;
        }

        protected override void Append(log4net.Core.LoggingEvent[] loggingEvents)
        {
            appendCalledCounter++;
        }

        public void SetError(string message)
        {
            ErrorHandler.Error(message);
        }

        public int AppendCalledCounter
        {
            get { return appendCalledCounter; }
        }
    }
}
