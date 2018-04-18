using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.Messaging;

namespace Ares.ObjectActions
{
    public class GrantInventorySpecial : ActionSpecial
    {
        public string Item;
        public int ItemCount = 1;

        private bool Locked;

        public override void Execute(ActionInvokerData data)
        {
            if (Locked)
                return;

            GameState.Instance.Player.AddItem(Item, ItemCount);
            QdmsMessageBus.Instance.PushBroadcast(new InventoryAddedMessage(Item, ItemCount));

            if (!Repeatable)
                Locked = true;
        }
    }
}