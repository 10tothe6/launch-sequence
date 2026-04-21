using UnityEngine;

[System.Serializable]
public class TextureProfile {
    // Settings
    public string name;
    public string filePath;

    // Texture settings
    public Vector2Int textureResolution; 
    public Texture2D texture;
    public NoiseLayer[] noiseLayers;
    // The indices of the noise layers added to each color channel (rgb)
    public int[] colorChannelR;
    public int[] colorChannelG;
    public int[] colorChannelB;
    public int[] colorChannelA;
    // The multiplier to put on each color channel
    public float[] colorMultipliers;
    public float[] colorOffsets;

    public bool useUnityNoise;

    public bool useAlphaChannel;
    public bool invertX;
    public bool invertY;
    public bool interpolateColor;
    public Color col1;
    public Color col2;
}

// A layer of noise, with amplitude, scale, etc.
[System.Serializable]
public class NoiseLayer {
    public int layerType; // x only (0), y only (1), spherical x,y,z (2), standard x,y (3)
    public float amplitude;
    public float frequency;
}
