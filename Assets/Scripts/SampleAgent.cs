using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SampleAgent : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); //set the agent variable to this game object's navmesh
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position); //each frame set the agent's destination to the target position
    }
}
