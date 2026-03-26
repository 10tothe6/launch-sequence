using TMPro;
using UnityEngine;

// creating this script to avoid a bunch of GetChild() calls

public class ui_planetpoint : MonoBehaviour
{
    public TextMeshProUGUI tx;

    public void Setup(string name)
    {
        tx.text = name;
    }
}
