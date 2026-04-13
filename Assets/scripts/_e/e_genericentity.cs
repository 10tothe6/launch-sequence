using UnityEngine;

// hopefully this doesn't exist already
public enum e_entitytype
{
    Fixed,
    Floating,
    Mimic,
}

// data that goes across all entity types
public class e_genericentity : MonoBehaviour
{
    // basic data for the entity
    public e_genericentitydata data;

    void Awake()
    {
        data.floatingData.generic = this;
        data.fixedData.generic = this;
        data.mimicData.generic = this;

        data.reference = transform;
    }
}
