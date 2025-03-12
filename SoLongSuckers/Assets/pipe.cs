using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class pipe : MonoBehaviour
{
    public Transform inP, outP, player;
    public GameObject playerq;
    public GameObject playerCam, pipeCam;
    public Transform pipeOrientation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerq.GetComponent<CharacterMovement>().isInPipe)   //leave the pipe
            {
                playerq.GetComponent<CharacterMovement>().isSuckersOn = false;
                playerq.GetComponent<CharacterMovement>().OnPipe(pipeOrientation);

                pipeCam.SetActive(false);
                playerCam.SetActive(true);
              //  pipeCam.GetComponent<CinemachineVirtualCamera>().Priority = 10;
              //  playerCam.GetComponent<CinemachineVirtualCamera>().Priority = 20;


                playerq.SetActive(false);
                player.position = outP.position;
                playerq.SetActive(true);
            }
            else if (!playerq.GetComponent<CharacterMovement>().isInPipe)  //enter the pipe
            {
                playerq.GetComponent<CharacterMovement>().OnPipe(pipeOrientation);

                playerCam.SetActive(false);
                pipeCam.SetActive(true);
               // playerCam.GetComponent<CinemachineVirtualCamera>().Priority = 10;
               // pipeCam.GetComponent<CinemachineVirtualCamera>().Priority = 20;

                playerq.SetActive(false);
                player.position = inP.position;
                playerq.SetActive(true);
            }

        }
    }
}
