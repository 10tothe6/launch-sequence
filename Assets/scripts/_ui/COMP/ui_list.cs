using UnityEngine;

// I've tried to make a modular list-thing for a while, and this time im getting it right

public class ui_list : MonoBehaviour
{
    public float spaceBetweenItems;
    public bool listUpwards;
    public GameObject p_listElement;

    public Transform t_listContainer;

    public Vector2 GetListDirection()
    {
        return listUpwards ? Vector2.up : Vector2.down;
    }

    public void SetItems(string[] data)
    {
        ClearAllListElements();
        AddItems(data);
    }

    public void AddItem(string data)
    {
        GameObject g_newElement = Instantiate(p_listElement, t_listContainer);
        g_newElement.name = "element " + ( t_listContainer.childCount - 1);

        g_newElement.GetComponent<ui_instantiatable>().SetData(data);
        g_newElement.transform.localPosition = Vector3.zero;
    }

    public void AddItems(string[] data)
    {
        
        float verticalSizeTotal = 0;
        
        for (int i = 0; i < data.Length; i++)
        {
            GameObject g_newElement = Instantiate(p_listElement, t_listContainer);
            g_newElement.name = "element " + i;

            g_newElement.GetComponent<ui_instantiatable>().SetData(data[i]);
            g_newElement.transform.localPosition = Vector2.zero + GetListDirection() * verticalSizeTotal;
            verticalSizeTotal += g_newElement.GetComponent<ui_instantiatable>().effectiveHeight + spaceBetweenItems;
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

    public float GetEffectiveHeight()
    {
        float sum = 0;
        for (int i = 0; i < t_listContainer.childCount; i++)
        {
            sum += t_listContainer.GetChild(i).GetComponent<ui_instantiatable>().effectiveHeight;
        }

        return sum;
    }
}
