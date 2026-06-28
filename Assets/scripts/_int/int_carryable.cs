using UnityEngine;

// an object that can be carries, sea of theives barrel-style, by a player

public class int_carryable : MonoBehaviour
{
    private InteractableObject3D ioComp;

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
        int_objectcarrier comp = carrier.GetComponent<int_objectcarrier>();
        if (comp == null) {return;}

        comp.CarryObject(gameObject);
    }
}
