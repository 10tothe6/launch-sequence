using UnityEngine;

public class TemporaryPerlin : MonoBehaviour
{
    private static TemporaryPerlin _instance;

    public static TemporaryPerlin Instance
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

    public Perlin perlin = new Perlin();
}
