Shader "thquinn/NebulaShader"
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
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float3 normalOS   : TEXCOORD2;
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
                OUT.normalOS = IN.normalOS;
                return OUT;
            }

// from https://github.com/keijiro/NoiseShader/
float wglnoise_mod(float x, float y)
{
    return x - y * floor(x / y);
}

float2 wglnoise_mod(float2 x, float2 y)
{
    return x - y * floor(x / y);
}

float3 wglnoise_mod(float3 x, float3 y)
{
    return x - y * floor(x / y);
}

float4 wglnoise_mod(float4 x, float4 y)
{
    return x - y * floor(x / y);
}

float wglnoise_fade(float t)
{
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float2 wglnoise_fade(float2 t)
{
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float3 wglnoise_fade(float3 t)
{
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float wglnoise_mod289(float x)
{
    return x - floor(x / 289) * 289;
}

float2 wglnoise_mod289(float2 x)
{
    return x - floor(x / 289) * 289;
}

float3 wglnoise_mod289(float3 x)
{
    return x - floor(x / 289) * 289;
}

float4 wglnoise_mod289(float4 x)
{
    return x - floor(x / 289) * 289;
}

float2 wglnoise_permute(float2 x)
{
    return wglnoise_mod289((x * 34 + 10) * x);
}

float3 wglnoise_permute(float3 x)
{
    return wglnoise_mod289((x * 34 + 10) * x);
}

float4 wglnoise_permute(float4 x)
{
    return wglnoise_mod289((x * 34 + 10) * x);
}
float ClassicNoise_impl(float3 pi0, float3 pf0, float3 pi1, float3 pf1)
{
    pi0 = wglnoise_mod289(pi0);
    pi1 = wglnoise_mod289(pi1);

    float4 ix = float4(pi0.x, pi1.x, pi0.x, pi1.x);
    float4 iy = float4(pi0.y, pi0.y, pi1.y, pi1.y);
    float4 iz0 = pi0.z;
    float4 iz1 = pi1.z;

    float4 ixy = wglnoise_permute(wglnoise_permute(ix) + iy);
    float4 ixy0 = wglnoise_permute(ixy + iz0);
    float4 ixy1 = wglnoise_permute(ixy + iz1);

    // Gradients: 7x7 points over a square, mapped onto an octahedron.
    // The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
    float4 gx0 = lerp(-1, 1, frac(ixy0 / 7));
    float4 gy0 = lerp(-1, 1, frac(floor(ixy0 / 7) / 7));
    float4 gz0 = 1 - abs(gx0) - abs(gy0);

    bool4 zn0 = gz0 < 0;
    gx0 += zn0 * (gx0 < 0 ? 1 : -1);
    gy0 += zn0 * (gy0 < 0 ? 1 : -1);

    float4 gx1 = lerp(-1, 1, frac(ixy1 / 7));
    float4 gy1 = lerp(-1, 1, frac(floor(ixy1 / 7) / 7));
    float4 gz1 = 1 - abs(gx1) - abs(gy1);

    bool4 zn1 = gz1 < 0;
    gx1 += zn1 * (gx1 < 0 ? 1 : -1);
    gy1 += zn1 * (gy1 < 0 ? 1 : -1);

    float3 g000 = normalize(float3(gx0.x, gy0.x, gz0.x));
    float3 g100 = normalize(float3(gx0.y, gy0.y, gz0.y));
    float3 g010 = normalize(float3(gx0.z, gy0.z, gz0.z));
    float3 g110 = normalize(float3(gx0.w, gy0.w, gz0.w));
    float3 g001 = normalize(float3(gx1.x, gy1.x, gz1.x));
    float3 g101 = normalize(float3(gx1.y, gy1.y, gz1.y));
    float3 g011 = normalize(float3(gx1.z, gy1.z, gz1.z));
    float3 g111 = normalize(float3(gx1.w, gy1.w, gz1.w));

    float n000 = dot(g000, pf0);
    float n100 = dot(g100, float3(pf1.x, pf0.y, pf0.z));
    float n010 = dot(g010, float3(pf0.x, pf1.y, pf0.z));
    float n110 = dot(g110, float3(pf1.x, pf1.y, pf0.z));
    float n001 = dot(g001, float3(pf0.x, pf0.y, pf1.z));
    float n101 = dot(g101, float3(pf1.x, pf0.y, pf1.z));
    float n011 = dot(g011, float3(pf0.x, pf1.y, pf1.z));
    float n111 = dot(g111, pf1);

    float3 fade_xyz = wglnoise_fade(pf0);
    float4 n_z = lerp(float4(n000, n100, n010, n110),
                      float4(n001, n101, n011, n111), fade_xyz.z);
    float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
    float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x);
    return 1.4 * n_xyz;
}

// Classic Perlin noise
float ClassicNoise(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    return ClassicNoise_impl(i, f, i + 1, f - 1);
}

            float2 _GridOffset;
            half4 frag(Varyings IN) : SV_Target
            {
                // Twist normals.
                float3 uv = IN.normalOS;
                float twist = 1.25;
                float angle = uv.y * twist;
                float s = sin(angle);
                float c = cos(angle);
                float3 twisted;
                twisted.x = uv.x * c - uv.z * s;
                twisted.y = uv.y;
                twisted.z = uv.x * s + uv.z * c;
                uv = normalize(twisted);
                // Calculate noise.
                float t = _Time.x * .4;
                uv += float3(t * .32, t * .11, t * -.43);
                float coarseFactor = 1.5;
                float fineFactor = 3;
                float r = max(0, max(ClassicNoise(uv * coarseFactor), ClassicNoise(uv * fineFactor)) * .25);
                uv.x += .2;
                float g = max(0, max(ClassicNoise(uv * coarseFactor), ClassicNoise(uv * fineFactor)) * .25);
                uv.x += .2;
                float b = max(0, max(ClassicNoise(uv * coarseFactor), ClassicNoise(uv * fineFactor)));
                float a = b * .15;
                // Detail.
                uv.x += 10;
                float ultrafineFactor = 20;
                float detail = ClassicNoise(uv * ultrafineFactor);
                detail = lerp(0.5, 1, (detail + 1) / 2);
                a *= detail;
                return half4(r, g, b, a);
            }
            ENDHLSL
        }
    }
}
