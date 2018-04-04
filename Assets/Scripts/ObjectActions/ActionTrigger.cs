using UnityEngine;
using System.Collections;

namespace Ares.ObjectActions
{

    public abstract class ActionTrigger : MonoBehaviour
    {
        public bool Repeatable = false;

        public ActionSpecialEvent Special;

    }
}