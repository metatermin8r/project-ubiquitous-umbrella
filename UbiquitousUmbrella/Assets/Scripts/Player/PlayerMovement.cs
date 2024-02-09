using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
//Fall speed when stepping off of a ledge is wack, needs to be much more like falling after jumping.

//Sliding needs to be possible when going down slopes, both on objects and terrain, without needing to sprint.
//^That'll be a math-induced heacache. Lydia, you like math, wanna take that one? Actually, I may have an answer to this
//Me, check U71 movement code, that controller had a solution to this

public class PlayerMovement : MonoBehaviour
{
    //Self explanatory
    public CharacterController controller;
    public GameObject playerCapsule; //public CapsuleCollider playerCapsule;

    //Variables for camera tilt and FOV change while wallrunning
    [Header("Player Camera Settings")]
    public Camera playerCamera;
    float normalFov;
    public float specialFov;
    public float cameraChangeTime;
    public float wallRunTilt;
    public float tilt;

    //Assign an empty object here to use its transform as a respawn point for the player
    [Header("Player Respawn Point")]
    public Transform respawnPoint = null;

    //A transform placed just under the "feet" of the player to check if they are on the ground
    [Header("Ground Check")]
    public Transform groundCheck;

    //Unity layers to specify what the ground is and what walls are
    [Header("Layer Masks")]
    public LayerMask groundMask;
    public LayerMask wallMask;

    //Vectors for horizontal world movement, vertical velocity, and player input
    Vector3 move;
    Vector3 Yvelocity;
    Vector3 input;
    Vector3 forwardDirection;

    [Header("Player State Bools (DEBUG)")]
    //Also pretty self explanatory, serialized for debug purposes to better reflect player state
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool isSprinting;
    [SerializeField] bool isCrouching;
    [SerializeField] bool isSliding;
    [SerializeField] bool isWallRunning;

    //Floats for the increase/decrease in speed the player goes through while stopping/starting sprinting or wallrunning
    [Header("Slide/Wallrun Speed Settings")]
    public float slideSpeedIncrease;
    public float slideSpeedDecrease;
    public float wallRunSpeedIncrease;
    public float wallRunSpeedDecrease;

    //For double jump
    [Header("Double Jump Charges")]
    public int jumpCharges = 1;

    //Local float for actual speed, public floats for setting the speed to whatever is required
    [Header("Player Movement Speeds")]
    [SerializeField] float speed;
    public float runSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float airSpeed;
    public float climbSpeed;

    //Physics for jumping, falling, and wallrunning
    [Header("Player Jump Settings")]
    public float jumpHeight;
    [SerializeField]
    float gravity;
    public float normalGravity;
    public float wallRunGravity;
    public float fallingGravity;

    //Heights for crouching
    float startHeight;
    float crouchHeight = 0.5f;
    Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    Vector3 standingCenter = new Vector3(0, 0, 0);

    //Slide floats
    [Header("Slide timers")]
    float slideTimer;
    public float maxSlideTimer;

    //Bools and raycasts for wallrunning logic
    bool hasWallRun = false;
    bool onLeftWall;
    bool onRightWall;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    //The opposite cordinates of the wall being run on, to determine how the player should move accross it.
    //Also stores the normal of the last wall run on, to avoid wallrunning on the same wall repeatedly
    Vector3 wallNormal;
    Vector3 lastWallNormal;

    //Bools for different states of climbing, and a raycast hit for detecting the wall when climbing
    bool isClimbing;
    bool canClimb;
    bool hasClimbed;
    RaycastHit climbWallHit;

    //Timers for climbing duration
    [Header("Climb Timer")]
    float climbTimer;
    public float maxClimbTimer;

    //Bool and timers for wall jumping
    [Header("Player Wall Jump Timer")]
    bool isWallJumping;
    float wallJumpTimer;
    public float maxWallJumpTimer;

    //Used for pause bool
    public MenuManager MenuManager;
    public bool pauseMenuActive;
    public FirstPersonWeaponMovement weaponSway;

    //reference to inventory controller for item pickup ray cast
    [SerializeField] InventoryController inventoryController;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        normalFov = playerCamera.fieldOfView;
        startHeight = transform.localScale.y;

