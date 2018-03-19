using UnityEngine;
using System.Collections;

namespace Ares.ObjectActions
{

    public class OnTriggerEnterTrigger : ActionTrigger
    {

        public bool OnPlayerOnly = true;
        public bool OnActorsOnly = true;

        public bool CheckAllCollisions = false;

        private bool Locked;

        void OnTriggerEnter(Collider other)
        {
            if (Locked)
                return;

            //reject not-player if we're not allowing not-player
            if (OnPlayerOnly && other.GetComponent<PlayerControl>() == null)
                return;

            //reject non-actors if we're not allowing not-actor
            if (OnActorsOnly && (other.GetComponent<PlayerControl>() == null || other.GetComponent<EnemyScript>() == null))
                return;

            //execute special
            var activator = other.gameObject;
            var data = new ActionInvokerData() {Activator = activator};
            Special.Invoke(data);

            //lock if not repeatable
            if (!Repeatable)
                Locked = true;

        }

        void OnCollisionEnter(Collision collision)
        {
            if (CheckAllCollisions)
                OnTriggerEnter(collision.collider);
        }

    }
}
