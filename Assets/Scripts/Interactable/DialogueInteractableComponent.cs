using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueInteractableComponent : InteractableComponent
{

    public string Dialogue;

    protected override void OnInteract(GameObject initiator)
    {
        GameState.Instance.LastScene = SceneManager.GetActiveScene().name;
        GameState.Instance.CurrentDialogue = Dialogue;
        SceneManager.LoadScene("DialogueScene");
    }
}
