using UnityEngine;
using System.Collections.Generic;

// replacing the 'Planet.cs' script from previous iterations with this more organized, better-named one

public class cbt_meshbody : MonoBehaviour
{
    public GameObject p_chunk;
    public Transform t_chunkContainer;

    // the resolution of one side of the chunk
    public int startingResolution = 10;

    private int bodyIndex;

    // ************ chunk data ************
    [HideInInspector]
    public List<cbt_meshchunk> chunks;
    [HideInInspector]
    public List<cbt_meshchunk> newChunks;
    [HideInInspector]
    public cbt_meshchunk[] parentChunks; // the 6 original faces making up the sphere

    // ************************

    // (LOD0 is the highest detail, LOD1 is next highest, LOD2 is next, etc.)
    public float[] detailLevelThresholds; // the DISTANCES that you have to get within to up the detail level
    // THESE ARE IN METERS, NOT UNITY UNITS
    // THEY HAVE TO BE MULTIPLIED BY THE UNIVERSAL SCALE FACTOR
    // DO NOT FORGET THIS

    public Transform t_decidingObject;

    // TODO: call this from a more managerial script
    void FixedUpdate()
    {
        if (chunks != null)
        {
            UpdateAllChunks();
        }
    }

    // ********************************************************************

    public void Initialize()
    {
        // **** just setting variables ****
        
        chunks = new List<cbt_meshchunk>();
        newChunks = new List<cbt_meshchunk>();

        parentChunks = new cbt_meshchunk[6];
        Vector3[] directions = new Vector3[6] { Vector3.up, -Vector3.up, Vector3.right, -Vector3.right, Vector3.forward, -Vector3.forward };
        // ****************************


        // initializing the 6 parent chunks
        for (int i = 0; i < 6; i++)
        {
            GameObject g_parentChunk = Instantiate(p_chunk, t_chunkContainer);
            parentChunks[i] = g_parentChunk.GetComponent<cbt_meshchunk>();

            parentChunks[i].Initialize(startingResolution, directions[i], new Vector3(0, 0, startingResolution), bodyIndex);
            parentChunks[i].level = 0;
            parentChunks[i].parent = null;

            parentChunks[i].startingFace = i;
            parentChunks[i].hashCode = null;
        }

        // now actually build their meshes
        for (int i = 0; i < 6; i++)
        {
            parentChunks[i].ConstructMesh(bodyIndex);
        }

        // now add the parents to the master list, and we can begin increasing/decreasing the LODs
        for (int i = 0; i < parentChunks.Length; i++)
        {
            chunks.Add(parentChunks[i]);
        }
    }

    void UpdateAllChunks()
    {
        // newChunks.Clear();

        // // oh god, remember when I used to use foreach loops?
        // // fuck it im keeping it in
        // // - 10^6, 3/30/2026
        // foreach (cbt_meshchunk current in chunks)
        // {   
        //     // active gameobjects means the chunk is actually being rendered, and therefore is relevant
        //     if (current.gameObject.activeSelf && current.level > 0)
        //     {
        //         int count = 0;
        //         for (int i = 0; i < 4; i++)
        //         {
        //             if (Vector3.Distance(current.parent.children[i].GetComponent<MeshRenderer>().bounds.center, t_player.position) > detailLevelThresholds[current.level - 1])
        //             {
        //                 count++;
        //             }
        //         }

        //         if (count == 4)
        //         {
        //             current.gameObject.SetActive(false);
        //             if (current.parent != null)
        //             {
        //                 current.parent.gameObject.SetActive(true);
        //             }
        //         }
        //     }

        //     if (current.children != null)
        //     {
        //         if (current.gameObject.name == "mesh")
        //         {
        //             for (int i = 0; i < 4; i++)
        //             {
        //                 newChunks.Add(current.children[i]);
        //             }

        //             current.gameObject.name = "cached mesh";
        //             current.gameObject.SetActive(false);
        //         }
        //     }
        //     else if (detailLevelThresholds.Length > current.level + 1 && Vector3.Distance(current.GetComponent<MeshRenderer>().bounds.center, t_player.position) < detailLevelThresholds[current.level + 1])
        //     {
        //         Subdivide(current);
        //     }

        //     // if (current.hashCode != null && current.reference.activeSelf)
        //     // {
        //     //     GetNeighborLOD(current.hashCode, current.reference.GetComponent<MeshRenderer>(), current.startingFace);
        //     // }
        // }

        // // transferring the new chunks over to the master chunks list
        // foreach(cbt_meshchunk current in newChunks)
        // {
        //     chunks.Add(current);
        // }
    }

