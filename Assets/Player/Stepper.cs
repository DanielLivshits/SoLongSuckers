using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stepper : MonoBehaviour
{
    [SerializeField] Transform homeTransform;

    [SerializeField] float wantStepAtDistance;

    [SerializeField] float moveDuration;

    public bool Moving;

    IEnumerator MoveToHome()
    {
        Moving = true;

        Quaternion startRot = transform.rotation;
        Vector3 startPoint = transform.position;

        Quaternion endRot = homeTransform.rotation;
        Vector3 endPoint = homeTransform.position;

        float timeElapsed = 0;


        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;

            transform.position = Vector3.Lerp(startPoint, endPoint, normalizedTime);
            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;

        }

        while (timeElapsed < moveDuration);

        Moving = false;

    }


    public void Update()
    {
        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        if (distFromHome > wantStepAtDistance)
        {
            StartCoroutine(MoveToHome());
        }


    }

}
