using UnityEngine;

// personally? I hate the name for this script
// but it really doesn't matter

public class cbr_applyatmosphere : MonoBehaviour
{
    public Material m;

    void Start()
    {
        UpdateShaderOnce();
    }

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        UpdateShaderPeriodic(); 

        Graphics.Blit(source, mod, m);
    }

    void UpdateShaderOnce()
    {
        
    }

    void UpdateShaderPeriodic()
    {
        // TODO: set the shader variables
    }
}
