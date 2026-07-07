using System;
using UnityEngine;

public class audio_radiofilter : MonoBehaviour
{
    // Example of applying a simple mathematical filter to PCM audio samples
    public void ApplyRadioFilter(float[] buffer, int sampleRate)
    {
        // High-Pass implementation to cut low frequencies (e.g., < 300Hz)
        float rcHigh = 1.0f / (2.0f * 3.14159f * 300.0f * (1.0f / sampleRate));
        float alphaHigh = rcHigh / (rcHigh + (1.0f / sampleRate));
        
        // Low-Pass implementation to cut high frequencies (e.g., > 3000Hz)
        float rcLow = 1.0f / (2.0f * 3.14159f * 3000.0f * (1.0f / sampleRate));
        float alphaLow = (1.0f / sampleRate) / (rcLow + (1.0f / sampleRate));

        float prevOutHigh = 0f;
        float prevOutLow = 0f;

        for (int i = 0; i < buffer.Length; i++)
        {
            // High-Pass step
            float highPassOut = alphaHigh * (prevOutHigh + buffer[i] - prevOutHigh); // Simplified RC filter approx
            prevOutHigh = highPassOut;

            // Low-Pass step
            float lowPassOut = prevOutLow + alphaLow * (highPassOut - prevOutLow);
            prevOutLow = lowPassOut;

            // Apply slight distortion/clipping
            buffer[i] = Math.Clamp(lowPassOut * 1.5f, -1f, 1f);
        }
    }
}