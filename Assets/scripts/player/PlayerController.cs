using UnityEngine;

// one of the first projects in a long while not to use my classic FirstPersonController3D.cs class



// this project is complicated enough that I'm writing another from scratch,
// with the idea in mind that gravity can be any direction

// also:
// because of character-switching, this controler does NOT just read keypresses
// it's given keypresses through a public function, 
// then uses those

public class PlayerController : MonoBehaviour
{
    public player_keypresspacket lastPacket;

    // just doing this through Update() and using Time.deltaTime instead of FixedUpdate()
    void Update()
    {
        // here is where the familiar player-controller stuff starts to kick in
    }

    public void SetKeypresses(player_keypresspacket keys)
    {
        lastPacket = keys;
    }
}
