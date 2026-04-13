using UnityEngine;

// see entities README!!!

// These entities are bound to a specific celestial body, and are rendered based on the position, rotation, and scale of that body
// in other words, they do not need to be a part of the floating rendering system
[System.Serializable]
public class e_mimicentitydata {
    public e_genericentitydata genericData;
    public float defaultScale;

    // public void Refresh() {
    //     // for now just set the unity position to game position
    //     Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
    //     reference.position = planetTransform.position + cb_renderingmanager.Instance.AdjustVector(defaultPosition, 0) * planetTransform.localScale.x;
    //     reference.eulerAngles = planetTransform.eulerAngles + defaultRotation;
    //     reference.localScale = Vector3.one * defaultScale * (planetTransform.localScale.x / cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.defaultScale);
    // }

    // public Vector3 GetPosition() {
    //     Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
    //     return planetTransform.position + defaultPosition * planetTransform.localScale.x;
    // }

    // public Vector3 GetRotation() {
    //     Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
    //     return reference.eulerAngles = planetTransform.eulerAngles + defaultRotation;
    // }
}