    // public void Subdivide(cbt_meshchunk input)
    // {
    //     input.children = new cbt_meshchunk[4];

    //     for (int i = 0; i < 4; i++)
    //     {
    //         // not using a prefab for some reason? fine
    //         GameObject meshObj = new GameObject("mesh");
    //         meshObj.transform.SetParent(t_chunkContainer);
    //         meshObj.transform.localScale = Vector3.one;

    //         meshObj.transform.position = cb_solarsystem.Instance.monoBodies[bodyIndex].transform.position;

    //         MeshFilter newFilter = meshObj.AddComponent<MeshFilter>();
    //         meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(planetMaterial);
    //         newFilter.sharedMesh = new Mesh();

    //         float res = (float)startingResolution;
            
    //         input.children[i].Initialize(newFilter.sharedMesh, resolution, input.localUp, new Vector3(input.dims.x + (res * (input.dims.z / res)) / 2f * ((float)i % 2f), input.dims.y + (res * (input.dims.z/ res)) / 2 * Mathf.Floor((float)i / 2f), (res * (input.dims.z/ res)) / 2f), radius);
    //         input.children[i].level = input.level + 1;
    //         input.children[i].parent = input;

    //         input.children[i].startingFace = input.startingFace;
    //         input.children[i].hashCode = input.hashCode + i.ToString();
    //     }

    //     for (int i = 0; i < 4; i++)
    //     {
    //         input.children[i].ConstructMesh(bodyIndex);

    //         if (input.level + 1 > detailLevelThresholds.Length - 5)
    //         {
    //             input.children[i].gameObject.AddComponent<MeshCollider>();
    //         }
    //     }
    // }

    // void GetNeighborLOD(string hashCode, MeshRenderer output, int startingFace)
    // {
    //     int lastDigit = int.Parse(hashCode[hashCode.Length - 1].ToString());

    //     bool lod1 = false;
    //     bool lod2 = false;

    //     if (lastDigit == 0)
    //     {
    //         lod1 = CheckNeighborLOD(hashCode, 0, startingFace);
    //         lod2 = CheckNeighborLOD(hashCode, 3, startingFace);
    //     }
    //     else if (lastDigit == 1)
    //     {
    //         lod1 = CheckNeighborLOD(hashCode, 0, startingFace);
    //         lod2 = CheckNeighborLOD(hashCode, 2, startingFace);
    //     }
    //     else if (lastDigit == 2)
    //     {
    //         lod1 = CheckNeighborLOD(hashCode, 1, startingFace);
    //         lod2 = CheckNeighborLOD(hashCode, 3, startingFace);
    //     }
    //     else if (lastDigit == 3)
    //     {
    //         lod1 = CheckNeighborLOD(hashCode, 1, startingFace);
    //         lod2 = CheckNeighborLOD(hashCode, 2, startingFace);
    //     }
        
    //     if (!lod1 || !lod2)
    //     {
    //         output.sharedMaterial = planetMaterial;
    //     }
    //     else
    //     {
    //         output.sharedMaterial = planetMaterial;
    //     }
    // }

    // bool CheckNeighborLOD(string hashCode, int dir, int startingFace)
    // {
    //     string neighborCode = ComputeHashValue(hashCode, dir);

    //     if (IsOnBorder(hashCode, dir))
    //     {
    //         startingFace = ChangeSide(startingFace, dir);
    //         neighborCode = ChangeHash(neighborCode, startingFace, dir);
    //     }
        
