using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

// same as test_mcunoise but this time utilizing the chunking system
public class test_mcuchunkednoise : MonoBehaviour
{
    public Transform t_chunks;
    public mcu_chunk chunk;

    void Start()
    {
        Test();
    }

    // what we're doing here is 
    void Test()
    {
        // last number (10) is resolution
        chunk.Generate(num_precisevector3.Zero(), num_precisevector3.One().Mul(10f), 10);
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            IncreaseDetail();
        }
    }

    public void IncreaseDetail()
    {
        int chunkCount = t_chunks.childCount;
        for (int i = 0; i < chunkCount; i++)
        {
            if (!t_chunks.GetChild(i).GetComponent<mcu_chunk>().isVisible) {continue;}
            t_chunks.GetChild(i).GetComponent<mcu_chunk>().Split();
        }
    }
}
