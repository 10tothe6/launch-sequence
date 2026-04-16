using UnityEngine;

// one of the first projects in a long while not to use my classic FirstPersonController3D.cs class



// this project is complicated enough that I'm writing another from scratch,
// with the idea in mind that gravity can be any direction

// also:
// because of character-switching, this controler does NOT just read keypresses
// it's given keypresses through a public function, 
// then uses those

// TODO: have a peek at the old player controller and steal some of its tricks (like camera tilting)

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    #region references
    private Rigidbody rb;
    private RaycastHit hit;
    public CapsuleCollider col;
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


    // for now this applies to both the visual model and the collider, 
    // but will eventually just be collider
    public float crouchPercentHeight; 
    private float colDefaultHeight;

    public float moveSpeed; // moving forwards/backwards
    public float strafeSpeed; // moving sideways
    public float turnSpeed; // looking around

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
     /* tracking variables */
    private bool isFlying;
    private bool isCrouching;

    private bool activeJump;
    private float sprintTimer;
    /**/
    #endregion

    void Awake()
    {
        // setting references
        rb = GetComponent<Rigidbody>();
    }

    // just doing this through Update() and using Time.deltaTime instead of FixedUpdate()
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked; // temp temp temp

        // here is where the familiar player-controller stuff starts to kick in
        Vector3 movementVector = Vector3.zero;

        movementVector += transform.right * lastPacket.GetHorizontal() * strafeSpeed;
        movementVector += transform.forward * lastPacket.GetVertical() * moveSpeed;

        transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * lastPacket.horizontalMouse, Space.World);
        transform.GetChild(0).Rotate(Vector3.right * Time.deltaTime * turnSpeed * -lastPacket.verticalMouse, Space.Self);

        //rb.linearVelocity += movementVector * Time.deltaTime;
    }

    // keypresses will not change unless updated, 
    // meaning the game will think you are holding down keys if packets stop at the wrong time
    public void SetKeypresses(player_keypresspacket keys)
    {
        lastPacket = keys;
    }

    #region toggling

    public void ToggleFlight()
    {
        isFlying = !isFlying;
    }

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
