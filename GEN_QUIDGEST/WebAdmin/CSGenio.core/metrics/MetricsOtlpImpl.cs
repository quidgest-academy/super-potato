using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using CSGenio.core.di;
using CSGenio.framework;

namespace GenioMVC.Metrics
{
    public class MetricsTimeRecorder : IDisposable
    {
        private readonly Action onDispose;

        public MetricsTimeRecorder(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            onDispose?.Invoke();
        }
    }

    public class MetricsOtlpImpl : IMetricsOtlp
    {
        private Meter meter;
        private readonly Dictionary<string, Histogram<long>> histograms = new();
        private readonly Dictionary<string, Counter<int>> counters = new();
        private readonly InstrumentAdvice<long> default_buckets = new() { HistogramBucketBoundaries = [10, 50, 100, 500, 1000, 5000, 10000, 50000, 500000] };

        public MetricsOtlpImpl(Meter meter)
        {
            this.meter = meter;
        }
        public MetricsOtlpImpl()
        {
            Log.Info("Open Telemetry metrics are not configured, telemtry will not be collected.");
        }

        private bool ValidateMeter()
        {
            return meter != null;
        }

        public IDisposable RecordTime(string metricName, List<KeyValuePair<string, object>> tags, string unit = null, string description = null)
        {
            TagList tagList = new TagList();
            tags.ForEach(tag => tagList.Add(tag.Key, tag.Value));

            return RecordTime(metricName, tagList, unit, description);
        }

        public IDisposable RecordTime(string metricName, TagList tags, string unit = null, string description = null)
        {
            if (!ValidateMeter()) return new MetricsTimeRecorder(() => {});

            Stopwatch timer = Stopwatch.StartNew(); // Start timer

            if (!histograms.TryGetValue(metricName, out var histmetric))
            {
                histmetric = meter.CreateHistogram<long>(
                    metricName,
                    unit,
                    description,
                    null,
                    default_buckets);
                histograms[metricName] = histmetric;
            }

            return new MetricsTimeRecorder(() =>
            {
                timer.Stop(); // Stop timer
                
                histmetric.Record(timer.ElapsedMilliseconds, tags);
            });
        }

        public void RegisterTime(string metricName, long time, TagList tags, string unit = null, string description = null)
        {
            if (!ValidateMeter()) return;

            if (!histograms.TryGetValue(metricName, out var histmetric))
            {
                histmetric = meter.CreateHistogram<long>(
                    metricName, 
                    unit, 
                    description,
                    null,
                    default_buckets);
                histograms[metricName] = histmetric;
            }
            histmetric.Record(time, tags);
        }

        public void IncrementCounter(string metricName, int incrementValue, List<KeyValuePair<string, object>> tags, string unit = null, string description = null)
        {
            TagList tagList = new TagList();
            tags.ForEach(tag => tagList.Add(tag.Key, tag.Value));

            IncrementCounter(metricName, incrementValue, tagList, unit, description);
        }

        public void IncrementCounter(string metricName, int incrementValue, TagList tags, string unit = null, string description = null)
        {
            if (!ValidateMeter()) return;

            if (!counters.TryGetValue(metricName, out var counter))
            {
                counter = meter.CreateCounter<int>(metricName, unit, description);
                counters[metricName] = counter;
            }
            counter.Add(incrementValue, tags);
        }
    }
}
