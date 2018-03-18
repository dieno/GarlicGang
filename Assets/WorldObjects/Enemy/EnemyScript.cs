using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour
{
    public UnityEvent DestroyEvent; //should we extend UnityEvent? ARES does
    public GameObject DestroyEffect;
    public float DamageThreshold = 0;
    public float DamageResistance = 0;
    public float MaxHealth = 1.0f;


    private float Health;

	void Start ()
    {
        Health = MaxHealth;
	}

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var bs = collision.gameObject.GetComponent<BulletScript>();
        if(bs != null)
        {
            Health -= DamageUtil.CalculateDamage(bs.Damage, bs.DamagePierce, DamageThreshold, DamageResistance);
            bs.Die();
            CheckIfDead();
        }
    }

    protected void CheckIfDead()
    {
        if (Health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        //TODO signal gamecontroller for score or whatever

        if (DestroyEffect != null)
            Instantiate<GameObject>(DestroyEffect, transform.position, Quaternion.identity, transform.root);

        if (DestroyEvent != null)
            DestroyEvent.Invoke();

        Destroy(gameObject);
    }
}
