using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public bool run = false;
    public GameObject target;

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

            //creates a ray from the camera
            Vector3 screenPoint = Input.mousePosition;
            Debug.Log("screenPoint: " + screenPoint);
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            float length = 200;
            Debug.DrawRay(ray.origin, ray.direction * length,Color.red,2);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity); //find all gameobjects
            Debug.Log("hits.Length: " + hits.Length);
            foreach(RaycastHit hit in hits)
            {
                Debug.Log("name: " + hit.transform.gameObject.name);
                if(hit.transform.gameObject.name == "ground")
                {
                    Debug.Log("'ground' hit!");
                    Debug.Log("hit.point: " + hit.point);
                    Debug.DrawRay(hit.point, Vector3.up * 50, Color.cyan, 2);
                    target.transform.position = hit.point;
                }
            }

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
