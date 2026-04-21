using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemList 
{
    public List<int> ids;

    public ItemList() {
        ids = new List<int>();
    }

    public ItemList(List<int> ids) {
        this.ids = ids;
    }

    public ItemList(int count) {
        ids = new List<int>();

        for (int i = 0; i < count; i++) {
            ids.Add(-1);
        }
    }
}
