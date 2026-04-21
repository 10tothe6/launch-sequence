using UnityEngine;

// the entity system V2 uses these to organize (server --> client) entity position updates

[System.Serializable]
public class e_entityupdatepackage
{
    public string[] independentData;
    //public string[] localizedData; // unused rn


    // no sleeping data because sleeping objects aren't updated
    // that's literally the definition of sleeping


    // these are special becaues they're split by clients
    public string[][] controllingData;  // unused rn
    //public string[][] influencedData;  // unused rn

    public e_entityupdatepackage() {}

    public e_entityupdatepackage(string[] independentData)
    {
        this.independentData =independentData;
    }

    // public e_entityupdatepackage(string[] independentData, string[] localizedData, string[][] controllingData, string[][] influencedData)
    // {
    //     this.independentData = independentData;
    //     this.localizedData = localizedData;
    //     this.controllingData = controllingData;
    //     this.influencedData = influencedData;
    // }
}
