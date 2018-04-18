using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Ares.ObjectActions
{

    public struct ActionInvokerData
    {
        public GameObject Activator;
    }

    [System.Serializable]
    public class ActionSpecialEvent : UnityEvent<ActionInvokerData> { }

    public abstract class ActionSpecial : MonoBehaviour
    {

        public bool Repeatable = true;

        public abstract void Execute(ActionInvokerData data);
        	
    }
}