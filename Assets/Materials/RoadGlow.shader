Shader "Custom/RoadGlow"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _CarLocation ("Car Location", Vector) = (0, 0, 0, 0)
        _CarDirection ("Car Direction", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
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
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            float3 _CarLocation;
            float2 _CarDirection;
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = _BaseColor;
                float2 delta = IN.positionWS.xz - _CarLocation.xz;
                float distance = length(delta);
                float d = dot(normalize(delta), _CarDirection);
                distance *= d;
                float minY = -0.5 * smoothstep(0, 2, distance) * smoothstep(5, 2, distance) * smoothstep(.7, 1, d);
                color.a *= smoothstep(minY, 0, IN.positionWS.y);
                return color;
            }
            ENDHLSL
        }
    }
}
