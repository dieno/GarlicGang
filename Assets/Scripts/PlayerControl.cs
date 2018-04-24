using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.RPG;
using CommonCore.Messaging;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance
    {
        get
        {
            return GameObject.Find("Player").GetComponent<PlayerControl>();
        }
    }

    [Header("Player Attributes")]
    [SerializeField]
    public float speed = 1.0f;
    public float runMult = 2.0f;
    public float HasArmorDR = 10.0f;
    public float HasArmorDT = 10.0f;

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
    [SerializeField] public AmmoType BulletType = AmmoType.NoAmmo;
    public GameObject FireEffectPrefab;
    public GameObject ReloadEffectPrefab;

    [Header("Aiming Stuff")]
    public float StickAimDeadzone = 0.1f;
    public float StickFireDeadzone = 0.25f;

    public Camera MainCamera;
    public GameObject gameOverScene; //the fuck?
    public GameObject gameUI;

    private Rigidbody2D rb;
    private Vector2 movementVec;

    private bool ReloadSignaled;
    private float nextFireAvailable;
    private bool LastAimedWithStick; //gross hack to make controller and mouse look okay
    public int BulletsInMagazine { get; private set; }  
    public string EquippedWeapon { get; private set; }

    private QdmsMessageInterface MessageInterface;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (MainCamera == null)
            MainCamera = Camera.main;

        PickWeapon();
        //WorldHUDController.Instance.UpdateAmmo(BulletsInMagazine);

        MessageInterface = new QdmsMessageInterface(gameObject);
    }

    private void PickWeapon()
    {
        InventoryItemInstance iii;
        WeaponItemModel wim;

        //get the weapon the player actually has, the shittiest way possible
        if (GameState.Instance.Player.CountItem("m1911") > 0)
        {
            wim = (WeaponItemModel)InventoryModel.GetModel("m1911");
        }
        else if (GameState.Instance.Player.CountItem("revolver") > 0)
        {
            wim = (WeaponItemModel)InventoryModel.GetModel("revolver");
        }
        else
        {
            Debug.LogWarning("The player apparently has no fucking weapons");
            return;
        }

        //Debug.Log(JsonConvert.SerializeObject(wim));

        PickWeapon(wim);
    }

    private void PickWeapon(WeaponItemModel wim)
    {
        BulletSpeed = wim.Velocity;
        BulletDamage = wim.Damage;
        BulletPierce = wim.DamagePierce;
        BulletSpread = wim.Spread;
        BulletType = wim.AType;
        FireRate = wim.FireRate;
        MagazineCapacity = wim.MagazineSize;
        MagazineReloadTime = wim.ReloadTime;
        FireEffectPrefab = Resources.Load<GameObject>("GunEffect/" + wim.FireEffect);
        ReloadEffectPrefab = Resources.Load<GameObject>("ReloadEffect/" + wim.ReloadEffect);

        EquippedWeapon = wim.Name;

        TryReload();
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        HandleMessages();
        MovementControl();
        ShootControl();
        
    }

    void HandleMessages()
    {
        if(MessageInterface.Valid && MessageInterface.HasMessageInQueue)
        {

            while(MessageInterface.HasMessageInQueue)
            {
                QdmsMessage msg = MessageInterface.PopFromQueue();

                if (msg.GetType() == typeof(RpgChangeWeaponMessage))
                    DoEquipmentThing();
            }

        }

        
    }

    private void DoEquipmentThing()
    {
        //do it the laziest way possible
        if (GameState.Instance.Player.EquippedWeapon != null && GameState.Instance.Player.EquippedWeapon.ItemModel.Name != EquippedWeapon)
        {
            TryUnload();
            PickWeapon((WeaponItemModel)GameState.Instance.Player.EquippedWeapon.ItemModel);
        }
    }

    private void TryUnload()
    {
        if(!string.IsNullOrEmpty(EquippedWeapon) && BulletType != AmmoType.NoAmmo)
        {
            if(BulletsInMagazine > 0)
            {
                GameState.Instance.Player.AddItem(BulletType.ToString(), BulletsInMagazine);
                BulletsInMagazine = 0;
            }
        }
    }

    private void TryReload()
    {
        if (!string.IsNullOrEmpty(EquippedWeapon) && BulletType != AmmoType.NoAmmo)
        {
            int numBulletsAvailable = GameState.Instance.Player.CountItem(BulletType.ToString());
            int numBulletsToUse = Math.Min(numBulletsAvailable, MagazineCapacity-BulletsInMagazine);
            BulletsInMagazine += numBulletsToUse;
            GameState.Instance.Player.UseItem(BulletType.ToString(), numBulletsToUse);
            
        }

        ReloadSignaled = false;
    }

    private void ForceReload()
    {
        //play reload effect
        if (ReloadEffectPrefab != null)
            Instantiate<GameObject>(ReloadEffectPrefab, BulletSpawn.position, BulletSpawn.rotation).transform.SetParent(BulletSpawn); //hacks!

        nextFireAvailable = Time.time + MagazineReloadTime;
        ReloadSignaled = true;
    }


    void MovementControl()
    {
        //Get vector input from both horizontal and vertical axix (WASD)
        movementVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float run = Input.GetButton("Run") ? runMult : 1;

        //Add force to player rigidbody based on the vector (direction and magnitude)
        rb.AddForce(movementVec * speed * run);
    }

    void ShootControl()
    {
        //When the time is past the next fire available time handle the shooting controls

        //handle reloading
        if (ReloadSignaled && Time.time > nextFireAvailable)
        {
            //reload is done
            TryReload();
            return;
        }

        if(Input.GetButtonDown("Fire3"))
        {
            ForceReload();
            return;
        }

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

        //a stupid hacky way of doing bullet spread
        direction = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-BulletSpread, BulletSpread) / 2f) * direction;

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
        //WorldHUDController.Instance.UpdateAmmo(BulletsInMagazine);

        if (BulletsInMagazine <= 0)
        {
            //play reload effect
            if (ReloadEffectPrefab != null)
                Instantiate<GameObject>(ReloadEffectPrefab, BulletSpawn.position, BulletSpawn.rotation).transform.SetParent(BulletSpawn); //hacks!
            
            nextFireAvailable = Time.time + MagazineReloadTime;
            //BulletsInMagazine = -1; //stupid and hacky
            ReloadSignaled = true;
        }

        //also do the effect
        if(FireEffectPrefab != null)
            Instantiate(FireEffectPrefab, BulletSpawn.position, BulletSpawn.rotation, BulletSpawn.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var bs = collision.gameObject.GetComponent<BulletScript>();
        if (bs != null)
        {
            float dt = 0, dr = 0;
            if(GameState.Instance.Player.Armor > 0)
            {
                dt = HasArmorDT;
                dr = HasArmorDR;
                GameState.Instance.Player.Armor -= bs.Damage + bs.DamagePierce;
            }
            float bulletDamage = DamageUtil.CalculateDamage(bs.Damage, bs.DamagePierce, dt, dr);
            GameState.Instance.Player.Health -= bulletDamage;
            //gameUI.GetComponent<HealthUI>().LoseHealth();
            if (GameState.Instance.Player.Health <= 0)
            {
                Debug.Log("Game Over");
                MessageInterface.PushToBus(new PlayerDeathMessage());
                //gameOverScene.SetActive(true); //TODO make this signal a gamecontroller, preferably using messaging, because I mean DAMN
            }
            Destroy(collision.gameObject);
        }
    }
}
