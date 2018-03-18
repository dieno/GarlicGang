using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionInteractableComponent : InteractableComponent
{
    public string NextScene;

    protected override void OnInteract(GameObject initiator)
    {
        //TODO any controller/other unload
        GameState.Instance.LastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(NextScene);
    }
}
