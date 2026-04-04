using UnityEngine;

public class util_math
{
    public static float RoundToInterval(float raw, float interval)
    {
        return Mathf.Round(raw / interval) * interval;
    }
    public static bool DiceRoll(float percentChance)
    {
        return Random.Range(0f,1000f) < 1000f * percentChance;
    }
    public static int GetRandomInt()
    {
        return Random.Range(0,int.MaxValue);
    }

    public static Vector3 RotateVector(Vector3 vector, Vector3 axis, float angle) {
        Vector3 rotated = new Vector3(0,0,0);
        rotated.x = vector.x * (     (axis.x * axis.x) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                  ) +   vector.y * (        (axis.y * axis.x) * (1 - Mathf.Cos(angle)) - (axis.z * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.x) * (1 - Mathf.Cos(angle)) + (axis.y * Mathf.Sin(angle))     );
        rotated.y = vector.x * (     (axis.x * axis.y) * (1 - Mathf.Cos(angle)) + (axis.z * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.y) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                    ) + vector.z * (        (axis.z * axis.y) * (1 - Mathf.Cos(angle)) - (axis.x * Mathf.Sin(angle))     );
        rotated.z = vector.x * (     (axis.x * axis.z) * (1 - Mathf.Cos(angle)) - (axis.y * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.z) * (1 - Mathf.Cos(angle)) + (axis.x * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.z) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                );

        return rotated;
    }
}
