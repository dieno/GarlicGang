using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {

    public GameObject door;
	
	// Update is called once per frame
	void Update () {
	    if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            Destroy(door);
        }	
	}
}
