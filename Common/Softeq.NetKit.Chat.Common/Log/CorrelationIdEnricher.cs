// Developed by Softeq Development Corporation
// http://www.softeq.com

using CorrelationId;
using Serilog.Core;
using Serilog.Events;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.Common.Log
{
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationIdEnricher(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId;
            var correlationIdProperty = new LogEventProperty(PropertyNames.CorrelationId, new ScalarValue(correlationId ?? "unknown"));

            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }
    }
}