using UnityEngine;

[System.Serializable]
public class strc_solidmaterial
{
    // all in newtons (N)
    public float maxTension;
    public float maxCompression;
    public float maxShear;

    public strc_solidmaterial() {}

    public strc_solidmaterial(float maxTension, float maxCompression, float maxShear)
    {
        this.maxTension = maxTension;
        this.maxCompression = maxCompression;
        this.maxShear = maxShear;
    }
}
