using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ui_rainbow : MonoBehaviour
{
    [Header("pick one")]
    public Image i;
    public TextMeshProUGUI tx;
    public float speed;

    void Update()
    {
        if (i != null)
        {
            i.color = util_misc.RainbowColor(speed);
        }
        if (tx != null)
        {
            tx.color = util_misc.RainbowColor(speed);
        }
    }
}
