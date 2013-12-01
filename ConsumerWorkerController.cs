using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class ConsumerWorkerController<T> : IDisposable where T : class
    {
        TaskQueue<T> queue;
        ConsumerWorker<T>[] consumers;
        ProducerWorker<T>[] producers;
        int consumerCount = 0;
        int producerCount = 0;

        object locker = new object();
        public int ProducerEndedCount { get; private set; }
        public int ConsumerEndedCount { get; private set; }
        public void EndProduce()
        {
            lock (locker)
            {
                Console.WriteLine("Pulse...");
                ProducerEndedCount++;
                Monitor.PulseAll(locker);
            }
        }

        public delegate ConsumerWorker<T> CreateConsumerEvent(ConsumerWorkerController<T> workerController, string consumerName);
        public event CreateConsumerEvent CreateConsumer;
        public delegate ProducerWorker<T> CreateProducerEvent(ConsumerWorkerController<T> workerController, string producerName);
        public event CreateProducerEvent CreateProducer;

        public TaskQueue<T> Queue { get { return queue; } set { queue = value; } }

        public ConsumerWorker<T>[] Consumers { get { return consumers; } }
        public ProducerWorker<T>[] Producers { get { return producers; } }

        public void Initialize()
        {
            if (consumerCount <= 0)
                throw new Exception("worker count couldn't less than 0...");
            if (CreateConsumer == null)
                throw new Exception("method to create worker not attached...");
            queue = new TaskQueue<T>();
            // Initialize the consumers
            consumers = new ConsumerWorker<T>[consumerCount];

            for (int i = 0; i < consumerCount; i++)
            {
                if (CreateConsumer != null)
                    consumers[i] = CreateConsumer(this, i.ToString());
            }

            // Initialize the producers
            producers = new ProducerWorker<T>[producerCount];

            for (int i = 0; i < producerCount; i++)
            {
                if (CreateProducer != null)
                    producers[i] = CreateProducer(this, i.ToString());
            }
        }

        public ConsumerWorkerController(int consumerCount, int producerCount)
        {
            this.consumerCount = consumerCount;
            this.producerCount = producerCount;
        }

        public void Dispose()
        {
            // Enqueue one null task per worker to make each exit.
            while (true)
            {
                // Once one Producer is completed, the producer thread will pulse the main thread
                // to check whether all producers are completed or not.
                // if true, then send the null task to Queue for each consumers
                Monitor.Enter(locker);
                try
                {
                    // Monitor.Wait(locker) -->这里Wait原来是放在这个位置的，但当把断点放在
                    // 下一行 if (ProducerEndedCount == this.Producers.Length) 上，
                    // 稍等片刻，就会卡死，主线程会卡在 Wait 这里， 因为几次Pulse已经都过去了(貌似Pulse是不用申请到锁的)，但主线程
                    // 在Wait，没有机会执行退出代码。总结出来的原则是，Wait不能放在具体逻辑之前。。。也就是Wait不能阻止逻辑的运行，
                    // 只能单纯的卡主循环
                    if (ProducerEndedCount == this.Producers.Length)
                    {
                        foreach (ConsumerWorker<T> worker in consumers)
                            queue.EnqueueTask(default(T));
                        foreach (ConsumerWorker<T> worker in consumers)
                            worker.Join();
                        break;

                    }
                    Monitor.Wait(locker);
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
        }
    }
}
