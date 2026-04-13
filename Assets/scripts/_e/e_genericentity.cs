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
}