    //     TerrainFace currentFace = terrainFaces[startingFace];
        
    //     for (int i = 0; i < neighborCode.Length; i++)
    //     {
    //         if (currentFace.hashCode == null && currentFace.children == null)
    //         {
    //             return false;
    //         }

    //         if (currentFace.children == null && currentFace.hashCode.Length != hashCode.Length)
    //         {
    //             return false;
    //         }
    //         else
    //         {
    //             currentFace = currentFace.children[int.Parse(neighborCode[i].ToString())];
    //         }
    //     }

    //     if (!currentFace.reference.activeSelf)
    //     {
    //         if (currentFace.children == null)
    //         {
    //             return false;
    //         }

    //         int count = 0;
    //         for (int i = 0; i < 4; i++)
    //         {
    //             if (!currentFace.children[i].reference.activeSelf)
    //             {
    //                 count++;
    //             }
    //         }

    //         if (count == 4)
    //         {
    //             return false;
    //         }
    //     }

    //     return true;
    // }

    // bool IsOnBorder(string hashCode, int dir)
    // {
    //     if (dir == 0)
    //     {
    //         if (!hashCode.Contains("2") && !hashCode.Contains("3"))
    //         {
    //             return true;
    //         }
    //     }
    //     else if (dir == 1)
    //     {
    //         if (!hashCode.Contains("1") && !hashCode.Contains("0"))
    //         {
    //             return true;
    //         }
    //     }
    //     else if (dir == 2)
    //     {
    //         if (!hashCode.Contains("0") && !hashCode.Contains("2"))
    //         {
    //             return true;
    //         }
    //     }
    //     else if (dir == 3)
    //     {
    //         if (!hashCode.Contains("3") && !hashCode.Contains("1"))
    //         {
    //             return true;
    //         }
    //     }

    //     return false;
    // }

    // string ChangeHash(string hashCode, int side, int dir)
    // {
    //     string newHashCode = null;

    //     if (dir == 0)
    //     {
    //         if (side % 2 == 0)
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode = "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode = "2";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode += "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode += "2";
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode = "3";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode = "1";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode += "3";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode += "1";
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     else if (dir == 1)
    //     {
    //         if (side % 2 == 0)
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode = "3";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode = "1";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode += "3";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode += "1";
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode = "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode = "2";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode += "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode += "2";
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     else if (dir == 2)
    //     {
    //         if (side % 2 != 0)
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode = "1";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode = "0";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode += "1";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode += "0";
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode = "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode = "3";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "0"))
    //                     {
    //                         newHashCode += "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "2"))
    //                     {
    //                         newHashCode += "3";
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     else if (dir == 3)
    //     {
    //         if (side % 2 != 0)
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode = "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode = "1";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode += "0";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode += "1";
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             for (int i = 0; i < hashCode.Length; i++)
    //             {
    //                 if (i == 0)
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode = "3";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode = "2";
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (string.Equals(hashCode[i].ToString(), "1"))
    //                     {
    //                         newHashCode += "3";
    //                     }
    //                     else if (string.Equals(hashCode[i].ToString(), "3"))
    //                     {
    //                         newHashCode += "2";
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     return newHashCode;
    // }
    
