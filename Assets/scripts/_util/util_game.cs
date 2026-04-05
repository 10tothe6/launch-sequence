using UnityEngine;

public class util_game
{
    public static string FormatDistance(double distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters < 10000)
        {
            // meters
            return util_math.RoundToInterval(distInMeters, 0.1f).ToString() + "m";
        } else if (distInMeters < 10000000)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters / 1000f, 0.1f).ToString() + "km";
        } else if (distInMeters < 10000000000)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters / 1000000f, 0.1f).ToString() + "Mm";
        } else if (distInMeters < 10000000000000)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters / 1000000000f, 0.1f).ToString() + "Gm";
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
            return util_math.RoundToInterval(distInMeters, 0.1f).ToString() + "m";
        } else if (distInMeters < 10000000)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters / 1000f, 0.1f).ToString() + "km";
        } else if (distInMeters < 10000000000)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters / 1000000f, 0.1f).ToString() + "Mm";
        } else if (distInMeters < 10000000000000)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters / 1000000000f, 0.1f).ToString() + "Gm";
        }

        // should never get here
        return "err";
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
