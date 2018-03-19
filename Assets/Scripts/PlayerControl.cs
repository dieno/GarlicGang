using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.RPG;

public class PlayerControl : MonoBehaviour
{

    [Header("Player Attributes")]
    [SerializeField]
    public float speed = 1.0f;
    public int lives = 3;

    [Header("Bullet Attributes")]
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    [SerializeField] public float BulletSpeed = 6.0f;
    [SerializeField] public float BulletLifetime = 2.0f;
    [SerializeField] public float BulletDamage = 2.0f;
    [SerializeField] public float BulletPierce = 0;
    [SerializeField] public float BulletSpread = 2.0f;
    [SerializeField] public float FireRate = 0.5f;
    [SerializeField] public int MagazineCapacity = 5;
    [SerializeField] public float MagazineReloadTime = 2.0f;
    public GameObject FireEffectPrefab;

    [Header("Aiming Stuff")]
    public float StickAimDeadzone = 0.1f;
    public float StickFireDeadzone = 0.25f;

    public Camera MainCamera;
    public GameObject gameOverScene; //the fuck?
    public GameObject gameUI;

    private Rigidbody2D rb;
    private Vector2 movementVec;

    private float nextFireAvailable;
    private bool LastAimedWithStick; //gross hack to make controller and mouse look okay
    private int BulletsInMagazine;    


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (MainCamera == null)
            MainCamera = Camera.main;

        PickWeapon();
        BulletsInMagazine = MagazineCapacity;

    }

    private void PickWeapon()
    {
        WeaponItemModel wim;

        //get the weapon the player actually has, the shittiest way possible
        if(GameState.Instance.Player.CountItem("m1911") > 0)
        {
            wim = (WeaponItemModel)InventoryModel.GetModel("m1911");
        }
        else if(GameState.Instance.Player.CountItem("revolver") > 0)
        {
            wim = (WeaponItemModel)InventoryModel.GetModel("revolver");
        }
        else
        {
            Debug.LogWarning("The player apparently has no fucking weapons");
            return;
        }

        //Debug.Log(JsonConvert.SerializeObject(wim));

        BulletSpeed = wim.Velocity;
        BulletDamage = wim.Damage;
        BulletPierce = wim.DamagePierce;
        BulletSpread = wim.Spread;
        FireRate = wim.FireRate;
        MagazineCapacity = wim.MagazineSize;
        MagazineReloadTime = wim.ReloadTime;
        FireEffectPrefab = Resources.Load<GameObject>("GunEffect/" + wim.FireEffect);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovementControl();
        ShootControl();
    }

    void MovementControl()
    {
        //Get vector input from both horizontal and vertical axix (WASD)
        movementVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Add force to player rigidbody based on the vector (direction and magnitude)
        rb.AddForce(movementVec * speed);
    }

    void ShootControl()
    {
        //When the time is past the next fire available time handle the shooting controls

        //shitty, tired hacky-but-it-works code

        //mouse, use cursor position for vector
        Vector3 mousePos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, MainCamera.nearClipPlane)); //change to raycast for 2.5D
        Vector2 fireVector = (Vector2)(mousePos - transform.position);

        Vector2 kbFireVector = new Vector2(Input.GetAxisRaw("Horizontal2"), Input.GetAxisRaw("Vertical2"));
        if (kbFireVector.magnitude >= StickAimDeadzone)
        {
            LastAimedWithStick = true;
            fireVector = kbFireVector;
            RotateMeToVector(fireVector);
        }            
        else if(Input.GetMouseButton(0))
        {
            LastAimedWithStick = false;
        }

        if(!LastAimedWithStick)
            RotateMeToVector(fireVector);

        if (Time.time > nextFireAvailable)
        {
            if(kbFireVector.magnitude >= StickFireDeadzone || Input.GetButton("Fire1"))
            {
                Fire(fireVector.normalized);
            }
                
        }
    
    }

    private void RotateMeToVector(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(transform.up, dir);
        transform.Rotate(Vector3.forward, angle);
    }

    void Fire(Vector3 direction)
    {
        if(BulletsInMagazine < 0)
        {
            //reload is done
            BulletsInMagazine = MagazineCapacity;
        }

        //a stupid hacky way of doing bullet spread
        direction = Quaternion.Euler(0, 0, Random.Range(-BulletSpread, BulletSpread) / 2f) * direction;

        //Set the time that the bullet can be shot again 
        //Time.time(current time) + the fire rate (the delay from the time you've shot)
        nextFireAvailable = Time.time + FireRate;

        //Instantiate (or Spawn) a new instance of BulletPrefab at the desired position and rotation
        //Instantiate(object to spawn, position to spawn at, rotation to be once spawned)
        GameObject bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation, transform.root);
        //Get the Rigidbody2D (Physics Component) of the bullet and set the velocity to the speed and direction that is desired.
        bullet.GetComponent<Rigidbody2D>().velocity = direction * BulletSpeed;

        //set bullet vars
        var bs = bullet.GetComponent<BulletScript>();
        bs.Damage = BulletDamage;
        bs.DamagePierce = BulletPierce;

        //Destroy(what to destroy, when to destroy it)
        Destroy(bullet, BulletLifetime); //oh yeah I forgot that was a thing

        BulletsInMagazine--;

        if(BulletsInMagazine == 0)
        {
            //play reload effect
            nextFireAvailable = Time.time + MagazineReloadTime;
            BulletsInMagazine = -1; //stupid and hacky
        }

        //also do the effect
        if(FireEffectPrefab != null)
            Instantiate(FireEffectPrefab, BulletSpawn.position, BulletSpawn.rotation, BulletSpawn.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            lives--;
            //gameUI.GetComponent<HealthUI>().LoseHealth();
            if (lives <= 0)
            {
                Debug.Log("Game Over");
                gameOverScene.SetActive(true); //TODO make this signal a gamecontroller, preferably using messaging, because I mean DAMN
            }
        }
    }
}
