using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ui_rainbow : MonoBehaviour
{
    [Header("pick one")]
    public Image i;
    public TextMeshProUGUI tx;
    private Color currentColor;
    public float speed;

    void Awake()
    {
        currentColor = Color.red;
    }

    void Update()
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(currentColor, out h, out s, out v);

        h += speed * Time.deltaTime;
        currentColor = Color.HSVToRGB(h,s,v);
        if (i != null)
        {
            i.color = currentColor;
        }
        if (tx != null)
        {
            tx.color = currentColor;
        }
    }
}
