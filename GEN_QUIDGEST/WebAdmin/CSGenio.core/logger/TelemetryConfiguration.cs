using System;

namespace CSGenio.core.logger
{
    public class TelemetryConfiguration
    {
        public enum LoggerConfigType
        {
            LOG4NET, // Default Value
            OTLP
        }

        public LoggerConfigType LoggerType { get; set; }
        public string CollectorAddress { get; set; }
        public bool EnableTracing { get; set; }
        public bool EnableInternalMetrics { get; set; }
        public string CustomApplicationId { get; set; }
        public string CustomInstanceId { get; set; }
    }
}
