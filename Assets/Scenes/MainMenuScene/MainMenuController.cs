using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    public GameObject VideoPanel;
    public VideoPlayer VideoComponent;
    public GameObject SettingsPanel;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void OnClickContinue()
    {
        GameState.Restore();
        SceneManager.LoadScene(GameState.Instance.CurrentScene);
    }

    public void OnClickNew()
    {
        GameState.Purge();
        SceneManager.LoadScene("CharacterCreationScene");
    }

    public void OnClickLoad()
    {
        GameState.Restore();
        SceneManager.LoadScene(GameState.Instance.CurrentScene);
    }

    public void OnClickMods()
    {
        Laugh();
    }

    public void OnClickSettings()
    {
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }

    public void OnClickCrew()
    {
        Laugh();
    }

    public void OnClickQuit()
    {
        //Environment.FailFast(@"¯\_(ツ)_/¯"); //fuck you mono and fuck you dead earbuds
        Application.Quit();
    }

    public void OnToggleGraphics(bool setting)
    {
        Camera.main.gameObject.SetActive(setting);
        this.gameObject.SetActive(setting);
    }

    private void Laugh()
    {
        //TODO bender
        VideoPanel.SetActive(true);
        VideoComponent.Play();

        StartCoroutine(VideoHideCoroutine());
    }

    IEnumerator VideoHideCoroutine()
    {
        yield return new WaitForSeconds((float)VideoComponent.clip.length);
        VideoPanel.SetActive(false);
    }
}
