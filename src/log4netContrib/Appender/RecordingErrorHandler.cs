#region licence
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion
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
        protected bool hasError;
        protected string prefix = string.Empty;

        public RecordingErrorHandler(string prefix)
        {
            this.prefix = prefix;
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
            hasError = true;
            if (ShouldLog)
                LogLog.Error("[" + this.prefix + "] " + message);
        }

        public void Error(string message, Exception exception)
        {
            hasError = true;
            if (ShouldLog)
                LogLog.Error("[" + this.prefix + "] " + message, exception);
        }

        public void Error(string message, Exception exception, ErrorCode errorCode)
        {
            hasError = true;
            if (ShouldLog)
                LogLog.Error("[" + this.prefix + "] " + message, exception);
        }

        private static bool ShouldLog
        {
            get
            {
                return !(LogLog.InternalDebugging == false || LogLog.QuietMode);
            }
        }

        internal void ResetError()
        {
            hasError = false;
        }
    }
}
