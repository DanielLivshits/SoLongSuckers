using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : IGuardState
{
    private GuardStateMachine guard;

    public SearchState(GuardStateMachine guard)
    {
        this.guard = guard;
    }

    public void Enter()
    {
        this.guard.target.gameObject.GetComponent<CharacterMovement>().OnHidden();
        this.guard.agent.SetDestination(this.guard.chaseState.LKL);
        Debug.Log("Search State");
        this.guard.agent.isStopped = false;
    }
    public void Update()
    {
        if ((this.guard.inView || this.guard.inProx) && !this.guard.target.gameObject.GetComponent<CharacterMovement>().isHidden)
        {
            this.guard.ChangeState(this.guard.chaseState);
            return;
        }
        if (this.guard.hasGrabbed)
        {
            this.guard.agent.isStopped = true;
            this.guard.transform.parent.position = this.guard.GOrigin;
            this.guard.ChangeState(this.guard.patrolState);
            this.guard.agent.isStopped = false;
        }
        if (!this.guard.agent.pathPending)
        {
            if (this.guard.agent.remainingDistance <= this.guard.agent.stoppingDistance)
                if (!this.guard.agent.hasPath || this.guard.agent.velocity.sqrMagnitude == 0f)
                {
                    this.guard.agent.isStopped = true;
                    Debug.Log("Searching LKL");
                    this.guard.ChangeState(this.guard.lookAroundState);
                }
        }
    }
    public void Exit()
    {
        this.guard.alert.SetActive(false);
    }
}
