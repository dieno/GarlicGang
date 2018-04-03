using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.Messaging
{

    //not actually an interface
    public class QdmsMessageInterface
    {
        internal Queue<QdmsMessage> MessageQueue;

        public QdmsMessageInterface()
        {
            MessageQueue = new Queue<QdmsMessage>();

            //register
            QdmsMessageBus.Instance.RegisterReceiver(this);
        }

        ~QdmsMessageInterface()
        {
            QdmsMessageBus.Instance.UnregisterReceiver(this);
        }

        public bool HasMessageInQueue()
        {
            return MessageQueue.Count > 0;
        }

        public int CountMessagesInQueue()
        {
            return MessageQueue.Count;
        }

        public QdmsMessage PopFromQueue()
        {
            if (MessageQueue.Count > 0)
                return MessageQueue.Dequeue();

            return null;
        }

        public void PushToBus(QdmsMessage msg)
        {
            msg.SetSender(this);
            QdmsMessageBus.Instance.PushBroadcast(msg);
        }

    }
}