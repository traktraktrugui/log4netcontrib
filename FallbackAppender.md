# What does it do? #

The `FallbackAppender` is similar to the `ForwardAppender` that comes with log4net in that it references other appenders via the `appender-ref` element. The difference is that while the `ForwardAppender` will append to all the contained appenders the `FallbackAppender` will instead queue each of the appenders and will try to append to the first in the queue if successful it will stop if it isn't successful it will try the next one in the queue and will do this until one is successful or all fail.

# Usage #

```
<appender name="FallbackAppender" type="log4netContrib.Appender.FallbackAppender, log4netContrib" >
    <appender-ref ref="FileAppender" />
    <appender-ref ref="ConsoleAppender" />
    <mode value="Indefinite" />
</appender>
```

Where `FileAppender` and `ConsoleAppender` are declared separately.

# Things to be aware of #

  * The appenders will be queued in the order they are placed in the XML declaration.
  * The appenders to be referenced **must inherit from `AppenderSkeleton` not just `IAppender`** I know this is a bit naff I will discuss this below

# How does it work? #

This explains why the appenders being referenced must inherit from `AppenderSkeleton`, appenders used in log4net don't allow exceptions to bubble up through the stack instead they log errors to an `ErrorHandler` property they all have from inheriting from `AppenderSkeleton` so in order to know whether an appender has caused an error we need to hook into this object, so I basically replace the `IErrorHandler` with a custom object so I can track when errors are caused from an appender, and can take action by using the next queued appender.

# The Modes #

One of the problems of the earlier implementations was what to do with an appender that has an error the simplest and the default is to ignore the appender indefinitely however this is not a good solution if the error was caused by a transient action i.e. the network was down for a few minutes as opposed to an action that is indefinite such as the config file being incorrectly configured.

We don't want to have active monitoring for the error'ed appenders as this starts to use up resources and moves away from the lightweight logging library it currently is, therefore I came up with 3 simple modes to allow more control over how to handle when an appender goes into a error'ed state:

## Details ##

| **Mode** | **Description** | **Settings** |
|:---------|:----------------|:-------------|
| Indefinite (Default) | This mode is the simplest and simply states that if an appender goes into an error'ed state then it will be ignored until the log4net configuration is reloaded | _N/A_        |
| Count    | This mode will actively count the number of appends that are attempted against the appender from the moment it goes into an error'ed state and when the append attempts reaches the `AppendCount` the appender will be reset from the error'ed state | `AppendCount Int32` _(20)_ |
| Time     | This mode will timestamp the moment that an appender goes into an error'ed state and at each append attempt it will check to see if the `MinutesTimeout` set has been reached and if so will reset the appender from being in the error'ed state | `MinutesTimeout Int32` _(5)_ |

## Examples ##

### Indefinite ###

```
<appender name="FallbackAppender" type="log4netContrib.Appender.FallbackAppender, log4netContrib" >
    <appender-ref ref="FileAppender" />
    <appender-ref ref="ConsoleAppender" />
    <mode value="Indefinite" />
</appender>
```

### Count ###

```
<appender name="FallbackAppender" type="log4netContrib.Appender.FallbackAppender, log4netContrib" >
    <appender-ref ref="FileAppender" />
    <appender-ref ref="ConsoleAppender" />
    <mode value="Count" />
    <AppendCount value="50"/>
</appender>
```

### Time ###

```
<appender name="FallbackAppender" type="log4netContrib.Appender.FallbackAppender, log4netContrib" >
    <appender-ref ref="FileAppender" />
    <appender-ref ref="ConsoleAppender" />
    <mode value="Time" />
    <MinutesTimeout value="10"/>
</appender>
```

## Notes ##

When deciding to use the **Indefinite** mode it's worth using push model appenders rather than pull i.e. email, messaging rather than file, database this way you are actively notified that your first appender has error'ed.