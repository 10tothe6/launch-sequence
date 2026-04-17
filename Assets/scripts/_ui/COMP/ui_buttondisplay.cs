using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ui_buttondisplay : MonoBehaviour
{
    public Button b;

    public void AddToOnClick(UnityAction action)
    {
        b.onClick.AddListener(action);
    }
}
