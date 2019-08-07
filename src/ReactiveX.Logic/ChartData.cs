using System;

namespace ReactiveX.Logic
{
    public class ChartData
    {
        public double Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long EventId { get; set; }

        public override string ToString()
        {
            return $"{nameof(EventId)}: {EventId}, {nameof(Timestamp)}: {Timestamp:ss:fff}, {nameof(Value)}: {Value}";
        }
    }
}