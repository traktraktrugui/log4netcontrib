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
    /// This appender takes care of falling back to another appender if appending causes
    /// an error
    /// </summary>
    /// <para>
    /// The appenders are checked in the order they are referenced  in the XML
    /// </para>
    /// <example>
    /// example of XML declaration
    /// <code lang="XML" escaped="true">
    /// <appender name="FallbackAppender" type="log4netContrib.Appender.FallbackAppender, log4netContrib" >
    ///     <appender-ref ref="FileAppender" />
    ///     <appender-ref ref="ConsoleAppender" />
    /// </appender>
    /// </code>
    /// In this example if FileAppender caused an error the append will fallback to ConsoleAppender
    /// </example>
    /// 
    /// <author>Michael Cromwell</author>
    public class FallbackAppender : ForwardingAppender
    {
        protected IList<SafeAppenderProxy> safeAppenderList;

        /// <summary>
        /// Wraps the Appenders with the <see cref="SafeAppenderProxy"/> object
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            safeAppenderList = new List<SafeAppenderProxy>(Appenders.Count);
            foreach (var appender in Appenders)
                safeAppenderList.Add(new SafeAppenderProxy(appender));
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var appenderQueue = new Queue<SafeAppenderProxy>(safeAppenderList);
            while (appenderQueue.Count > 0)
            {
                var appender = appenderQueue.Dequeue();
                
                if (appender.TryAppend(loggingEvent))
                    break;
               
                RecordAppenderError(appenderQueue, appender);
            }
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents == null)
                throw new ArgumentNullException("loggingEvents");
            
            if (loggingEvents.Length == 0)
                throw new ArgumentException("loggingEvents array must not be empty", "loggingEvents");

            if (loggingEvents.Length == 1)
            {
                Append(loggingEvents[0]);
                return;
            }

            var appenderQueue = new Queue<SafeAppenderProxy>(safeAppenderList);
            while (appenderQueue.Count > 0)
            {
                var appender = appenderQueue.Dequeue();
                
                var bulkAppender = appender.Appender as IBulkAppender;
                if (bulkAppender != null)
                {
                    if (appender.TryAppend(loggingEvents))
                        break;
                    
                    RecordAppenderError(appenderQueue, appender);
                }
                else
                {
                    foreach (var logEvent in loggingEvents)
                        if (appender.TryAppend(logEvent))
                            break;
                    
                    RecordAppenderError(appenderQueue, appender);
                }
            }
        }

        private static void RecordAppenderError(Queue<SafeAppenderProxy> appenderQueue, SafeAppenderProxy appender)
        {
            LogLog.Error("appender [" + appender.Appender.Name + "] has an error so is not being appended to.");
            if (appenderQueue.Count > 0)
            {
                var nextAppender = appenderQueue.Peek();
                LogLog.Debug("Chaining through to appender [" + nextAppender.Appender.Name + "]");
            }
            else
                LogLog.Error("No more appenders exist to chain through to");
        }
    }
}
