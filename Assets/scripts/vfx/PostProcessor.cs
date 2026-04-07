using UnityEngine;

public class PostProcesser : MonoBehaviour
{
    private static PostProcesser _instance;

    public static PostProcesser Instance
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

    public Material m_atmosphere;
    public float densityFalloff;

    public float scatterStrength = 1;

    //public cbr_shaderdata shaderData;

    void CompileShaderData()
    {
        
    }

    // void OnRenderImage(RenderTexture source, RenderTexture mod)
    // {
    //     int[] planetIndicesDepthSorted = SortPlanetIndices();

    //     // just using another function to handle the wavelength -> coefficient calculation for us
    //     Vector4[] convertedWaveLengths = GetScatterCoefficients();
    //     m_atmosphere.SetVectorArray("scatterCoefficients", convertedWaveLengths);

    //     // so the problem with all of this data is that it needs to be packaged into arrays
    //     // doing this with a custom data class is better, because that way the data can be stored away without having way too many refs

    //     m_atmosphere.SetFloatArray("planetScale", new float[] { TrackingManager.Instance.bodies[1].transform.localScale.x, TrackingManager.Instance.bodies[2].transform.localScale.x, TrackingManager.Instance.bodies[3].transform.localScale.x });

    //     m_atmosphere.SetVector("sunPosition", TrackingManager.Instance.bodies[0].pose.GetPosition().Sub(CameraController.Instance.GetPosition()).ToVector3());
    //     m_atmosphere.SetVectorArray("planetCentre", new Vector4[] { TrackingManager.Instance.bodies[1].pose.GetPosition().Sub(CameraController.Instance.GetPosition()).ToVector3(), TrackingManager.Instance.bodies[2].pose.GetPosition().Sub(CameraController.Instance.GetPosition()).ToVector3(), TrackingManager.Instance.bodies[3].pose.GetPosition().Sub(CameraController.Instance.GetPosition()).ToVector3() });

    //     m_atmosphere.SetFloatArray("atmosphereRadius", new float[] { TrackingManager.Instance.bodies[1].config.atmosphericRadius, TrackingManager.Instance.bodies[2].config.atmosphericRadius, TrackingManager.Instance.bodies[3].config.atmosphericRadius });
    //     m_atmosphere.SetFloatArray("surfaceRadius", new float[] { TrackingManager.Instance.bodies[1].config.equitorialRadius, TrackingManager.Instance.bodies[2].config.equitorialRadius, TrackingManager.Instance.bodies[3].config.equitorialRadius });

    //     m_atmosphere.SetFloatArray("densityFalloff", new float[] { densityFalloff, densityFalloff, densityFalloff });

    //     m_atmosphere.SetFloatArray("minCloudRadius", new float[] { TrackingManager.Instance.bodies[1].config.minCloudRadius, TrackingManager.Instance.bodies[2].config.minCloudRadius, TrackingManager.Instance.bodies[3].config.minCloudRadius });
    //     m_atmosphere.SetFloatArray("maxCloudRadius", new float[] { TrackingManager.Instance.bodies[1].config.maxCloudRadius, TrackingManager.Instance.bodies[2].config.maxCloudRadius, TrackingManager.Instance.bodies[3].config.maxCloudRadius });

    //     m_atmosphere.SetFloatArray("theta", new float[] { TrackingManager.Instance.universalTime / 150f, TrackingManager.Instance.universalTime / 150f, TrackingManager.Instance.universalTime / 150f });

    //     m_atmosphere.SetFloatArray("densityMultiplier", new float[] { 0.15f, 0.15f, 0.15f });
    //     m_atmosphere.SetFloatArray("luminance", new float[] { 50f, 50f, 50f });
    //     m_atmosphere.SetFloatArray("externalBrightness", new float[] { 1f, 1f, 1f });
    //     m_atmosphere.SetFloatArray("cloudBrightness", new float[] { 7f, 7f, 7f });
    //     m_atmosphere.SetFloatArray("scatterFactor", new float[] { 0.5f, 0.5f, 0.5f });

    //     Graphics.Blit(source, mod, m_atmosphere);
    // }

    // // sorting by depth, so the post-processing effect renders correctly
    // int[] SortPlanetIndices()
    // {
    //     List<int> result = new List<int>();

    //     List<float> planetDepths = new List<float>();
    //     List<int> planetIndices = new List<int>();

    //     for (int i = 0; i < TrackingManager.Instance.bodies.Length; i++)
    //     {
    //         planetIndices.Add(i);
    //         planetDepths.Add((float)CameraController.Instance.GetPosition().Sub(TrackingManager.Instance.bodies[i].pose.GetPosition()).Mag());
    //     }

    //     // now, the sort
    //     for (int j = 0; j < TrackingManager.Instance.bodies.Length; j++)
    //     {
    //         int smallestIndex = 0;
    //         for (int i = 1; i < planetIndices.Count; i++)
    //         {
    //             if (planetDepths[i] < smallestIndex)
    //             {
    //                 smallestIndex = i;
    //             }
    //         }

    //         result.Add(planetIndices[smallestIndex]);

    //         planetDepths.RemoveAt(smallestIndex);
    //         planetIndices.RemoveAt(smallestIndex);
    //     }

    //     return result.ToArray();
    // }

    // Vector4[] GetScatterCoefficients()
    // {
    //     Vector4[] result = new Vector4[TrackingManager.Instance.bodies.Length];

    //     for (int i = 0; i < result.Length; i++)
    //     {
    //         result[i] = new Vector4(
    //         Mathf.Pow(400f / TrackingManager.Instance.bodies[i].config.waveLengths.x, 4) * scatterStrength,
    //         Mathf.Pow(400f / TrackingManager.Instance.bodies[i].config.waveLengths.y, 4) * scatterStrength,
    //         Mathf.Pow(400f / TrackingManager.Instance.bodies[i].config.waveLengths.z, 4) * scatterStrength,
    //         0
    //         );
    //     }

    //     return result;
    // }
}
