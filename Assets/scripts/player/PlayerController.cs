using UnityEngine;
using UnityEngine.InputSystem;

public enum player_movementmode
{
    Walking,
    Crouching,
    Flying,
    NoClip,
}

// one of the first projects in a long while not to use my classic FirstPersonController3D.cs class



// this project is complicated enough that I'm writing another from scratch,
// with the idea in mind that gravity can be any direction

// also:
// because of character-switching, this controler does NOT just read keypresses
// it's given keypresses through a public function, 
// then uses those

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public bool isActive; // very, very important
    public Vector3 gravityDirection; // the most important addition to this controller
    public float gravitationalAcceleration;

    #region references
    private e_genericentity entityData;
    private Rigidbody rb;
    private RaycastHit hit;
    public CapsuleCollider col;
    private Transform t_camera;
    // keep in mind that eventually I will have to create ghost copies of player entities (for moving vehicle physics)
    // this means I'll need a controlling object of sorts, or similar system
    #endregion

    // since this is a multiplayer game, the player controller takes in this data class 
    // instead of reading keypresses directly
    public player_keypresspacket lastPacket;

    # region flags
    [Header("Flags")]
    public bool allowJump = true;
    public bool allowSprint = true;
    // FLIGHT IS NOT FREECAM, THEY ARE DIFFERENT
    // flight literally moves the robot model itself
    public bool allowFlight = false; 
    public bool allowCrouch = false;
    # endregion

    #region locks
    [Header("Locks")]
    [Header("Locks")]
    public bool lockCameraHorizontal = false;
    public bool lockCameraVertical = false;
    public bool lockMovement = false;
    public bool lockCursor = true;
    #endregion

    #region parameters
    // used for raycast checks with the ground
    public Transform t_foot;
    public float raycastDistanceFromFoot;


    // for now this applies to both the visual model and the collider, 
    // but will eventually just be collider
    public float crouchPercentHeight; 
    private float colDefaultHeight;

    public float moveSpeed; // moving forwards/backwards
    public float sprintBoost;
    public float strafeSpeed; // moving sideways
    public float turnSpeed; // looking around
    public float flySpeed;

    public float jumpStrength;

    // the player's camera oscillates when walking, which for now is just done by changing the camera offset
    // I likely won't change this, even after adding the player model - 
    // though I may just parent the camera to a bone and do that instead
    public float cameraBounceAmplitude;
    public float cameraBounceFrequency;
    private float defaultCameraHeight;
    private float currentCameraHeight;



    private bool isSprinting;
    public float maxSprint; // this may need to change variably
    [HideInInspector]
    public float sprintValue; // the amount of sprint that the player has left
    public LayerMask whatIsGround;
    public LayerMask whatIsVehicle; // TODO: this, later (for mptest-3)
    #endregion
    
    #region tracking variables
    private bool isCrouching;
    /* tracking variables */
    public player_movementmode mode;

    private bool activeJump;
    private float sprintTimer;


    private float lastWalkingTime;
    private float walkingTime;

    private bool isFlying; // either flying OR noclip

    public Vector3 shoveFactor;
    private Vector3 oldPosition;
    /**/
    #endregion

    void Awake()
    {
        // setting references
        rb = GetComponent<Rigidbody>();
        hit = new RaycastHit();

        entityData = GetComponent<e_genericentity>();
        t_camera = transform.GetChild(0);

        colDefaultHeight = col.height;
        sprintValue = maxSprint;
    }

    // called when the someone assumes control
    public void EnterControl()
    {
        // making sure its actually the local player taking control
        if (!LocalPlayer.localClient.controllingEntity == this)
        {
            return;
        }

        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson); // auto-sets the parent

        defaultCameraHeight = CameraController.Instance.transform.localPosition.y;
        currentCameraHeight = defaultCameraHeight;

        mode = player_movementmode.Walking;
        isActive = true;
    }
    public void ExitControl()
    {
        isActive = false;
    }

    // just doing this through Update() and using Time.deltaTime instead of FixedUpdate()
    void Update()
    {
        if (!isActive) {return;}

        if (entityData.data.index < 0)
        {
            // sanbox
            gravityDirection = -Vector3.up;
        } else
        {
            gravityDirection = cb_solarsystem.Instance.monoBodies[WorldManager.Instance.GetSOIIndex()].pose.data.GetPosition().Sub(entityData.data.GetPosition()).Norm().ToVector3();
        }
        if (Vector3.Angle(transform.up, -gravityDirection) > 5)
        {
            transform.up = -gravityDirection;
        }

        // updating the entity position from the rigidbody
        entityData.data.SetPosition(entityData.data.localPosition.Add(new num_precisevector3(transform.position - oldPosition -shoveFactor)));
        shoveFactor = Vector3.zero;
        oldPosition = transform.position;

        if (t_camera != null) t_camera.localPosition = new Vector3(0, currentCameraHeight, 0);
        float cameraTiltTarget = 0;

        if (isCrouching)
        {
            col.height = colDefaultHeight * crouchPercentHeight;
            t_foot.transform.localPosition = new Vector3(0,-col.height/2f*col.transform.localScale.y, 0);
        } else {col.height = colDefaultHeight;t_foot.transform.localPosition = new Vector3(0,-col.height/2f*col.transform.localScale.y, 0);}

        //Debug.Log(ImprovedRaycast());
        if (!lockMovement && ImprovedRaycast() && !isFlying)
        {
            
            if (lastPacket.forward)
            {
                //Debug.Log(2222);
                if (isSprinting)
                {
                    walkingTime += Time.deltaTime * sprintBoost;
                } else
                {
                    walkingTime += Time.deltaTime;
                }
                if (walkingTime * cameraBounceFrequency > Mathf.PI * 3f/2f && lastWalkingTime * cameraBounceFrequency < Mathf.PI * 3f/2f)
                {
                    //footstepController.Step();
                }
                if (walkingTime * cameraBounceFrequency > Mathf.PI*2f)
                {
                    walkingTime -= Mathf.PI*2f/cameraBounceFrequency;
                }
                currentCameraHeight = defaultCameraHeight + Mathf.Sin(walkingTime * cameraBounceFrequency) * cameraBounceAmplitude;
                if (lastPacket.sprint && sprintValue > 0 && allowSprint)
                {
                    isSprinting = true;
                    
                    rb.linearVelocity += transform.forward * moveSpeed * sprintBoost * Time.deltaTime;

                    if (Time.time > sprintTimer + 0.05f)
                    {
                        sprintValue--;
                        sprintTimer = Time.time;
                    }
                }
                else
                {
                    isSprinting = false;
                    //Debug.Log(3333);
                    rb.linearVelocity += transform.forward * moveSpeed * Time.deltaTime;
                }
            }
            else {
                isSprinting = false;
            }

            if (lastPacket.back)
            {
                rb.linearVelocity -= transform.forward * moveSpeed * Time.deltaTime;
            }
            if (lastPacket.right)
            {
                rb.linearVelocity += transform.right * moveSpeed * Time.deltaTime;
                cameraTiltTarget = -1;

            }

            if (lastPacket.left)
            {
                rb.linearVelocity -= transform.right * moveSpeed * Time.deltaTime;
                cameraTiltTarget = 1;
            }

            if (!lastPacket.right && !lastPacket.left)
            {
                cameraTiltTarget = 0;
            }
        }
        else if (isFlying) {
            transform.position += (transform.right * Input.inputAxisHorizontal + transform.forward * Input.inputAxisForward + transform.up * Input.inputAxisVertical) * (lastPacket.sprint ? 2.5f : 1) * flySpeed;
            rb.linearVelocity = Vector3.zero;
        } else {
            cameraTiltTarget = 0;
        }

        if (t_camera.parent != null) t_camera.localRotation = Quaternion.Lerp(Quaternion.Euler(t_camera.localEulerAngles), Quaternion.Euler(new Vector3(t_camera.localEulerAngles.x, t_camera.localEulerAngles.y, cameraTiltTarget)), 0.4f);

        if (!isFlying) {
            if (ImprovedRaycast())
            {
                if (activeJump && util_math.ProjectedMagnitude(rb.linearVelocity, -gravityDirection) <= 0)
                {
                    // originally I had the Vector3.up here as hit.normal, 
                    // but that seemed to cause really weird drifting bugs when walking/jumping on angled terrain

                    // so, we doin' Vector3.up now

                    Vector3 lateralVelocity = rb.linearVelocity - Vector3.Project(rb.linearVelocity, gravityDirection);
                    rb.linearVelocity -= lateralVelocity;

                    activeJump = false;
                    // if (timeJumpStarted < Time.time - 0.75f) { readyForImpactSound = true; }
                    // if (readyForImpactSound) { GetComponent<FootstepController>().Step(true); readyForImpactSound = false; }
                    // GetComponent<GenericCreature>().ApplyFallDamage();
                }

                // friction
                Vector3 fric = rb.linearVelocity * 0.1f;
                rb.linearVelocity -= (fric - Vector3.Project(fric, gravityDirection));
            }
            else
            {
                // drag
                //rb.linearVelocity -= new Vector3(rb.linearVelocity.x * 0.0001f, 0, rb.linearVelocity.z * 0.0001f);
                if (!activeJump)
                {
                    activeJump = true;
                }
            }
        }

        if (!lastPacket.sprint && sprintValue < maxSprint)
        {
            sprintValue += 0.5f;
        }
        isCrouching = lastPacket.crouch;

        lastWalkingTime = walkingTime;

        Cursor.lockState = CursorLockMode.Locked; // temp temp temp

        // mouse x leads to a rotation AROUND the players's up vector
        // (not the camera's)
        if (!lockCameraHorizontal)
        {
            //transform.rotation *= Quaternion.Euler(new Vector3(0, 1, 0) * Input.GetAxis("Mouse X") * cameraTurnSpeed);
            transform.Rotate(-gravityDirection * lastPacket.horizontalMouse * turnSpeed * Time.deltaTime, Space.World);
        }

        // mouse y leads to a rotation around the CAMERA's right vector
        // it obeys limits to avoid rotational glitches when looking straight up
        if (!lockCameraVertical)
        {
            t_camera.localRotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * turnSpeed * Time.deltaTime);
            // float maxAngle = 0.3f;
            // if (Input.mouseMovement.y < 0)
            // {
            //     // looking further down
            //     if (t_camera.forward.y > -maxAngle)
            //     {
            //         t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * turnSpeed * Time.deltaTime);
            //     }
            //     else if (Vector3.Dot(Vector3.up, util_math.RotateVector(t_camera.up, new Vector3(-1, 0, 0), Input.mouseMovement.y * turnSpeed * Time.deltaTime * Mathf.PI / 180)) > maxAngle)
            //     {
            //         t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * turnSpeed * Time.deltaTime);  
            //     }
            // }
            // else
            // {
            //     // looking further down
            //     if (t_camera.forward.y < maxAngle)
            //     {
            //         t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * turnSpeed * Time.deltaTime);
            //     }
            //     else if (Vector3.Dot(Vector3.up, util_math.RotateVector(t_camera.up, new Vector3(-1, 0, 0), Input.mouseMovement.y * Time.deltaTime * turnSpeed * Mathf.PI / 180)) > maxAngle)
            //     {
            //         t_camera.rotation *= Quaternion.Euler(new Vector3(-1, 0, 0) * Input.mouseMovement.y * turnSpeed * Time.deltaTime);
            //     }
            // }
        }

        /* jumping */
        if (lastPacket.jump && allowJump && !activeJump)
        {
            rb.linearVelocity += -gravityDirection.normalized * jumpStrength;
            activeJump = true;

            // TODO: investigate a bug with fall damage
            //GetComponent<GenericCreature>().PrimeFallDamage();
        }
        /**/



        // GRAVITY
        rb.linearVelocity += gravityDirection * gravitationalAcceleration * Time.deltaTime;
    }

    // keypresses will not change unless updated, 
    // meaning the game will think you are holding down keys if packets stop at the wrong time
    public void SetKeypresses(player_keypresspacket keys)
    {
        lastPacket = keys;
    }

    // kept getting stuck on everything because the raycast was missing (like standing on a ledge)
    // so we're shooting more rays now
    bool ImprovedRaycast()
    {
        if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f, -transform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.right * 0.15f, -transform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.right * -0.15f, -transform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.forward * 0.15f, -transform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        } else if (Physics.Raycast(t_foot.position + -gravityDirection * 0.05f + transform.forward * -0.15f, -transform.up, out hit, raycastDistanceFromFoot + 0.001f, whatIsGround))
        {
            return true;
        }
        
        return false;
    }

    #region toggling

    public void DisableCollider()
    {
        col.enabled = false;
        rb.useGravity = false;
    }

    public void EnableCollider()
    {
        col.enabled = true;
        rb.useGravity = true;
    }

    # endregion
}
