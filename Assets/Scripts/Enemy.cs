using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Soldier
{
    [Header("Weapon")]
    public GameObject startingWeapon;

    [Header("Aiming Variables")]
    public float maxAimDuration = 3f; //maximum time allowed to spend facing a certain direction before a change occurs
    private float currentAimDuration = 3f;  //how long the enemy is aimed in a particular direction in seconds
    public float aimTimer; //the current time spent facing a certain direction
    public float visionRange = 6f; //how far away the enemy can see (radius of their vision circle)
    public Transform target; //the target they are aiming at with facingAngle

    private void Start()
    {
        //Get target's transform by looking for a Player Script within the scene
        target = GameObject.FindObjectOfType<Player>().transform;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //parent or connect this enemy GameObject to "All Enemies" GameObject in the scene
        this.transform.parent = GameObject.Find("All Enemies").transform;
        hitbox = GetComponent<CircleCollider2D>();
         //Instantiate a weapon for the soldier to be equipped with at start of game
        Weapon weaponToEquip = Instantiate(startingWeapon).GetComponent<Weapon>();
        EquipWeapon(weaponToEquip);
    }

    public override void Aim()
    {
        //increase timer gradually over each computer cycle/frame
        aimTimer += Time.deltaTime;
        //check if the timer has reached the currentAimDuration set
        if (aimTimer >= currentAimDuration)
        {
            //reset timer
            aimTimer = 0;
            //randomly pick an angle between 0 and 360 degrees
            facingAngle = Random.Range(0f, 360f);
            //randomly set time between 0 and maxAimDuration seconds
            currentAimDuration = Random.Range(0f, maxAimDuration);
        }
        //call a method to search vision range for target
        CheckVision2();

        //call the base/soldier class Aim() method to handle rotating enemy
        base.Aim();
    }

    public void CheckVision2()
    {
        Vector3 distance = target.position-transform.position;
        
        if (distance.magnitude <= visionRange)
        {
            //using the formula of dot product
            //U . V = |U| * |V| * cos(angle)
            //angle = acos(E . V / |U| * |V|)
            //where U is transform.right (1x, 0y) and |U| = 1
            float absV = Mathf.Sqrt(distance.x*distance.x + distance.y*distance.y);
            float angle = Mathf.Acos(Vector2.Dot(transform.right, distance)/absV) * Mathf.Rad2Deg;
            Debug.Log(angle);

            //alternatively, you can use the following if statement without the above
            //if(Vector2.Angle(transform.right, distance.normalized) < visionConeAngle / 2f)
            if(angle <= visionConeAngle/2)
            {
                facingAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
                isFiring = true;
                //if distance is a third of the vision range...
                if (distance.magnitude < visionRange/2f)
                {
                    //then don't get too close to target and back away by negating the velocity
                    transform.position -= velocity;
                    velocity = Vector2.zero;
                }
                return;
            }
        }
        else
        {
            isFiring = false;
        }

    }

    public void CheckVision()
    {
        //if target has been destroyed or is missing (null)
        if (target == null)
        {
            return;
        }

        //calculate distance between enemy and its target
        Vector3 distance = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, 0f);

        //Aim at the target if they're in vision range
        if (distance.magnitude < visionRange)
        {
            //using Tan(angle) = Opposite / Adjacent
            //get facingAngle to turn enemy towards target
            facingAngle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
            isFiring = true;

            //if distance is a third of the vision range...
            if(distance.magnitude < visionRange/3f)
            {
                //then don't get too close to target and back away by negating the velocity
                transform.position -= velocity;
            }
        }
        //otherwise, stop firing if target is not close
        else 
        {
            isFiring = false;
        }

        //let's visualize the angle between the enemy and the player
        Debug.DrawRay(transform.position, (transform.forward + distance).normalized*3, Color.red); 
    }

    public override void Move()
    {
        //Calculate Y and X using Sin and Cosine, convert degrees to radians
        xAxis = Mathf.Cos(facingAngle * Mathf.Deg2Rad);
        yAxis = Mathf.Sin(facingAngle * Mathf.Deg2Rad);

        //call the inherited Move() method from Soldier/base class
        base.Move();
    }

    //Let's visualize the enemy vision range 
    public void OnDrawGizmos() 
    {
        //Set your colour of Gizmo, colour up to you!
        Gizmos.color = Color.cyan;
        //this draws a circle around the center of the enemy
        //Gizmos.DrawWireSphere(transform.position, visionRange);
    }

    private void OnDestroy()
    {
       if (isDead)
       {
         GameManager.enemyList.Remove(this.gameObject);
       }
    } 

}
