using UnityEngine;

[System.Serializable]
public class crft_genericpartdata
{
    public string partName;
    public Vector3 position;
    public Vector3 euler_angles;

    public string additional_part_data;

    public crft_genericpartdata() {}

    public crft_genericpartdata(string partName, Vector3 position)
    {
        this.partName = partName;
        this.position = position;
        this.additional_part_data = "";
    }

    public crft_genericpartdata(string partName, Vector3 position, Vector3 euler_angles)
    {
        this.partName = partName;
        this.position = position;
        this.additional_part_data = "";

        this.euler_angles = euler_angles;
    }

    public crft_genericpartdata(string partName, Vector3 position, string additional_part_data)
    {
        this.partName = partName;
        this.position = position;
        this.additional_part_data = additional_part_data;
    }

    public crft_genericpartdata(string partName, Vector3 position, Vector3 euler_angles, string additional_part_data)
    {
        this.partName = partName;
        this.position = position;
        this.additional_part_data = additional_part_data;

        this.euler_angles = euler_angles;
    }
}
