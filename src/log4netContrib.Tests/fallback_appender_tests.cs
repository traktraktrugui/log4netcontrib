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
    public class when_fallback_appender_is_activated_: fallback_appender_testing_base
    {
        [Test]
        public void given_mode_is_indefinite_should_wrap_appenders_with_indefinite_proxy()
        {
            var appender = new StubbingAppender();
            var sut = new TestableFallbackAppender();
            sut.AddAppender(appender);
            sut.Mode = FallbackAppenderMode.Indefinite;
            sut.ActivateOptions();
            Assert.That(sut.SafeAppenderList[0], Iz.InstanceOfType(typeof(IndefiniteAppenderProxy)));
        }

        [Test]
        public void given_mode_is_time_should_wrap_appenders_with_time_appender_proxy()
        {
            var appender = new StubbingAppender();
            var sut = new TestableFallbackAppender();
            sut.AddAppender(appender);
            sut.Mode = FallbackAppenderMode.Time;
            sut.ActivateOptions();
            Assert.That(sut.SafeAppenderList[0], Iz.InstanceOfType(typeof(TimeAppenderProxy)));
        }

        [Test]
        public void given_mode_is_count_should_wrap_appenders_with_count_appender_proxy()
        {
            var appender = new StubbingAppender();
            var sut = new TestableFallbackAppender();
            sut.AddAppender(appender);
            sut.Mode = FallbackAppenderMode.Count;
            sut.ActivateOptions();
            Assert.That(sut.SafeAppenderList[0], Iz.InstanceOfType(typeof(CountAppenderProxy)));
        }
    }


    [TestFixture]
    public class when_setting_minutes_timeout : fallback_appender_testing_base
    {
        [Test]
        public void given_the_amount_is_zero_or_less_should_ignore()
        {
            var existing = sut.MinutesTimeout;
            sut.MinutesTimeout = -1;
            Assert.That(sut.MinutesTimeout, Iz.EqualTo(existing));
        }
    }


    [TestFixture]
    public class when_setting_append_count : fallback_appender_testing_base
    {
        [Test]
        public void given_the_amount_is_less_than_one_should_ignore()
        {
            var existing = sut.AppendCount;
            sut.AppendCount = 0;
            Assert.That(sut.AppendCount, Iz.EqualTo(existing));
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
    [Category("integration")]
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


    [TestFixture]
    [Category("integration")]
    public class when_fallback_appender_has_mode_set_in_xml
    {
        [Test]
        public void should_be_available_in_runtime()
        {
            XmlConfigurator.Configure();
            var appender = LogManager.GetRepository().GetAppenders()[0] as FallbackAppender;
            Assert.That(appender, Iz.Not.Null);
            Assert.That(appender.Mode, Iz.EqualTo(FallbackAppenderMode.Time));
        }
    
    }

}
