using UnityEngine;
using System.Collections;

namespace Ares.ObjectActions
{

    public class ButtonPressTrigger : ActionTrigger
    {
        //TODO: use the new input system as an option
        public string InputCode;

        private bool Locked;

        void Update()
        {
            if (Locked)
                return;

            if (Input.GetButtonDown(InputCode))
            {
                ActionInvokerData d = new ActionInvokerData { Activator = GameObject.Find("Player") }; //TODO utility functions
                Special.Invoke(d);

                if (!Repeatable)
                    Locked = true;
            }

        }
    }
}