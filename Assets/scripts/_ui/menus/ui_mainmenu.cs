using System.Collections;
using UnityEngine;

public class ui_mainmenu : MonoBehaviour
{

    public GameObject g_normalButtonContainer;
    public GameObject[] g_normalButtons;
    public GameObject g_devButtons;


    public GameObject g_foregroundObjectContainer;

    public void LoadMainMenu()
    {
        if (Settings.GetBool("dev_mode"))
        {
            AudioManager.Instance.PlayMusic(0);

            g_devButtons.SetActive(true);
            g_normalButtonContainer.SetActive(false);
            g_foregroundObjectContainer.SetActive(false);

        } else
        {

            g_devButtons.SetActive(false);


            g_foregroundObjectContainer.SetActive(true);
            AudioManager.Instance.PlayMusic(1);
            g_normalButtonContainer.SetActive(true);
            for (int i = 0; i < g_normalButtons.Length; i++) {g_normalButtons[i].SetActive(false);}


            UIManager.Instance.FadeOutTransitionAndThen(() =>
            {
                StartCoroutine(AnimateInButtons());
            });
            
        }
    }

    // spawns in the button objects one by one
    private IEnumerator AnimateInButtons()
    {
        for (int i = 0; i < g_normalButtons.Length; i++)
        {
            g_normalButtons[i].SetActive(true);
            // AudioManager.Instance.PlayStaticSound(0);

            yield return new WaitForSeconds(0.2f);
        }
    }
}
