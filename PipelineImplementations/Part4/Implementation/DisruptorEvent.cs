﻿namespace PipelineImplementations.Part4.Implementation
{
    internal sealed class DisruptorEvent
    {
        public object Value { get; set; }

        public object TaskCompletionSource { get; set; }

        public DisruptorEvent()
        {
        }

        public T Read<T>()
        {
            // TODO: use unsafe code to prevent boxing for small blittable value types.
            return (T) Value;
        }

        public void Write<T>(T value)
        {
            // TODO: use unsafe code to prevent boxing for small blittable value types.
            Value = value;
        }
    }
}