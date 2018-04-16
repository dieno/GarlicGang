using UnityEngine;
using System.Collections;

namespace Ares.ObjectActions
{

    public class OnTriggerEnterTrigger : ActionTrigger
    {

        public bool OnPlayerOnly = true;
        public bool OnActorsOnly = true;

        public bool CheckAllCollisions = false;
        public bool Enabled3D = true;
        public bool Enabled2D = true;

        private bool Locked;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!Enabled2D)
                return;

            HandleCollision(collision.gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if(CheckAllCollisions)
                OnTriggerEnter2D(collision.collider);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!Enabled3D)
                return;

            HandleCollision(other.gameObject);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (CheckAllCollisions)
                OnTriggerEnter(collision.collider);
        }

        private void HandleCollision(GameObject other)
        {
            //Debug.Log(other);

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
            var data = new ActionInvokerData() { Activator = activator };
            Special.Invoke(data);

            //lock if not repeatable
            if (!Repeatable)
                Locked = true;
        }

    }
}
