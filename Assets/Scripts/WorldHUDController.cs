﻿using CommonCore.RPG;
using System;
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


    public WeakReference PlayerM;
    public WeakReference PlayerC;
    public Text HealthText;
    public Text AmmoText;
    public Text ArmorText;
    public RawImage FaceImage;
    public RawImage GunImage;
    public Texture2D[] FemaleFaceImages;
    public Texture2D[] MaleFaceImages;

    private string CurrentGun;


    void Update()
    {
        if (PlayerM == null)
            PlayerM = new WeakReference(GameState.Instance.Player);

        if (PlayerC == null)
            PlayerC = new WeakReference(GameObject.Find("Player").GetComponent<PlayerControl>());

        PlayerModel playerModel = (PlayerModel)PlayerM.Target;
        PlayerControl playerController = (PlayerControl)PlayerC.Target;

        float health = playerModel.Health;
        UpdateHealth(health);
        UpdateFace(health, playerModel.MaxHealth, playerModel.Gender);

        int ammo = playerController.BulletsInMagazine;
        UpdateAmmo(ammo);
        UpdateGun(playerController.EquippedWeapon);

        float armor = playerModel.Armor;
        UpdateArmor(armor);
    }

    //decided to go with an observer/polling model which is slower but less interdependent
    private void UpdateHealth(float newHealth)
    {
        HealthText.text = string.Format("{0}", (int)newHealth);

    }

    private void UpdateAmmo(int newAmmo)
    {
        if (newAmmo < 0)
            newAmmo = 0;

        AmmoText.text = string.Format("{0}", newAmmo); 
    }

    private void UpdateArmor(float newArmor)
    {
        ArmorText.text = string.Format("{0}", (int)newArmor);
    }

    private void UpdateFace(float newHealth, float maxHealth, Sex gender)
    {
        Texture2D[] faceTex = gender == Sex.Female ? FemaleFaceImages : MaleFaceImages;
        
        if(newHealth == 0)
        {
            FaceImage.texture = faceTex[faceTex.Length - 1];
        }
        else
        {
            int numSelectable = faceTex.Length - 1;

            float healthFraction = newHealth / maxHealth;
            float healthStep = 1.0f / numSelectable;

            float healthFractionInvert = 1.0f - healthFraction;
            int idx = (int)(healthFractionInvert / healthStep);

            //Debug.Log(idx);
            FaceImage.texture = faceTex[idx];
        }


    }

    private void UpdateGun(string newGun)
    {
        if (string.IsNullOrEmpty(CurrentGun) && newGun == CurrentGun)
            return;

        CurrentGun = newGun;
        var newTex = Resources.Load<Texture2D>("InventoryIcon/" + newGun);
        if(newTex != null)
            GunImage.texture = newTex;
    }
}
