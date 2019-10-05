using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SampleAgent : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    public GameObject[] targets;
    public float changeTargetDistance = 3;
    int t;

    // Start is called before the first frame update
    void Start()
    {
        //grab targets using tags
        if (targets == null || targets.Length == 0)
        {
            targets = GameObject.FindGameObjectsWithTag("Target");
        }
        Debug.Log(this.name + " has " + targets.Length + "Targets");

        agent = GetComponent<NavMeshAgent>(); //set the agent variable to this game object's navmesh
        t = 0;
        target = targets[t].transform;
        agent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {       
        float distanceToTarget = Vector3.Distance(agent.transform.position, target.position);
        if (changeTargetDistance > distanceToTarget)
        {
            t++;
            if(t == targets.Length)
            {
                t = 0;
            }
            Debug.Log(this.name + " Change Target: " + t);
            target = targets[t].transform;
            agent.SetDestination(target.position); //each frame set the agent's destination to the target position
        }        
    }
}
