using TMPro;
using UnityEngine;

public class ui_stringdisplay : MonoBehaviour
{
    public TextMeshProUGUI tx;

    public void Display(string str)
    {
        tx.text = str;
    }

    public void SetColor(Color col)
    {
        tx.color = col;
    }
}
