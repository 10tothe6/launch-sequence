using UnityEngine;

public class vfx_cameraglitch : MonoBehaviour
{
    [Header("SHADER VAR.S")]
    // no need for any of these variables to be public


    [SerializeField]
    private float chance_for_dead_pixel = 0.01f;
    [SerializeField]
    private float chance_for_random_pixel = 0.01f;


    // areas of the screen that have more dead and random pixels
    [SerializeField]
    private float chance_for_dead_zone = 0.15f;


    [SerializeField]
    private int screen_resolution_x = 1920;
    [SerializeField]
    private int screen_resolution_y = 1080;

    // a multiplier, applied to the built-in _Time shader variable
    [SerializeField]
    private float noise_scroll_speed = 0.5f;



    [SerializeField]
    private float uv_offset_speed = 0.5f;
    [SerializeField]
    private float max_uv_offset = 0.5f;




    // the material to apply to the camera's output texture
    [Header("(applies from top to bottom)")]
    public Material m_effect;

    public void SetBadQuality()
    {
        screen_resolution_x = 1920/8;
        screen_resolution_y = 1080/8;

        max_uv_offset = 0.025f;
        uv_offset_speed = 0.05f;
    
        chance_for_dead_pixel = 0.025f;
        chance_for_random_pixel = 0.025f;

        noise_scroll_speed = 0.75f;
    }
    public void SetMediumQuality()
    {
        screen_resolution_x = 1920/4;
        screen_resolution_y = 1080/4;

        max_uv_offset = 0.01f;
        uv_offset_speed = 0.1f;

        chance_for_dead_pixel = 0.015f;
        chance_for_random_pixel = 0.015f;

        noise_scroll_speed = 0.5f;
    }
    public void SetGoodQuality()
    {
        screen_resolution_x = 1920/2;
        screen_resolution_y = 1080/2;
        
        max_uv_offset = 0f;
        uv_offset_speed = 0f;

        chance_for_dead_pixel = 0.01f;
        chance_for_random_pixel = 0.01f;

        noise_scroll_speed = 0.25f;
    }
    public void SetFullQuality()
    {
        screen_resolution_x = 1920;
        screen_resolution_y = 1080;

        max_uv_offset = 0f;
        uv_offset_speed = 0f;

        chance_for_dead_pixel = 0f;
        chance_for_random_pixel = 0f;

        noise_scroll_speed = 0f;
    }

    void OnRenderImage(RenderTexture source, RenderTexture mod)
    {
        // updating all of the glitch shader vars
        m_effect.SetFloat("chance_for_dead_pixel", chance_for_dead_pixel);
        m_effect.SetFloat("chance_for_random_pixel", chance_for_random_pixel);
        m_effect.SetFloat("chance_for_dead_zone", chance_for_dead_zone);

        m_effect.SetInt("resolution_x", screen_resolution_x);
        m_effect.SetInt("resolution_y", screen_resolution_y);

        m_effect.SetFloat("noise_speed", noise_scroll_speed);

        m_effect.SetFloat("max_uv_offset", max_uv_offset);
        m_effect.SetFloat("uv_offset_speed", uv_offset_speed);


        Graphics.Blit(source, mod, m_effect);
    }
}
