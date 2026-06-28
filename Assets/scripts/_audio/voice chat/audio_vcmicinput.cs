using MetaVoiceChat.Input.Mic;
using UnityEngine;

public class audio_vcmicinput : MonoBehaviour
{
    public VcMicAudioInput input;

    void Update()
    {
        if (input.IsInitialized)
        {
            Input.micPeakValue = LevelMax();
        }
    }

    float LevelMax()
    {
        int _sampleWindow = 128;

        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition (Microphone.devices[1]) - (_sampleWindow + 1);
        if (micPosition < 0) {
            return 0;
        }
        input.Mic.AudioClip.GetData (waveData, micPosition);
        for (int i = 0; i < _sampleWindow; ++i) {
            float wavePeak = waveData [i] * waveData [i];
            if (levelMax < wavePeak) {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }
}
