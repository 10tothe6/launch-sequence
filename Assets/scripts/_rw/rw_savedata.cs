using System;
using UnityEngine;

// the save data for one game - contains all the info on entities, players, inventory, etc.
[System.Serializable]
public class rw_savedata
{
    public float saveName; // the name of the world
    public DateTime dateCreated; // when the world was created
    public DateTime dateLastPlayed;

    public float universeTime; // what time to "set" the solar system to
    public int worldSeed;

    // TODO: storing entity data and craft data somehow
    public e_genericentitydata[] entities; // this will include crafts ig???

    public rw_savedata() {}
}
