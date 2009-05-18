#region licence
//  Copyright 2009 Michael Cromwell

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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using log4net.Core;
using log4net.Util;
using log4netContrib.Appender;

namespace log4netContrib.Tests
{
    [TestFixture]
    public class when_asked_to_record_error_with_message
    {
        [Test]
        public void should_record_as_an_error()
        {
            var sut = new RecordingErrorHandler(
                        new OnlyOnceErrorHandler());
            sut.Error("foo");
            Assert.That(sut.HasError, Iz.True);
        }
    }

    [TestFixture]
    public class when_asked_to_record_error_with_message_and_exception
    {
        [Test]
        public void should_record_as_an_error()
        {
            var sut = new RecordingErrorHandler(
                        new OnlyOnceErrorHandler());
            sut.Error("foo", new Exception("foo"));
            Assert.That(sut.HasError, Iz.True);
        }
    }

    [TestFixture]
    public class when_asked_to_record_error_with_message_and_exception_and_error_code
    {
        [Test]
        public void should_record_as_an_error()
        {
            var sut = new RecordingErrorHandler(
                        new OnlyOnceErrorHandler());
            sut.Error("foo", new Exception("foo"), ErrorCode.GenericFailure);
            Assert.That(sut.HasError, Iz.True);
        }
    }
}
