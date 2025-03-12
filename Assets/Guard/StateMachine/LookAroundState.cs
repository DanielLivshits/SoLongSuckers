using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundState : IGuardState
{
    private GuardStateMachine guard;

    private float animTimer;

    public LookAroundState(GuardStateMachine guard)
    {
        this.guard = guard;
        
    }
    public void Enter()
    {
        
        this.guard.animator.Play("LookAroundAnim", 0, 0f);
        animTimer = 0;
        Debug.Log("Looking");
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
            this.guard.transform.parent.position = this.guard.GOrigin;
            this.guard.ChangeState(this.guard.patrolState);
        }
        animTimer += Time.deltaTime;
        if (animTimer >= 4f)
        {
            this.guard.ChangeState(this.guard.patrolState);
        }
    }

    public void Exit()
    {

    }
}
