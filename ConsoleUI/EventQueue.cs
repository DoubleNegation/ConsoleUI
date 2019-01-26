using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleUI {

    class EventQueue {

        private Queue<ThreadStart> asapQueue;
        private Queue<ThreadStart> queue;
        private Queue<ThreadStart> postQueueQueue;

        public EventQueue() {
            asapQueue = new Queue<ThreadStart>();
            queue = new Queue<ThreadStart>();
            postQueueQueue = new Queue<ThreadStart>();
        }

        public void CallAllActions() {
            while((queue.Count + postQueueQueue.Count) > 0) {if(queue.Count > 0) {
                    queue.Dequeue()();
                } else {
                    postQueueQueue.Dequeue()();
                }
            }
        }

        /// <summary>
        /// Adds an action to the main queue.
        /// </summary>
        public void Add(ThreadStart action) {
            queue.Enqueue(action);
        }

        /// <summary>
        /// Adds an action to the post-queue queue, which is executed once the main queue is empty.
        /// </summary>
        public void PQQAdd(ThreadStart action) {
            postQueueQueue.Enqueue(action);
        }

    }

}

