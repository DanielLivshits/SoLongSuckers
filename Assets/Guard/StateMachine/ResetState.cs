using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetState : IGuardState
{
    private GuardStateMachine guard;
    public ResetState(GuardStateMachine guard)
    {
        this.guard = guard;
    }

    public void Enter()
    {
        Debug.Log("guardReset");

        this.guard.agent.isStopped = true;
        this.guard.transform.parent.position = this.guard.GOrigin;
        this.guard.hasGrabbed = false;
        this.guard.agent.isStopped = false;
    }
    public void Update()
    {
        this.guard.ChangeState(this.guard.patrolState);

    }
    public void Exit()
    {

    }

}

