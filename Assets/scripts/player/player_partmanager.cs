using UnityEngine;

public class player_partmanager : MonoBehaviour
{
    private e_craft craftComp;
    public string[] defaultPartNames;

    void Awake()
    {
        craftComp = GetComponent<e_craft>();

        InitializeFirstTime();
    }

    public void InitializeFirstTime()
    {
        AddDefaultParts();
    }

    public void AddDefaultParts()
    {
        for (int i = 0; i < defaultPartNames.Length; i++)
        {
            // position doesn't really matter for these cuz they have no model
            crft_genericpartdata newPartData = new crft_genericpartdata(defaultPartNames[i], Vector3.zero);

            craftComp.AddPart(newPartData);
        }
    }
}
