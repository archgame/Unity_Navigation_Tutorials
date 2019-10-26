using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeStop : MonoBehaviour
{
    string SpotName = "ParkingSpot";
    public GameObject[] spots;
    public GameObject[] bikes;

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0;i<transform.childCount;i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if(child.name.Contains(SpotName))
            {
                children.Add(child);
            }
        }

        spots = children.ToArray();
        bikes = new GameObject[spots.Length];

        spots = Shuffle(spots);
    }

    GameObject[] Shuffle(GameObject[] objects)
    {
        GameObject tempGO;
        for (int i = 0; i < objects.Length; i++)
        {
            //Debug.Log("i: " + i);
            int rnd = Random.Range(0, objects.Length);
            tempGO = objects[rnd];
            objects[rnd] = objects[i];
            objects[i] = tempGO;
        }
        return objects;
    }
}
