using UnityEngine;

// I've tried to make a modular list-thing for a while, and this time im getting it right

public class ui_list : MonoBehaviour
{
    public GameObject p_listElement;

    public Transform t_listContainer;

    public void DisplayList(string[] data)
    {
        ClearAllListElements(); // god im lazy

        for (int i = 0; i < data.Length; i++)
        {
            GameObject g_newElement = Instantiate(p_listElement, t_listContainer);
            g_newElement.name = "element " + i;

            g_newElement.GetComponent<ui_instantiatable>().SetData(data[i]);
        }
    }

    // removes all objects currently a part of the list
    public void ClearAllListElements()
    {
        for (int i = t_listContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(t_listContainer.GetChild(i).gameObject);
        }
    }
}
