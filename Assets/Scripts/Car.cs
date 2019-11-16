using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Car : MonoBehaviour
{
    #region GLOBAL VARIABLES
    GameObject target;
    NavMeshAgent agent;
    public bool isRider = false;


    [Header("Target Info")]
    public string targetTag = "Target";
    public string[] targetNames;
    public char splitter = '-';
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

    DayNightCycle timeScript;
    bool nightime = true;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //target setup
        gameObject.tag = "Agent";
        agent = GetComponent<NavMeshAgent>();

        targets = GetTargets(targetTag, new string[] { targetNames[0] });
        targets = OrderByLastNamePart(targets, splitter);
        if(shuffleTargets)
        {
            targets = Shuffle(targets);
        }      
        target = targets[0];
        agent.SetDestination(target.transform.position);

        //time
        timeScript = Camera.main.GetComponent<DayNightCycle>();
        float now = timeScript.Get_Time();
        if (nightime && now > 21600 && now < 64800) //daytime
        {
            nightime = false;
        }
        else
        {
            nightime = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //time
        float now = timeScript.Get_Time();
        if(nightime && now > 21600 && now < 64800) //daytime
        {
            nightime = false;
            targets = new GameObject[0];
            targets = GetTargets(targetTag, new string[] { targetNames[1] });
            targets = OrderByLastNamePart(targets, splitter);
            if (shuffleTargets)
            {
                targets = Shuffle(targets);
            }
            target = targets[0];
            agent.SetDestination(target.transform.position);
        }
        if (!nightime)
        {
            if (now < 21600 || now > 64800)//nighttime
            {
                nightime = true;
                targets = new GameObject[0];
                targets = GetTargets(targetTag, new string[] { targetNames[0] });
                targets = OrderByLastNamePart(targets, splitter);
                if (shuffleTargets)
                {
                    targets = Shuffle(targets);
                }
                target = targets[0];
                agent.SetDestination(target.transform.position);
            }
        }

        //agent control
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
                    if (pickups.Length > 0)
                    {
                        int riderCount = pickups[0].peopleAtStop;
                        Debug.Log("riderCount: " + riderCount);
                        if (riderCount > 0)
                        {
                            waitTime = waitTimeShortMax * riderCount;
                        }
                        else
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pedestrian"))
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

    private GameObject[] Shuffle(GameObject[] objects)
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

    private GameObject[] GetTargets(string tag, string[] targetByNames)
    {
        //get all game objects tagged with "Target"
        GameObject[] goArray = GameObject.FindGameObjectsWithTag(tag);

        List<GameObject> targetList = new List<GameObject>();
        foreach (GameObject go in goArray) //search all "Target" game objects
        {
            //Debug.Log("go: " + go.name);
            foreach (string targetName in targetByNames)
            {
                //Debug.Log("targetName: " + targetName);
                // "Target" contains: "Tar", "Targ", "get", ! "Trgt"
                if (go.name.Contains(targetName)) //if GameObject has the same name as targetName, add to list
                {
                    targetList.Add(go);
                }
            }
        }
        return targetList.ToArray(); //Convert List to Array, because other code is still using array
        //Debug.Log(this.name + " has " + targets.Length + "Targets");
    }

    private GameObject[] OrderByLastNamePart(GameObject[] unsortedArray, char c)
    {
        GameObject[] sortedArray = unsortedArray.OrderBy(go => 
        go.name.Split(c)[go.name.Split(c).Length - 1]
        ).ToArray();
        //sortedArray = sortedArray.Reverse().ToArray();
        return sortedArray;
    }
}
