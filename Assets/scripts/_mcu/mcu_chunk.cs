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
    public num_precisevector3 minimumPoint;
    public num_precisevector3 maximumPoint;
    private num_precisevector3 originOffset;

    public float size; // length of an edge of the cube
    private int resolution;

    public void SetBounds(num_precisevector3 min,num_precisevector3 max)
    {
        minimumPoint = min;
        maximumPoint = max;

        size = max.x.Sub(min.x).AsFloat();

        // Debug.Log(minimumPoint.ToVector3());
        // Debug.Log(maximumPoint.ToVector3());

        originOffset = min.Add(max).Div(2f);

        // could do this, but instead I'm telling the mcu chunk to move its vertices
        //transform.localPosition += min.ToVector3() - originOffset.ToVector3();

        rend.SetOffset(min.ToVector3() - originOffset.ToVector3());
    }

    public void Generate(num_precisevector3 min, num_precisevector3 max, int resolution)
    {
        this.resolution = resolution;
        SetBounds(min,max);
        Generate(resolution);
    }

    public void Generate(int resolution)
    {
        isVisible = true;

        int res = resolution;

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

        num_precisevector3 min = minimumPoint;
        num_precisevector3 max = maximumPoint;
        num_precisevector3 half = minimumPoint.Add((maximumPoint.Sub(minimumPoint))).Div(2f);

        daughterChunks[0].transform.position = new num_precisevector3(min.x,min.y,min.z).ToVector3();
        daughterChunks[0].SetBounds(new num_precisevector3(min.x,min.y,min.z),
        new num_precisevector3(half.x,half.y,half.z));

        daughterChunks[1].transform.position = new num_precisevector3(half.x,min.y,min.z).ToVector3();
        daughterChunks[1].SetBounds(new num_precisevector3(half.x,min.y,min.z),
        new num_precisevector3(max.x,half.y,half.z));

        daughterChunks[2].transform.position = new num_precisevector3(half.x,min.y,half.z).ToVector3();
        daughterChunks[2].SetBounds(new num_precisevector3(half.x,min.y,half.z),
        new num_precisevector3(max.x,half.y,max.z));

        daughterChunks[3].transform.position = new num_precisevector3(min.x,min.y,half.z).ToVector3();
        daughterChunks[3].SetBounds(new num_precisevector3(min.x,min.y,half.z),
        new num_precisevector3(half.x,half.y,max.z));




        daughterChunks[4].transform.position = new num_precisevector3(min.x,half.y,min.z).ToVector3();
        daughterChunks[4].SetBounds(new num_precisevector3(min.x,half.y,min.z),
        new num_precisevector3(half.x,max.y,half.z));

        daughterChunks[5].transform.position = new num_precisevector3(half.x,half.y,min.z).ToVector3();
        daughterChunks[5].SetBounds(new num_precisevector3(half.x,half.y,min.z),
        new num_precisevector3(max.x,max.y,half.z));

        daughterChunks[6].transform.position = new num_precisevector3(half.x,half.y,half.z).ToVector3();
        daughterChunks[6].SetBounds(new num_precisevector3(half.x,half.y,half.z),
        new num_precisevector3(max.x,max.y,max.z));

        daughterChunks[7].transform.position = new num_precisevector3(min.x,half.y,half.z).ToVector3();
        daughterChunks[7].SetBounds(new num_precisevector3(min.x,half.y,half.z),
        new num_precisevector3(half.x,max.y,max.z));

        for (int i = 0; i < daughterChunks.Length; i++)
        {
            daughterChunks[i].Generate(resolution);
        }

        rend.gameObject.SetActive(false);
    }

    // converting a vertex index to a 3D position,
    // based on the min and max points
    public num_precisevector3 IndexToPosition(int x,int y,int z)
    {
        //Debug.Log((float)(mcu_utils.chunkResolution-1) / (float)x / 5f);

        return new num_precisevector3(
            Mathf.Lerp(minimumPoint.x.AsFloat(),maximumPoint.x.AsFloat(),1f / (float)(resolution-1) * (float)x),
            Mathf.Lerp(minimumPoint.y.AsFloat(),maximumPoint.y.AsFloat(),1f / (float)(resolution-1) * (float)y),
            Mathf.Lerp(minimumPoint.z.AsFloat(),maximumPoint.z.AsFloat(),1f / (float)(resolution-1) * (float)z)
        );

        //Debug.Log(new num_precise(1f / (float)(resolution-1) * (float)x).AsFloat());
        //Debug.Log(num_precise.Lerp(minimumPoint.x,maximumPoint.x,new num_precise(1f / (float)(resolution-1) * (float)x)).AsFloat());
    }

    // sort of a temporary way of getting point data
    float GetPoint(num_precisevector3 pos)
    {
        //return -(pos.Mag().AsFloat() - 20f);

        return -(pos.Mag().AsFloat() - GetComponent<cbt_marchedchunk>().actualRadius);
    }
}
