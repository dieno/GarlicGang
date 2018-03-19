using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;


namespace Ares.ObjectActions
{
    public class ChangeSceneScript : ActionSpecial
    {
        public string NextScene;
        public string SpawnPoint;
        public Vector3 SpawnPosition;
        public Vector3 SpawnRotation;

        public void ChangeScene()
        {
            throw new NotImplementedException();
        }

        public override void Execute(ActionInvokerData data)
        {
            ChangeScene();
        }
    }
}