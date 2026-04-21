using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Collision : MonoBehaviour
{
    void OnCollisionEnter(Collision col) {
        Db.Instance.ShowCollisionPoint(col.contacts[0].point);
    }
}
