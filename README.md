EasyGelf
========
Goals: to support up to date version of Gelf and provide reliable integration with popular .Net logging libraries.

Here is a list of supported statuses of popular .net logging library:
1. Apache log4net(fully supported)
2. NLog(not supported, in future plans)


## Usage

#Here is a minimal configuration:

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


