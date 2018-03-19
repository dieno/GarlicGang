using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldHUDController : MonoBehaviour
{

    public static WorldHUDController Instance { get
        {
            return GameObject.Find("HUDCanvas").GetComponent<WorldHUDController>(); //slow but who fucking cares
        } }


    public Text HealthText;
    public Text AmmoText;

    //TODO figure out messaging instead of stupid method calls

    public void UpdateHealth(float newHealth)
    {
        HealthText.text = string.Format("Health: {0}", (int)newHealth);
    }

    public void UpdateAmmo(int newAmmo)
    {
        AmmoText.text = string.Format("Ammo: {0}", newAmmo); 
    }
}
