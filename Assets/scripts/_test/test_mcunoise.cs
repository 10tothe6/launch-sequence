using UnityEngine;

public class test_mcunoise : MonoBehaviour
{
    private Perlin p = new Perlin();
    public float offset;
    public bool run;

    void Start()
    {
        Test();
    }

    void Update()
    {
        if (run)
        {
            run = false;
            Test();
        }
    }

    void Test()
    {
        int size = 30;
        float [,,] noiseGrid = new float[size,size,size];

        float centerX = size / 2f;
        float centerY = size / 2f;
        float centerZ = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    float freq = 0.1f;
                    float amp = 10f;

                    noiseGrid[x,y,z] = (float)p.Noise(x * freq,0,z * freq) * amp - y + offset;
                    //noiseGrid[x,y,z] = Mathf.Sqrt(Mathf.Pow((float)(x - centerX) ,2) + Mathf.Pow((float)(y - centerY), 2) + Mathf.Pow((float)(z - centerZ), 2));
                }
            }
        }

        GetComponent<mcu_drawmesh>().Initialize(noiseGrid, size,size,size);
    }
}
