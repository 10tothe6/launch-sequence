using System.Collections.Generic;
using UnityEngine;

public class aero_dragprofile : MonoBehaviour
{
    // drag is stored as a bunch of drag coefficients, each with a vector that tells the direction where the coefficient applies
    // this is a far similar/easy to use approach than trying to raycast the drag

    // and also this is a game, not a sim, so
    public List<Vector3> directionVectors;
    
    public List<float> dragCoefficients; // the coefficients themselves
}
