namespace RedTop.Common.NotificationManager
{   
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public abstract class TaskQueue : ITaskQueue
    {

        private static Queue _Queue = new Queue();
        private static bool _Busy = false;
        private static object _LockObject = new object();
        private static Dictionary<QueueTaskDelegate, dynamic> _QueueTaskDataDictionary = new Dictionary<QueueTaskDelegate, dynamic>();

        private delegate void QueueTaskDelegate(dynamic data);
        private QueueTaskDelegate _queueTaskDelegate;
        protected abstract void PerformTask(dynamic userData);

        public void Enqueue(dynamic data)
        {
            _queueTaskDelegate = new QueueTaskDelegate(PerformTask);

            lock (_LockObject)
            {
                if (_Busy)
                {
                    _Queue.Enqueue(_queueTaskDelegate);
                    _QueueTaskDataDictionary.Add(_queueTaskDelegate, data);
                }
                else
                {
                    _Busy = true;
                    _queueTaskDelegate.BeginInvoke(data, new AsyncCallback(this.QueueTaskCallback), _queueTaskDelegate);
                }
            }
        }


        private void QueueTaskCallback(IAsyncResult ar)
        {
            QueueTaskDelegate queueTaskDelegate = ar.AsyncState as QueueTaskDelegate;

            if (queueTaskDelegate.Equals(_queueTaskDelegate))
            {
                queueTaskDelegate.EndInvoke(ar);
            }

            RunNextTaskInQueue();
        }
        private void RunNextTaskInQueue()
        {
            QueueTaskDelegate queueTaskDelegate;
            lock (_LockObject)
            {
                if (_Queue.Count > 0)
                {
                    queueTaskDelegate = (QueueTaskDelegate)_Queue.Dequeue();
                    dynamic data = _QueueTaskDataDictionary[queueTaskDelegate];
                    _QueueTaskDataDictionary.Remove(queueTaskDelegate);
                    queueTaskDelegate.BeginInvoke(data, new AsyncCallback(this.QueueTaskCallback), queueTaskDelegate);
                }
                else
                    _Busy = false;
            }
        }

    }
}
