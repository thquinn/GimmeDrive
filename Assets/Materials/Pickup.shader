Shader "thquinn/PickupShader"
{
    Properties
    {
        [PerRendererData] _Tint("Base Color", Color) = (1, 0, 0, 1)
        _FresnelExponent("Fresnel Exponent", float) = 2
        _FresnelAmount("Fresnel Amount", Range(0, 1)) = 1
    }

    SubShader
    {
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                return OUT;
            }

            half4 _Tint;
            float _FresnelExponent, _FresnelAmount;
            half4 frag(Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                // Fresnel term based on the angle between N and V
                float fresnel = pow(1.0 - saturate(dot(N, V)), _FresnelExponent);

                half4 color = _Tint;
                color.rgb = lerp(color, half3(1, 1, 1), fresnel * _FresnelAmount);
                return color;
            }
            ENDHLSL
        }
    }
}
