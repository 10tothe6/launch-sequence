    Shader "Custom/water_spherewaves" {
        Properties{
            [Space]
            [Header(Tesselation)]
            _MaxTessDistance("Max Tessellation Distance", Range(10,100)) = 50
            _Tess("Tessellation", Range(1,32)) = 20

            [Space]
            
            _LightCol ("Light Color", Color) = (0.5,0.5,0.5,1)
            _DarkCol ("Dark Color", Color) = (0.5,0.5,0.5,1)

            _Foam1 ("Foam Texture 1", 2D) = "white" {}
            _Foam2 ("Foam Texture 2", 2D) = "white" {}

            _SpecularNormal ("Normal Map", 2D) = "white" {}
        }
    
        SubShader{
            Tags{ "RenderType" = "Opaque" }
            LOD 200
    
            CGPROGRAM
    
            // custom lighting function that uses a texture ramp based
            // on angle between light direction and normal
            #pragma surface surf ToonRamp vertex:vert addshadow nolightmap tessellate:tessDistance fullforwardshadows
            #pragma target 4.0
            #pragma require tessellation tessHW
            #include "Tessellation.cginc"

            sampler2D _MainTex, _Foam1, _Foam2, _SpecularNormal;

            float4 _LightCol;
            float4 _DarkCol;
            float3 sunDir;
            int isUnderWater;
            float timeValue;
    
            inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
            {
                lightDir = normalize(lightDir);
                viewDir = normalize(viewDir);

                half4 c;
                
                c.rgb = dot(lightDir, s.Normal);
                
                c.a = 0;
                return c;
            }
            uniform float3 _Position;
            uniform sampler2D _GlobalEffectRT;
            uniform float _OrthographicCamSize;
            float baseWaveFrequency;
    
            float _Tess;
            float _MaxTessDistance;
            float3 waveVectors[12];
            
    
            float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
            {
                float3 worldPosition = mul(unity_ObjectToWorld, vertex).xyz;
                float dist = distance(worldPosition, _WorldSpaceCameraPos);
                float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);
                return f * tess;
            }
    
            float4 DistanceBasedTess(float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess)
            {
                float3 f;
                f.x = CalcDistanceTessFactor(v0, minDist, maxDist, tess);
                f.y = CalcDistanceTessFactor(v1, minDist, maxDist, tess);
                f.z = CalcDistanceTessFactor(v2, minDist, maxDist, tess);
    
                return UnityCalcTriEdgeTessFactors(f);
            }
    
            float4 tessDistance(appdata_full v0, appdata_full v1, appdata_full v2)
            {
                float minDist = 10.0;
                float maxDist = _MaxTessDistance;
    
                return DistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
            }
    
            struct Input {
                float2 uv_MainTex : TEXCOORD0;
                float3 worldPos; // world position built-in value
                float3 viewDir;// view direction built-in value we're using for rimlight
            };

            float3 ProjectOnVector(float3 v, float3 n)
            {

                return n * dot(v, n);
            }

            float getAdvancing(float3 dir, float3 pos) {
                dir = normalize(dir);

                float3 proj = ProjectOnVector(pos, dir);
                float d = dot(dir, pos);

                if (d < 0) {
                    return length(proj) * -1;
                } else {
                    return length(proj);
                }
            }

            float getHeight(float3 pos) {
                float sum = 0;

                int waveCount = 2;

                for (int i = 0; i < waveCount; i++) {
                    
                    sum += (1 + sin(getAdvancing(waveVectors[i], pos)*100)/10);
                }

                return sum;
            }

            float3 getNormal(float3 pos) {
                return normalize(pos);
            }
    
            void vert(inout appdata_full v)
            {   
                
                float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                float waterHeight = getHeight(worldPosition);
                
                // move vertices up where snow is, and where there is no path   
                v.vertex.xyz += normalize(v.normal) * waterHeight;
                v.normal = getNormal(worldPosition);
            }
    
            void surf(Input IN, inout SurfaceOutput o) {

                // float heightTerm = clamp(IN.worldPos.y - 6, 0, 10)/5;
                // float diffuseTerm = lerp(0, 1, clamp(dot(sunDir, o.Normal)-0.5, 0, 1));
                // float distanceTerm = clamp(2/length(IN.worldPos - _WorldSpaceCameraPos), 0, 0.3);

                // float2 x = IN.worldPos.xz/50;
                // float t = timeValue/30;
                // float foamValue = tex2D(_Foam1, x/2 + t/2).r + tex2D(_Foam2, x/3-t/3).r + tex2D(_Foam1, x+t/3).r;
                // if (isUnderWater) {
                //     foamValue *= 0.4; // foam is less apparent when viewing from the bottom
                // }

                // o.Albedo = heightTerm + diffuseTerm + distanceTerm;
                // o.Normal = normalize(lerp(o.Normal, UnpackNormal(tex2D(_SpecularNormal, x)), 0.3));
                // o.Alpha = foamValue * 0.25;
                // o.Emission = 0;

                o.Albedo = float3(1,1,1);
                o.Alpha = 1;
                o.Emission = 0;
            }
            ENDCG
    
        }
        
        Fallback "Diffuse"
    }