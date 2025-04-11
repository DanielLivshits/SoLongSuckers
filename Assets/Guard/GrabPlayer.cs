using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPlayer : MonoBehaviour
{
    public static GrabPlayer Instance;

    public GuardStateMachine guardSM;
    private void Awake()
    {

        guardSM = GetComponentInParent<GuardStateMachine>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guardSM.hasGrabbed = true;
            other.gameObject.GetComponent<CharacterMovement>().GoCheckpoint();
        }
    }
}
