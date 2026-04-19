using UnityEngine;

// the utility class for oddball things that I don't know where they go

public class util_misc
{
    public static Color RainbowColor(float speed)
    {
        float h = Time.time * speed;
        h -= Mathf.Floor(h);
        float s = 1;
        float v = 1;

        return Color.HSVToRGB(h, s, v);
    }
}
