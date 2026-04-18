using UnityEngine;

public class test_distancecheck : MonoBehaviour
{
    public bool active;
    public cbt_meshbody body;
    public Transform t_player;

    void Update()
    {
        if (!active) {return;}
        transform.position = body.chunks[0].GetClampedPosition(t_player.position - body.transform.position);
    }
}
