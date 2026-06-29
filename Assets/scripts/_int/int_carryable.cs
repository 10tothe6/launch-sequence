using UnityEngine;

// an object that can be carries, sea of theives barrel-style, by a player

public class int_carryable : MonoBehaviour
{
    private InteractableObject3D ioComp;

    private float cooldownStartTime;
    public float cooldownInterval = 1;
    private bool isOnCooldown;

    void Awake()
    {
        ioComp = GetComponent<InteractableObject3D>();

        ioComp.onInteractByObject.AddListener((x) =>
        {
            Carry(x);
        });
    }

    public void Carry(GameObject carrier)
    {
        if (isOnCooldown) {return;}

        
        int_objectcarrier comp = carrier.GetComponent<int_objectcarrier>();
        if (comp == null) {return;}

        comp.CarryObject(gameObject);
    }

    public void DropCooldown()
    {
        isOnCooldown = true;
        cooldownStartTime = Time.time;
    }

    void Update()
    {
        if (isOnCooldown && Time.time > cooldownStartTime + cooldownInterval)
        {
            isOnCooldown = false;
        }
    }
}
