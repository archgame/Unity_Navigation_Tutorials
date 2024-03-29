﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Bicycle : MonoBehaviour
{
    #region GLOBAL VARIABLES
    GameObject target;
    NavMeshAgent agent;
    public bool isRider = false;


    [Header("Target Info")]
    public string[] targetNames;
    [HideInInspector]
    public Vector3 position;
    public float changeTargetDistance = 3;
    private int t;
    public bool shuffleTargets = true;
    public GameObject[] targets;

    [Header("Wait Times")]
    public float waitTime = 60;
    private bool waiting = false;
    private float waited = 0;
    private bool aligned = false;
    private bool parking = false;
    GameObject lastTarget;


    [Header("Agent Size")]
    public float TurningMultiplier = 1;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Agent";

        //grab targets using tags
        if (targets.Length == 0)
        {
            //get all game objects tagged with "Target"
            targets = GameObject.FindGameObjectsWithTag("Target");

            List<GameObject> targetList = new List<GameObject>();           
            foreach(GameObject go in targets) //search all "Target" game objects
            {
                //Debug.Log("go: " + go.name);
                foreach (string targetName in targetNames)
                {
                    //Debug.Log("targetName: " + targetName);
                    // "Target" contains: "Tar", "Targ", "get", ! "Trgt"
                    if (go.name.Contains(targetName)) //if GameObject has the same name as targetName, add to list
                    {
                        targetList.Add(go);
                    }
                }
            }
            targets = targetList.ToArray(); //Convert List to Array, because other code is still using array
        }

        //shuffle targets
        if (shuffleTargets)
        {
            targets = Shuffle(targets);
        }

        agent = GetComponent<NavMeshAgent>(); //set the agent variable to this game object's navmesh
        t = 0;
        target = targets[t];
        agent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.enabled)
        {
            //update target if it moves
            if (target.transform.position != position)
            {
                position = target.transform.position;
                agent.SetDestination(position);
            }

            //original text if (!waiting) // (waiting == false) (1 == 0)
            if (waiting) // (waiting == false) (1 == 0)
            {
                if (waited > waitTime)
                {
                    waiting = false;
                    agent.isStopped = false;
                    waited = 0;
                    PickUp[] pickups = gameObject.GetComponentsInChildren<PickUp>();
                    if (pickups.Length > 0)
                    {
                        pickups[0].peopleAtStop = 0;
                    }
                }
                else
                {
                    waited += Time.deltaTime;
                }

            } //if waiting
            else
            {
                //see agent's next destination
                Debug.DrawLine(transform.position, agent.steeringTarget, Color.black);
                Debug.DrawLine(transform.position, agent.pathEndPosition, Color.cyan);
                Debug.DrawRay(agent.pathEndPosition, Vector3.up * 40, Color.red);
                Debug.DrawRay(target.transform.position, Vector3.up * 40, Color.yellow);

                float distanceToTarget = Vector3.Distance(agent.transform.position, target.transform.position);
                //change target once it is reached
                if (changeTargetDistance > distanceToTarget) //have we reached our target
                {

                    parking = true;
                    lastTarget = target;

                    t++;
                    if (t == targets.Length)
                    {
                        t = 0;
                    }
                    //Debug.Log(this.name + " Change Target: " + t);
                    target = targets[t];
                    agent.SetDestination(target.transform.position); //each frame set the agent's destination to the target position
                    waiting = true;
                    agent.isStopped = true;                   
                } // changeTargetDistance test

                //Debug.Log(gameObject.name + " : " + agent.hasPath);
                if (!agent.hasPath) //cath agent error when agent doesn't resume
                {
                    position = target.transform.position;
                    agent.SetDestination(position);
                }

                if (agent.hasPath)
                {
                    Vector3 toSteeringTarget = agent.steeringTarget - transform.position;
                    float turnAngle = Vector3.Angle(transform.forward, toSteeringTarget);
                    //agent.acceleration = turnAngle * agent.speed * TurningMultiplier;
                }
            }

            if(target.name.Contains("BikeStop"))
            {
                GameObject[] spots = target.GetComponent<BikeStop>().spots;
                GameObject[] bikes = target.GetComponent<BikeStop>().bikes;

                for(int i = 0;i<bikes.Length;i++)
                {
                    if(bikes[i] == null)
                    {
                        bikes[i] = gameObject;
                        target = spots[i];
                        position = target.transform.position;
                        agent.SetDestination(position);
                        break;
                    } //if there is no bike in the spot
                } // for each bike spot       
                
            } // if "BikeStop"
        }

        if (parking)
        {
            Vector3 parkingPosition = lastTarget.transform.position + lastTarget.transform.right * 1.8f; //parking spot position
            Debug.DrawRay(parkingPosition, Vector3.up * 10, Color.magenta);
            Debug.DrawRay(lastTarget.transform.position, Vector3.up * 10,Color.black);                       
            Vector3 levelPosition = new Vector3(
                parkingPosition.x, 
                transform.position.y, 
                parkingPosition.z); //parking spot position at bicycle height

            float dist = Vector3.Distance(levelPosition, transform.position);
            if (transform.position != levelPosition)
            {
                Debug.Log("parking...");
                transform.position = Vector3.Lerp(
                    transform.position,
                    levelPosition,
                    0.02f); //smoothly moving bicycle to parking spot
                transform.forward = Vector3.Lerp(
                    transform.forward,
                    -lastTarget.transform.right,
                    0.04f); //smoothly moving bicycle to parking spot
                if (dist < 0.1f)
                {
                    parking = false;
                    aligned = true;
                }
            }
        }
        if (aligned)
        {
            Vector3 parkingPosition = lastTarget.transform.position; //parking spot position
            Vector3 levelPosition = new Vector3(
                parkingPosition.x,
                transform.position.y,
                parkingPosition.z); //parking spot position at bicycle height

            float dist = Vector3.Distance(levelPosition, transform.position);
            if (transform.position != levelPosition)
            {
                Debug.Log("parking...");
                transform.position = Vector3.Lerp(
                    transform.position,
                    levelPosition,
                    0.02f); //smoothly moving bicycle to parking spot
                transform.forward = Vector3.Lerp(
                    transform.forward,
                    -lastTarget.transform.right,
                    0.04f); //smoothly moving bicycle to parking spot
                if (dist < 0.1f)
                {
                    aligned = false;
                }
            }





        }

        NavMeshHit navHit;
        agent.SamplePathPosition(-1, 0.0f, out navHit);
        Debug.Log("mask: " + navHit.mask);
        int parkArea = 1 << NavMesh.GetAreaFromName("Park");
        Debug.Log("parkArea " + parkArea);
        if (parkArea == navHit.mask)
        {
            agent.speed = 2;
            agent.acceleration = 2;
            Debug.Log("Change Speed");
        }
        else
        {
            agent.speed = 16;
            agent.acceleration = 16;
        }

        string[] names = GameObjectUtility.GetNavMeshAreaNames();
        for (int i = 0; i < names.Length; ++i)
        {
            int mask = 1 << NavMesh.GetAreaFromName(names[i]);
            if ((navHit.mask & mask) != 0)
            {
                //Debug.Log(".. that's \"" + names[i] + "\"");
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {

    }

    void OnTriggerExit(Collider collision)
    {

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
