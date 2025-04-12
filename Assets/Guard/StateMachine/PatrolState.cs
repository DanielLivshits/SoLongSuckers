using UnityEngine;
using System.Collections.Generic;

public class PatrolState : IGuardState {
    private GuardStateMachine guard;
    private List<Transform> patrolPoints;

    private int curIdx;

   // private bool canSee = this.guard.target.gameObject.GetComponent<CharacterMovement>().isHidden;
   // private Vector3 curPoint;

    public PatrolState(GuardStateMachine guard, List<Transform> patrolPoints) {
        this.guard = guard;
        this.patrolPoints = patrolPoints;
        this.curIdx = 0;
    }

    private void nextPoint() {
        this.curIdx = (this.curIdx + 1) % this.patrolPoints.Count;
    }
    private void setGuardDest() {

        Vector3 conv = new Vector3(this.patrolPoints[this.curIdx].position.x, this.patrolPoints[this.curIdx].position.y, this.patrolPoints[this.curIdx].position.z);

        this.guard.agent.SetDestination(conv);


      //  this.guard.agent.SetDestination(this.patrolPoints[this.curIdx]);
    }

    public void Enter() {
        Debug.Log("GUARD STATE: PATROL");

        this.guard.animator.SetBool("IsWalking", true);
        this.guard.hasGrabbed = false;

        //this.curPoint = this.patrolPoints[curIdx];
        if (this.patrolPoints.Count > 0) {
            this.setGuardDest();
            this.guard.agent.isStopped = false;
        }
    }
    public void Update() {
        
        // check for state change
        if ((this.guard.inView || this.guard.inProx) && !this.guard.target.gameObject.GetComponent<CharacterMovement>().isHidden) {
            this.guard.ChangeState(this.guard.chaseState);
            return;
        }
        if (this.guard.hasGrabbed)
        {
            this.guard.ChangeState(this.guard.resetState);
        }
        // if current position == target
        // then get next position
        if (this.patrolPoints.Count > 0) {

            if (!this.guard.agent.pathPending) {
                if (this.guard.agent.remainingDistance <= this.guard.agent.stoppingDistance)
                    if (!this.guard.agent.hasPath || this.guard.agent.velocity.sqrMagnitude == 0f) {
                        Debug.Log("Patrol desitination reached");
                        this.nextPoint();

                        this.guard.ChangeState(this.guard.lookAroundState);

                        this.setGuardDest();
                    }
            }

            /*if (this.guard.agent.transform.position == this.guard.agent.destination) {
                Debug.Log("Patrol desitination reached");
                this.nextPoint();
                //this.curPoint = this.patrolPoints[this.curIdx];
                //this.guard.agent.SetDestination(curPoint);
                this.setGuardDest();
            }*/
        }
    }
    public void Exit() {
        this.guard.agent.isStopped = true;
    }
}