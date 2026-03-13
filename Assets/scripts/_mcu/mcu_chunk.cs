using UnityEngine;

// this is really the important class in the mcu system

// it is responsible for drawing a mesh representing the result of a function (perlin, probably)
// over a certain amount of 3D space
// (ALL CHUNKS ARE CUBES)


public class mcu_chunk : MonoBehaviour
{
    public bool isVisible;
    public GameObject p_chunk;
    public Transform t_chunkContainer;
    private Perlin p = new Perlin();
    public mcu_drawmesh rend;

    // the coordinates that the chunk represents, in whatever space we're dealing with
    // for testing this is just engine-space but in-game this is planet-space
    public Vector3 minimumPoint;
    public Vector3 maximumPoint;

    public float size; // length of an edge of the cube

    public void SetBounds(Vector3 min,Vector3 max)
    {
        minimumPoint = min;
        maximumPoint = max;

        size = max.x - min.x;
    }

    public void Generate(Vector3 min, Vector3 max)
    {
        SetBounds(min,max);
        Generate();
    }

    public void Generate()
    {
        isVisible = true;

        int res = mcu_utils.chunkResolution;
        //constructing the points array for the rend
        float[,,] points = new float[res,res,res];

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                for (int z = 0; z < res; z++)
                {
                    points[x,y,z] = GetPoint(IndexToPosition(x,y,z));
                }
            }
        }

        rend.Initialize(points,res,res,res,size,size,size);
    }

    // make four chunks out of a single chunk
    public void Split()
    {
        isVisible = false;
        mcu_chunk[] daughterChunks = new mcu_chunk[8];
        for (int i = 0; i < 8; i++)
        {
            daughterChunks[i] = Instantiate(p_chunk, t_chunkContainer).GetComponent<mcu_chunk>();
            daughterChunks[i].t_chunkContainer = t_chunkContainer;
        }
        
        // daughter chunks are indexed in exactly the same way as vertices on a cube
        // see (mcu_utils for the convention)

        Vector3 min = minimumPoint;
        Vector3 max = maximumPoint;
        Vector3 half = minimumPoint + (maximumPoint - minimumPoint) / 2f;

        daughterChunks[0].transform.position = new Vector3(min.x,min.y,min.z);
        daughterChunks[0].SetBounds(new Vector3(min.x,min.y,min.z),
        new Vector3(half.x,half.y,half.z));

        daughterChunks[1].transform.position = new Vector3(half.x,min.y,min.z);
        daughterChunks[1].SetBounds(new Vector3(half.x,min.y,min.z),
        new Vector3(max.x,half.y,half.z));

        daughterChunks[2].transform.position = new Vector3(half.x,min.y,half.z);
        daughterChunks[2].SetBounds(new Vector3(half.x,min.y,half.z),
        new Vector3(max.x,half.y,max.z));

        daughterChunks[3].transform.position = new Vector3(min.x,min.y,half.z);
        daughterChunks[3].SetBounds(new Vector3(min.x,min.y,half.z),
        new Vector3(half.x,half.y,max.z));




        daughterChunks[4].transform.position = new Vector3(min.x,half.y,min.z);
        daughterChunks[4].SetBounds(new Vector3(min.x,half.y,min.z),
        new Vector3(half.x,max.y,half.z));

        daughterChunks[5].transform.position = new Vector3(half.x,half.y,min.z);
        daughterChunks[5].SetBounds(new Vector3(half.x,half.y,min.z),
        new Vector3(max.x,max.y,half.z));

        daughterChunks[6].transform.position = new Vector3(half.x,half.y,half.z);
        daughterChunks[6].SetBounds(new Vector3(half.x,half.y,half.z),
        new Vector3(max.x,max.y,max.z));

        daughterChunks[7].transform.position = new Vector3(min.x,half.y,half.z);
        daughterChunks[7].SetBounds(new Vector3(min.x,half.y,half.z),
        new Vector3(half.x,max.y,max.z));

        for (int i = 0; i < daughterChunks.Length; i++)
        {
            daughterChunks[i].Generate();
        }

        rend.gameObject.SetActive(false);
    }

    // converting a vertex index to a 3D position,
    // based on the min and max points
    public Vector3 IndexToPosition(int x,int y,int z)
    {
        //Debug.Log((float)(mcu_utils.chunkResolution-1) / (float)x / 5f);
        return new Vector3(
            Mathf.Lerp(minimumPoint.x,maximumPoint.x,1f / (float)(mcu_utils.chunkResolution-1) * (float)x),
            Mathf.Lerp(minimumPoint.y,maximumPoint.y,1f / (float)(mcu_utils.chunkResolution-1) * (float)y),
            Mathf.Lerp(minimumPoint.z,maximumPoint.z,1f / (float)(mcu_utils.chunkResolution-1) * (float)z)
        );
    }

    // sort of a temporary way of getting point data
    float GetPoint(Vector3 pos)
    {
        // float freq = 0.1f;
        // float amp = 10f;

        return -(Vector3.Distance(pos, Vector3.one * 5) - 4);

        //return (float)p.Noise(pos.x * freq,0,pos.z * freq) * amp - pos.y + 5;

        //return Mathf.Clamp(5f - pos.y,0,1);
    }
}
