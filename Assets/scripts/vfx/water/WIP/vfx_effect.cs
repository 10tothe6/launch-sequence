using UnityEngine;

// generic script for applying post-processing materials to a camera
// "pst_" stands for "post"

public class pst_effect : MonoBehaviour
{
    // the material to apply to the camera's output texture
    [Header("(applies from top to bottom)")]
    public Material m_effect;

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        Graphics.Blit(source, mod, m_effect);
    }
}
