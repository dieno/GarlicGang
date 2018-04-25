using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeEnemySprite : MonoBehaviour {

    public Sprite[] enemySprites;

	// Use this for initialization
	void Start () {
        if(enemySprites.Length > 0)
        {
            int index = Random.Range(0, enemySprites.Length - 1);

            GetComponent<SpriteRenderer>().sprite = enemySprites[index];
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
