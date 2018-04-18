using Ares.ObjectActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInteractableComponent : InteractableComponent
{
    public ActionSpecialEvent Special;

    protected override void OnInteract(GameObject initiator)
    {
        Special.Invoke(new ActionInvokerData { Activator = initiator });
    }
}
