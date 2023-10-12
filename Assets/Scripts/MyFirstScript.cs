using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstScript : MonoBehaviour
{
    // Awake is called before the first frame update, before start, used for initialization
    void Awake() {
        Debug.Log("Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");        
    }

    // On Enable is called everytime a gameobject becomes active within the scene
    void OnEnable() {
        Debug.Log("OnEnable");    
    }

    // FixedUpdate is called a fixed amount of time per frame, used in Physics calculations
    void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
    }

    // Late Update is called once per frame after everything else, useful for Camera updates or after physics calculations
    void LateUpdate() {
       Debug.Log("Late Update"); 
    }
}
