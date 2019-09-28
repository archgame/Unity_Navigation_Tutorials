using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public bool run = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        //Left mouse button click example
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Left Click");
        }

        //Right mouse button click example
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click");
        }

        //Middle mouse button click example
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Middle Click");
        }

        //x key click example
        if (Input.GetKeyDown("x"))
        {
            Debug.Log("x pressed.");
        }

        //Space key click example
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space pressed.");
        }
    }
}
