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
        // just using the number 8, which is the limit in the shader


        // now we define some arrays and variables, to be shipped to the shadeer
        Vector3 sunPosition = Vector3.zero;
        Vector3[] planetCentres = new Vector3[8];
        Vector3[] scatterCoefficients = new Vector3[8];
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

        for (int i = 0; i < 8; i++)
        {
            // TODO: re-package the data
        }
    }
}
