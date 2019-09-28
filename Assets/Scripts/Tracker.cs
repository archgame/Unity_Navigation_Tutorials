using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public bool run = false;
    public GameObject target;
    public float smoothSpeed = 0.125f;
    private Vector3 newPosition = Vector3.zero;
    //float example = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start");
        newPosition = Camera.main.transform.position;
        Debug.Log("addition example: " + CoolAddition(2, 3));
    }

    // Update is called once per frame
    void Update()
    {
        //float example = 0;
        //Debug.Log("before example: " + example);
        //example = Mathf.Lerp(example, 10, 0.5f);
        //Debug.Log("after example: " + example);

        //smoothly move camera
        Vector3 cameraPosition = Camera.main.transform.position;
        newPosition.y = cameraPosition.y;
        Vector3 smoothedPosition = Vector3.Lerp(cameraPosition, newPosition, smoothSpeed);
        Camera.main.transform.position = smoothedPosition;

        Vector3 clickPosition = Vector3.zero;
        //Left mouse button click example
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Left Click");
            Vector3 hit = GetClickHit("ground");
            if(hit != Vector3.zero)
            {
                target.transform.position = hit;
            }
        }

        //Right mouse button click example
        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("Right Click");
            Vector3 hit = GetClickHit("ground");
            if (hit != Vector3.zero)
            {
                newPosition = hit;
            }
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

    int CoolAddition(int a, int b)
    {
        return a + b;
    }

    Vector3 GetClickHit(string tag)
    {
        //creates a ray from the camera
        Vector3 screenPoint = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("name: " + hit.transform.gameObject.name);
            if (hit.transform.gameObject.name == tag)
            {
                return hit.point;
            }
        }

        return Vector3.zero;
    }
}
