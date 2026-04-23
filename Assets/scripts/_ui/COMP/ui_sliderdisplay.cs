using UnityEngine;
using UnityEngine.UI;

public class ui_sliderdisplay : MonoBehaviour
{
    public Slider sComp;
    public ui_instantiatable iComp;
    public float min;
    public float max;

    public void Setup(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public void SetupFromDisplayData()
    {
        string[] split = util_string.SplitByChar(iComp.displayInfo,',');

        min = float.Parse(split[0]);
        max = float.Parse(split[1]);
    }

    void Awake()
    {
        sComp.onValueChanged.AddListener((x) => {iComp.onDataUpdate.Invoke((min + x*(max-min)).ToString());});
        iComp.onDataUpdate.AddListener((x) => SetupFromDisplayData());
    }
}
