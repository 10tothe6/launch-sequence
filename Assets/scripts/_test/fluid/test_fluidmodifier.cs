using UnityEngine;

public class test_fluidmodifier : MonoBehaviour
{
    [Header("CONFIG")]
    public bool showPath;
    public float speed;
    public bool addFluid;
    public bool suckFluid;

    public LayerMask whatIsFluid;

    void Update()
    {
        if (showPath)
        {
            Debug.DrawLine(transform.position, transform.position - Vector3.up * 25f);
        }
        if (addFluid)
        {
            ModifyFluidBelow(speed * Time.deltaTime);
        } 
        if (suckFluid)
        {
            ModifyFluidBelow(-speed * Time.deltaTime);
        }
    }

    public void ModifyFluidBelow(float amt)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 10f, whatIsFluid))
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.GetComponent<fluid_collider>() != null)
            {
                hit.collider.GetComponent<fluid_collider>().parent.ModifyVolume(amt);
            }
        }
    }
}
