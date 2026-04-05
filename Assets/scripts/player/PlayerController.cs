using UnityEngine;

// one of the first projects in a long while not to use my classic FirstPersonController3D.cs class



// this project is complicated enough that I'm writing another from scratch,
// with the idea in mind that gravity can be any direction

// also:
// because of character-switching, this controler does NOT just read keypresses
// it's given keypresses through a public function, 
// then uses those

// TODO: have a peek at the old player controller and steal some of its tricks (like camera tilting)

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public player_keypresspacket lastPacket;

    public float moveSpeed;
    public float strafeSpeed;
    public float turnSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // just doing this through Update() and using Time.deltaTime instead of FixedUpdate()
    void Update()
    {
        // temp temp temp
        Cursor.lockState = CursorLockMode.Locked;

        // here is where the familiar player-controller stuff starts to kick in
        Vector3 movementVector = Vector3.zero;

        movementVector += transform.right * lastPacket.GetHorizontal() * strafeSpeed;
        movementVector += transform.forward * lastPacket.GetVertical() * moveSpeed;

        transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * lastPacket.horizontalMouse, Space.World);
        transform.GetChild(0).Rotate(Vector3.right * Time.deltaTime * turnSpeed * -lastPacket.verticalMouse, Space.Self);

        rb.linearVelocity += movementVector * Time.deltaTime;
    }

    public void SetKeypresses(player_keypresspacket keys)
    {
        lastPacket = keys;
    }
}