        if (GameObject.Find("RespawnPoint") == true)
        {
            respawnPoint = GameObject.Find("RespawnPoint").transform;
        }

        weaponSway = gameObject.GetComponentInChildren<FirstPersonWeaponMovement>();
    }

    //Call this to increase the player's speed by the desired amount, passed as a float
    void IncreaseSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }

    //Same as IncreaseSpeed, but speed go down instead of up
    void DecreaseSpeed(float speedDecrease)
    {
        speed -= speedDecrease * Time.deltaTime; //This is normalized to deltaTime so the decrease isn't so jarring
    }

    void HandleInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        //Handles character moving reletive to camera orientation
        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        //Input for jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumpCharges > 0)
        {
            Jump();
        }

        //Inputs for crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ExitCrouch();
        }

        //Inputs for sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
    }

    //Handles all camera fov and tilt effects
    void CameraEffects()
    {
        //Sets the fov float to whatever our fov should be based on player state
        float fov = isWallRunning ? specialFov : isSliding ? specialFov : normalFov;

        //Actually sets the player camera to the correct fov, lerped to Time.deltaTime to make it smooth
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, cameraChangeTime * Time.deltaTime);

        //Handles tilting the camera based on what direction the wall we're running on is relative to the player
        if (isWallRunning)
        {
            if (onRightWall)
            {
                tilt = Mathf.Lerp(tilt, wallRunTilt, cameraChangeTime * Time.deltaTime);
            }
            if (onLeftWall)
            {
                tilt = Mathf.Lerp(tilt, -wallRunTilt, cameraChangeTime * Time.deltaTime);
            }
        }

        //Resets camera tilt if we aren't wallrunning
        if (!isWallRunning)
        {
            tilt = Mathf.Lerp(tilt, 0, cameraChangeTime * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if game is paused
        if (!MenuManager.gameIsPaused)
        {
        //Basic call for general inputs
            HandleInput();
            weaponSway.EnableAll();
        }
        else if (MenuManager.gameIsPaused)
        {
            weaponSway.DisableAll();
        }


        //Checks for walls around the player, handles entering the wallrunning state
        CheckWallRun();

        //If statement determining what movement state the player should be in under the given paramaters,
        //and some associated logic
        if (isGrounded && !isSliding)
        {
            GroundMovement();
        }
        else if (!isGrounded && !isWallRunning && !isClimbing)
        {
            AirMovement();
        }
        else if (isSliding)
        {
            SlideMovement();
            DecreaseSpeed(slideSpeedDecrease);
            slideTimer -= 1f * Time.deltaTime;
            if (slideTimer < 0)
            {
                isSliding = false;
            }
        }
        else if (isWallRunning)
        {
            WallRunMovement();
            DecreaseSpeed(wallRunSpeedDecrease);
        }
        else if (isClimbing)
        {
            ClimbMovement();

            //Logic for the climb timer
            climbTimer -= 1f * Time.deltaTime;
            if (climbTimer <= 0)
            {
                isClimbing = false;
                hasClimbed = true;
            }
        }

        //TODO: Move this to Player Action, trying keep non-movement related player stuff to a minimum in this script
        //checks if pickupable item is within range to be picked up
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 3))
            {
                ItemPickupable item = hitInfo.collider.gameObject.GetComponent<ItemPickupable>();

                if (item != null)
                {
                   //trigger void
                }
            }
        }

        //Checks if the player is grounded, and applies our gravity to the player
        Groundcheck();
        ApplyGravity();

        //Checks for camera effects
        CameraEffects();

        //Handles the controller actually moving, set to system time
        controller.Move(move * Time.deltaTime);

        //Checks to see if the requirements for player death are met
        FallRespawn();

        //Checks to see if the player is climbing
        CheckClimbing();
    }

    void GroundMovement()
    {
        //Sets local speed variable to publicly set speed variables, depending on player state
        speed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;

        //Moves the character on the x axis with input, stops it without input
        if (input.x != 0)
        {
            move.x += input.x * speed;
        }
        else
        {
            move.x = 0;
        }

        //Moves the character on the z axis with input, stops it without input
        if (input.z != 0)
        {
            move.z += input.z * speed;
        }
        else
        {
            move.z = 0;
        }

        //Clamps movement speed on X and Z to our set speed
        move = Vector3.ClampMagnitude(move, speed);
    }

    //Handles the players movement while in the air
    void AirMovement()
    {
        move.x += input.x * airSpeed;
        move.z += input.z * airSpeed;

        if (isWallJumping)
        {
            move += forwardDirection;
            wallJumpTimer -= 1f * Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
        }

        move = Vector3.ClampMagnitude(move, speed);
    }

    //Handles the player's movement while sliding
    void SlideMovement()
    {
        move += forwardDirection;
        move = Vector3.ClampMagnitude(move, speed);
    }

    //Handles logic for moving while on the wall
    void WallRunMovement()
    {
        //Allows the player to move forward along a wall even if they're input isn't entirely forward
        //Basically, you can press the left or right key while also pressing forward and still wallrun
        //If those inputs go too far in one direction, or the player lets go of the inputs, exit the wallrun
        if (input.z > (forwardDirection.z - 10f) && input.z < (forwardDirection.z + 10f))
        {
            move += forwardDirection;
        }
        else if (input.z < (forwardDirection.z - 10f) && input.z > (forwardDirection.z + 10f))
        {
            move.x = 0f;
            move.z = 0f;
            ExitWallRun();
        }
        move.x += input.x * airSpeed;

        move = Vector3.ClampMagnitude(move, speed);
    }

    //Handles movement while in the climbing state, mainly making sure the player can only go up at the speed previously set
    void ClimbMovement()
    {
        forwardDirection = Vector3.up;
        move.x += input.x * airSpeed;
        move.z += input.z * airSpeed;

        Yvelocity += forwardDirection;
        speed = climbSpeed;

        move = Vector3.ClampMagnitude(move, speed);
        Yvelocity = Vector3.ClampMagnitude(Yvelocity, speed);
    }

    //Handles the player jumping. If the player jumps when grounded, you jump normally.
    //If the player jumps while in the air, you use a jump charge and double jump.
    //The else if covers jumping while wallrunning, which just ejects you from the wall and gives you a slight speed boost
    void Jump()
    {
        if (!isGrounded && !isWallRunning)
        {
            jumpCharges -= 1;
        }
        else if (isWallRunning)
        {
            ExitWallRun();
            IncreaseSpeed(wallRunSpeedIncrease);
        }
        hasClimbed = false;
        climbTimer = maxClimbTimer;
        Yvelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
    }

    //Uses Unity Physics to check if the player is colliding with an object marked as ground, and sets the bool accordingly
    //Also resets jump charges for double jumping while grounded
    void Groundcheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        if (isGrounded)
        {
            jumpCharges = 1;
            hasWallRun = false;
            hasClimbed = false;
            climbTimer = maxClimbTimer;
        }
    }

    void CheckWallRun()
    {
        //Checks to see if the player is close to a wall that can be run on and sets the respective bool to true
        onLeftWall = Physics.Raycast(transform.position, -transform.right, out leftWallHit, 0.7f, wallMask);
        onRightWall = Physics.Raycast(transform.position, transform.right, out rightWallHit, 0.7f, wallMask);

        //If one of the above conditions is met, and the player isn't wall running, start wallrunning
        if ((onRightWall || onLeftWall) && !isWallRunning)
        {
            TestWallRun();
        }

        //If the player is wallrunning, but not next to any walls, stop
        if ((!onRightWall && !onLeftWall) && isWallRunning)
        {
            ExitWallRun();
        }
    }

    void TestWallRun()
    {
        //Finds the normal (opposite direction) of the wall
        wallNormal = onLeftWall ? leftWallHit.normal : rightWallHit.normal;

        //Checks if the player has wall run, and then checks to see if the angle of the wall the player is trying to run on has
        //a difference of greater than 15 units from the normal of the last wall run on
        //If the player hasn't run on a wall, we just skip to the normal logic
        if (hasWallRun)
        {
            float wallAngle = Vector3.Angle(wallNormal, lastWallNormal);
            if (wallAngle > 15)
            {
                WallRun();
            }
        }
        else
        {
            WallRun();
            hasWallRun = true;
        }
    }

    //Handles climbing logic
    void CheckClimbing()
    {
        //Checks if the player is close to a wall
        canClimb = Physics.Raycast(transform.position, transform.forward, out climbWallHit, 0.7f, wallMask);

        //Gets the angle of the wall
        float wallAngle = Vector3.Angle(-climbWallHit.normal, transform.forward);

        //Checks if the wall's angle is greater than 15 units, and that the player hasn't climbed recently and can climb
        if (wallAngle < 15f && !hasClimbed && canClimb)
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }

    //This is basically just writing in our own form of Gravity, because Unity's built in physics are mid at best for players
    void ApplyGravity()
    {
        //Magic piece of code I stole from my very first project. Fixes player falling at lightspeed if they go off a ledge without
        //jumping. I don't understand why setting that specific value in that specific way fixes it, but it does.
        if (isGrounded && Yvelocity.y < 0)
        {
            Yvelocity.y = -2f;
        }

        //Checks if we're wallrunning or falling, and applies the respective gravity values, otherwise just uses normal gravity
        gravity = isWallRunning ? wallRunGravity : isClimbing ? 0f : !isGrounded ? fallingGravity : normalGravity;
        Yvelocity.y += gravity * Time.deltaTime;
        controller.Move(Yvelocity * Time.deltaTime);
    }

    //Puts the player in the crouch state, and starts a slide if they are sprinting
    void Crouch()
    {
        controller.height = crouchHeight;
        controller.center = crouchingCenter;

        //Shift the local position of the player capsule collider up so it doesn't clip through the ground
        playerCapsule.transform.localPosition = new Vector3(0f, 0.43f, 0f);

        //Change the transform of the capsule and its collider to the proper crouch height
        playerCapsule.transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        isCrouching = true;

        //Logic for handling sliding, which occurs when you hold crouch while sprinting
        if (speed > runSpeed)
        {
            isSliding = true;
            forwardDirection = transform.forward;

            if (isGrounded)
                IncreaseSpeed(slideSpeedIncrease);

            slideTimer = maxSlideTimer;
        }
    }

    //Resets the player back to normal after they exit the crouch state
    void ExitCrouch()
    {
        controller.height = (startHeight * 2);
        controller.center = standingCenter;

        //Resets the local position and scale to the proper stats for upright movement
        playerCapsule.transform.localPosition = new Vector3(0f, 0f, 0f);
        playerCapsule.transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);

        isCrouching = false;
        isSliding = false;
    }

    //Handles wallrunning logic
    void WallRun()
    {
        isWallRunning = true;
        jumpCharges = 1;

        //Increase player speed based on whatever is set for this variable in the inspector
        IncreaseSpeed(wallRunSpeedIncrease);

        //Reset Y axis velocity so the player doesn't move up or down
        Yvelocity = new Vector3(0f, 0f, 0f);

        //This crosses the player's forward direction with the normal of the wall
        //I don't fully understand this, but in the immortal words of Bethesda's Todd Howard: "it just works".
        forwardDirection = Vector3.Cross(wallNormal, Vector3.up);

        //Stops the player from wallrunning backwards
        if (Vector3.Dot(forwardDirection, transform.forward) < 0)
        {
            forwardDirection = -forwardDirection;
        }
    }

    //Handles logic for exiting a wall run sets the lastWallNormal
    void ExitWallRun()
    {
        isWallRunning = false;
        lastWallNormal = wallNormal;

        //Sub logic for wall jumping
        forwardDirection = wallNormal;
        isWallJumping = true;
        wallJumpTimer = maxWallJumpTimer;
    }

    //Handles "killing" the player, but the current logic in here just handles respawning them
    void FallRespawn()
    {
        //If the player falls below -100 on the Y axis
        if (transform.position.y < -100)
        {
            //If a respawn point has been assigned, move the player back there. Otherwise, just destroy the player
            //NOTE: Destroying the player forces you to resart play mode in the editor, or to manually close the game in build mode
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.transform.position;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    //GIZMO DEBUG METHODS
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(groundCheck.position, 0.2f);
    //}
}
