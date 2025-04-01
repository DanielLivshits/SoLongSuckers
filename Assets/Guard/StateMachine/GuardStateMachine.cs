using UnityEngine;
using System.Collections.Generic;


public class GuardStateMachine : Detector {
    public IGuardState currentState { get; private set; }
    public PatrolState patrolState;
    public ChaseState chaseState;
    public SearchState searchState;
    public LookAroundState lookAroundState;
    public List<Transform> patrolPoints;
    public Animator animator;

    public GameObject alert;

    public Vector3 GOrigin;

    public bool hasGrabbed;


  //  public Vector3 LKL;

    public UnityEngine.AI.NavMeshAgent agent;

    public void ChangeState(IGuardState nextState) {
        if (currentState != null) {
            currentState.Exit();
        }
        currentState = nextState;
        if (currentState != null) {
            nextState.Enter();
        }
       
    }

    void Awake() {
        hasGrabbed = false;
        GOrigin = this.gameObject.transform.parent.position;
        patrolState = new PatrolState(this, this.patrolPoints);
        chaseState = new ChaseState(this);
        searchState = new SearchState(this);
        lookAroundState = new LookAroundState(this);

      //  animator = GetComponentInChildren<Animator>();
    }

    void Start() {
        ChangeState(this.patrolState);
    }
    
    void Update() {
        checkAngleView();
        checkProximity();
        if (currentState != null) {
            currentState.Update();
        }
    }

  
  
}