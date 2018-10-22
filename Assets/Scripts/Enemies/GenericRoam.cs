﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericRoam : MonoBehaviour {
    public float roamingDestMaxDist = 20f;

    private NavMeshAgent enemyNav;
    private TargetPlayer triggerScript;
    bool shouldRoam = true;
	// Use this for initialization
	void Start () {
        enemyNav = GetComponent<NavMeshAgent>();
        triggerScript = GetComponentInParent<TargetPlayer>();
        chooseRoamingDest();
	}
	
	// Update is called once per frame
	void Update () {

        if(!enemyNav.pathPending && !triggerScript.getPlayerTargeted() && shouldRoam) //Check if enemy has reached dest
        {
            if(enemyNav.remainingDistance <= enemyNav.stoppingDistance)
            {
                if(!enemyNav.hasPath || enemyNav.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    chooseRoamingDest();
                }
            }
        }
    }

    void chooseRoamingDest()
    {
        //Create a sphere trigger to check if the area is suitable for a destination
        GameObject checkArea = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        checkArea.AddComponent(typeof(SphereCollider));
        checkArea.AddComponent(typeof(MeshRenderer));
        checkArea.AddComponent(typeof(RoamingDestDetector));
        //checkArea.GetComponent<MeshRenderer>().enabled = false;
        checkArea.GetComponent<SphereCollider>().isTrigger = true;
        checkArea.name = "Check Sphere";
        findValidDest(checkArea);
        
    }

    void findValidDest(GameObject checkArea)
    {
        checkArea.transform.position = new Vector3(transform.position.x, transform.position.y + 0.46f, transform.position.z);
        Vector3 raycastDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); //Choose a random direction to go in
        RaycastHit dest;
        if (!Physics.Raycast(transform.position, raycastDir, out dest, roamingDestMaxDist)) //If the raycast didnt find any obstructions
        {
            Vector3 endPos = transform.position + raycastDir * roamingDestMaxDist;
            checkArea.transform.position = endPos;
            if (checkArea.GetComponent<RoamingDestDetector>().getCollidingObj().Count <= 0)
            {
                enemyNav.destination = endPos;
            }
            else
            {
                findValidDest(checkArea);
            }
            
            Destroy(checkArea);
        }
        else
        {
            findValidDest(checkArea);
        }
    }

    public void toggleRoaming(bool roam)
    {
        shouldRoam = roam;
        enemyNav.SetDestination(transform.position);
    }
}