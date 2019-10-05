using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SampleAgent : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    public Transform[] targets;
    public float changeTargetDistance = 3;
    int t;

    // Start is called before the first frame update
    void Start()
    {


        agent = GetComponent<NavMeshAgent>(); //set the agent variable to this game object's navmesh
        t = 0;
        target = targets[t];
        agent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        
        float distanceToTarget = Vector3.Distance(agent.transform.position, target.position);
        //Debug.Log("distanceToTarget: " + distanceToTarget);
        if (changeTargetDistance > distanceToTarget)
        {
            t++;
            if(t == targets.Length)
            {
                t = 0;
            }

            Debug.Log(this.name + " Change Target: " + t);
            target = targets[t];
            agent.SetDestination(target.position); //each frame set the agent's destination to the target position
        }        
    }
}
