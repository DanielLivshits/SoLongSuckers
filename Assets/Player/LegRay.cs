using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegRay : MonoBehaviour
{
    public Transform targetPos1, targetPos2;

    public Transform legTarget;

    private void Start()
    {


    }
    private void Update()
    {

        RaycastHit hit;


        if (!Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            if (!Physics.Raycast(targetPos1.position, targetPos1.forward, out hit, 1f))
            {
                Physics.Raycast(targetPos2.position, targetPos2.forward, out hit, 1f);
               
            }
        }

        legTarget.position = hit.point;
    }



}

