using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class ConsumerWorker<T> where T : class
    {
        public ConsumerWorkerController<T> Controller { get; private set; }
        public TaskQueue<T> Queue { get { return Controller.Queue; } }
        public string Name { get; private set; }
        private Thread worker;

        public ConsumerWorker(string name, ConsumerWorkerController<T> ctrl)
        {
            this.Controller = ctrl;
            this.Name = name;
            worker = new Thread(ConsumerStart);
            worker.Name = name;
            worker.Start();
        }

        public void ConsumerStart()
        {
            while (true)
            {
                T task = Queue.Dequeue();
                if (task == null)
                    return;
                else
                    PullProduct(task);
            }
        }

        protected virtual void PullProduct(T task)
        {
            throw new NotImplementedException();
        }

        internal void Join()
        {
            worker.Join();
        }
    }
}
