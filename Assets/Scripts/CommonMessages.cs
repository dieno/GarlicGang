using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.Messaging;

public class RpgChangeWeaponMessage : QdmsMessage
{

}

public class PlayerDeathMessage : QdmsMessage
{

}

public class InventoryAddedMessage : QdmsMessage
{
    public string Item { get; private set; }
    public int Count { get; private set; }

    public InventoryAddedMessage(string item, int count)
    {
        Item = item;
        Count = count;
    }
}