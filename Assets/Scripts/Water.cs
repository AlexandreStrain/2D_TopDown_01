using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Wall
{

    public bool isWater = true;

    private void Start()
    {
        //get the component that shows the image of the wall to Unity's scene view
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.parent = GameObject.Find("All Walls").transform;
        width = spriteRenderer.bounds.size.x;
        height = spriteRenderer.bounds.size.y;
        left = transform.position.x - (width / 2f);
        top = transform.position.y - (height / 2f);
    }
}
