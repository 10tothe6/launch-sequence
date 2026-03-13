using System.Collections.Generic;
using UnityEngine;

public class util_string
{   
    // uses spaces ' ' as the split character, doesn't include them in the result
    public static string[] SplitIntoWords(string raw)
    {
        List<string> results = new List<string>();

        // could just do this with a string, I realized
        // dont care enough to change it
        List<char> currentWord = new List<char>();

        for (int i = 0; i < raw.Length; i++)
        {
            if (raw[i] == ' ')
            {
                results.Add(CharsToString(currentWord.ToArray()));
                currentWord = new List<char>();
            } else
            {
                currentWord.Add(raw[i]);
            }

            if (i == raw.Length - 1)
            {
                results.Add(CharsToString(currentWord.ToArray()));
                currentWord = new List<char>();
            }
        }

        return results.ToArray();
    }

    public static string CharsToString(char[] chars)
    {
        string result = "";

        for (int i = 0; i < chars.Length; i++)
        {
            result+=chars[i];
        }

        return result;
    }

    public static int FindFirstOccurance(string input, char toLookFor)
    {
        return FindOccurance(input, toLookFor, 0);
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

        return -1;
    }
}
