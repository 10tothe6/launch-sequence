using UnityEngine;

public class cbt_marchedchunk : MonoBehaviour
{
    public cbt_marchedbody body;
    int resolution; // the number of vertices per side of the mesh (meshes are square)
    public float actualSize;
    int bodyIndex;

    public mcu_chunk mcu;


    // culling info
    public bool isCulledByAngle;
    public bool isCulledByLOD;


    public cbt_marchedchunk parent;
    public cbt_marchedchunk[] children;


    // UNUSED RN
    public float directRadius;
    public float actualRadius;


    
    public string hashCode; // also not super used, but i still feel its important
    public int levelOfDetail; // the LOD level of the chunk


    private bool hasBeenConstructed; // whether the mesh has been made yet or not

    private num_precisevector3 bounds_min;
    private num_precisevector3 bounds_max;

    public void InitializeDirect(int startingResolution, float directRadius)
    {
        this.bodyIndex = -1;
        
        this.resolution = startingResolution;
        this.directRadius = directRadius;

        ConstructMesh();
    }

    public void Initialize(int startingResolution, int bodyIndex)
    {
        this.bodyIndex = bodyIndex;

        this.resolution = startingResolution;

        // tell the mcu_drawmesh to do its thing
        ConstructMesh();
    }

    private void ConstructMesh()
    {
        hasBeenConstructed = true;

        // here we are creating the bounds of the chunk
        // in order to fit the planet, the square must have a side length equal to the planet's equatorial radius plus a bit to allow for mountains and such

        float rad = directRadius != 0 ? directRadius : cb_solarsystem.Instance.monoBodies[bodyIndex].data.tConfig.equitorialRadius;
        this.actualRadius = rad;

        // this 'extent' value is half of the side length
        //float box_extent = rad + 5000 * WorldData.universalScaleFactor; // 5 km margin
        float box_extent = rad;

        num_precisevector3 min = new num_precisevector3(-box_extent, -box_extent, -box_extent);
        num_precisevector3 max = new num_precisevector3(box_extent, box_extent, box_extent);

        this.bounds_min = min;
        this.bounds_max = max;

        mcu.Generate(min, max, resolution);
    }

    public void SetDebugColor(Color col)
    {
        mcu.rend.mr.material.color = col;
    }

    public void Subdivide()
    {
        mcu.Split();
    }


    // used to check for subdivision
    public bool IsLocalPlayerInBounds()
    {
        if (!hasBeenConstructed) {return false;}
        if (LocalPlayer.localClient == null) {return false;}
        if (LocalPlayer.localClient.controllingEntity == null) {return false;}

        num_precisevector3 playerPos = LocalPlayer.localClient.controllingEntity.data.GetPosition();

        num_precisevector3 actual_min = bounds_min.Add(body.eComp.data.GetPosition());
        num_precisevector3 actual_max = bounds_max.Add(body.eComp.data.GetPosition());

        // Debug.Log(actual_min.AsString());
        // Debug.Log(actual_max.AsString());
        // Debug.Log(playerPos.AsString());

        return num_precisevector3.BoundingBoxCheck(actual_min, actual_max, playerPos);
    }
}
