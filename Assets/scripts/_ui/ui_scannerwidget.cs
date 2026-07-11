using UnityEngine;
using UnityEngine.InputSystem;

public class ui_scannerwidget : MonoBehaviour
{

    public Transform t_scannerParent;
    public Transform[] t_scannerNestPoints;

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
        // here is the actual logic that decides what the signal strength should be
        num_precisevector3[] signalEmitterPositions = new num_precisevector3[] {new num_precisevector3(Vector3.zero)};


        // find the signal emitter that has the smallest angle (from where the player is looking)
        int kingIndex = 0;
        float kingAngle = Vector3.Angle(CameraController.t_cam.forward, signalEmitterPositions[0].Sub(LocalPlayer.localClient.controllingEntity.data.GetPosition()).ToVector3());
        for (int i = 1; i < signalEmitterPositions.Length; i++)
        {
            float theta = Vector3.Angle(CameraController.t_cam.forward, signalEmitterPositions[i].Sub(LocalPlayer.localClient.controllingEntity.data.GetPosition()).ToVector3());
            if (theta < kingAngle)
            {
                kingIndex = i;
                kingAngle = theta;
            }
        }

        signalStrength = Mathf.Cos(kingAngle * Mathf.PI / 180);

        Vector2[] waveform_points = new Vector2[waveformPointCount];

        for (int i = 0; i < waveform_points.Length; i++)
        {
            float offset = actual_size / (waveform_points.Length - 1) * i;
            waveform_points[i] = new Vector2(offset, amp*Mathf.Sin(Time.time*time_scale + offset*freq));

            waveform_points[i] = new Vector2(waveform_points[i].x, Mathf.Lerp(waveform_points[i].y, Random.Range(-amp, amp), 1-signalStrength));
        }

        rt_strengthBar.sizeDelta = new Vector2(rt_strengthBar.sizeDelta.x, signalStrength * max_bar_scale + Random.Range(0, bar_jitter) * (1-signalStrength));

        waveform.Draw(waveform_points);

        if (Keyboard.current.leftAltKey.isPressed)
        {
            t_scannerParent.position = t_scannerNestPoints[1].position;
        } else
        {
            t_scannerParent.position = t_scannerNestPoints[0].position;
        }
    }
}
