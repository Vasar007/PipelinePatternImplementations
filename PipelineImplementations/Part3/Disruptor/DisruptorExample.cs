﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;

namespace PipelineImplementations.Part3.Disruptor
{
    internal sealed class EventExample
    {
        public object Value { get; set; }

        public EventExample()
        {
        }
    }

    internal sealed class MyHandler : IEventHandler<EventExample>, IWorkHandler<EventExample>
    {
        public MyHandler()
        {
        }

        public void OnEvent(EventExample data, long sequence, bool endOfBatch)
        {
            OnEvent(data);
        }

        public void OnEvent(EventExample data)
        {
            Console.WriteLine($"EventExample handled: Value = {data.Value} ");
            data.Value += "Finished1";
            Thread.Sleep(1);
        }
    }

    internal sealed class MyHandler2 : IEventHandler<EventExample>, IWorkHandler<EventExample>
    {
        public MyHandler2()
        {
        }

        public void OnEvent(EventExample data, long sequence, bool endOfBatch)
        {
            OnEvent(data);
        }

        public void OnEvent(EventExample data)
        {
            Console.WriteLine($"Handling part 2: Value = {data.Value} ");
            data.Value += "Finished2";
            Thread.Sleep(1);
        }
    }

    internal sealed class DisruptorExample
    {
        private Disruptor<EventExample> _disruptor;

        public DisruptorExample()
        {
        }
        
        public void CreatePipeline()
        {
            _disruptor = new Disruptor<EventExample>(() => new EventExample(), 1024, TaskScheduler.Default, ProducerType.Multi, new BlockingSpinWaitWaitStrategy());

            _disruptor.HandleEventsWithWorkerPool(new MyHandler()).HandleEventsWithWorkerPool(new IWorkHandler<EventExample>[] { new MyHandler2() });
            _disruptor.Start();
        }

        public void Execute(string data)
        {
            var sequence = _disruptor.RingBuffer.Next();
            var disruptorEvent = _disruptor[sequence];
            disruptorEvent.Value = data;
            _disruptor.RingBuffer.Publish(sequence);

        }
    }
}
