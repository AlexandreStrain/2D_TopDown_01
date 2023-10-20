using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Variables")]
    public float speed = 10f;
    public float angle;
    public Vector3 velocity;
    public float lifetime = 2f; // the lifetime of the bullet in seconds
    public float damage = 1f; //how much damage a bullet deals to an object
    public string ownerTag; //the Unity tag of who fired the bullet
    private CircleCollider2D hitbox;


    // The code between the ( ) is known as parameters
    // they are data variables we require to make the init method work
    // bullet start position does nothing right now, but will with weapons
    public void Init(float angle, Vector3 bulletStartPosition, string ownerTag)
    {
        hitbox = GetComponent<CircleCollider2D>();
        GameManager.bulletList.Add(this.gameObject);
        //the "this" keyword references the current bullet we are dealing with
        this.angle = angle;
        //add position so the bullet gets created at the barrel of the weapon
        this.transform.position += bulletStartPosition;

        // Calculate X and Y using Cosine and Sin. Convert to radians
        float xMove, yMove;
        xMove = Mathf.Cos(angle * Mathf.Deg2Rad);
        yMove = Mathf.Sin(angle * Mathf.Deg2Rad);

        //normalized means the vector stays at a value between -1 and 1
        //large vectors still have the same angles as smaller vectors
        velocity = new Vector3(xMove, yMove).normalized * speed * Time.deltaTime;

        //parent or connect this bullet gameobject to "All Bullets" in the scene
        this.transform.parent = GameObject.Find("All Bullets").transform;
        this.ownerTag = ownerTag;
    }

    void Update()
    {
        Move();
        CheckLifeTime();
    }

    public void Move()
    {
        //for every GameObject stored within the wallList from GameManager
        for (int i = 0; i < GameManager.wallList.Count; i++)
        {
            //get the wall script from the GameObject
            Wall w = GameManager.wallList[i].gameObject.GetComponent<Wall>();
            //if the bullet is touching the wall
            if (!(w is Water) && IsTouchingWall(w))
            {
                //stop the bullet from going through the wall by applying a negative velocity (cancels out previous)
                transform.position -= velocity;
                //calculate reflection of bullet from the wall like a mirror
                BounceFrom(w.NormalAtNearestPoint(transform.position));
                break;
            }   
        }

        //Apply velocity to the bullet's position
        transform.position += velocity;
    }

    public void CheckLifeTime()
    {
        //decrement lifetime gradually using deltaTime
        lifetime -= Time.deltaTime;
        //if lifetime reaches equal to or below the value of zero, destroy this GameObject
        if (lifetime <= 0)
        {
            GameManager.bulletList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        //Get the GameObject of what we hit
        GameObject hitObject = collision.gameObject;

        //check if we hit an enemy if the bullet was fired from player
        if (ownerTag == "Player" && hitObject.CompareTag("Enemy"))
        {
            //polymorphism in Object-Oriented Programming will work here
            //get the correct script on the GameObject
            Soldier s = hitObject.GetComponent<Enemy>();
            s.TakeDamage(damage);
            //Destroy the bullet once it hits its mark
            Destroy(this.gameObject);
        }
        //otherwise check if we hit the player if the bullet was fired from enemy
        else if (ownerTag == "Enemy" && hitObject.CompareTag("Player"))
        {
            Soldier s = hitObject.GetComponent<Player>();
            s.TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }

    public bool IsTouchingWall(Wall w)
    {
        //Find the nearestPoint the soldier comes into contact with the wall
        Vector2 nearestPoint = w.PointNearestTo(this.transform.position);

        //Calculate the X and Y components of the distance between the soldier and the wall
        float xDist = nearestPoint.x - this.transform.position.x;
        float yDist = nearestPoint.y - this.transform.position.y;

        //Calculate closest distance between soldier and wall using Pythagorean's Theorem
        float closestDistance = Mathf.Sqrt(xDist * xDist + yDist * yDist);

        //return if the closest distance is less than the radius of the player
        return closestDistance < hitbox.radius;
    }

    public void BounceFrom(Vector2 normal)
    {
        //applying Dot product to create the "b" in the equation R = I - 2b
        float b =  (velocity.x * normal.x + velocity.y * normal.y);
        
        //create the 2b in the equation R = I - 2b
        b *= 2;

        //create the new reflection vector for the equation R = I - 2b
        //reflection = the new velocity vector of the bullet
        //I = old velocity vector (the initial trajectory of the bullet)
        //b = the component of I that is parallel to the normal 
        //    (either x or y depending on which side of the wall it bounces off of)
        Vector2 reflection = new Vector2(velocity.x - normal.x * b, velocity.y - normal.y * b);

        //set this bullet's new velocity equal to the reflected one
        velocity = reflection;
    }
}
