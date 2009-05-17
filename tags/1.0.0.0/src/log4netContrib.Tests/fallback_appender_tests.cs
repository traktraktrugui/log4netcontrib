using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using log4net.Config;
using log4net.Util;
using log4netContrib.Appender;
using NUnit.Framework.SyntaxHelpers;
using log4net;

namespace log4netContrib.Tests
{
    public class fallback_appender_testing_base
    {
        protected FallbackAppender sut;

        [SetUp]
        public void Setup()
        {
            sut = createSUT();
            sut.Threshold = Level.All;
            sut.ActivateOptions();
        }

        protected virtual FallbackAppender createSUT()
        {
            return new FallbackAppender();
        }

        [TearDown]
        public void TearDown()
        {
            sut = null;
        }
    }
            
    [TestFixture]
    public class when_fallback_appender_is_appending_single_event : fallback_appender_testing_base
    {
        StubbingAppender firstAppender;
        StubbingAppender secondAppender;

        protected override FallbackAppender createSUT()
        {
            var sut = base.createSUT();
            firstAppender = new StubbingAppender();
            secondAppender = new StubbingAppender();
            sut.AddAppender(firstAppender);
            sut.AddAppender(secondAppender);
            sut.ActivateOptions();
            return sut;
        }
        
        [Test]
        public void given_first_appender_did_not_succeed_appending_should_use_next_appender()
        {
            firstAppender.SetError("foo");
            var loggingEvent = new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            });
            sut.DoAppend(loggingEvent);
            Assert.That(secondAppender.AppendCalledCounter, Iz.EqualTo(1));
        }

        [Test]
        public void given_first_appender_succeeded_in_appending_should_not_forward_to_next_appender()
        {
            var loggingEvent = new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            });
            sut.DoAppend(loggingEvent);
            Assert.That(firstAppender.AppendCalledCounter, Iz.EqualTo(1), "firstAppender");
            Assert.That(secondAppender.AppendCalledCounter, Iz.EqualTo(0), "secondAppender");
        }

        [Test]
        public void given_all_appenders_do_not_succeed_in_appending_should_handle_ok()
        {
            firstAppender.SetError("foo");
            secondAppender.SetError("bar");
            var loggingEvent = new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            });
            sut.DoAppend(loggingEvent);
        }
    }

    [TestFixture]
    public class when_fallback_appender_is_appending_multiple_events : fallback_appender_testing_base
    {
        StubbingAppender firstAppender;
        StubbingAppender secondAppender;
        LoggingEvent[] loggingEvents = new LoggingEvent[] { 
            new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            }),
            new LoggingEvent(new LoggingEventData()
            {
                Level = Level.Error
            })};
        
        protected override FallbackAppender createSUT()
        {
            var sut = base.createSUT();
            firstAppender = new StubbingAppender();
            secondAppender = new StubbingAppender();
            sut.AddAppender(firstAppender);
            sut.AddAppender(secondAppender);
            sut.ActivateOptions();
            return sut;
        }

        [Test]
        public void given_first_appender_did_not_succeed_in_appending_should_use_next_appender()
        {
            firstAppender.SetError("foo");
            sut.DoAppend(loggingEvents);
            Assert.That(secondAppender.AppendCalledCounter, Iz.EqualTo(1));
        }

        [Test]
        public void given_first_appender_succeeded_in_appending_should_not_forward_to_next_appender()
        {
            sut.DoAppend(loggingEvents);
            Assert.That(firstAppender.AppendCalledCounter, Iz.EqualTo(1), "firstAppender");
            Assert.That(secondAppender.AppendCalledCounter, Iz.EqualTo(0), "secondAppender");
        }

        [Test]
        public void given_all_appenders_do_not_succeed_in_appending_should_handle_ok()
        {
            firstAppender.SetError("foo");
            secondAppender.SetError("bar");
            sut.DoAppend(loggingEvents);
        }
    }

    [TestFixture]
    public class when_fallback_appender_is_being_appended_to 
    {
        [Test]
        public void should_log_to_console()
        {
            XmlConfigurator.Configure();
            var log = LogManager.GetLogger("logger");
            log.Debug("foo");
        }
    }

}
