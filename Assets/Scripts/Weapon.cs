using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool isEquipped; //if some soldier has equipped this weapon
    public string ownerTag; //determines who currently equipped this weapon
    public float facingAngle; //deteremines where the weapon faces
    public CapsuleCollider2D hitbox;
    public SpriteRenderer spriteRenderer;

    [Header("Bullet Variables")]
    //The Bullet GameObject the player will fire and its starting position
    public GameObject bulletPrefab;
    public Vector3 bulletStartPosition;
    public float timeBetweenShots = 1f;     //the amount of time in seconds to wait between shots
    public float shootTimer;     //counts up to the max time between shots before player fires bullet
    public float damageMultiplier = 1f; //sacale how much damage each bullet does

    public virtual void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, facingAngle));
    }

    public virtual void Shoot()
    {
        // shoot timer continues to gradually increase over time
        shootTimer += Time.deltaTime;

        //if shoot timer hasn't reached the max wait time between shots
        if (shootTimer < timeBetweenShots)
        {
            //exit method early with a return statement
            return;
        }

        //reset shoot timer to zero to be able to shoot again
        shootTimer = 0;
        //Call abstract method
        CreateBullet();
    }

    //an abstract method, meaning the body of the method does not exist!
    public abstract void CreateBullet();

    public void OnDestroy()
    {
        GameManager.weaponList.Remove(this.gameObject);
    }
}
