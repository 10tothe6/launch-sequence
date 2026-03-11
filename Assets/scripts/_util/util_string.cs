using UnityEngine;

public class util_string
{
    public static int FindFirstOccurance(string input, char toLookFor)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == toLookFor)
            {
                return i;
            }
        }

        return -1; // because fuck you
    }

    public static int FindOccurance(string input, char toLookFor, int numSkips)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == toLookFor)
            {
                if (numSkips == 0)
                {
                    return i;
                }
                else
                {
                    numSkips--;
                }
            }
        }

        return -1; // because fuck you
    }
}
