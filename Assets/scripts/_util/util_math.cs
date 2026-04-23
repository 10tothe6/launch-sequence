using System;
using System.Numerics;
using UnityEngine;

public class util_math
{

    public static float ExpandToRange(float val, float min, float max)
    {
        return Mathf.Lerp(min, max, val);
    }
    public static float ProjectedMagnitude(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
    {
        UnityEngine.Vector3 p = UnityEngine.Vector3.Project(a,b);
        if (UnityEngine.Vector3.Dot(b, a) < 0)
        {
            return -p.magnitude;
        } else
        {
            return p.magnitude;
        }
    }
    public static BigInteger Sqrt(BigInteger n)
    {
        if (n == 0) {return n;}
        if (n > 0)
        {
            int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n,2)));
            BigInteger root = BigInteger.One << (bitLength / 2);
            
            while (!IsSqrt(n, root))
            {
                root += n / root;
                root /= 2;
            }
            
            return root;
        }

        throw new ArithmeticException("NaN");
    }
    public static bool IsSqrt(BigInteger n, BigInteger root)
    {
        BigInteger lowerBound = root *root;
        BigInteger upperBound = (root + 1) * (root + 1);

        return  (n >= lowerBound && n < upperBound);
    }
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

    public static UnityEngine.Vector3 RotateVector(UnityEngine.Vector3 vector, UnityEngine.Vector3 axis, float angle) {
        UnityEngine.Vector3 rotated = new UnityEngine.Vector3(0,0,0);
        rotated.x = vector.x * (     (axis.x * axis.x) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                  ) +   vector.y * (        (axis.y * axis.x) * (1 - Mathf.Cos(angle)) - (axis.z * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.x) * (1 - Mathf.Cos(angle)) + (axis.y * Mathf.Sin(angle))     );
        rotated.y = vector.x * (     (axis.x * axis.y) * (1 - Mathf.Cos(angle)) + (axis.z * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.y) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                    ) + vector.z * (        (axis.z * axis.y) * (1 - Mathf.Cos(angle)) - (axis.x * Mathf.Sin(angle))     );
        rotated.z = vector.x * (     (axis.x * axis.z) * (1 - Mathf.Cos(angle)) - (axis.y * Mathf.Sin(angle))       ) +   vector.y * (        (axis.y * axis.z) * (1 - Mathf.Cos(angle)) + (axis.x * Mathf.Sin(angle))         ) + vector.z * (        (axis.z * axis.z) * (1 - Mathf.Cos(angle)) + Mathf.Cos(angle)                );

        return rotated;
    }

    //a function that intersects a ray with a sphere
    //CREDIT: Sebastian Lague [Coding Adventures: Atmosphere]
    // returns UnityEngine.Vector2(dist to, dist through)
    public static UnityEngine.Vector2 RaySphere(UnityEngine.Vector3 sphereCentre, float sphereRadius, UnityEngine.Vector3 rayOrigin, UnityEngine.Vector3 rayDir) {
        UnityEngine.Vector3 offset = rayOrigin - sphereCentre;
        float a = 1;
        float b = 2 * UnityEngine.Vector3.Dot(offset, rayDir);
        float c = UnityEngine.Vector3.Dot(offset, offset) - sphereRadius * sphereRadius;
        float d = b * b - 4 * a * c;

        if (d > 0) {
            float s = Mathf.Sqrt(d);
            float dstToSphereNear = Mathf.Max(0, (-b - s) / (2 * a));
            float dstToSphereFar = (-b + s) / (2 * a);

            if (dstToSphereFar >= 0) {
                return new UnityEngine.Vector2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
            }
        }

        return new UnityEngine.Vector2(-1, 0);
    }
}
