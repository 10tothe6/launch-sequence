using UnityEngine;

public class util_game
{
    public static string FormatDistance(num_precise distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters.raw < new num_precise(10000).raw)
        {
            // meters
            return util_math.RoundToInterval(distInMeters.AsDouble(), 0.1f).ToString() + "m";
        } else if (distInMeters.raw < new num_precise(10000000).raw)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters.AsDouble() / 1000f, 0.1f).ToString() + "km";
        } else if (distInMeters.raw < new num_precise(new System.Numerics.BigInteger(10000000000 * 10000)).raw)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters.AsDouble() / 1000000f, 0.1f).ToString() + "Mm";
        } else if (distInMeters.raw < new num_precise(new System.Numerics.BigInteger(10000000000000 * 10000)).raw)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters.AsDouble() / 1000000000f, 0.1f).ToString() + "Gm";
        }

        // should never get here
        return "err";
    }

    public static string FormatDistance(double distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters < 10000)
        {
            // meters
            return util_math.RoundToInterval(distInMeters, 0.01f) + "m";
        } else if (distInMeters < 10000000)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters / 1000f, 0.01f) + "km";
        } else if (distInMeters < 10000000000)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters / 1000000f, 0.01f) + "Mm";
        } else if (distInMeters < 10000000000000)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters / 1000000000f, 0.01f) + "Gm";
        }

        // should never get here
        return "err";
    }
    public static string FormatDistance(float distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters < 10000)
        {
            // meters
            return util_math.RoundToInterval(distInMeters, 0.1f) + "m";
        } else if (distInMeters < 10000000)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters / 1000f, 0.1f) + "km";
        } else if (distInMeters < 10000000000)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters / 1000000f, 0.1f) + "Mm";
        } else if (distInMeters < 10000000000000)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters / 1000000000f, 0.1f) + "Gm";
        }

        // should never get here
        return "err";
    }

    public static string FormatRawDistance(num_precise distInMeters)
    {
        return FormatDistance(distInMeters.Mul(1f / WorldData.universalScaleFactor));
    }

    public static string FormatRawDistance(float distInMeters)
    {
        return FormatDistance(distInMeters / WorldData.universalScaleFactor);
    }

    public static string FormatRawDistance(double distInMeters)
    {
        return FormatDistance(distInMeters / WorldData.universalScaleFactor);
    }
}
