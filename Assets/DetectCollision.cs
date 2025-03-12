using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public Transform player;
    public AudioSource soundClip;


    private void Update()
    {
        transform.position = player.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 9.5f)
        {
            Debug.Log("hit");
            soundClip.Play();
        }
    }
}
