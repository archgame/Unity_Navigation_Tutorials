using System.Collections;
using System.Collections.Generic;
using System; //added

using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Celestial Objects")]
    public GameObject sun;
    public GameObject moon;

    [Header("Time")]
    public Text text;
    private float time;
    private TimeSpan currentTime;
    private int days;
    private float speed = 1000f;

    [Header("Lighting")]
    private float sunintensity;
    private float moonintensity;
    public float rotation;

    void Start()
    {
        time = 21600; //6 * 60 * 60
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTime();
    }

    private void ChangeTime()
    {
        //update time
        time += Time.deltaTime * speed;
        //Debug.Log("time: " + time/60/60);
        if(time > 86400)
        {
            days += 1;
            if(days > 7)
            {
                days = 0;
            }
            time = 0;
        }

        //print time
        currentTime = TimeSpan.FromSeconds(time); //convert seconds to TimeSpan variable
        //Debug.Log("time: " + currentTime.ToString().Split(":"[0])); //"string"[3] = 'i';
        string[] temptime = currentTime.ToString().Split(':'); //10:00:00.0000       
        text.text = temptime[0] + ":" + temptime[1]; //hours + minutes

        //celestial body rotations
        rotation = (time - 64800) / 86400 * -360; //64800
        sun.transform.rotation = Quaternion.Euler(new Vector3(rotation, 90, 0));
        moon.transform.rotation = Quaternion.Euler(new Vector3(-rotation, -90, 0));

        //lighting
        if(time > 21600 && time < 64800) //daytime
        {
            sunintensity = 1 - (Math.Abs(43200 - time) / 21600);
            moonintensity = 0;
        }
        if(time > 64800 || time < 21600) //nightime
        {
            sunintensity = 0;
            if (time < 21600) //morning
            {
                moonintensity = 1 - (time / 21600);
            }
            else //night
            {
                moonintensity = 1 - (86400-time) / 21600;
            }
        }
        sun.GetComponent<Light>().intensity = sunintensity;
        moon.GetComponent<Light>().intensity = moonintensity;
    }

    public float Get_Time()
    {
        return time;
    }

    public int Get_Days()
    {
        return days;
    }

    public float Get_Speed()
    {
        return speed;
    }

    public void Change_Speed(float speedUpdate)
    {
        speed = speedUpdate;
    }
}
