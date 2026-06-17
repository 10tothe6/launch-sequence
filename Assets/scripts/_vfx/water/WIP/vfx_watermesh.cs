using UnityEngine;

public class vfx_watermesh : MonoBehaviour
{
    private static vfx_watermesh _instance;

    public static vfx_watermesh Instance
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

    private float[] waveAngles;
    public bool isUnderwater;
    public float baseWaveFrequency;
    public Transform lightTransform;
    public bool useWorldTime;
    public MeshRenderer waterObj;

    void Start()
    {
        waveAngles = new float[] { -0.68f, 1.45f, -2.59f, 1, -2, 0.5f, 3, 5, 0.25f, -1.53f, 5, 9 };
    }

    void Update()
    {
        if (GetHeight(transform.position) > transform.position.y)
        {
            if (!isUnderwater) { Debug.Log("Went under!"); waterObj.GetComponent<PlaneGenerator>().Generate(true); }
            isUnderwater = true;
        } else
        {
            if (isUnderwater) { Debug.Log("Went out!"); waterObj.GetComponent<PlaneGenerator>().Generate(false);}
            isUnderwater = false;
        }

        waterObj.sharedMaterial.SetFloatArray("waveAngles", waveAngles);
        waterObj.sharedMaterial.SetFloat("baseWaveFrequency", baseWaveFrequency);
        waterObj.sharedMaterial.SetVector("sunDir", -lightTransform.forward);

        waterObj.sharedMaterial.SetInt("isUnderWater", isUnderwater ? 1 : 0);
        
        waterObj.sharedMaterial.SetFloat("timeValue", GetTime());
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

    public static float GetHeight(Vector3 pos) {
        float a = 50;
        float f = 0.05f;

        float sum = 0;
        int iterations = 12;

        for (int i = 0; i < iterations; i++) {
            float angle = Instance.waveAngles[i];

            float x = Mathf.Cos(angle) * (pos.x * Instance.baseWaveFrequency) - Mathf.Sin(angle) * (pos.z * Instance.baseWaveFrequency);
            x += Time.time*4;
            
            sum += Mathf.Pow(2.71828f, Mathf.Sin(x*f) - 1) * a;

            f *= 1.5f;
            a = Mathf.Lerp(a, 1, 0.5f);
        }

        return sum/iterations;
    }
}