    // int ChangeSide(int side, int dir)
    // {
    //     if (dir == 0)
    //     {
    //         if (side == 0)
    //         {
    //             side = 4;
    //         }
    //         else if (side == 1)
    //         {
    //             side = 4;
    //         }
    //         else if (side == 2)
    //         {
    //             side = 0;
    //         }
    //         else if (side == 3)
    //         {
    //             side = 0;
    //         }
    //         else if (side == 4)
    //         {
    //             side = 2;
    //         }
    //         else if (side == 5)
    //         {
    //             side = 2;
    //         }
    //     }
    //     else if (dir == 1)
    //     {
    //         if (side == 0)
    //         {
    //             side = 5;
    //         }
    //         else if (side == 1)
    //         {
    //             side = 5;
    //         }
    //         else if (side == 2)
    //         {
    //             side = 1;
    //         }
    //         else if (side == 3)
    //         {
    //             side = 1;
    //         }
    //         else if (side == 4)
    //         {
    //             side = 3;
    //         }
    //         else if (side == 5)
    //         {
    //             side = 3;
    //         }
    //     }
    //     else if (dir == 2)
    //     {
    //         if (side == 0)
    //         {
    //             side = 2;
    //         }
    //         else if (side == 1)
    //         {
    //             side = 3;
    //         }
    //         else if (side == 2)
    //         {
    //             side = 4;
    //         }
    //         else if (side == 3)
    //         {
    //             side = 5;
    //         }
    //         else if (side == 4)
    //         {
    //             side = 0;
    //         }
    //         else if (side == 5)
    //         {
    //             side = 1;
    //         }
    //     }
    //     else if (dir == 3)
    //     {
    //         if (side == 0)
    //         {
    //             side = 3;
    //         }
    //         else if (side == 1)
    //         {
    //             side = 2;
    //         }
    //         else if (side == 2)
    //         {
    //             side = 5;
    //         }
    //         else if (side == 3)
    //         {
    //             side = 4;
    //         }
    //         else if (side == 4)
    //         {
    //             side = 1;
    //         }
    //         else if (side == 5)
    //         {
    //             side = 0;
    //         }
    //     }

    //     return side;
    // }

    // string ComputeHashValue(string input, int dir)
    // {
    //     string hashCode = input;

    //     for (int i = hashCode.Length - 1; i >= 0; i--)
    //     {
    //         int digit = int.Parse(hashCode[i].ToString());

    //         int newDigit = -1;
    //         string newHash = null;

    //         newDigit = FlipNumber(digit, dir);
    //         for (int _i = 0; _i < hashCode.Length; _i++)
    //         {
    //             if (_i == i)
    //             {
    //                 if (string.IsNullOrEmpty(newHash))
    //                 {
    //                     newHash = newDigit.ToString();
    //                 }
    //                 else
    //                 {
    //                     newHash += newDigit.ToString();
    //                 }
    //             }
    //             else
    //             {
    //                 if (string.IsNullOrEmpty(newHash))
    //                 {
    //                     newHash = hashCode[_i].ToString();
    //                 }
    //                 else
    //                 {
    //                     newHash += hashCode[_i];
    //                 }
    //             }
    //         }

    //         hashCode = newHash;

    //         if (dir == 0)
    //         {
    //             if (digit == 2 || digit == 3)
    //             {
    //                 break;
    //             }
    //         }
    //         else if (dir == 1)
    //         {
    //             if (digit == 1 || digit == 0)
    //             {
    //                 break;
    //             }
    //         }
    //         else if (dir == 2)
    //         {
    //             if (digit == 0 || digit == 2)
    //             {
    //                 break;
    //             }
    //         }
    //         else if (dir == 3)
    //         {
    //             if (digit == 1 || digit == 3)
    //             {
    //                 break;
    //             }
    //         }
    //     }

    //     return hashCode;
    // }

    // int FlipNumber(int num, int dir)
    // {
    //     if (dir == 0 || dir == 1)
    //     {
    //         if (num == 0)
    //         {
    //             num = 2;
    //         }
    //         else if (num == 1)
    //         {
    //             num = 3;
    //         }
    //         else if (num == 2)
    //         {
    //             num = 0;
    //         }
    //         else if (num == 3)
    //         {
    //             num = 1;
    //         }
    //     }
    //     else if (dir == 2 || dir == 3)
    //     {
    //         if (num == 0)
    //         {
    //             num = 1;
    //         }
    //         else if (num == 1)
    //         {
    //             num = 0;
    //         }
    //         else if (num == 2)
    //         {
    //             num = 3;
    //         }
    //         else if (num == 3)
    //         {
    //             num = 2;
    //         }
    //     }

    //     return num;
    // }
}
