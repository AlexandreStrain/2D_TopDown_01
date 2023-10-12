using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Soldier
{
    [Header("Weapon")]
    public GameObject startingWeapon;

    [Header("Components")]
    private Keyboard input;
    private Mouse mouse;

    void Start()
    {
        //Get the current mouse and keyboard connected to the computer
        input = Keyboard.current;
        mouse = Mouse.current;
        //get the spriteRenderer and Animator from Player
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<CircleCollider2D>();

        //Instantiate a weapon for the soldier to be equipped with at start of game
        Weapon weaponToEquip = Instantiate(startingWeapon).GetComponent<Weapon>();
        EquipWeapon(weaponToEquip);
    }

    public override void FixedUpdate()
    {
        //Calling created methods
        CheckInput();
        //we still want to use the FixedUpdate() method from the Soldier/base class
        base.FixedUpdate();
    }
    
    public override void Aim()
    {
        //Get mouse input axis from Unity
        Vector3 mousePos = mouse.position.ReadValue();
        //Get the player's position based where the camera is in the game window
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        //Find vector direction of the mouse relative to the player's position
        mousePos.x -= screenPos.x;
        mousePos.y -= screenPos.y;

        //Get angle between mouse cursor and player using trigonometry Tan = O / A
        //convert this angle from radians into degrees
        facingAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        //apply the rotation to the player's transform component in Unity
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, facingAngle));
    }

    //Pick up weapons on the ground if player runs over them
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Weapon collidedWeapon = collision.gameObject.GetComponent<Weapon>();
        //if we collided with a weapon and it isn't already equipped by someone (meaning it's on the ground)
        if (collidedWeapon != null && !collidedWeapon.isEquipped)
        {
            EquipWeapon(collidedWeapon);
        }
    }
    
    //Creating a method to handle WASD input and mouse Left click
    private void CheckInput()
    {
        //Up and Down Movement
        if (input.wKey.isPressed)
        {
            yAxis = 1; //positive for upwards
        }
        else if (input.sKey.isPressed)
        {
            yAxis = -1; //negative for downwards
        }
        else {
            yAxis = 0; //No Y axis Movement
        }

        //Left and Right Movement
        if (input.dKey.isPressed)
        {
            xAxis = 1; //positive for right
        }
        else if (input.aKey.isPressed)
        {
            xAxis = -1; //negative for left
        }
        else {
            xAxis = 0; //No X axis movement
        }

        //Mouse left click to shoot
        if(mouse.leftButton.isPressed)
        {
            isFiring = true;
        }
        else
        {
            isFiring = false;
        }
    }
}
