using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    [Header("Controls")]
    public bool switchLights = false;

    [Header("Lights")]
    public GameObject[] trafficLights;
    public GameObject[] crosswalks;

    // Update is called once per frame
    void Update()
    {
        if(switchLights)
        {
            SwitchLights(true, false, true, false);
        }
        else
        {
            SwitchLights(false,true,false,true);
        }
    }

    public void SwitchLights(bool l1, bool l2, bool l3, bool l4)
    {
        trafficLights[0].SetActive(l1);
        trafficLights[1].SetActive(l2);
        trafficLights[2].SetActive(l3);
        trafficLights[3].SetActive(l4);

        crosswalks[0].SetActive(!l1);
        crosswalks[1].SetActive(!l2);
        crosswalks[2].SetActive(!l3);
        crosswalks[3].SetActive(!l4);
    }
}
