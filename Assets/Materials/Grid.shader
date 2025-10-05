Shader "thquinn/GridShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
        _CloudMap("Cloud Map", 2D) = "white"
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
     
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float3 positionWS  : TEXCOORD2;

            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_CloudMap);
            SAMPLER(sampler_CloudMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _CloudMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.positionOS = IN.positionOS;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            float2 _GridOffset;
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.positionWS.xz + _GridOffset) * _BaseColor;
                // Fade the edges.
                float2 fadeUV = abs(IN.uv - 0.5) * 2;
                color.a *= smoothstep(1, .85, max(fadeUV.x, fadeUV.y));
                // Scrolling cloud opacity.
                float2 cloudUV = IN.positionWS.xz * 0.05;
                cloudUV.x += _Time.y * .02;
                cloudUV.y += _Time.y * .023;
                color.a *= lerp(.05, 1, SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, cloudUV).r);
                return color;
            }
            ENDHLSL
        }
    }
}
