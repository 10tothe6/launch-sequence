using UnityEngine;

public class PartManager : MonoBehaviour
{
    private static PartManager _instance;

    public static PartManager Instance
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

    public GameObject[] p_parts;

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetPartPrefabFromName(string partName)
    {
        for (int i = 0; i < p_parts.Length; i++)
        {
            if (p_parts[i].name == "part_" + partName)
            {
                return p_parts[i];
            }
        }

        return null;
    }
}
