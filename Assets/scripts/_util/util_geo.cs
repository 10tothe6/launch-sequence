using System;
using System.Collections.Generic;
using UnityEngine;

// i suspect a lot of geometry-related stuff in the future of this game,
// and i don't want util_math to get full

// so here we are

public class util_geo
{
    /// <summary>
    /// Generates a list of points representing a catenary curve between two 3D points.
    /// </summary>
    /// <param name="p1">Start point</param>
    /// <param name="p2">End point</param>
    /// <param name="slack">Extra length added to the minimum distance (must be > 0)</param>
    /// <param name="segments">Number of points to generate</param>
    public static List<Vector3> GetCatenaryPoints(Vector3 p1, Vector3 p2, float slack, int segments = 20)
    {
        List<Vector3> points = new List<Vector3>();

        // 1. Find the midpoint between the two points
        Vector3 midPoint = (p1 + p2) / 2f;

        // 2. Create a "Control Point" by moving the midpoint straight down
        // We subtract the sag from the Y axis to make it hang
        Vector3 controlPoint = midPoint;
        controlPoint.y -= slack;

        // 3. Sample the Bezier Curve
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            points.Add(CalculateQuadraticBezier(p1, controlPoint, p2, t));
        }

        return points;
    }

    public static List<Vector3> GetLinearPoints(Vector3 p1, Vector3 p2, int segments = 20)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= segments; i ++)
        {
            points.Add(Vector3.Lerp(p1, p2, (float)i / (segments)));
        }

        return points;
    }

    private static Vector3 CalculateQuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        // Standard Quadratic Bezier Formula: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * start;       // (1-t)^2 * P0
        p += 2 * u * t * control;     // 2(1-t)t * P1
        p += tt * end;                // t^2 * P2

        return p;
    }

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
