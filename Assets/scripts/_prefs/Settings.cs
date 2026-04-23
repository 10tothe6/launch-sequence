using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// TODO: figure out how to group settings by category

public class Settings : MonoBehaviour
{
    private static Settings _instance;

    public static Settings Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
        settings = ins_settings; // this sets the parameters of the settings, we'll have to read the file to get the actual values

        // just in case
        for (int i = 0; i < settings.Count;i++)
        {
            settings[i].isFilled = false;
        }
    }

    void Start()
    {
        ReadSettingsFile();
    }

    public static string emptyString = "err";
    public static double emptyDouble = -999d;
    public static float emptyFloat = -999f;

    // to be edited by the user
    public  List<prefs_genericentry> ins_settings;
    // realistically, this will never be accessed directly
    public static List<prefs_genericentry> settings;


    // annoying conversion, but the modular menu system does need to be standardized
    public static ui_modularmenuentry[] GetModularEntries()
    {
        List<ui_modularmenuentry> result = new List<ui_modularmenuentry>();

        for (int i = 0; i < settings.Count; i++)
        {
            ui_modularmenuentry newEntry = new ui_modularmenuentry();

            int j = i;
            newEntry.data = settings[j].value;
            newEntry.onDataUpdate.AddListener((x) => {settings[j].value = x;});
            newEntry.displayInfo = ""; // TODO: display info
            new

            result.Add(newEntry);
        }

        return result.ToArray();
    }

    // this part works similarly to the OLD WPILib communications protocol
    // ************************************
    
    public static string GetString(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return emptyString;}

        return entry.value;
    }


    public static double GetDouble(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return emptyDouble;}

        double parsedValue = 0;
        if (double.TryParse(entry.value,out parsedValue))
        {
            return parsedValue;
        }
        return emptyDouble;
    }
    public static float GetFloat(string key)
    {
        prefs_genericentry entry = Instance.GetEntryByName(key);
        if (entry == null) {return emptyFloat;}

        float parsedValue = 0;
        if (float.TryParse(entry.value,out parsedValue))
        {
            return parsedValue;
        }
        return emptyFloat;
    }
    //***************************************

    public prefs_genericentry GetEntryByName(string key)
    {
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].key == key)
            {
                return settings[i];
            }
        }

        return null;
    }

    public void WriteSettingsFile()
    {
        
    }

    // unlike before, this is read line-by-line
    // we don't actually need to pass in a file path, because we know where the file will be
    public void ReadSettingsFile()
    {
        string actualFilePath = "";
        string theoreticalFilePath = util_file.GetWorkingDirectory() + "user.settings";

        if (File.Exists(theoreticalFilePath))
        {
            // oh cool we found the file, read it
            actualFilePath = theoreticalFilePath;
        } else
        {
            // ah well we didn't find the file
            // now we look in the previous version
            string backupPath = util_file.GetRawWorkingDirectory() + Program.Instance.GetPreviousVersion() + "/user.settings";

            if (File.Exists(backupPath)) {actualFilePath = backupPath;}
        }

        if (actualFilePath.Length > 0)
        {
            // somewhere we found a file, so let's read what we can
            string[] lines = File.ReadLines(actualFilePath).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                // keep in mind any accidental spaces will be read too, causing potential issues
                // I'm not dealing with them
                // if the user edits the settings file they better be careful
                string[] elements = util_string.SplitByChar(lines[i],':');

                // first element is always the key
                prefs_genericentry entry = GetEntryByName(elements[0]);
                if (entry == null) {continue;} // that key doesn't match anything

                if (entry.IsValidValue(elements[1]))
                {
                    entry.value = elements[1]; 
                    entry.isFilled = true;
                }
            }
        }

        // here we fill in any data that wasn't found in the file
        for (int i = 0; i < settings.Count; i++)
        {
            if (!settings[i].isFilled)
            {
                settings[i].value = settings[i].defaultValue;
            }
        }

        // we're not going to bother with writing anything to disk, that's another function's job
    }
    void OnApplicationQuit()
    {
        Settings.Instance.WriteToSettingsFile();
    }

    public void WriteToSettingsFile()
    {
        if (!Directory.Exists(util_file.GetWorkingDirectory())) {Directory.CreateDirectory(util_file.GetWorkingDirectory());}
        string filePath = util_file.GetWorkingDirectory() + "user.settings";

        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        List<string> lines = new List<string>();

        for (int i = 0; i < settings.Count; i++)
        {
            lines.Add(settings[i].key + ":" + settings[i].value);
        }

        File.WriteAllLines(filePath, lines.ToArray());
    }
}
