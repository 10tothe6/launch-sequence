using UnityEngine;

// singleton script that stores static data for all the items in the game,
// as well as their prefabs (if they're not spacecraft parts)

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;

    public static ItemManager Instance
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

    public inv_itemdata[] items;


    // for the items that are NOT spacecraft parts
    public GameObject[] p_items;
}
