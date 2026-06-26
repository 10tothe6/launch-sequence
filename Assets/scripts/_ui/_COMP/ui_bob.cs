using UnityEngine;

public class ui_bob : MonoBehaviour
{
    [Header("set to self if left null")]
    public Transform t_target;



    [Space(30)]
    [Header("CONFIG")]
    public float freq;
    public float amp;
    private Vector3 startPosition;

    void Awake()
    {
        if (t_target == null) {t_target = transform;}
        startPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPosition + Vector3.up * amp * Mathf.Sin(Time.time * freq);
    }
}
