using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class ProducerWorker<T> where T : class
    {
        public ConsumerWorkerController<T> Controller { get; private set; }
        //public TaskQueue<T> Queue { get; private set; }
        public TaskQueue<T> Queue { get { return Controller.Queue; } }
        public string Name { get; private set; }
        private Thread worker;

        public ProducerWorker(string name, ConsumerWorkerController<T> ctrl)
        {
            //this.Queue = queue;
            this.Controller = ctrl;
            this.Name = name;
            worker = new Thread(ProducerStart);
            worker.Name = name;
            worker.Start();
        }

        public void ProducerStart()
        {
            while (true)
            {
                T task = PushProduct();
                Queue.EnqueueTask(task);
                if (task == null)
                    return;
            }
        }

        /// <summary>
        /// Push product
        /// </summary>
        /// <returns>Return null if needs end.</returns>
        protected virtual T PushProduct()
        {
            throw new NotImplementedException();
        }

        internal void Join()
        {
            worker.Join();
        }
    }
}
