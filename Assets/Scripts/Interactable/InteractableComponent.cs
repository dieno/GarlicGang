using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HACKY AS HELL but will work as long as there is only one player
public abstract class InteractableComponent : MonoBehaviour
{
    public GameObject TooltipObject;
    public float TooltipTimeout = 1.0f;

    public bool AllowBumpToUse = false;
    public bool Repeatable = true;

    private float TooltipTimeLeft;
    private bool Locked;
    private bool PlayerInZone;

	protected virtual void Start ()
    {
		
	}
	
	protected virtual void Update ()
    {
		if(PlayerInZone)
        {             
            //handle use
            if(Input.GetButtonDown("Use") && !Locked)
            {
                OnInteract(GameObject.Find("Player"));

                if (!Repeatable)
                    Locked = true;
            }
        }
        else
        {
            //handle tooltip
            TooltipTimeLeft -= Time.deltaTime;
            if (TooltipTimeLeft <= 0)
                TooltipObject.SetActive(false);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(AllowBumpToUse && !Locked)
        {
            OnInteract(collision.gameObject);

            if (!Repeatable)
                Locked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerControl>() != null)
        {
            PlayerInZone = true;
            TooltipObject.SetActive(true);            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControl>() != null && PlayerInZone)
        {
            PlayerInZone = false;
            TooltipTimeLeft = TooltipTimeout;
        }
    }

    protected abstract void OnInteract(GameObject initiator);


}
