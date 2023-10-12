using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperRifle : Weapon
{
    void Start()
    {
        //get the components of each bullet for drawing and collisions
        hitbox = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //set unique properties of our weapon type
        timeBetweenShots = 2.5f;
        damageMultiplier = 5f; //how much damage an individual bullet deals (useful for weapons that fire multiple bullets at once)
    }

    public override void CreateBullet()
    {
        // Locally store the new bullet in a variable
        GameObject newBullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);

        //Get the bullet script that exists on the bullet prefab
        Bullet newBulletScript = newBullet.GetComponent<Bullet>();
        //add the damage multiplier to the bullet
        newBulletScript.damage *= damageMultiplier;
        newBulletScript.speed = 7f;
        
        //Call the bullet's init method and pass through the player's facing angle and bulletstartPosition as arguments
        newBulletScript.Init(facingAngle, bulletStartPosition, ownerTag);
    }
}
