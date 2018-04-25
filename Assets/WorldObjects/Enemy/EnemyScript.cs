using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyScript : MonoBehaviour
{
    public UnityEvent DestroyEvent; //should we extend UnityEvent? ARES does
    public GameObject DestroyEffect;
    public GameObject FireEffectPrefab;
    public float DamageThreshold = 0;
    public float DamageResistance = 0;
    public float MaxHealth = 1.0f;
    public float speed = 2.0f;
    public float stoppingDistance = 5.0f;
    public bool isActive = true;//false;

    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    [SerializeField] public float BulletSpeed = 6.0f;
    [SerializeField] public float BulletLifetime = 2.0f;
    [SerializeField] public float BulletDamage = 2.0f;
    [SerializeField] public float BulletPierce = 0;
    [SerializeField] public float BulletSpread = 2.0f;
    [SerializeField] public int MagazineCapacity = 5;
    [SerializeField] public float MagazineReloadTime = 2.0f;
    [SerializeField] public float FireRate = 1.0f;
    private float nextFireAvailable;

    public Transform target;
    private float Health;
    private Rigidbody2D rb;
    private Vector2 directionVector;

    void Start ()
    {
        Health = MaxHealth;
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player").transform;
        FireEffectPrefab = Resources.Load<GameObject>("GunEffect/RevolverEffect");
    }

    void FixedUpdate()
    {
        if(isActive)
        {
            directionVector = target.position - transform.position;

            Chase();
            RotateMeToVector(directionVector);
            Shoot();
        }
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

    //Chase the player 
    public void Chase()
    {
        //Debug.Log("dist: " + Vector3.Distance(target.position, transform.position) + ", sd: "+ stoppingDistance);


        if(Vector3.Distance(target.position, transform.position) > stoppingDistance)
        {
            rb.velocity = directionVector.normalized * speed;
        } else {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    //Shoot at the player, with some delay
    public void Shoot()
    {
        if (Time.time > nextFireAvailable)
        {
            //Shoot at the players direction
            Fire(directionVector.normalized);

            //also do the effect
            if (FireEffectPrefab != null)
                Instantiate(FireEffectPrefab, BulletSpawn.position, BulletSpawn.rotation, BulletSpawn.transform);
        }
    }

    private void RotateMeToVector(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(transform.up, dir);
        transform.Rotate(Vector3.forward, angle);
    }

    void Fire(Vector3 direction)
    {
        //Set the time that the bullet can be shot again 
        //Time.time(current time) + the fire rate (the delay from the time you've shot)
        nextFireAvailable = Time.time + FireRate;
        //Instantiate (or Spawn) a new instance of BulletPrefab at the desired position and rotation
        //Instantiate(object to spawn, position to spawn at, rotation to be once spawned)
        GameObject bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        //Get the Rigidbody2D (Physics Component) of the bullet and set the velocity to the speed and direction that is desired.
        bullet.GetComponent<Rigidbody2D>().velocity = direction * BulletSpeed;

        //set bullet vars
        var bs = bullet.GetComponent<BulletScript>();
        bs.Damage = BulletDamage;
        bs.DamagePierce = BulletPierce;


        //Destroy(what to destroy, when to destroy it)
        Destroy(bullet, BulletLifetime);
    }

}
