using UnityEngine;

public class vfx_hydrogenflamecontroller : MonoBehaviour
{
    [Range(0,1)]
    public float throttle;

    public float ambientAirPressure; // in kPa

    public bool isActive;

    public ParticleSystem[] ps;

    public float[] startLifetimesMax;
    public float[] startLifetimesMin;

    public float[] startSizesMin;
    public float[] startSizesMax;
    
    public float[] defaultScales;
    public Vector3[] defaultPositions;

    void Awake()
    {
        InitializeFlame();
    }

    void InitializeFlame()
    {
        startLifetimesMin = new float[ps.Length];
        startLifetimesMax = new float[ps.Length];

        startSizesMin = new float[ps.Length];
        startSizesMax = new float[ps.Length];

        defaultScales = new float[ps.Length];
        defaultPositions = new Vector3[ps.Length];

        for (int i = 0; i < ps.Length; i++)
        {
            startLifetimesMin[i] = ps[i].main.startLifetime.constantMin;
            startLifetimesMax[i] = ps[i].main.startLifetime.constantMax;

            startSizesMin[i] = ps[i].main.startSize.constantMin;
            startSizesMax[i] = ps[i].main.startSize.constantMax;

            defaultScales[i] = ps[i].transform.localScale.x;
            defaultPositions[i] = ps[i].transform.localPosition;
        }
    }

    void Update()
    {
        UpdateFlame();
    }

    void UpdateFlame()
    {
        for (int i = 0; i < ps.Length; i++)
        {
            var m = ps[i].main;
            m.startLifetime = new ParticleSystem.MinMaxCurve(startLifetimesMin[i] * throttle, startLifetimesMax[i] * throttle);
            m.startSize = new ParticleSystem.MinMaxCurve(startSizesMin[i] * throttle, startSizesMax[i] * throttle);

            
            //ps[i].transform.localScale = Vector3.one * throttle * defaultScales[i];
            ps[i].transform.localPosition = defaultPositions[i] * throttle;
        }
    }
}
