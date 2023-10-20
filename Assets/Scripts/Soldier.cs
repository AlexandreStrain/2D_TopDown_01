using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    //We add headers to keep things organized in the Unity Inspector
    [Header("Variables")]
    public float speed = 10f; //the speed in pixels of movement per frame
    public float facingAngle; //which angle in degrees is the soldier facing?
    //////
    public float visionConeAngle = 60f;
    //////
    public float xAxis, yAxis; //the direction of movement on each particular axis
    public Vector3 velocity; //the vector in which the soldier moves with
    public bool isFiring; //if the soldier is firing their weapon
    public float hp = 5f; //how much health the soldier has before they are defeated

    [Header("Animation")]
    public bool isDead;
    public bool isMoving;
    public bool canSwim;
    public bool isHit; //the booleans here describe the various animation states we will handle
    public Animator animator; //a reference variable to the animator component on the gameobject
    public float hitTimer; //a timer for how long to toggle the soldier's image off and on when hit
    public SpriteRenderer spriteRenderer; //A renderer is what actually draws the image of the soldier on screen
    public CircleCollider2D hitbox; //the shape of the collider on the soldier
    public bool isPushed; //is the soldier already being pushed away from an object?

    [Header("Weapon")]
    public Transform hands; //where weapons are going to be held
    public Weapon currentWeapon; // the currently equipped weapon
    
    public virtual void FixedUpdate()
    {
        Animate();

        if (isDead)
        {
            return;
        }

        Aim();
        Move();
        UpdateWeapon();
    }

    public virtual void Animate()
    {
        //we're moving when velocity is not zero
        isMoving = velocity != Vector3.zero;
        //set the parameters found on the animator controller
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isDead", isDead);

        //if we are hit by a bullet
        if (isHit)
        {
            //toggle the spriteRenderer off and on
            spriteRenderer.enabled = !spriteRenderer.enabled;
            //weapon must also flash off and on
            currentWeapon.spriteRenderer.enabled = !currentWeapon.spriteRenderer.enabled;
            //reduce how how this effect lasts for
            hitTimer -= Time.deltaTime;
            //check once the time for this effect runs out
            if (hitTimer <= 0)
            {
                //we're no longer hit and our sprite renderer remains on
                isHit = false;
                spriteRenderer.enabled = true;
                currentWeapon.spriteRenderer.enabled = true;
            }
        }
    }

    public void EquipWeapon(Weapon w)
    {
        //if there is already a weapon equipped, unequip it
        if (currentWeapon != null)
        {
            currentWeapon.isEquipped = false;
            currentWeapon.ownerTag = "";
            //owner is removed & weapon ends up on the ground in all weapons
            currentWeapon.transform.parent = GameObject.Find("All Weapons").transform;
            currentWeapon = null;
        }

        //set weapon to be equipped & owned by soldier, placed in their hands
        currentWeapon = w;
        currentWeapon.ownerTag = gameObject.tag;
        currentWeapon.transform.parent = hands.transform;
        currentWeapon.isEquipped = true;
    }

    public virtual void Aim()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, facingAngle));
    }

    public virtual void Move()
    {
        //Create the velocity vector to apply our player's position
        //this is based on xAxis and yAxis movement
        velocity = new Vector2(xAxis, yAxis) * speed * Time.deltaTime;

        //apply the velocity vector to the player position
        transform.position += velocity;
    }

    public virtual void UpdateWeapon()
    {
        //make sure weapon is placed within hands
        currentWeapon.transform.position = hands.position;
        //make sure weapon faces the same way as soldier
        currentWeapon.facingAngle = facingAngle;
        if (isFiring)
        {
            currentWeapon.Shoot();
        }
    }

    //Note there is a float amount parameter for this method in the brackets ( )
    public virtual void TakeDamage(float amount)
    {
        //decrement amount from the hp variable
        hp -= amount;
        //if hp is less than or equal to zero, destroy the GameObject this script is attached to in the scene
        if (hp <= 0)
        {
            isDead = true;
        }
        else
        {
            //start toggling image off and on for a duration in seconds
            isHit = true;
            hitTimer = 0.5f;
        }
    }

    //This is called by an animation event inside the explode animation,
    //we will import this animation event to save time
    public void Remove()
    {
        GameManager.weaponList.Remove(currentWeapon.gameObject);
        Destroy(currentWeapon.gameObject);
        Destroy(this.gameObject);
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

    public void PushFrom(Vector2 p, float amount)
    {
        //Calculate current x and y components of both objects from each other
        float xDist = p.x - transform.position.x;
        float yDist = p.y - transform.position.y;

        //calculate current total distance using Pythagorean's theorem
        float distance = Mathf.Sqrt(xDist * xDist + yDist * yDist);

        //if distance is 0, then no push necessary
        if (distance <= 0) { return; }

        //the push distance we want to push the soldier away from the wall is depending on an amount
        float pushDistance = hitbox.radius + amount;

        //Push soldier back by a ratio amount per frame depending on distance
        float pushRatio = pushDistance / distance;

        //find the direction to move the soldier
        Vector2 moveDirection = new Vector2(this.transform.position.x - p.x, this.transform.position.y - p.y);

        //apply the ratio to the direction of movement
        moveDirection *= pushRatio;

        //add the move direction to where soldier made contact will wall, pushing it away
        transform.position = new Vector2(p.x + moveDirection.x, p.y + moveDirection.y);
    }

    //this is a built-in method from unity using colliders and rigidbodies
    public void OnCollisionStay2D(Collision2D collision)
    {
        //if we collided with a soldier AND we're not pushing away from a wall
        if (collision.gameObject.GetComponent<Soldier>() && !isPushed)
        {
            //push away from it by a fixed amount
            PushFrom(collision.transform.position, 0.5f);
        }
        else
        {
            //get the wall script from what we collided with (Unity Collision)
            Wall w = collision.gameObject.GetComponent<Wall>();
            //if soldier is touching a wall and it exists (the wall is not null)
            if (w != null && IsTouchingWall(w))
            {
                if (w is Water && canSwim)
                {
                    isPushed = false;
                }
                else
                {
                    transform.position -= velocity;
                    //push away from the nearestPoint the soldier is from the wall
                    PushFrom(w.PointNearestTo(transform.position), 0.15f);
                    isPushed = true;
                }
            }
            else
            {
                isPushed = false;
            }
        }
    }
    
}
