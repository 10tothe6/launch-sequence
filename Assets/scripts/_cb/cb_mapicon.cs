using UnityEngine.UI;
using UnityEngine;

public class cb_mapicon : MonoBehaviour
{
    
    public void SetBodyIndex(int index)
    {
        GetComponent<Image>().color = WorldManager.Instance.cbIconColors[cb_solarsystem.Instance.monoBodies[index].data.bodyType];
    }
}
