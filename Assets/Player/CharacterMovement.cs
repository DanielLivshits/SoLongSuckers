using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   //for accessing new input system
using UnityEngine.UI;            //for accessing UI
//using UnityEngine.UIElements;
using TMPro;                     //for accessing UI texts
using UnityEngine.SceneManagement;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]  //components required for the script

public class CharacterMovement : MonoBehaviour
{

    //Basic Variables

    private CharacterController controller; //Character controller

    private PlayerInput playerInput;        //Input Action Asset

    private InputAction moveAction;         //Input actions

    private InputAction stickAction;

    private InputAction camoAction;

    private InputAction removeAction;

    private InputAction pauseAction;

    private Transform cameraTransform;      //Position of the camera

    float delta;                            //Time.deltaTime



    //Movement Variables

    Vector2 moveInput;                      //Player's movement input

    private Vector3 playerVelocity;         //May not be necessary- mostly adds gravity on y 

    private bool groundedPlayer;            //Whether or not the player is touching the ground

    public float playerSpeed = 6.0f;        //Player speed

    public float rotationSpeed = 2.0f;       //Player rotation speed

    public float gravityValue = -9.81f;     //gravity




    //climbing Variables

    public bool isClimbing;                 //Whether or not the player is climbing  

    bool inPosition;                        //Whether or not the player is in position

    bool isLerping;                         //Whether or not the player is currently moving between postions

    float posT;                             //Keeps track of movement timing

    Vector3 startPos;                       //Starting position of a climb segment

    Vector3 targetPos;                      //Target position of a climb segment

    Quaternion startRot;                    //Unused, may be used for IK

    Quaternion targetRot;                   //Unused, may be used for IK

    public float offsetFromWall = 0.3f;     //How close to the wall the player is while climbing

    public float climbSpeed = 3.0f;         //Player's climb speed

    public float rotateSpeed = 5.0f;        //How fast the player can rotate while climbing

    public float rayTowardsMoveDir = 0.5f;  //How far the ray towards the move direction goes

    public float rayForwardTowardsWall = 1.0f;  //How far the ray towards the wall goes

    public float horizontal;                //Move input on the x axis

    public float vertical;                  //Move input on the y axis

    Transform helper;                       //A target that helps determine the position for the player to move to

    public float originVertOffset = 0.25f;  //Height of the vertical offset when you start climbing  



    //Camoflauge Variables

    public Renderer playerRender;          //The player's renderer component

    Renderer camoMat;                      //Renderer of the object player is attempting to camouflage as

    public Material playerMat;             //Player's default material

    

    //State Switch Variables

    public bool isSuckersOn;                      //Whether or not the suckers are currently active

    string currentMode;                    //Current player state: "Move", "Climb", "Pipe"
 



    //Pipe movement (WIP)



    public bool isInPipe;     //whether or not the player is in a pipe

    Transform pipeOrientPass;


    public Image SuckersUI;

    public Sprite SuckersOn, SuckersOff;

    public Image staminaBar;

    public Image stamiaEmpty;

    public float stamina, maxStamina;

    public float climbCost;

    public bool staminaCooldown = false;



    public Transform underClimber;

    public Transform legCenter;

    public bool isHidden;

    Vector2 input;   //Movement input is a vector2 with x and y values

    public AudioSource impactSound;

    // Adding walking sounds here

    public AudioSource walkSource;

    public AudioClip[] walkingSounds; 
    public AudioClip[] pipeWalkingSounds;

    public float nextPlayTime = 0f;
    public float SFXbuffer = 0.2f;

    public Transform currentCheckP;

  //s  public Color alphaCol;

    public Sprite CamoHidden, CamoSpotted;

    public Image camoIcon;

    public GameObject spottedIcon;

    public Transform bodyPos;

    //  public Rig OllieRig;

    public GameObject MainModel, PipeModel;
   
    private void Awake()   //Runs on startup
    {
   
        controller = GetComponent<CharacterController>();    //Gets character controller component
        playerInput = GetComponent<PlayerInput>();           //Gets player input system

        cameraTransform = Camera.main.transform;             //Gets main camera transform

        playerRender.material = playerMat;

      //  alphaCol = staminaBar.color;
       // alphaCol.a = 0.5f;

        moveAction = playerInput.actions["Move"];            //Gets actions from inout action asset
        stickAction = playerInput.actions["Stick"];
        camoAction = playerInput.actions["Camo"];
        removeAction = playerInput.actions["RemoveCamo"];
        pauseAction = playerInput.actions["Pause"];

       // Cursor.lockState = CursorLockMode.Locked;

        isSuckersOn = false;                                //Suckers start as off

        currentMode = "Move";                               //Player starts in Move mode

        helper = new GameObject().transform;                //Creates an empty gameobject to be the climbing helper
        helper.name = "Climb Helper";                       //Renames the climbing helper

        stamina = maxStamina;
        staminaBar.fillAmount = stamina/maxStamina;

        stickAction.performed += onSuckers;
        stickAction.canceled += OffSuckers;
    }

