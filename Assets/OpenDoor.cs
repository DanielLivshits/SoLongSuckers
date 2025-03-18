using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door;
    public GameObject screen;
    public Material green;

    public AudioSource audioSource;

    public AudioClip doorOpenSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.clip = doorOpenSound;
            audioSource.Play();

            door.SetActive(false);
            screen.GetComponent<Renderer>().material = green;
        }
    }
}
