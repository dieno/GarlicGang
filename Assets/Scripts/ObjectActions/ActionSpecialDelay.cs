﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ares.ObjectActions
{

    public class ActionSpecialDelay : ActionSpecial
    {
        public ActionSpecialEvent Special;
        public float Delay;
        public bool Concurrent;

        private bool Locked;

        public override void Execute(ActionInvokerData data)
        {
            if (Locked)
                return;

            StartCoroutine(WaitAndExecute(data));

            if (!Concurrent || !Repeatable)
                Locked = true;
        }

        private IEnumerator WaitAndExecute(ActionInvokerData data)
        {
            yield return new WaitForSeconds(Delay);
            Special.Invoke(data);

            if (Repeatable)
                Locked = false;
        }

    }
}