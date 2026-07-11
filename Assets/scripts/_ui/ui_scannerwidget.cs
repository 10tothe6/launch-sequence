using UnityEngine;

public class ui_scannerwidget : MonoBehaviour
{
    [Range(0,1)]
    public float signalStrength;

    public ui_linerenderer waveform;
    public int waveformPointCount;

    [Header("CONFIG")]
    public float actual_size;
    public float freq;
    public float amp;
    public float time_scale;

    [Space(12)]
    public RectTransform rt_strengthBar;
    public float bar_jitter;
    private float max_bar_scale;

    void Awake()
    {
        max_bar_scale = rt_strengthBar.sizeDelta.y;
    }

    void Update()
    {
        Vector2[] waveform_points = new Vector2[waveformPointCount];

        for (int i = 0; i < waveform_points.Length; i++)
        {
            float offset = actual_size / (waveform_points.Length - 1) * i;
            waveform_points[i] = new Vector2(offset, amp*Mathf.Sin(Time.time*time_scale + offset*freq));

            waveform_points[i] = new Vector2(waveform_points[i].x, Mathf.Lerp(waveform_points[i].y, Random.Range(-amp, amp), 1-signalStrength));
        }

        rt_strengthBar.sizeDelta = new Vector2(rt_strengthBar.sizeDelta.x, signalStrength * max_bar_scale + Random.Range(0, bar_jitter) * (1-signalStrength));

        waveform.Draw(waveform_points);
    }
}
