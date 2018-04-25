using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float Damage = 1.0f;
    public float DamagePierce = 0; //DT breaking
    public GameObject HitEffect;

    public void Die()
    {
        if (HitEffect != null)
            Instantiate(HitEffect, transform.position, Quaternion.identity, transform.root);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Wall")
        {
            Die();
        }
    }
}
