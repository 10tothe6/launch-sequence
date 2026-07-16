Shader "vfx/camera_glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)){
                //make value smaller to avoid artefacts
                float3 smallValue = sin(value);
                //get scalar value from 3d vector
                float random = dot(smallValue, dotDir);
                //make value more random by making it bigger and then taking the factional part
                random = frac(sin(random) * 143758.5453);
                return random;
            }

            float3 rand3dTo3d(float3 value){
            return float3(
                rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
                rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
                rand3dTo1d(value, float3(73.156, 52.235, 09.151))
                );
            }

            float chance_for_dead_pixel;
            float chance_for_random_pixel;
            float chance_for_dead_zone;

            float noise_speed; // speed at which the noise morphs

            float max_uv_offset;
            float uv_offset_speed;

            float resolution_x;
            float resolution_y;

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                float interval_x = (float)1 / resolution_x;
                float interval_y = (float)1 / resolution_y;

                float uv_x = round(i.uv.x / interval_x) * interval_x;
                float uv_y = round(i.uv.y / interval_y) * interval_y;
                
                uv_x += rand3dTo1d(float3(uv_x, uv_y, _Time.x * uv_offset_speed)) * max_uv_offset;
                uv_y += rand3dTo1d(float3(uv_x, uv_y, _Time.x * uv_offset_speed)) * max_uv_offset;

                float low_factor = 30;

                float low_uv_x = round(i.uv.x / interval_x / low_factor) * interval_x * low_factor;
                float low_uv_y = round(i.uv.y / interval_y / low_factor) * interval_y * low_factor;

                fixed4 col = tex2D(_MainTex, float2(uv_x,uv_y));

                float rand_low = rand3dTo1d(float3(low_uv_x, low_uv_y, _Time.x * noise_speed));

                float rand = rand3dTo1d(float3(uv_x, uv_y, _Time.x * noise_speed));
                
                if (rand_low > 1- chance_for_dead_zone && rand > 1 -chance_for_dead_pixel) {
                    return 0;
                }
                else if (rand > 1 -chance_for_dead_pixel) {
                    return 0;
                }

                rand = rand3dTo1d(float3(uv_x, uv_y, -_Time.x * noise_speed));

                if (rand_low > 1- chance_for_dead_zone && rand > 1 - chance_for_random_pixel) {
                    float3 random_color = rand3dTo3d(float3(uv_x, uv_y, _Time.x * noise_speed));
                    return half4(random_color.x, random_color.y, random_color.z, 1);
                }
                
                return col;
            }
            ENDCG
        }
    }
}
