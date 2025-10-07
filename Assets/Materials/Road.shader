Shader "thquinn/RoadShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _AlphaMinX("Alpha Min X", Float) = 0
        _AlphaMaxX("Alpha Max X", Float) = 1
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 positionWS  : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _AlphaMinX;
                float _AlphaMaxX;
                float _Smoothness;
                float _Metallic;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionOS = v.positionOS.xyz;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.positionHCS = TransformWorldToHClip(o.positionWS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 normalWS = normalize(i.normalWS);
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(i.positionWS));

                // Direct light
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(normalWS, lightDir));
                float3 direct = mainLight.color * NdotL;

                // Indirect/environment light (SH probe, skybox, etc.)
                float3 indirect = SampleSH(normalWS);

                // Combine lighting
                float3 lighting = direct + indirect;

                // Alpha fade based on object-space X
                float alphaT = saturate((i.positionOS.x - _AlphaMinX) / (_AlphaMaxX - _AlphaMinX));
                float alpha = _BaseColor.a * alphaT;

                float3 color = _BaseColor.rgb * lighting;
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
