using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using log4net.Core;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using log4net.Appender;
using log4netContrib.Appender;

namespace log4netContrib.Tests
{
    [TestFixture]
    public class when_safe_appender_proxy_is_created
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void given_appender_to_decorate_does_not_inherit_from_appender_skeleton_should_throw()
        {
            var sut = new SafeAppenderProxy(
                        MockRepository.GenerateStub<IAppender>());
        }

        [Test]
        public void should_set_recording_error_handler_on_appender()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            Assert.That(appender.ErrorHandler, Iz.InstanceOfType(typeof(RecordingErrorHandler)));
        }
    }

    [TestFixture]
    public class when_safe_appender_proxy_asked_to_try_and_append_single_event
    {
        [Test]
        public void given_it_is_the_first_time_through_should_call_append_on_decorated_appender()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            Assert.That(appender.AppendCalledCounter, Iz.EqualTo(1));
        }

        [Test]
        public void given_it_is_first_time_through_and_error_raised_should_return_if_succeeded()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            appender.SetError("foo");
            var result = sut.TryAppend(new LoggingEvent(
                                        new LoggingEventData()));
            Assert.That(result, Iz.False);
        }

        [Test]
        public void given_second_time_through_and_appender_has_no_error_should_append()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            Assert.That(appender.AppendCalledCounter, Iz.EqualTo(2));
        }

        [Test]
        public void given_second_time_through_and_appender_has_error_should_not_append()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            appender.SetError("foo");
            sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            Assert.That(appender.AppendCalledCounter, Iz.EqualTo(1));
        }

        [Test]
        public void given_second_time_through_and_appender_has_error_should_return_not_succeeded()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            appender.SetError("foo");
            var result = sut.TryAppend(new LoggingEvent(
                            new LoggingEventData()));
            Assert.That(result, Iz.False);
        }
    }

    [TestFixture]
    public class when_safe_appender_proxy_asked_to_try_and_append_multiple_events
    {
        LoggingEvent[] loggingEvents = new LoggingEvent[] { 
            new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            }),
            new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            })};

        [Test]
        public void given_it_is_the_first_time_through_should_call_append_on_decorated_appender()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(loggingEvents);
            Assert.That(appender.AppendCalledCounter, Iz.EqualTo(1));
        }

        [Test]
        public void given_it_is_first_time_through_and_error_raised_should_return_if_succeeded()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            appender.SetError("foo");
            var result = sut.TryAppend(loggingEvents);
            Assert.That(result, Iz.False);
        }

        [Test]
        public void given_second_time_through_and_appender_has_no_error_should_append()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(loggingEvents);
            sut.TryAppend(loggingEvents);
            Assert.That(appender.AppendCalledCounter, Iz.EqualTo(2));
        }

        [Test]
        public void given_second_time_through_and_appender_has_error_should_not_append()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(loggingEvents);
            appender.SetError("foo");
            sut.TryAppend(loggingEvents);
            Assert.That(appender.AppendCalledCounter, Iz.EqualTo(1));
        }

        [Test]
        public void given_second_time_through_and_appender_has_error_should_return_not_succeeded()
        {
            var appender = new StubbingAppender();
            var sut = new SafeAppenderProxy(appender);
            sut.TryAppend(loggingEvents);
            appender.SetError("foo");
            var result = sut.TryAppend(loggingEvents);
            Assert.That(result, Iz.False);
        }
    }
}
