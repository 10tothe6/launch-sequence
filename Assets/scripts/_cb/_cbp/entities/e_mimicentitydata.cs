using UnityEngine;

// see entities README!!!

// These entities are bound to a specific celestial body, and are rendered based on the position, rotation, and scale of that body
// in other words, they do not need to be a part of the floating rendering system
[System.Serializable]
public class e_mimicentitydata {
    public Transform reference;
    public int bodyIndex; // What celestial body is this bound to?
    public Vector3 defaultPosition; // local position at planet's full scale
    public Vector3 defaultRotation; // local rotation at planet's full scale (although scale doesn't matter for this one)
    public float defaultScale;

    public e_mimicentitydata(Transform _ref, int _body) {
        reference = _ref;
        bodyIndex = _body;
        Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
        defaultPosition = (_ref.position - planetTransform.position) / planetTransform.localScale.x; // difference in position relative to planet
        defaultRotation = _ref.eulerAngles - planetTransform.eulerAngles; // difference in rotation relative to planet
        defaultScale = _ref.localScale.x;
    }

    public void Refresh() {
        // for now just set the unity position to game position
        Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
        reference.position = planetTransform.position + cb_renderingmanager.Instance.AdjustVector(defaultPosition, 0) * planetTransform.localScale.x;
        reference.eulerAngles = planetTransform.eulerAngles + defaultRotation;
        reference.localScale = Vector3.one * defaultScale * (planetTransform.localScale.x / cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.defaultScale);
    }

    public Vector3 GetPosition() {
        Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
        return planetTransform.position + defaultPosition * planetTransform.localScale.x;
    }

    public Vector3 GetRotation() {
        Transform planetTransform = cb_renderingmanager.Instance.bodyEntities[bodyIndex].data.reference;
        return reference.eulerAngles = planetTransform.eulerAngles + defaultRotation;
    }
}