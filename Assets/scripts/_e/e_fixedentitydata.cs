using UnityEngine;

// see entities README!!!

[System.Serializable]
public class e_fixedentitydata
{
    // very important for keeping track of entities in the multiplayer system
    public int index;

    public Transform reference;

    public num_precisevector3 localPosition; // THIS IS LOCAL

    public num_precisevector3 rotation;
    public num_precisevector3 velocity;

    public num_precisevector3 GetPosition()
    {
        return localPosition;
    }

    public net_packagedentitydata Package()
    {
        return new net_packagedentitydata();
    }

    public int GetPrefabIndex()
    {
        return 0;
        // TODO: this
    }
}
