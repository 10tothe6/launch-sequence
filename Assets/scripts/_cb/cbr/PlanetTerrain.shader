Shader "Custom/Planet Shaders/Fake Terrain" {
Properties {
    _MainTex ("Biome Map", 2D) = "white" {}
    _Biome1 ("Biome Texture 1", 2D) = "white" {}
    _Biome2 ("Biome Texture 2", 2D) = "white" {}
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    CGPROGRAM
    #pragma surface surf SimpleLambert
    
    float angle;
    half3 sunPosition;
    half3 position;

    // Lighting for the terrain (custom so that the sun direction can change)
    half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
        half NdotL = saturate(0.4 + dot(s.Normal, normalize(sunPosition - position)));
        half4 c;
        c.rgb = s.Albedo * (NdotL);
        c.a = 1;
        return c;
    }

    float3 AdjustVector(float3 _v) {
        return float3(_v.z * sin(-angle * (3.14192 / 180)) + _v.x * cos(-angle * (3.14192 / 180)), _v.y, _v.z * cos(-angle * (3.14192 / 180)) + _v.x * -sin(-angle * (3.14192 / 180)));
    }
  
    struct Input {
        float3 worldPos;
        float4 screenPos;
        float2 uv_MainTex;
    };
        
    sampler2D _MainTex;
    sampler2D _CameraDepthTexture;
    sampler2D _Biome1;
    sampler2D _Biome2;

    float blendPower;
    float3 planetCentre;

    int realView;

    float2 UVFromPoint(float3 _point) {
        float u = atan2(_point.x, _point.z) / (2*3.14159) + 0.5;
        float v = -asin(_point.y) / 3.14159 + 0.5;
        return float2(u, v);
    }

    void surf (Input IN, inout SurfaceOutput o) {
        float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, IN.screenPos);
        float sceneDepth = LinearEyeDepth(sceneDepthNonLinear);

        float2 uv = -UVFromPoint(AdjustVector(normalize(IN.worldPos - planetCentre)));

        //float4 col = lerp(tex2D(_MainTex, uv1), tex2D(_MainTex, uv2), (uv.x - uv1.x) / (uv2.x - uv1.x));
        float4 colS = tex2D(_MainTex, uv).r < 0.725 ? tex2D(_Biome1, uv * 20000) : tex2D(_Biome2, uv * 20000);
        float4 colB = tex2D(_MainTex, uv).r < 0.725 ? tex2D(_Biome1, uv * 1000) : tex2D(_Biome2, uv * 1000);

        float4 col = lerp(colS, colB, saturate(sceneDepth));
        
        if (realView == 0) {
            o.Albedo = col;
        }
        else {
            o.Albedo = lerp(float4(0, 0, 0, 1), float4(1, 0, 0, 1), (length(IN.worldPos - planetCentre) - 200));
        }
    }
    ENDCG
    }
    Fallback "Diffuse"
}