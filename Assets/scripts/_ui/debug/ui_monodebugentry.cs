using System;
using TMPro;
using UnityEngine;

public class ui_monodebugentry : MonoBehaviour
{
    public TextMeshProUGUI tx;
    public string title;
    public Func<string> data;

    public void Initialize(string title, Func<string> data)
    {
        this.title = title;
        this.data = data;
    }

    public void UpdateData()
    {
        tx.text = title + ":  " + data.Invoke();
    }
}
