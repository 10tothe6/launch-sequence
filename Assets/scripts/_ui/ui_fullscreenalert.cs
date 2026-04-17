using UnityEngine;
using UnityEngine.UI;

public class ui_fullscreenalert : MonoBehaviour
{
    public Button b_close;

    void Awake()
    {
        b_close.onClick.AddListener(ui_infoalerts.Instance.RemoveMostRecentFullscreenAlert);
    }
}
