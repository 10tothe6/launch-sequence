using UnityEngine;

public class randomtransform : MonoBehaviour
{
    void Awake()
    {
        transform.up = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));
    }
}
