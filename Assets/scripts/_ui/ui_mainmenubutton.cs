using TMPro;
using UnityEngine;

public class ui_mainmenubutton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private string rawText;

    void Awake()
    {
        rawText = buttonText.text;
    }

    public void Hover()
    {
        buttonText.text = "<" + rawText + ">";
    }

    public void UnHover()
    {
        buttonText.text = rawText;
    }
}
