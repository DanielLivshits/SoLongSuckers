using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GrabPlayer grabPlayer;

    private void Awake()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
 
            other.gameObject.GetComponent<CharacterMovement>().currentCheckP = this.transform;
        }
    }

}
