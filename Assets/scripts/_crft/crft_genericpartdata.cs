using UnityEngine;

[System.Serializable]
public class crft_genericpartdata
{
    public string partName;
    public Vector3 position;

    public crft_genericpartdata() {}

    public crft_genericpartdata(string partName, Vector3 position)
    {
        this.partName = partName;
        this.position = position;
    }
}
