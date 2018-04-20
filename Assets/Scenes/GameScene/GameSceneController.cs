using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : BaseSceneController
{
    public override void Update ()
    {
        base.Update();

        while(MessageInterface.HasMessageInQueue)
        {
            var msg = MessageInterface.PopFromQueue();
            if(msg is PlayerDeathMessage)
            {
                EndLevel("GameOverScene", false);
            }
        }
	}
}
