using UnityEngine;

// i suspect a lot of geometry-related stuff in the future of this game,
// and i don't want util_math to get full

// so here we are

public class util_geo
{
    // both directions must be normalized, obviously
    public static Vector3[] GenerateCirclePoints(Vector3 upDir, Vector3 rightDir, int pointCount, float radius)
    {
        Vector3[] result = new Vector3[pointCount];

        for (int i = 0; i < result.Length; i++)
        {
            float theta = Mathf.PI * 2 / (result.Length-1) * (float)i;

            result[i] = rightDir * radius * Mathf.Cos(theta) + upDir * radius * Mathf.Sin(theta);
        }

        return result;
    }
}
