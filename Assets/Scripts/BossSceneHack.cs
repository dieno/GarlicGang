using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSceneHack : MonoBehaviour
{
    public string NextScene = "DialogueScene";
    public string NextDialogue = "end.intro1";
    public float HealthFraction = 0.2f;
    public EnemyScript EScript;

	void Start ()
    {
        if(EScript == null)
            EScript = GetComponent<EnemyScript>();
	}
	

	void Update ()
    {
        float health = EScript.Health;
        float maxHealth = EScript.MaxHealth;

        if (health / maxHealth <= HealthFraction)
            GotoNextScene();
		
	}

    private void GotoNextScene()
    {
        //note that this does not set flags properly for return, it's meant for the end of the game only

        if (!string.IsNullOrEmpty(NextDialogue))
            GameState.Instance.CurrentDialogue = NextDialogue;

        SceneManager.LoadScene(NextScene);
    }
}
