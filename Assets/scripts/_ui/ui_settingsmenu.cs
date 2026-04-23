using UnityEngine;

public class ui_settingsmenu : MonoBehaviour
{
    public void EnterMenu()
    {
        GetComponent<ui_modularmenu>().DrawMenu(Settings.GetModularEntries());
    }
}
