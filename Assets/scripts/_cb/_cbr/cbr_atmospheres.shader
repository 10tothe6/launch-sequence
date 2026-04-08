Shader "cbr/atmospheres"
{
    Properties
    {
        // the color buffer coming in from the camera
        _MainTex ("Texture", 2D) = "white" {} 

        // there WAS a cloud map but I have to figure out how to combine textures, since this shader now supports n planets
        // maybe just have 8 maps?
        // _CloudTex ("Cloud Map", 2D) = "white" {} 
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
                float3 viewVector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                //calculating the forward vector of the camera
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                
                return o;
            }

            // texures are not per-planet right now
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _CloudTex;

            // okay, this is gonna be dumb
            // but hlsl is dumb so i gotta be dumb with it

            // tldr: you cant initialize arrays with variables, so I'm setting a HARD MAX LIMIT on celestial bodies of 8
            // bodies without atmospheres need not be included, so thats not 8 bodies thats 12 bodies with atmospheres

            // c_ means center
 
            float3 planetCentre[8];
            float3 c_planetCentre;

            float atmosphereRadius[8];
            float c_atmosphereRadius;

            float surfaceRadius[8];
            float c_surfaceRadius;

            float densityFalloff[8];
            float c_densityFalloff;

            float3 sunPosition;
            float3 dirToSun;

            float3 scatterCoefficients[8];
            float3 c_scatterCoefficients;

            float planetScale[8];
            float c_planetScale;

            float densityMultiplier[8];
            float c_densityMultiplier;
            float luminance[8];
            float c_luminance;
            float externalBrightness[8];
            float c_externalBrightness;
            float scatterFactor[8];
            float c_scatterFactor;
            float cloudBrightness[8];
            float c_cloudBrightness;

            float minCloudRadius[8];
            float c_minCloudRadius;
            float maxCloudRadius[8];
            float c_maxCloudRadius;

            float theta;
            
            // move a vector by an angle
            // (2D rotation matrix around the (0,1,0) vector, I think)
            float3 AdjustVector(float3 _v) {
                float a = theta;
                return float3(_v.z * sin(-a * (3.14192 / 180)) + _v.x * cos(-a * (3.14192 / 180)), _v.y, _v.z * cos(-a * (3.14192 / 180)) + _v.x * -sin(-a * (3.14192 / 180)));
            }

            // getting the uv coordinate OF A SPHERE-MAPPED TEXTURE from a point in 3D space
            float2 UVFromPoint(float3 _point) {
                float u = atan2(_point.x, _point.z) / (2*3.14159) + 0.5;
                float v = -asin(_point.y) / 3.14159 + 0.5;
                return float2(u, v);
            }

            //a function that intersects a ray with a sphere
            //CREDIT: Sebastian Lague [Coding Adventures: Atmosphere]
            float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
                float3 offset = rayOrigin - sphereCentre;
                float a = 1;
                float b = 2 * dot(offset, rayDir);
                float c = dot(offset, offset) - sphereRadius * sphereRadius;
                float d = b * b - 4 * a * c;

                if (d > 0) {
                    float s = sqrt(d);
                    float dstToSphereNear = max(0, (-b - s) / (2 * a));
                    float dstToSphereFar = (-b + s) / (2 * a);

                    if (dstToSphereFar >= 0) {
                        return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
                    }
                }

                return float2(-1, 0);
            }

            float densityAtPoint(float3 samplePoint) {
                float heightAboveSurface = length(samplePoint - c_planetCentre) - c_surfaceRadius;
                float height01 = heightAboveSurface / ((c_atmosphereRadius - c_surfaceRadius));

                float localDensity = (1 - height01) * exp(-height01 * c_densityFalloff);
                return localDensity * c_densityMultiplier;
            }

            float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) {
                float3 samplePoint = rayOrigin;

                int numOpticalDepthPoints = 10;
                float stepSize = rayLength / (numOpticalDepthPoints-1);

                float opticalDepth = 0;

                for (int i = 0; i < numOpticalDepthPoints; i++) {
                    float localDensity = densityAtPoint(samplePoint);
                    opticalDepth += localDensity * stepSize;
                    samplePoint += rayDir * stepSize;
                }

                return opticalDepth;
            }

            float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalColor) { //
                // no engine units here
                int numInscatteringPoints = 10;
                float stepSize = rayLength / (numInscatteringPoints-1);

                float3 inScatterPoint = rayOrigin;

                float3 inScatteredLight = 0;

                float viewRayOpticalDepth = 0;
                float sunRayOpticalDepth = 0;
                float startingSunRayOpticalDepth = 0;

                float3 transmittance = 0;

                for (int i = 0; i < numInscatteringPoints; i++) {
                    viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize * i);

                    float sunRayLength = raySphere(c_planetCentre, c_atmosphereRadius, inScatterPoint, dirToSun).y;
                    sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
                    if (startingSunRayOpticalDepth == 0) {
                        startingSunRayOpticalDepth = sunRayLength + 0.01;
                    }

                    float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth) * c_scatterCoefficients);
                    float localDensity = densityAtPoint(inScatterPoint);
                    
                    inScatteredLight += localDensity * transmittance * c_scatterCoefficients * stepSize;
                    inScatterPoint += rayDir * stepSize;
                }

                //return inScatteredLight;
                float3 brightnessFactor = clamp(exp(-startingSunRayOpticalDepth/c_luminance * c_scatterCoefficients), 0, 1);
                return lerp(originalColor * brightnessFactor, lerp(normalize(c_scatterCoefficients) * c_externalBrightness * dot(normalize(rayOrigin - c_planetCentre), dirToSun) * brightnessFactor, originalColor * brightnessFactor, exp(-viewRayOpticalDepth)) + inScatteredLight * c_scatterFactor, clamp(dot(normalize(dirToSun), normalize(rayOrigin - c_planetCentre)) + 1, 0, 1));
            }

            float3 calculateClouds(float3 rayOrigin, float3 rayDir, float2 hitInfoMin, float2 hitInfoMax, float3 originalColor) {
                // start and end points of ray inside cloud layer
                float3 p1 = rayOrigin + (hitInfoMin.x == 0 && (hitInfoMin.y + 0.01) > 0 ? rayDir * hitInfoMin.y : rayDir * (hitInfoMax.x + 0.01));
                float3 p2 = p1 + rayDir * (hitInfoMax.y - hitInfoMin.y);

                float colMag = 0;
                float stepSize = 5;
                int numPoints = floor(length(p2 - p1) / stepSize);

                float3 position = p1 - c_planetCentre;

                float cloudAltitude = (c_maxCloudRadius + c_minCloudRadius) / 2;
                float colorMult = 1;

                [unroll]
                for (int j = 0; j < 59; j++) {
                    if (j > numPoints) {break;}

                    float cloudHeight = tex2D(_CloudTex, UVFromPoint(AdjustVector(normalize(position)))).x;
                    float cloudOffset = 0;

                    if (length(position - (normalize(position) * cloudAltitude + cloudOffset)) < cloudHeight * (c_maxCloudRadius - c_minCloudRadius) / 2) {
                        if (colorMult == 1) {colorMult = ((length(position) - (cloudAltitude + cloudOffset)) / ((c_maxCloudRadius - c_minCloudRadius)) + 1) / 2;}
                        
                        colMag += 0.1 * stepSize;
                    }

                    position += rayDir * stepSize;
                }

                colorMult = clamp(colorMult, 0, 0.8);
                colMag = clamp(colMag, 0, 3);

                float lightingAmount = saturate(0.25 + dot(normalize(position), dirToSun));
                return lerp(originalColor, c_scatterCoefficients * c_cloudBrightness * colorMult * lightingAmount, clamp(colMag / 10, 0, 1));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float epsilon = 0.001f;

                // eve-like
                // densityMultiplier = 0.15;
                // luminance = 50;
                // externalBrightness = 1;
                // cloudBrightness = 7;
                // scatterFactor = 0.5;
                //ss of 0.3, wl of (415.393, 446.732, 426.767)

                // earth-like
                // densityMultiplier = 0.025;
                // luminance = 1000;
                // externalBrightness = 0.5;
                // scatterFactor = 0.8;

                // duna-like
                // densityMultiplier = 0.02;
                // luminance = 600;
                // externalBrightness = 0.3;
                // scatterFactor = 0.6;

                // *** variables that apply to ALL planets ***

                // direct from the camera
                fixed4 originalColor = tex2D(_MainTex, i.uv);

                float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float rawSceneDepth = LinearEyeDepth(sceneDepthNonLinear)*length(i.viewVector);
                
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                // *** SORTING ***

                int planetIndices[8];
                for (int n = 0; n < 8; n++) {
                    if (atmosphereRadius[n] == 0) {
                        planetIndices[n] = -1;
                    } else {
                        planetIndices[n] = n;
                    }
                }

                for (int i = 0; i < 8; i++) {
                    if (planetIndices[i] != -1) {
                        if (atmosphereRadius[planetIndices[i]] != 0) {
                            c_planetCentre = planetCentre[planetIndices[i]];

                            c_atmosphereRadius = atmosphereRadius[planetIndices[i]];
                            c_surfaceRadius = surfaceRadius[planetIndices[i]];

                            c_densityFalloff = densityFalloff[planetIndices[i]];
                            c_scatterCoefficients = scatterCoefficients[planetIndices[i]];

                            c_planetScale = planetScale[planetIndices[i]];
                            float sceneDepth = rawSceneDepth / c_planetScale;

                            c_densityMultiplier = densityMultiplier[planetIndices[i]];
                            c_luminance = luminance[planetIndices[i]];
                            c_externalBrightness = externalBrightness[planetIndices[i]];
                            c_scatterFactor = scatterFactor[planetIndices[i]];
                            c_cloudBrightness = cloudBrightness[planetIndices[i]];

                            c_minCloudRadius = minCloudRadius[planetIndices[i]];
                            c_maxCloudRadius = maxCloudRadius[planetIndices[i]];

                            dirToSun = normalize(sunPosition - c_planetCentre);

                            // all in engine units (not actual dist, physically speaking)
                            float2 planetHitInfo = raySphere(c_planetCentre, c_atmosphereRadius, rayOrigin, rayDir);
                            float distanceToAtmosphere = planetHitInfo.x;
                            float distanceThroughAtmosphere = min(sceneDepth - distanceToAtmosphere, planetHitInfo.y);

                            float2 minCloudHitInfo = raySphere(c_planetCentre, c_minCloudRadius, rayOrigin, rayDir);
                            float2 maxCloudHitInfo = raySphere(c_planetCentre, c_maxCloudRadius, rayOrigin, rayDir);

                            minCloudHitInfo.y = min(sceneDepth - minCloudHitInfo.x, minCloudHitInfo.y);
                            maxCloudHitInfo.y = min(sceneDepth - maxCloudHitInfo.x, maxCloudHitInfo.y);
                            
                            if (distanceThroughAtmosphere > 0) {
                                // also engine units
                                float3 pointInAtmosphere = rayOrigin + rayDir * (distanceToAtmosphere + epsilon);
                                float3 light = calculateLight(pointInAtmosphere, rayDir, distanceThroughAtmosphere - epsilon, originalColor);
                                originalColor = float4(light, 1);
                            }
                            if (maxCloudHitInfo.y > 0) {
                                originalColor = float4(calculateClouds(rayOrigin, rayDir, minCloudHitInfo, maxCloudHitInfo, originalColor), 1);
                            }
                        }
                    }
                }

                return originalColor;
            }
            ENDCG
        }
    }
}
