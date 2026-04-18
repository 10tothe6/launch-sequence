using TMPro;
using UnityEngine;

public class ui_splashtext : MonoBehaviour
{
    public TextMeshProUGUI tx;
    void Start()
    {
        tx.text = Program.Instance.version;
    }
}
