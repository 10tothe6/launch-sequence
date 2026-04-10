using UnityEngine;

// personally? I hate the name for this script
// but it really doesn't matter

public class cbr_applyatmosphere : MonoBehaviour
{
    public Material m;

    public bool isInGame;
    private cbr_atmosphererenderingdata[] data;

    void Start()
    {
        UpdateShaderOnce();
    }

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        if (isInGame)
        {
            UpdateDataFromVisibleBodies();
        } 
        UpdateShaderPeriodic();

        Graphics.Blit(source, mod, m);
    }

    void UpdateShaderOnce()
    {
        
    }

    
    // called by the BodyEditor.cs script too
    public void ApplyAtmosphereRenderingData(cbr_atmosphererenderingdata[] data)
    {
        this.data = data;
    }

    void UpdateDataFromVisibleBodies()
    {
        if (data == null) {return;}

        int[] sortedBodyIndices = WorldManager.Instance.GetVisibleAtmosphericBodyIndices();
        if (sortedBodyIndices.Length > 8)
        {
            Debug.Log("Number of visible atmospheric bodies exeeded limit! Skipping the extras...");
        }

        int bodyCount = Mathf.Min(data.Length, 8);

        cbr_atmosphererenderingdata[] resultingData = new cbr_atmosphererenderingdata[sortedBodyIndices.Length];

        // TODO: fill it all out

        ApplyAtmosphereRenderingData(resultingData);
    }

    // this function takes all the data and re-packages it for the shader
    void UpdateShaderPeriodic()
    {
        if (data == null) {return;}
        // just using the number 8, which is the limit in the shader


        // now we define some arrays and variables, to be shipped to the shadeer
        Vector3 sunPosition = Vector3.right * 10000;
        Vector4[] planetCentres = new Vector4[8];
        Vector4[] scatterCoefficients = new Vector4[8];
        float[] atmosphereRadii = new float[8];
        float[] surfaceRadii = new float[8];
        float[] planetScales = new float[8];
        float[] densityMultipliers = new float[8];
        float[] densityFalloffs = new float[8];
        float[] luminances = new float[8];
        float[] externalBrightnesses = new float[8];
        float[] scatterFactors = new float[8];
        float[] cloudBrightnesses = new float[8];
        float[] minCloudRadii = new float[8];
        float[] maxCloudRadii = new float[8];

        for (int i = 0; i < Mathf.Min(8,data.Length); i++)
        {
            // re-packaging the data into separate arrays
            // (better organization for the shader, worse for me)
            planetCentres[i] = data[i].bodyCenter;
            scatterCoefficients[i] = data[i].scatterCoefficients;
            atmosphereRadii[i] = data[i].atmosphereRadius;
            surfaceRadii[i] = data[i].surfaceRadius;
            planetScales[i] = data[i].planetScale;
            densityMultipliers[i] = data[i].densityMultiplier;
            densityFalloffs[i] = data[i].densityFalloff;
            luminances[i] = data[i].luminance;
            externalBrightnesses[i] = data[i].externalBrightness;
            scatterFactors[i] = data[i].scatterFactor;
            cloudBrightnesses[i] = data[i].cloudBrightness;
            minCloudRadii[i] = data[i].minCloudRadius;
            maxCloudRadii[i] = data[i].maxCloudRadius;
        }

        // passing the packaged arrays off to the shader
        m.SetVector("sunPosition",sunPosition);

        m.SetVectorArray("planetCentre", planetCentres);
        m.SetVectorArray("scatterCoefficients", scatterCoefficients);
        
        m.SetFloatArray("atmosphereRadius", atmosphereRadii);
        m.SetFloatArray("surfaceRadius",surfaceRadii);
        m.SetFloatArray("planetScale",planetScales);
        m.SetFloatArray("densityMultiplier",densityMultipliers);
        m.SetFloatArray("densityFalloff", densityFalloffs);
        m.SetFloatArray("luminance", luminances);
        m.SetFloatArray("externalBrightness",externalBrightnesses);
        m.SetFloatArray("scatterFactor",scatterFactors);
        m.SetFloatArray("cloudBrightness",cloudBrightnesses);
        m.SetFloatArray("minCloudRadius", minCloudRadii);
        m.SetFloatArray("maxCloudRadius", maxCloudRadii);
    }
}