    void Update()       //Runs once every frame
    {
     //   if (stickAction.triggered)       //if the player presses an input for the suckers
     //   {
     //       isSuckersOn = true;
    //    }
    //    else
    //    {
    //        isSuckersOn = false;
     //   }
        if (camoAction.triggered && !staminaCooldown && (input.x == 0 && input.y == 0))       //if the player presses an input for the camo
        {
            OnCamo();
        }
        if (removeAction.triggered)
        {
            colourReset();
        }
        if (pauseAction.triggered)
        {
            OnPause();
        }


       // onSuckers();
        CheckHidden();



        UpdateState();     

    }

    void UpdateState()   //Movement states generally need to be updated every frame, so a separate state was made to handle which movement system is currently being used
    {


        if (currentMode == "Pipe")
        {
            regenStamina();
            PipeMove();
        }

        if (currentMode == "Move")
        {
            if (isSuckersOn)       //The player can only enter the climb state if they have suckers on while moving
            {
                CheckForClimb();    //checks if the player is able to climb
            }
            else
            {
                regenStamina();
                playerMoveState();
            }

        }
        else if (currentMode == "Climb")
        {
 
            playerClimbState();
        }
    }

    void playerMoveState()       //Handles standard player movement
    {
        groundedPlayer = controller.isGrounded;      //will need to test
        if (groundedPlayer && playerVelocity.y < 0)  
        {
            playerVelocity.y = 0f;     
        }

        input = moveAction.ReadValue<Vector2>();   //Movement input is a vector2 with x and y values

        Vector3 move = new Vector3(input.x, 0, input.y);  //Player is moving in 3D space, but the input only handles 2 axis. Y value here is up and down

        move = move.x * cameraTransform.right.normalized + move.z * this.cameraTransform.forward.normalized;  //left/right and back/forward movement is relative to the camera's postion and rotation
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);   //Moves the player based off the vector3 

        playerVelocity.y += gravityValue * Time.deltaTime;     
        controller.Move(playerVelocity * Time.deltaTime);    //adds gravity to keep the player on the ground

  

        if (input.y != 0 && input.x != 0)           //rotate towards camera  (do only when there is move input)
        {
            float targetAngle = cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);  //character turns smoothly because of the Lerp


