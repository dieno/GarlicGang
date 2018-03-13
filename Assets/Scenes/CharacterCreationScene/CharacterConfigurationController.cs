using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CharacterConfigurationController : MonoBehaviour
{
    private enum GunType
    {
        Revolver, Pistol
    }

    public GameObject NotImplementedMessageObject;

    public GameObject ProfileF;
    public GameObject ProfileM;

    public GameObject GunR;
    public GameObject GunP;

    public AudioSource AudioFlip;

    private Sex PickedSex;
    private GunType PickedGun;
	
	void Start ()
    {
        PickedSex = Sex.Male;
        PickedGun = GunType.Revolver;
	}
	
	
	void Update ()
    {
		
	}

    public void OnClickGender()
    {
        if(PickedSex == Sex.Male)
        {
            ProfileM.SetActive(false);
            ProfileF.SetActive(true);
            PickedSex = Sex.Female;
        }
        else if(PickedSex == Sex.Female)
        {
            ProfileF.SetActive(false);
            ProfileM.SetActive(true);
            PickedSex = Sex.Male;
        }

        AudioFlip.Play();
    }

    public void OnClickGun()
    {
        if(PickedGun == GunType.Revolver)
        {
            GunR.SetActive(false);
            GunP.SetActive(true);
            PickedGun = GunType.Pistol;
        }
        else if(PickedGun == GunType.Pistol)
        {
            GunP.SetActive(false);
            GunR.SetActive(true);
            PickedGun = GunType.Revolver;
        }
        AudioFlip.Play();
    }

    public void OnClickFakeButton()
    {
        AudioFlip.Play();
        NotImplementedMessageObject.SetActive(true);
    }

    public void OnClickExit()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnClickFinish()
    {
        FinishConfig();
    }

    private void FinishConfig()
    {
        GameState.Purge();
        GameState.Instance.Player.Gender = PickedSex;
        switch (PickedGun)
        {
            case GunType.Revolver:
                GameState.Instance.Player.AddItem("revolver");
                break;
            case GunType.Pistol:
                GameState.Instance.Player.AddItem("m1911");
                break;
        }
        GameState.Save();

        GameState.Instance.CurrentDialogue = "intro.intro1";
        SceneManager.LoadScene("DialogueScene");
    }
}
