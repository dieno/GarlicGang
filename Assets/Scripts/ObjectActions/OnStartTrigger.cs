using UnityEngine;
using System.Collections;

namespace Ares.ObjectActions
{

    public class OnStartTrigger : ActionTrigger
    {

        void Start()
        {

            ActionInvokerData d = new ActionInvokerData();
            Special.Invoke(d);

        }


    }
}
