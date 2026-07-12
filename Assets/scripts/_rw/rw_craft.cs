using System.Collections.Generic;
using System.IO;
using UnityEngine;

// reading/writing functions related to spacecraft

public class rw_craft : MonoBehaviour
{
    public static void WriteCraftDataToFile(crft_craftdata data, string filePath)
    {
        // craft data is stored in plain text
        List<string> lines = new List<string>();

        lines.Add(data.craft_name);

        // each part is stored on one line, 
        // without keywords because these files aren't really meant for human eyes other than mine
        for (int i = 0; i < data.parts.Length; i++)
        {
            string partData = "";

            partData += data.parts[i].partName + ",";

            partData += data.parts[i].position.x + ",";
            partData += data.parts[i].position.y + ",";
            partData += data.parts[i].position.z + ",";

            partData += data.parts[i].additional_part_data;

            lines.Add(partData);
        }

        File.WriteAllLines(filePath, lines.ToArray());
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

            // anything LARGER than 4 simply has part data associated with it
            // if its SMALLER than 4 its wrong and will be skipped
            if (elements.Length < 4) {continue;}

            float x = 0;
            float y = 0;
            float z = 0;
            if (!float.TryParse(elements[1], out x)) {continue;}
            if (!float.TryParse(elements[2], out y)) {continue;}
            if (!float.TryParse(elements[3], out z)) {continue;}

            string partData = "";
            if (elements.Length > 4)
            {
                partData = elements[4];
            }

            readParts.Add(new crft_genericpartdata(elements[0], new Vector3(x,y,z), partData));
        }

        result.parts = readParts.ToArray();

        return result;
    }
}
