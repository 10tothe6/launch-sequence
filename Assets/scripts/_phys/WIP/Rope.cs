// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// // look up verlet integration
// // also S. Lague's superhero game video for the original implementation of this in 2D

// public class Rope : MonoBehaviour
// {
//     public Transform t_lockStart;
//     public Transform t_lockEnd;
//     [Header("CONFIG")]
//     public GameObject p_segment;
//     public float segmentSpacing;
//     public int segmentCount;

//     // TODO: suport for this
//     public bool useDifferentEnds;
//     public GameObject p_endSegment; // only used if the bool above is ticked

//     [Header("DATA")]
//     public RopePoint[] points;
//     // the system that I'm referencing for this (S. Lague's implementation) has sticks, but this is simpler so we don't need them
//     public int iterations;

//     void Start()
//     {
//         // test code
//         Initialize(segmentCount);
//     }

//     public void Initialize(int segmentCount)
//     {
//         points = new RopePoint[segmentCount];

//         // initialize the physical objects
//         for (int i = 0; i < segmentCount; i++)
//         {
//             Transform newSegment;

//             newSegment = Instantiate(p_segment, transform).transform;
//             newSegment.forward = Vector3.up;

//             bool toLock = i == 0 || i == segmentCount - 1;

//             if (i == 0)
//             {
//                 // the segments have snapping points as the first and second children
//                 newSegment.localPosition = (newSegment.GetChild(1).position - newSegment.GetChild(0).position) / 2f;
//             }
//             else
//             {
//                 newSegment.position = points[i - 1].t_reference.GetChild(1).position + (newSegment.GetChild(1).position - newSegment.GetChild(0).position) / 2f;
//             }

//             if (toLock)
//             {
//                 newSegment.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
//             }

//             points[i] = new RopePoint(newSegment.position, toLock);
//             points[i].t_reference = newSegment;
//         }
//     }

//     void FixedUpdate()
//     {
//         UpdateRope();
//     }

//     void UpdateRope()
//     {
//         for (int i = 0; i < points.Length; i++)
//         {
//             points[i].prevPos = points[i].pos;
//             points[i].pos = points[i].t_reference.position;
//         }

//         for (int j = 0; j < iterations; j++)
//         {
//             for (int i = 0; i < points.Length - 1; i++)
//             {
//                 Vector3 centre = (points[i + 1].pos + points[i].pos) / 2;
//                 Vector3 dir = (points[i + 1].pos - points[i].pos).normalized;

//                 if (!points[i].isLocked)
//                 {
//                     points[i].t_reference.position = centre - dir * segmentSpacing / 2;
//                     points[i].pos = points[i].t_reference.position;
//                 }
//                 if (!points[i + 1].isLocked)
//                 {
//                     points[i + 1].t_reference.position = centre + dir * segmentSpacing / 2;
//                     points[i + 1].pos = points[i + 1].t_reference.position;
//                 }

//                 if (i == 0)
//                 {
//                     points[i].t_reference.position = t_lockStart.position;
//                 } else if (i == points.Length - 2)
//                 {
//                     points[i + 1].t_reference.position = t_lockEnd.position;
//                 }
//             }
//         }

//         for (int i = 0; i < points.Length; i++)
//         {
//             points[i].t_reference.GetComponent<Rigidbody>().velocity = (points[i].pos - points[i].prevPos) / Time.deltaTime;
//             if (i < points.Length - 1)
//             {
//                 points[i].stretchAmt = (points[i + 1].pos - points[i].pos).magnitude - segmentSpacing;
//                 points[i+1].stretchAmt = (points[i].pos - points[i + 1].pos).magnitude - segmentSpacing;
//             }
//         }
//     }
// }

// [System.Serializable]
// public class RopePoint
// {
//     public Transform t_reference;
//     public Vector3 pos, prevPos;
//     public bool isLocked;
//     public float stretchAmt;

//     public RopePoint(Vector3 pos)
//     {
//         this.pos = pos;
//         this.prevPos = pos;
//     }
//     public RopePoint(Vector3 pos, bool isLocked)
//     {
//         this.pos = pos;
//         this.prevPos = pos;

//         this.isLocked = isLocked;
//     }
// }

// [System.Serializable]
// public class RopeStick {
//     public RopePoint a, b;
//     public float length;

//     public RopeStick(RopePoint _a, RopePoint _b, float _length) {
//         a = _a;
//         b = _b; 
//         length = _length;
//     }
// }