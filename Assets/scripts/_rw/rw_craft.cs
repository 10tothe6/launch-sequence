using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class rw_craft : MonoBehaviour
{
    public static void WriteCraftDataToFile(crft_craftdata data, string filePath)
    {
        // craft data is stored in plain text
        
    }


    public static crft_craftdata ReadCraftDataFromFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        crft_craftdata result = new crft_craftdata();
        result.craft_name = lines[0];

        List<crft_genericpartdata> readParts = new List<crft_genericpartdata>();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] elements = util_string.SplitByChar(lines[i], ',');

            if (elements.Length != 4) {continue;}

            float x = 0;
            float y = 0;
            float z = 0;
            if (!float.TryParse(elements[1], out x)) {continue;}
            if (!float.TryParse(elements[2], out y)) {continue;}
            if (!float.TryParse(elements[3], out z)) {continue;}

            readParts.Add(new crft_genericpartdata(elements[0], new Vector3(x,y,z)));
        }

        result.parts = readParts.ToArray();

        return result;
    }
}
