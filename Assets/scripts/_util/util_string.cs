using System.Collections.Generic;
using System.Linq;

public class util_string
{   
    public static string[] AddToArray(string[] old, string toAdd)
    {
        string[] result = new string[old.Length + 1];

        for (int i = 0; i < old.Length; i++)
        {
            result[i] = old[i];
        }
        result[result.Length - 1] = toAdd;

        return result;
    }
    public static string RemoveChars(string raw, char[] toRemove)
    {
        string result = "";

        for (int i = 0; i < raw.Length; i++)
        {
            if (!toRemove.Contains(raw[i]))
            {
                result += raw[i];
            }
        }

        return result;
    }
    public static string[] SplitByChar(string raw, char c)
    {
        string current = "";
        List<string> results = new List<string>();

        for (int i = 0; i < raw.Length; i++)
        {
            if (raw[i] == c)
            {
                if (current.Length > 0) {results.Add(current);}
                current = "";
            } else if (i == raw.Length - 1)
            {
                current += raw[i];
                if (current.Length > 0) {results.Add(current);}
            } else
            {
                current += raw[i];
            }
        }

        return results.ToArray();
    }
    
    // uses spaces ' ' as the split character, doesn't include them in the result
    public static string[] SplitIntoWords(string raw)
    {
        return SplitByChar(raw, ' ');
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
