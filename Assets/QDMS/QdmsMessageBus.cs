using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.Messaging
{

    public class QdmsMessageBus
    {
        public static QdmsMessageBus Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new QdmsMessageBus();

                return _Instance;
            }
        }
        private static QdmsMessageBus _Instance;

        private QdmsMessageBus()
        {
            Receivers = new List<QdmsMessageInterface>();
        }

        private List<QdmsMessageInterface> Receivers;

        internal void PushBroadcast(QdmsMessage msg)
        {
            foreach(QdmsMessageInterface r in Receivers)
            {
                try
                {
                    r.MessageQueue.Enqueue(msg);
                }
                catch(Exception e) //steamroll errors
                {
                    Debug.Log(e);
                }
            }
        }

        internal void RegisterReceiver(QdmsMessageInterface receiver)
        {
            Receivers.Add(receiver);
        }

        internal void UnregisterReceiver(QdmsMessageInterface receiver)
        {
            Receivers.Remove(receiver);
        }

        public void ForceCreate()
        {
            Instance.GetType();
        }

    }
}