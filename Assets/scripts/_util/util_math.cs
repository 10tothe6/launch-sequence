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
}
