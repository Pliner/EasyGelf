EasyGelf(The latest version is 0.3.6.0)
========
Goals: to support up to date version of Gelf and provide reliable integration with popular .Net logging libraries.

Now log4net and NLog are supported. Also Udp, Tcp and Amqp protocols are supported.

## Usage(log4net)

###Minimal configuration:

``` 
<?xml version="1.0" encoding="utf-8"?>
<log4net>
  <appender name="GelfUdpAppender" type=" EasyGelf.Log4Net.GelfUdpAppender, EasyGelf.Log4Net">
    <remoteAddress value="127.0.0.1" />
    <remotePort value="12201" />   
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%message%newline" />
    </layout>
  </appender>
  <appender name="GelfAmqpAppender" type=" EasyGelf.Log4Net.GelfAmqpAppender, EasyGelf.Log4Net">
    <connectionUri value="amqp://" />
    <!-- Will connect to localhost with default amqp credentials-->
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%message%newline" />
    </layout>
  </appender>
  <root>
    <level value="ALL" />
    <appender-ref ref="GelfUdpAppender" />
    <appender-ref ref="GelfAmqpAppender" />
  </root>
</log4net>
```                                

## Usage(NLog)

###Minimal configuration:

```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="EasyGelf.NLog"/>
  </extensions>
  <targets>
    <target name="GelfUdp" xsi:type="GelfUdp" facility="Easy Gelf Example Application" remoteAddress="127.0.0.1" remotePort="12201" layout="${message}"/>
    <target name="GelfAmqp" xsi:type="GelfAmqp" facility="Easy Gelf Example Application" connectionUri="amqp://" layout="${message}"/>
  </targets>
  <rules>
    <logger name="*" minlevel="Info" writeTo="GelfUdp" />
    <logger name="*" minlevel="Info" writeTo="GelfAmqp" />
  </rules>
</nlog>

```



##Additional configuration
###Common

* `includeSource` (default: `true`)
  * Whether the source of the log message should be included

* `hostName` (default: the machine name)
  * The host name of the machine generating the logs

* `facility` (default: `gelf`)
  * The application specific name

* `useRetry` (default: `true`)
  * Allow to retry send log message
  * `retryCount` (default: 5) 
	* Count of retry attemps 
  * `retryDelay` (default: 50ms)
	* Pause between retry attempts


* `includeStackTrace` (default: `true`)
  * Will include exception message and exception stack trace
	

