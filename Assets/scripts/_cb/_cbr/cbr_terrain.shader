Shader "cbr/terrain" {
Properties {
    _MainTex ("Biome Map", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    CGPROGRAM
    #pragma surface surf SimpleLambert
    
    float3 sunPosition; // always (0,0,0) for now
    int isStar;
    float3 position;

    // Lighting for the terrain (custom so that the sun direction can change)
    half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
        if (isStar == 1) {
            return 1;
        }
        half NdotL = saturate(dot(s.Normal, normalize(sunPosition - position)));
        
        half4 c;
        c.rgb = s.Albedo * (NdotL);
        c.a = 1;
        return c;
    }
  
    struct Input {
        float3 worldPos;
        float4 screenPos;
        float2 uv_MainTex;
    };
        
    sampler2D _MainTex;
    fixed4 _Color;

    void surf (Input IN, inout SurfaceOutput o) {
        o.Albedo = _Color;
    }
    ENDCG
    }
    Fallback "Diffuse"
}