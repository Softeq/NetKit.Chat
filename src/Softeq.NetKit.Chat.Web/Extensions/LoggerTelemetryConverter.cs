// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Softeq.Serilog.Extension;
using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Web.Extensions
{
    internal class LoggerTelemetryConverter : TelemetryConverterBase
    {
        public override IEnumerable<ITelemetry> Convert(LogEvent serilogLogEvent, IFormatProvider formatProvider)
        {
            if (serilogLogEvent.Exception == null)
            {
                if (serilogLogEvent.Properties.ContainsKey(PropertyNames.EventId))
                {
                    var eventTelemetry = new EventTelemetry(serilogLogEvent.Properties[PropertyNames.EventId].ToString())
                    {
                        Timestamp = serilogLogEvent.Timestamp
                    };
                    ForwardPropertiesToTelemetryProperties(serilogLogEvent, eventTelemetry, formatProvider);
                    yield return eventTelemetry;
                }
                else
                {
                    var exceptionTelemetry = new ExceptionTelemetry(new Exception($"Event does not contain '{PropertyNames.EventId}' property"))
                    {
                        Timestamp = serilogLogEvent.Timestamp
                    };
                    ForwardPropertiesToTelemetryProperties(serilogLogEvent, exceptionTelemetry, formatProvider);
                    yield return exceptionTelemetry;
                }
            }
            yield return ToExceptionTelemetry(serilogLogEvent, formatProvider);
        }
    }
}
