using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class TestConsumer1 : ConsumerWorker<Book>
    {
        public TestConsumer1(string name, ConsumerWorkerController<Book> ctrl)
            : base(name, ctrl)
        {
        }


        protected override void PullProduct(Book task)
        {
            Console.WriteLine(string.Format("Consuming...ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId, task.Name));
            Thread.Sleep(1000);
        }
    }
}
