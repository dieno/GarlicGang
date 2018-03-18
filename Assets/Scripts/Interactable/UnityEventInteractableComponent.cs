using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInteractableComponent : InteractableComponent
{
    public UnityEvent Event;

    protected override void OnInteract(GameObject initiator)
    {
        Event.Invoke();
    }
}
