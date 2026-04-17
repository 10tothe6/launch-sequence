Shader "cbr/terrain" {
Properties {
    _MainTex ("Planet Map", 2D) = "white" {}
    _NormalTex ("Normal Map", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    CGPROGRAM
    #pragma surface surf SimpleLambert
    
    float3 sunPosition; // always (0,0,0) for now
    int isStar;
    float3 position;
    float3 worldPosition;

    struct SurfaceOutputCustom {
        fixed3 Albedo;
        fixed3 Normal;
        fixed3 Emission;
        half Specular;
        fixed Gloss;
        fixed Alpha;

        float3 worldPos; // thank you chatgpt
    };


    // Lighting for the terrain (custom so that the sun direction can change)
    half4 LightingSimpleLambert (SurfaceOutputCustom s, half3 lightDir, half atten) {
        if (isStar == 1) {
            return 1;
        }
        
        half dotSphere = saturate(dot(normalize(s.worldPos - worldPosition), normalize(sunPosition - position)));
        half dotTerrain = saturate(dot(s.Normal, normalize(sunPosition - position)));
        
        half4 c;
        c.rgb = s.Albedo * dotSphere * dotTerrain;
        c.a = 1;
        return c;
    }
  
    struct Input {
        float3 worldPos;
        float4 screenPos;
        float2 uv_MainTex;
    };
        
    sampler2D _MainTex;
    sampler2D _NormalTex;
    
    fixed4 _Color;

    void surf (Input IN, inout SurfaceOutputCustom  o) {
        o.Albedo = tex2D(_MainTex, IN.uv_MainTex.xy * 10) * _Color;
        o.worldPos = IN.worldPos;
    }
    ENDCG
    }
    Fallback "Diffuse"
}