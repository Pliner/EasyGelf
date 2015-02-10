using System;
using System.Reflection;

// EasyGelf version number: <major>.<minor>.<non-breaking-feature>.<build>
[assembly: AssemblyVersion("0.4.0.0")]
[assembly: CLSCompliant(true)]

//0.4.0.0 Downgrade to .Net 4.0 and log4net 1.2.10                                
//0.3.8.0 Inconsistent GelfMessage serialization, Increase default udp packet size                                
//0.3.7.0 Remove JetBrains.Annotation and Newtonsoft.Json dependencies                                
//0.3.6.0 IncludeStackTrace option                                
//0.3.5.0 RemoteAddress could be dns name                                 
//0.3.4.0 Now retries are supported                                 
//0.3.3.0 AmqpTarget for NLog                                 
//0.3.2.0 Delete UseBuffering option. Now it's true forever.
//0.3.1.0 UdpTarget RemoteAddress initialization 
//0.3.0.0 Basic NLog support 
//0.2.3.0 Facility, LoggerName, ThreadName and Source Info
//0.2.2.0 Transport Encoders
//0.2.1.0 Buffered Transport
//0.2.0.0 Merge all external dependencies except log4net into one assembly
//0.1.0.0 Initial

