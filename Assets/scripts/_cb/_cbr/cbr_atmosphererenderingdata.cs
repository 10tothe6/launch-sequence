using UnityEngine;

// THIS IS ALL THE DATA NEEDED TO SEND TO THE SHADER

[System.Serializable]
public class cbr_atmosphererenderingdata
{
    public Vector3 bodyCenter;
    public float atmosphereRadius;
    public float surfaceRadius;

    public float densityFalloff;
    public Vector3 scatterCoefficients;

    public float planetScale;
    public float densityMultiplier;
    public float luminance;
    public float externalBrightness;
    public float scatterFactor;
    public float cloudBrightness;
    public float minCloudRadius;
    public float maxCloudRadius;
}
