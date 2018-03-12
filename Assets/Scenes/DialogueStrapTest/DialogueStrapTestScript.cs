using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueStrapTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GameState.Instance.NextDialogue = "coldopen.matsuda1";
        SceneManager.LoadScene("DialogueScene");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
