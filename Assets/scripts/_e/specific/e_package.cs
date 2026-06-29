using UnityEngine;
using UnityEngine.InputSystem;

public class e_package : MonoBehaviour
{
    private e_genericentity eComp;
    private int_carryable carryComp;

    public string containedPartName; // what part is in the package

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
        carryComp = GetComponent<int_carryable>();
    }

    void Update()
    {
        if (carryComp.isBeingCarried)
        {
            // 'g' is the key for opening the package
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                carryComp.ForceDrop();
                // now that we've dropped the thing, we can spawn the spacecraft...
                EntityManager.SpawnNewSinglePartSpaceCraft(containedPartName, eComp.data.GetPosition());
                // ...and destroy this object
                EntityManager.Instance.RemoveEntity(eComp.data.index);

                AudioManager.Instance.PlayStaticSound(1); // box opening sound (TEMP)
            }
        }
    }
}
