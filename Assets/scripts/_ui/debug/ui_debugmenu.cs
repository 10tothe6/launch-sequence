using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ui_debugmenu : MonoBehaviour
{
    public List<ui_debugentry> entries;
    public GameObject p_entry;

    public Transform t_entryContainer;

    public float entrySpacing;

    public void AddEntry(string title, UnityAction<string> dataSource)
    {
        AddEntry(new ui_debugentry(title, dataSource));
    }

    public void AddEntry(ui_debugentry entry)
    {
        entries.Add(entry);
        SpawnEntryObject(entry);
    }

    public void RemoveEntryByName(string name)
    {
        for (int i = 0; i < entries.Count;i ++)
        {
            if (entries[i].title == name)
            {
                entries.RemoveAt(i);
                break; // otherwise index math goes wrong
            }
        }
    }

    void Update()
    {
        // temp?
        UpdateAllEntries();
    }

    public void UpdateAllEntries()
    {
        if (t_entryContainer.childCount != entries.Count)
        {
            RefreshEntryList();
        }
    }

    public void RefreshEntryList()
    {
        ui_canvasutils.DestroyChildren(t_entryContainer.gameObject);

        for (int i = 0; i < entries.Count;i++)
        {
            SpawnEntryObject(entries[i],i);
        }
    }

    // assuming last in list
    public void SpawnEntryObject(ui_debugentry entry)
    {
        SpawnEntryObject(entry, entries.Count);
    }
    public void SpawnEntryObject(ui_debugentry entry, int indexInList)
    {
        GameObject g_newEntry = Instantiate(p_entry, t_entryContainer);
        g_newEntry.name = entry.title;

        g_newEntry.transform.localPosition = -Vector3.up * entries.Count * entrySpacing;
    }
}
