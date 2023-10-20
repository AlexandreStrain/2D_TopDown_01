using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    protected float left, top, width, height;
    protected SpriteRenderer spriteRenderer;

    void Start()
    {
        //get the component that shows the image of the wall to Unity's scene view
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.parent = GameObject.Find("All Walls").transform;
        width = spriteRenderer.bounds.size.x;
        height = spriteRenderer.bounds.size.y;
        left = transform.position.x - (width / 2f);
        top = transform.position.y - (height / 2f);
    }

    //Determine where a GameObject is nearest to the wall
    public Vector2 PointNearestTo(Vector2 position)
    {
        //Where the object is closest to the wall
        Vector2 nearestPoint = new Vector2();

        //set the nearestPoint's x value based on given position
        if (left > position.x)
        {
            //the object's x position is to the left of the wall
            nearestPoint.x = left;
        }
        else if (left + width < position.x)
        {
            //the object's x position is to the right of the wall
            nearestPoint.x = left + width;
        }
        else
        {
            //the object's x position is at or in the wall
            nearestPoint.x = position.x;
        }

         //set the nearestPoint's x value based on given position
        if (top > position.y)
        {
            //the object's x position is to the top of the wall
            nearestPoint.y = top;
        }
        else if (top + height < position.y)
        {
            //the object's x position is to the below of the wall
            nearestPoint.y = top + height;
        }
        else
        {
            //the object's x position is at or in the wall
            nearestPoint.y = position.y;
        }

        return nearestPoint;
    }

    //find the 90 degree angle (normal) of where the bullet impacted an object
    public Vector2 NormalAtNearestPoint(Vector2 p)
    {
        //Create a Vector called nearestPoint
        //set it equal to a point on this wall nearest to point p
        Vector2 nearestPoint = PointNearestTo(p);

        //Create a vector called normal
        //set its direction pointing from nearestPoint on wall to point p
        Vector2 normal = new Vector2(p.x - nearestPoint.x, p.y - nearestPoint.y);

        //ensure the normal vector's length is set to 1 (normalized unit vector)
        //if the length of the normal is zero, we can't resize it, return normal as is
        if (normal == Vector2.zero)
        {
            return normal;
        }

        //create a float called factor and set it equal to the INVERSE of 
        //normal's length using Pythagorean Therorem
        float factor = 1f / Mathf.Sqrt(normal.x * normal.x + normal.y * normal.y);
        //multiply normal by that factor gives the normal a length between 0 and 1
        normal *= factor;

        return normal;
    }
}
