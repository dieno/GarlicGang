using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;


namespace Ares.ObjectActions
{
    public class ChangeSceneScript : ActionSpecial
    {
        public string NextScene;
        public bool BypassNormalExit;
        public string SetDialogue;

        public string SpawnPoint; //these don't work
        public bool OverrideSpawn;
        public bool SaveSpawn;
        public Vector3 SpawnPosition;
        public Vector3 SpawnRotation;

        public void ChangeScene()
        {
            if (!string.IsNullOrEmpty(SetDialogue))
                GameState.Instance.CurrentDialogue = SetDialogue;

            if (OverrideSpawn)
            {
                GameState.Instance.OverridePosition = SpawnPosition;
                GameState.Instance.OverrideRotation = Quaternion.Euler(SpawnRotation);
            }
            else if (SaveSpawn)
            {
                var player = GameObject.Find("Player");
                GameState.Instance.OverridePosition = player.transform.position;
                GameState.Instance.OverrideRotation = player.transform.rotation;
            }

            if (BypassNormalExit)
                SceneManager.LoadScene(NextScene);
            else
                GameObject.Find("WorldRoot").GetComponent<BaseSceneController>().EndLevel(NextScene);
            
            
        }

        public override void Execute(ActionInvokerData data)
        {
            ChangeScene();
        }
    }
}