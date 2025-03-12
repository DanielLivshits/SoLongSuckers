using UnityEngine;

public abstract class Detector : MonoBehaviour {

    public Transform target;        // target object to be detected
    public Vector3 faceDirection;   // direction that the detector faces
    public float viewAngle = 40.0F; // furthest angle that the detector can see in either direction from its forward vector
                                    // (the total fov angle is double this value) 
    public float viewDist = 20.0F;   // maximum distance the detector can see
    public float proxDist = 5.0F;
    public bool inView = false;    // keeps track of if the target is in view or not
    public bool inProx = false;

   

    public void checkProximity() {
        Vector3 toTarget = target.position - transform.position;
        float distToTarget = toTarget.magnitude;

        if (distToTarget <= proxDist) { 
            // check if target moved to "in view"
            /*if (!inProx) {
                inProxOnce();
                inProx = true;
            }*/
            inProx = true;
        } else {
            inProx = false;
        }
        

        // check if target moved to "out of view"
        /*else if (inProx) {
            outOfProxOnce();
            inProx = false;
        }*/

    }
    public void checkAngleView() {
        Vector3 toTarget = target.position - transform.position;
        float distToTarget = toTarget.magnitude;
        float angleToTarget = Vector3.Angle(toTarget, transform.forward);



        // check distance and angles are within range
        if (distToTarget <= viewDist && angleToTarget <= viewAngle) { 
            // check if target moved to "in view"
            /*if (!inView) {
                inViewOnce();
                inView = true;
            }*/
            inView = true;
        } else {
            inView = false;
        }
        

        // check if target moved to "out of view"
        /*else if (inView) {
            outOfViewOnce();
            inView = false;
        }*/
    }

    //public abstract void inViewOnce();
    //public abstract void inProxOnce();

    //public abstract void outOfProxOnce();
    //public abstract void outOfViewOnce();



    // to see viewing range in scene editor
    void OnDrawGizmos() {
    

        Vector3 leftLimit = Quaternion.Euler(0, -viewAngle, 0) * transform.forward * viewDist;
        Vector3 rightLimit = Quaternion.Euler(0, viewAngle, 0) * transform.forward * viewDist;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftLimit);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit);
    }   
}
