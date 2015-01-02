EasyGelf
========
Goals: to support up to date version of Gelf and provide reliable integration with popular .Net logging libraries.

Here is a list of supported statuses of popular .net logging library: log4net and NLog.

## Usage

###Minimal configuration:

``` 
<?xml version="1.0" encoding="utf-8"?>
<log4net>
  <appender name="EasyGelfUdpAppender" type=" EasyGelf.Log4Net.GelfUdpAppender, EasyGelf.Log4Net">
    <remoteAddress value="127.0.0.1" />
    <remotePort value="12201" />   
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%message%newline" />
    </layout>
  </appender>
  <appender name="EasyGelfAmqpAppender" type=" EasyGelf.Log4Net.GelfAmqpAppender, EasyGelf.Log4Net">
    <connectionUri value="amqp://" />
    <!-- Will connect to localhost with default amqp credentials-->
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%message%newline" />
    </layout>
  </appender>
  <root>
    <level value="ALL" />
    <appender-ref ref="EasyGelfUdpAppender" />
    <appender-ref ref="EasyGelfAmqpAppender" />
  </root>
</log4net>
``` 

###Additional configuration
####Common

* `includeSource` (default: `true`)
  * Whether the source of the log message should be included

* `hostName` (default: the machine name)
  * The host name of the machine generating the logs

* `facility` (default: `gelf`)
  * The application specific name

* `useBuffering` (default: `true`)
  * Use background thread for IO
 





