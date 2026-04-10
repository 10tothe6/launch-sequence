using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pst_water : MonoBehaviour
{
    private static pst_water _instance;

    public static pst_water Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public Material m_water;

    public Color waterColor;

    public RenderTexture waterDepthTex;
    public RenderTexture waterColorTex;
    private float[] waveAngles;

    public MeshRenderer waterObj;
    public float baseWaveFrequency;
    public Transform lightTransform;

    public bool useWorldTime;

    void Start()
    {
        waveAngles = new float[] { -0.68f, 1.45f, -2.59f, 1, -2, 0.5f, 3, 5, 0.25f, -1.53f, 5, 9 };
    }

    float GetTime()
    {
        if (useWorldTime)
        {
            return WorldManager.Instance.worldTime;
        } else
        {
            return Time.time;
        }
    }
    
    void Update()
    {
        m_water.SetVector("waterCol", new Vector3(waterColor.r, waterColor.g, waterColor.b));
        m_water.SetTexture("_WaterColor", waterColorTex);
        m_water.SetTexture("_WaterDepth", waterDepthTex);
        m_water.SetFloatArray("waveAngles", waveAngles);
        m_water.SetFloat("baseWaveFrequency", baseWaveFrequency);
        m_water.SetVector("sunDir", -lightTransform.forward);
        m_water.SetFloat("timeValue", GetTime());
    }

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        Graphics.Blit(source, mod, m_water);
    }
}
