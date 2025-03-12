using UnityEngine;
using UnityEngine.AI;

public class MovingTargetPathfinding : Detector
{
    public NavMeshAgent agent;
    //public Transform target;
    
    private Vector3 curTargetPos;

    void Start() {
        // initially grab target's position
        curTargetPos = target.position;
    }

    void Update()
    { 
        // from Detector: ensure target is in view
        checkAngleView();
        checkProximity();
        /*
        if (inView || inProx) {
            inViewUpdate();
        } else {
            outOfViewUpdate();
        }*/
    }

    /* *public override void inViewOnce() {
        * // only update AI targetposition if target has moved
        * /*if (curTargetPos != target.position) {
        *    curTargetPos = target.position;
        *    agent.SetDestination(curTargetPos);
        * */
        /*Debug.Log("In view");
    }

    public override void inViewUpdate() {
        if (curTargetPos != target.position) {
            curTargetPos = target.position;
            agent.SetDestination(curTargetPos);
        }
    }

    public override void outOfViewOnce() {
        // once target is out of view, stop moving
        //curTargetPos = agent.transform.position; 
        Debug.Log("Out of view");
    }

    public override void outOfViewUpdate() {

    }

    public override void inProxOnce() {

    }

    public override void outOfProxOnce() {

    }*/
}
