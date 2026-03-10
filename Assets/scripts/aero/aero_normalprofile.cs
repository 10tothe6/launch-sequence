using System.Collections.Generic;
using UnityEngine;

// this is just the surface normal in a given direction, used to calculate normal lift (different than bernoulli lift)
// storage is the same as bernoulli for now

// oh and yes, I could just figure this out with a raycast, but this allows me to be more sure
// (plus I can approximate more complicated meshes instead of forcing the computer to figure out their topology)

public class aero_normalprofile : MonoBehaviour
{
    public List<Vector3> directionVectors;
    
    public List<Vector3> surfaceNormals;
}
