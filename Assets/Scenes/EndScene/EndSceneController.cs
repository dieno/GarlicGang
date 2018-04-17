using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    public float EndingLength;
    public float CreditsLength;

	void Start ()
    {
        StartCoroutine(StartCreditsCoroutine());
	}
	

	void Update ()
    {
		
	}

    IEnumerator StartCreditsCoroutine()
    {
        yield return new WaitForSeconds(EndingLength);

        GameObject.Find("Video Player 2").GetComponent<VideoPlayer>().Play();
    }

    IEnumerator EndEverythingCoroutine()
    {
        yield return new WaitForSeconds(CreditsLength);

        SceneManager.LoadScene("MainMenuScene");
    }
}
