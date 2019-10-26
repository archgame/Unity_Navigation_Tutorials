using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SampleAgent : MonoBehaviour
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
    public float waitTimeShortMin = 0;
    public float waitTimeShortMax = 0;
    public float waitTimeLongMin = 0;
    public float waitTimeLongMax = 0;

    public float waitTime = 0;
    private bool waiting = false;
    private float waited = 0;

    [Header("Agent Size")]
    public bool randomScale = false;
    public float xmin = 1;
    public float xmax = 1;
    public float ymin = 1;
    public float ymax = 1;
    public float zmin = 1;
    public float zmax = 1;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Agent";

        //scale the gameobject randomly
        if (randomScale)
        {
            float x = Random.Range(xmin, xmax);
            float y = Random.Range(ymin, ymax);
            float z = Random.Range(zmin, zmax);
            transform.localScale = new Vector3(x, y, z);
        }

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
        //Debug.Log(this.name + " has " + targets.Length + "Targets");

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
                    PickUp[] pickups = gameObject.GetComponentsInChildren<PickUp>();
                    if(pickups.Length > 0)
                    {
                        int riderCount = pickups[0].peopleAtStop;
                        Debug.Log("riderCount: " + riderCount);
                        if (riderCount > 0)
                        {                          
                            waitTime = waitTimeShortMax * riderCount;
                        }else
                        {
                            waitTime = waitTimeShortMin;
                        }
                    }
                    else
                    {
                        waitTime = waitTimeShortMin;
                    }
                    Debug.Log("waitTime: " + waitTime);

                    //type of stop
                    /*
                    if (target.name.Contains("long"))
                    {
                        //Debug.Log("long wait");
                        waitTime = Random.Range(waitTimeLongMin, waitTimeLongMax);
                    }
                    if (target.name.Contains("short"))
                    {
                        //Debug.Log("short");
                        waitTime = Random.Range(waitTimeShortMin, waitTimeShortMax);
                    }
                    //*/
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

                Debug.Log(gameObject.name + " : " + agent.hasPath);
                if (!agent.hasPath) //cath agent error when agent doesn't resume
                {
                    position = target.transform.position;
                    agent.SetDestination(position);
                }
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("collision: " + collision.gameObject.name);
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pedestrian"))
        {
            agent.isStopped = true;
            obstacles++; // obstacles = obstacles + 1; || obstacles += 1;
        }       
    }

    void OnTriggerExit(Collider collision)
    {
        //Debug.Log("exited");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pedestrian"))
        {
            obstacles--; //obstacles = obstacles - 1; || obstacles -= 1;
        }
        if (obstacles == 0) //once there are zero obstacles, start the agent moving
        {
            agent.isStopped = false;
        }
    }

    private int obstacles = 0;

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
