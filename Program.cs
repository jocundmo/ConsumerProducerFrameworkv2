using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ConsumerProducerFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // When consumer count is larger than producer count.
            // Currnet implementation is not working, coz there is only one EndProd is produced
            // So there is only one consumer get thie EndProd and stopped, the other two are still waiting......
            using (ConsumerWorkerController<Book> ctrl = new ConsumerWorkerController<Book>(5, 2))
            {
                ctrl.CreateConsumer += new ConsumerWorkerController<Book>.CreateConsumerEvent(ctrl_CreateWorker);
                ctrl.CreateProducer += new ConsumerWorkerController<Book>.CreateProducerEvent(ctrl_CreateProducer);
                ctrl.Initialize();
            }

            sw.Stop();
            Console.WriteLine("Last Seconds: " + sw.Elapsed.Seconds);
            Console.Read();
        }

        static ProducerWorker<Book> ctrl_CreateProducer(ConsumerWorkerController<Book> workerController, string producerName)
        {
            return new TestProducer1(producerName, workerController);
        }

        static ConsumerWorker<Book> ctrl_CreateWorker(ConsumerWorkerController<Book> workerController, string workerName)
        {
            return new TestConsumer1(workerName, workerController);
        }
    }
}
