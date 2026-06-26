using UnityEngine;

public class ui_conditional : MonoBehaviour
{
    public ui_instantiatable a;
    public ui_instantiatable b;

    public bool checkOnAwake = true;

    void Awake()
    {
        a.onDataUpdate.AddListener((x) => CheckConditional());
        b.onDataUpdate.AddListener((x) => CheckConditional());

        if (checkOnAwake)
        {
            CheckConditional();
        }
    }

    public void CheckConditional()
    {
        if (a.heldData == b.heldData)
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }
    }
}
