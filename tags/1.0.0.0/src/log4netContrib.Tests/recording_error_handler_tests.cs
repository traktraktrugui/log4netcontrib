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
