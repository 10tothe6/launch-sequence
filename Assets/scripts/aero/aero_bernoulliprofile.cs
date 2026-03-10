using System.Collections.Generic;
using UnityEngine;

// similar to dragprofile, but instead of scalar drag coefficients we have a vector3 lift force

public class aero_bernoulliprofile : MonoBehaviour
{
    public List<Vector3> directionVectors;

    public List<Vector3> liftForces;
}
