using UnityEngine;

public class ChaseState : IGuardState {
    private GuardStateMachine guard;

    public Vector3 LKL;
    public ChaseState(GuardStateMachine guard) {
        this.guard = guard;
    }

    public void Enter() {
        Debug.Log("GUARD STATE: CHASE");
        this.guard.alert.SetActive(true);
        this.guard.target.gameObject.GetComponent<CharacterMovement>().OnSpotted();

        this.guard.agent.SetDestination(this.guard.target.position);
        this.guard.agent.isStopped = false;
    }
    public void Update() {
        // check for change state
        if (!this.guard.inView && !this.guard.inProx) {
            this.LKL.x = this.guard.target.position.x;
            this.LKL.y = this.guard.target.position.y;
            this.LKL.z = this.guard.target.position.z;
            this.guard.ChangeState(this.guard.searchState);
            return;
        }
        if (this.guard.hasGrabbed)
        {
            this.guard.ChangeState(this.guard.resetState);

        }
        if (this.guard.target.position != this.guard.agent.destination) {
            this.guard.agent.SetDestination(this.guard.target.position);
        }

    }
    public void Exit() {
        this.guard.agent.isStopped = true;
    }

}