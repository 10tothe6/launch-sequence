using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public int id;
    public int count;

    // THIS REPLACES THE CONTAINED ITEMS FEATURE
    public List<string> keys;
    public List<string> values;

    public string GetName()
    {
        if (keys.Contains("custom name"))
        {
            return values[keys.IndexOf("custom name")];
        }
        else
        {
            return WorldData.Instance.items[id].name;
        }
    }

    public ItemStack(string name, int count)
    {
        int index = -1;
        for (int i = 0; i < WorldData.Instance.items.Length; i++)
        {
            if (WorldData.Instance.items[i].name == name)
            {
                index = i;
                break;
            }
        }
        if (index == -1) return;

        this.id = index;
        this.count = count;

        this.keys = new List<string>();
        this.values = new List<string>();
    }

    public ItemStack(int id, int count)
    {
        this.id = id;
        this.count = count;

        this.keys = new List<string>();
        this.values = new List<string>();
    }

    // the full constructor, nothing assumed/empty
    public ItemStack(int id, int count, List<string> keys, List<string> values)
    {
        this.id = id;
        this.count = count;

        this.keys = keys;
        this.values = values;
    }

    public string GetValueAt(string key)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
            {
                return values[i];
            }
        }

        return "";
    }

    public void SetValueAt(string key, string newValue)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
            {
                values[i] = newValue;
                return;
            }
        }

        keys.Add(key);
        values.Add(newValue);
    }

    public Item GetClass()
    {
        return WorldData.Instance.items[id];
    }
}
