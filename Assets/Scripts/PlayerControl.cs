using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] public float FireRate = 0.5f;

    private Rigidbody2D rb;
    private Vector2 movementVec;

    private float nextFireAvailable;


    public GameObject gameOverScene;
    public GameObject gameUI;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (Time.time > nextFireAvailable)
        {
            //If the keyboard press is the right arrow key
            if (Input.GetKey(KeyCode.RightArrow))
            {
                //Fire a bullet in the direction of the right of the transform object
                Fire(transform.right);
            }
            //If the keyboard press is the left arrow key
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                //Fire a bullet in the direction of the left of the transform object
                //or negative of the right direction (opposite)
                Fire(-transform.right);
            }
            //If the keyboard press is the up arrow key
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                //Fire a bullet in the direction of the up the transform object
                Fire(transform.up);
            }
            //If the keyboard press is the down arrow key
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                //Fire a bullet in the direction of the down of the transform object
                //or negative of the up direction (opposite)
                Fire(-transform.up);
            }
        }
    
    }

    void Fire(Vector3 direction)
    {
        //Set the time that the bullet can be shot again 
        //Time.time(current time) + the fire rate (the delay from the time you've shot)
        nextFireAvailable = Time.time + FireRate;
        //Instantiate (or Spawn) a new instance of BulletPrefab at the desired position and rotation
        //Instantiate(object to spawn, position to spawn at, rotation to be once spawned)
        //GameObject bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        //Get the Rigidbody2D (Physics Component) of the bullet and set the velocity to the speed and direction that is desired.
        //bullet.GetComponent<Rigidbody2D>().velocity = direction * BulletSpeed;
        //Destroy(what to destroy, when to destroy it)
        //Destroy(bullet, BulletLifetime);
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
                gameOverScene.SetActive(true);
            }
        }
    }
}
