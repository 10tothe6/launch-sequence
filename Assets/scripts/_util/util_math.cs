using System;
using UnityEngine;

public class util_math
{
    public static double RoundToInterval(double raw, double interval)
    {
        return Math.Round(raw / interval) * interval;
    }
    public static float RoundToInterval(float raw, float interval)
    {
        return Mathf.Round(raw / interval) * interval;
    }
    public static bool DiceRoll(float percentChance)
    {
        return UnityEngine.Random.Range(0f,1000f) < 1000f * percentChance;
    }
    public static int GetRandomInt()
    {
        return UnityEngine.Random.Range(0,int.MaxValue);
    }

    public static Vector3 RotateVector(Vector3 vector, Vector3 axis, float angle) {
        Vector3 rotated = new Vector3(0,0,0);
        rotated.x = vector.x * (     (axis.x * axis.x) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                  ) +   vector.y * (        (axis.y * axis.x) * (1 - Mathf.Cos(angle)) - (axis.z * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.x) * (1 - Mathf.Cos(angle)) + (axis.y * Mathf.Sin(angle))     );
        rotated.y = vector.x * (     (axis.x * axis.y) * (1 - Mathf.Cos(angle)) + (axis.z * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.y) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                    ) + vector.z * (        (axis.z * axis.y) * (1 - Mathf.Cos(angle)) - (axis.x * Mathf.Sin(angle))     );
        rotated.z = vector.x * (     (axis.x * axis.z) * (1 - Mathf.Cos(angle)) - (axis.y * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.z) * (1 - Mathf.Cos(angle)) + (axis.x * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.z) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                );

        return rotated;
    }

    //a function that intersects a ray with a sphere
    //CREDIT: Sebastian Lague [Coding Adventures: Atmosphere]
    // returns Vector2(dist to, dist through)
    public static Vector2 RaySphere(Vector3 sphereCentre, float sphereRadius, Vector3 rayOrigin, Vector3 rayDir) {
        Vector3 offset = rayOrigin - sphereCentre;
        float a = 1;
        float b = 2 * Vector3.Dot(offset, rayDir);
        float c = Vector3.Dot(offset, offset) - sphereRadius * sphereRadius;
        float d = b * b - 4 * a * c;

        if (d > 0) {
            float s = Mathf.Sqrt(d);
            float dstToSphereNear = Mathf.Max(0, (-b - s) / (2 * a));
            float dstToSphereFar = (-b + s) / (2 * a);

            if (dstToSphereFar >= 0) {
                return new Vector2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
            }
        }

        return new Vector2(-1, 0);
    }
}
