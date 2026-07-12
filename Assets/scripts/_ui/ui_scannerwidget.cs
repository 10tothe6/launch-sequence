using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ui_scannerwidget : MonoBehaviour
{
    public TextMeshProUGUI tx_distanceDisplay;
    public TextMeshProUGUI tx_frequencyDisplay;
    public Transform t_scannerParent;
    public Transform[] t_scannerNestPoints;

    [Range(0,1)]
    public float signalStrength;
    [Range(0,1.99f)]
    public float frequency;

    public ui_linerenderer waveform;
    public int waveformPointCount;

    [Header("CONFIG")]
    public float actual_size;
    public float freq;
    public float amp;
    public float time_scale;
    public float frequency_scroll_speed;

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
        tx_frequencyDisplay.text = "f: " + (Mathf.Round(frequency * 100f) / 100f).ToString();

        if (tx_frequencyDisplay.text.Length > 7)
        {
            tx_frequencyDisplay.text = tx_frequencyDisplay.text.Substring(0, 7);
        }
        else if (tx_frequencyDisplay.text.Length < 6)
        {
            tx_frequencyDisplay.text += ".00";
        } else if (tx_frequencyDisplay.text.Length < 7)
        {
            tx_frequencyDisplay.text += 0;
        }

        // here is the actual logic that decides what the signal strength should be
        // ALSO ACCOUNTS FOR ANTENNA PING RANGE
        num_precisevector3[] signalEmitterPositions = EntityManager.GetSignalEmitterPositionsForFrequencyWithinRange(frequency, LocalPlayer.localClient.controllingEntity.data.GetPosition());


        if (signalEmitterPositions.Length > 0)
        {
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

            if (signalStrength > 0.8f)
            {
                tx_distanceDisplay.gameObject.SetActive(true);
                tx_distanceDisplay.text = "dist: " + Math.Round(signalEmitterPositions[kingIndex].Sub(LocalPlayer.localClient.controllingEntity.data.GetPosition()).Mag().AsDouble()) + "m";
            } else
            {
                tx_distanceDisplay.gameObject.SetActive(false);
            }
        } else
        {
            signalStrength = 0;
            tx_distanceDisplay.gameObject.SetActive(false);
        }

        Vector2[] waveform_points = new Vector2[waveformPointCount];

        for (int i = 0; i < waveform_points.Length; i++)
        {
            float offset = actual_size / (waveform_points.Length - 1) * i;
            waveform_points[i] = new Vector2(offset, amp*Mathf.Sin(Time.time*time_scale + offset*freq));

            waveform_points[i] = new Vector2(waveform_points[i].x, Mathf.Lerp(waveform_points[i].y, UnityEngine.Random.Range(-amp, amp), 1-signalStrength));
        }

        rt_strengthBar.sizeDelta = new Vector2(rt_strengthBar.sizeDelta.x, signalStrength * max_bar_scale + UnityEngine.Random.Range(0, bar_jitter) * (1-signalStrength));

        waveform.Draw(waveform_points);

        if (Keyboard.current.leftAltKey.isPressed)
        {
            t_scannerParent.position = t_scannerNestPoints[1].position;

            // scrolling to change the frequency
            frequency += Mathf.Round(Input.scrollWheelAxis * frequency_scroll_speed * 100f) / 100f;
            frequency = Mathf.Clamp(frequency, 0, 1.99f);
        } else
        {
            t_scannerParent.position = t_scannerNestPoints[0].position;
        }
    }
}
