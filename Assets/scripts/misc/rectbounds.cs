using UnityEngine;

// could be used for either UI or in-game stuff
// I made this for the purposes of UI bounding boxes though

[System.Serializable]
public class rectbounds
{
    public Vector3 center;

    public Vector3 extents;

    public rectbounds() {}

    public rectbounds(Vector3 center, Vector3 extents)
    {
        this.center = center;
        this.extents = extents;
    }
}
