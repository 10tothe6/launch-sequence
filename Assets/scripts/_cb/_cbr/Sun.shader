Shader "Custom/Sun" {
Properties {
    _MainTex ("Albedo", 2D) = "white" {}
    _GlobalAmount ("Global Lighting Amount", Float) = 0.5
    _Strength ("Light Strength", Float) = 0.5
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    CGPROGRAM
    #pragma surface surf SimpleLambert
    
    half3 viewDir;
    half3 sunPos;

    float _GlobalAmount;
    float _Strength;

    sampler2D _MainTex;

    // Lighting for the terrain (custom so that the sun direction can change)
    half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
        //half NdotL = (dot(s.Normal, viewDir) + 1) / 2;
        half4 c;
        
        // float nh = max(0, dot(s.Normal, normalize(sunPosition - position)));
        // float spec = pow (nh, 12) * 0.6;

        c.rgb = s.Albedo;
        c.a = 1;
        return c;
    }
  
    struct Input {
        float3 worldPos;
        float2 uv_MainTex;
    };
        
    void surf (Input IN, inout SurfaceOutput o) {
        half NdotL = (dot(normalize(sunPos - IN.worldPos), viewDir) + 1) / 2;
        o.Albedo = float3(1,1,0) + float3(1,1,1) * NdotL * NdotL;
    }
    ENDCG
    }
    Fallback "Diffuse"
}