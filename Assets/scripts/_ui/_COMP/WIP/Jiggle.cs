using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// almost as bad as the boolean variable I named "clunk"

// anyways, this script makes an object, well, jiggle
// same idea as screenshake and UI shake

public class Jiggle : MonoBehaviour
{
    public bool shouldBeJiggling;
    public float jiggleAmt;
    public float jiggleAmtTarget;

    private Vector3 jiggleOffset;

    [Header("Axis Toggles (local)")]
    public bool jiggleX;
    public bool jiggleY;
    public bool jiggleZ;

    private bool shouldLerp;

    public float lerpFactor;

    void Awake()
    {
        if (lerpFactor == 0)
        {
            lerpFactor = 0.005f; // using this as a default val bc this was the hardcoded val before i made it a property
        }
    }

    void Update()
    {
        if (shouldLerp) jiggleAmt = Mathf.Lerp(jiggleAmt, jiggleAmtTarget, lerpFactor);

        // here, i don't want to deal with local position and whatever, so I've come up with a better sol'n:
        // subtract the last offset, then recalc the offset, then add it. that's it.

        transform.position -= jiggleOffset;

        if (shouldBeJiggling)
        {
            float xAmt = 0;
            float yAmt = 0;
            float zAmt = 0;

            if (jiggleX) { xAmt = Random.Range(-jiggleAmt, jiggleAmt); }
            if (jiggleY) { yAmt = Random.Range(-jiggleAmt, jiggleAmt); }
            if (jiggleZ) { zAmt = Random.Range(-jiggleAmt, jiggleAmt); }


            jiggleOffset = transform.right * xAmt + transform.up * yAmt + transform.forward * zAmt;
        }

        transform.position += jiggleOffset;
    }

    public void TemporaryJiggle(float amt)
    {
        jiggleAmtTarget = 0;
        jiggleAmt = amt;

        shouldLerp = true;
        shouldBeJiggling = true;
    }
}
