using UnityEngine;

public class util_inventory
{
    public static Vector2 ApplyRotation(Vector2 raw, int rotationIndex)
    {
        if (rotationIndex == 0)
        {
            return raw;
        } else if (rotationIndex == 1)
        {
            return new Vector2(-raw.y, raw.x);
        } else if (rotationIndex == 2)
        {
            return new Vector2(-raw.x, -raw.y);
        } else if (rotationIndex == 3)
        {
            return new Vector2(raw.y, -raw.x);
        }

        return Vector2.zero; // should never get here
    }
}