            colourReset();
            ollieWalkSound();
        
        }

    }

    void ollieWalkSound()
    {
        if (!walkSource.isPlaying && Time.time >= nextPlayTime) // Has a buffer time so the SFX don't play on top of each other.
        {
            if (currentMode == "Pipe")
            {
                int index = Random.Range(0, pipeWalkingSounds.Length);
                walkSource.clip = pipeWalkingSounds[index];
            }
            else
            {
                int index = Random.Range(0, walkingSounds.Length);
                walkSource.clip = walkingSounds[index];
            }

            walkSource.Play();
            nextPlayTime = Time.time + SFXbuffer; 
        }
        
        
    }

    void playerClimbState()   //may need to remove and just run CLimbUpdate from UpdateState
    {
        ClimbUpdate();
    }

    void OffSuckers(InputAction.CallbackContext context)
    {
        isSuckersOn = false;
        SuckersUI.sprite = SuckersOff;

      //  alphaCol.a = 0.5f;
      //  staminaBar.color = alphaCol;
        //  suckersUI.GetComponent<TMP_Text>().text = "Suckers:OFF";
        isClimbing = false;
        if (!isSuckersOn && currentMode == "Climb")     //If suckers are turned off while climbing, player returns to the move state
        {
            currentMode = "Move";
            RotateLegs();
        }
    }

    void onSuckers(InputAction.CallbackContext context)   //Runs when the suckers button is pressed
    {

        if (!staminaCooldown)
        {

            isSuckersOn = true;
            SuckersUI.sprite = SuckersOn;

           // alphaCol.a = 1f;
         //   staminaBar.color = alphaCol;
                

        }
        


    }

    void ClimbUpdate()     //may be unecessary, test
    {

        stamina -= climbCost * Time.deltaTime;
        if (stamina < 0) stamina = 0;
        staminaBar.fillAmount = stamina/maxStamina;

        if (stamina == 0)
        {
           // ColorBlock cb = staminaBar.color;
          //  cb.normalColor = Color.red;
            staminaBar.color = Color.red;
          //  alphaCol = staminaBar.color;



            isSuckersOn = false;
            isClimbing = false;
            SuckersUI.sprite = SuckersOff;
            currentMode = "Move";
            RotateLegs();

            colourReset();


            staminaCooldown = true;
        }


        delta = Time.deltaTime;

        Tick(delta);


    }


    public void CheckForClimb()       //checks if the player can climb
    {
        Vector3 origin = transform.position;  //gets current player position
        origin.y += originVertOffset;           //vertical offset for the start of the climb
        Vector3 direction = transform.forward;    //player's forward direction
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 1.5f))        //creates a raycast
        {
            helper.position = PosWithOffset(origin, hit.point);    //If the raycast hits a surface, places the climb helper at the hit location and prepare for the climb
            InitForClimb(hit);                                    
        }
        else
        {
            if (Physics.Raycast(origin, underClimber.forward, out hit, 1.5f))        //creates a raycast
            {
                helper.position = PosWithOffset(origin, hit.point);    //If the raycast hits a surface, places the climb helper at the hit location and prepare for the climb

                InitForClimb(hit);
            }
            else
            {
                playerMoveState();       //if nothing is hit, player continues with the move state
            }

        }
    }

    void InitForClimb(RaycastHit hit)     //prepares for the climb, takes the data from the previous raycast hit
    {

        isClimbing = true;             //changes the current state to climbing

        currentMode = "Climb";


        helper.transform.rotation = Quaternion.LookRotation(-hit.normal);      //helper faces towards the wall
        startPos = transform.position;                                     //starting position is where the player currently is
        targetPos = hit.point + (hit.normal * offsetFromWall);           //target position is where the player is climbing to, including an offset from the wall
        posT = 0;                                                   //no time has elapsed
        inPosition = false;                                       //player is not in position
        RotateLegs();
    }

    public void Tick(float delta)        //plays once per frame while in the climb state
    {
        if (!inPosition)                  //if the player is not in position, tell them to get into position
        {
            GetInPosition();              
            return;
        }

        if (!isLerping)               //if the player is in position but hasn't started moving
        {

            moveInput = moveAction.ReadValue<Vector2>();          //reads user input


            horizontal = moveInput.x;                    //separates horizontal and vertical inputs
            vertical = moveInput.y;


            Vector3 h = helper.right * horizontal;       //finds coordinates for the player to move to
            Vector3 v = helper.up * vertical;
            Vector3 moveDir = (h + v).normalized;

            bool canMove = CanMove(moveDir);              //check if the player can move
            if (!canMove || moveDir == Vector3.zero)      //if the player can't move or there is no input, nothing happens
                return;                                   //nothing happens

            posT = 0;                               //time passed at 0
            isLerping = true;                       //start moving between positions
            startPos = transform.position;          //get starting and end positions

            targetPos = helper.position;

        }
        else        //if the player is moving between spots
        {

            colourReset();
            posT += delta * climbSpeed;     //moving according to time and climb speed
            if (posT > 1)                  
            {
                posT = 1;            
                isLerping = false;        //stops lerping
            }
            Vector3 cp = Vector3.Lerp(startPos, targetPos, posT);      //create a lerp between the starting and ending position
            transform.position = cp;                                  //go to interperlated postion
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);   //rotate to follow the helper's rotation
            
            // Adding walk sounds to the climbing
            ollieWalkSound();

            if (moveInput.y < 0)
            {
                LookForGround();
            }

        }
    }


    bool CanMove(Vector3 moveDir)        //check if the player can move, takes in direction player is trying to move
    {
        Vector3 origin = transform.position;   //origin is current position
        float dis = rayTowardsMoveDir;        //distance towards move direction
        Vector3 dir = moveDir;               //direction player is moving

  
     //   Debug.DrawRay(origin, dir * dis, Color.red);   
        RaycastHit hit;


        if (Physics.Raycast(origin, dir * dis, out hit, dis))       //Raycast towards the direction you want to move
        {
            helper.position = PosWithOffset(origin, hit.point);        //changes helper position and rotation to match the raycast hit
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;                                                //bool is true
        }


        if (Physics.Raycast(origin, dir, out hit, dis))        //Check if its a corner
        {
  
            return false;
        }

        origin += moveDir * dis;
        dir = helper.forward;
        float dis2 = rayForwardTowardsWall; 

 
       // Debug.DrawRay(origin, dir * dis2, Color.green);   

        if (Physics.Raycast(origin, dir, out hit, dis2))          //Raycast forwards towards the wall
        {
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        origin = origin + (dir * dis2);
        dir = -moveDir;

      //  Debug.DrawRay(origin, origin + dir, Color.magenta);

        if (Physics.Raycast(origin, dir, out hit, rayForwardTowardsWall))
        {
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }


        origin += dir * dis2;
        dir = -Vector3.up;

     //   Debug.DrawRay(origin, dir, Color.blue);
        if (Physics.Raycast(origin, dir, out hit, dis2))
        {
            float angle = Vector3.Angle(-helper.forward, hit.normal);
            if (angle < 40)
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

        }

        return false;
    }



    void GetInPosition()
    {
        colourReset();
        posT += delta * 5f;

        if (posT > 1)
        {
            posT = 1;
            inPosition = true;

        }

        Vector3 tp = Vector3.Lerp(startPos, targetPos, posT);
        transform.position = tp;
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
    }



    Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }

    void OnCamo()   //runs when the camo input is used
    {

        if (!staminaCooldown)
        {
            RaycastHit hit;

            if (isClimbing)
            {
                Physics.Raycast(this.transform.position, this.transform.forward, out hit, 2f);  //if the player is climbing, creates a raycast forward
            }
            else
            {
                Physics.Raycast(this.transform.position, -this.transform.up, out hit, 2f);  //if player is not climbing, creates a raycast down
            }


            if (hit.transform != null)
            {
                camoMat = hit.transform.gameObject.GetComponent<Renderer>();  //gets renderer information from the raycast hit

                playerRender.sharedMaterial = camoMat.sharedMaterial;   //changes player's material to an instance of the material it detected
            }
        }

    
    }


    public void OnPipe(Transform PipeOrient)   //wip, for when player enter/exits a pipe
    {
        isInPipe = !isInPipe;

        if (isInPipe && currentMode != "Pipe")  //enter pipe
        {
            // if (currentMode == "Climb")
            // {
            //    RotateLegs();
            // }
            legCenter.gameObject.SetActive(false);
            //  float targetAngle = cameraTransform.eulerAngles.y;
            // Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            MainModel.SetActive(false);
            PipeModel.SetActive(true);
            currentMode = "Pipe";

            RotateLegs();

          //  OllieRig.weight = 0;

            pipeOrientPass = PipeOrient;



        }
        else if (!isInPipe && currentMode == "Pipe")  //exit pipe
        {
            currentMode = "Move";
            PipeModel.SetActive(false);
            MainModel.SetActive(true);
            legCenter.gameObject.SetActive(true);

            RotateLegs();

           // OllieRig.weight = 1;
        }


    }


    void PipeMove()
    {

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 moveDir = pipeOrientPass.forward;

        Vector3 move = new Vector3(0, 0, input.x);



        controller.Move(moveDir.normalized * input.x * Time.deltaTime * playerSpeed);


    }

    void colourReset()
    {
        playerRender.material = playerMat;
    }


    void regenStamina()
    {
        stamina += climbCost * Time.deltaTime;
        if (stamina > maxStamina) stamina = maxStamina;
        staminaBar.fillAmount = stamina/maxStamina;


        if (staminaCooldown && stamina >= 50)
        {
        //    ColorBlock cb = staminaBar.colors;
        //    cb.normalColor = Color.green;
            staminaBar.color = Color.white;

            staminaCooldown = false;
        }
    }


    void RotateLegs()
    {
        if (currentMode == "Climb")
        {
           // legCenter.transform.Rotate(-90f, 0, 0, Space.Self);
            bodyPos.transform.Rotate(0, 0, -90f, Space.Self);
        }
        else if (currentMode == "Move")
        {
          //  legCenter.transform.Rotate(90f, 0, 0, Space.Self);
            bodyPos.transform.Rotate(0, 0, 90f, Space.Self);

        }
    }



    void CheckHidden()
    {
        RaycastHit hit;

        if (isClimbing)
        {
            Physics.Raycast(this.transform.position, this.transform.forward, out hit, 2f);  //if the player is climbing, creates a raycast forward
        }
        else
        {
            Physics.Raycast(this.transform.position, -this.transform.up, out hit, 2f);  //if player is not climbing, creates a raycast down
        }

        if (hit.transform != null)
        {
            Material checkMat = hit.transform.gameObject.GetComponent<Renderer>().sharedMaterial;  //gets renderer information from the raycast hit

            if (checkMat == playerRender.sharedMaterial && input.x == 0 && input.y == 0)
            {
              //  hiddenIcon.SetActive(true);
                isHidden = true;
                camoIcon.sprite = CamoHidden;
            }
            else
            {
              //  hiddenIcon.SetActive(false);
                isHidden = false;
                camoIcon.sprite = CamoSpotted;

            }
        }
        else
        {
          //  hiddenIcon.SetActive(false);
            isHidden = false;
        }
    }

   void OnPause()
    {
        SceneManager.LoadScene("Gameplay");
    }
    
    public void OnSpotted()
    {
        spottedIcon.SetActive(true);
    }

    public void OnHidden()
    {
        spottedIcon.SetActive(false);
    }

    void LookForGround()
    {
        Vector3 origin = transform.position;
        Vector3 direction = -Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, 1.2f))
        {
            isClimbing = false;
            SuckersUI.sprite = SuckersOff;
            isSuckersOn = false;
            currentMode = "Move";
            RotateLegs();
        }

    }

  

}