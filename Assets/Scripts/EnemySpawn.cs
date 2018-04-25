using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    private GameObject enemy;

    // Use this for initialization
    void Start () {
        GameObject enemyObj = Resources.Load<GameObject>("Prefabs/Enemy");

        enemy = (GameObject) Instantiate(enemyObj, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(enemy != null)
            {
                enemy.GetComponent<EnemyScript>().isActive = true;
            }
        }
    }
}
