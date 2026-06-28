using UnityEngine;

public class e_cargodrop : MonoBehaviour
{
    public LayerMask whatIsGround; 
    private bool hasBeenPositioned;

    void Update()
    {
        if (!hasBeenPositioned)
        {
            Position();
        }
    }

    public void Position()
    {
        if (!ServerNetworkManager.Instance.isServerActive) {hasBeenPositioned = true; return;}

        // pick a spawn position and go there
        RaycastHit hit;

        Vector3 upDir = GetComponent<e_genericentity>().data.GetPosition().Sub(cb_solarsystem.Instance.monoBodies[2].pose.data.GetPosition()).ToVector3().normalized;
        transform.up = upDir;
        GetComponent<e_genericentity>().data.SetRotation(transform.rotation);

        if (Physics.Raycast(transform.position + upDir * 50f, -upDir, out hit, Mathf.Infinity, whatIsGround))
        {
            if (hit.collider.gameObject == transform.GetChild(0).GetChild(0).gameObject) {return;}
            hasBeenPositioned = true;
            GetComponent<e_genericentity>().data.SetPosition(GetComponent<e_genericentity>().data.GetPosition().Add(hit.point - transform.position).Add(Vector3.right * 0.075f));
        }
    }
}
