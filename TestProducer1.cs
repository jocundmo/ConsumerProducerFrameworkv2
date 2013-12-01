using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class TestProducer1 : ProducerWorker<Book>
    {
        public int Index { get; private set; }

        public TestProducer1(string name, ConsumerWorkerController<Book> ctrl)
            : base(name, ctrl)
        {
        }

        protected override Book PushProduct()
        {
            Thread.Sleep(250);
            if (Index == 10)
            {
                this.Controller.EndProduce();
                return null;
            }
            Book b = new Book(Index + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(string.Format("Producingggggggg...ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId, b.Name));
            
            Index++;
            return b;
        }
    }
}
