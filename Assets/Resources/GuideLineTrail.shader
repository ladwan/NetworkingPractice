// The movement guide line's trail. Lives in Resources so it's always in builds, since
// MovementGuideLine creates its material in code and nothing in a scene references it.
//  - Multiplies in the LineRenderer's vertex colors (the passive blue -> normal green gradient).
//  - Applies _BaseMap tiling/offset, which the scrolling chevron animation depends on.
//  - Fades the sides: the middle _SolidCenterWidth of the width is solid, then alpha follows
//    one smooth S-curve down to 0 at both edges. The curve is flat where it leaves the solid
//    center and flat where it reaches the edge, so there's no visible line at either end.
//  - Cuts the head into a centered point over the last _TipLength (0.25 matches the chevrons'
//    angle). Done here rather than with the LineRenderer's width curve, whose geometry came out
//    lopsided and hard-edged at the tip.
Shader "ForeverFight/Guide Line Trail"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1, 1, 1, 1)
        _SolidCenterWidth ("Solid Center Width", Range(0, 1)) = 0.1111
        _TrailLength ("Trail Length (set from code)", Float) = 1
        _TipLength ("Tip Length", Float) = 0.25
        _TipSoftness ("Tip Edge Softness", Range(0.01, 1)) = 0.25
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _SolidCenterWidth;
                float _TrailLength;
                float _TipLength;
                half _TipSoftness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half acrossWidth : TEXCOORD1; // half precision is plenty for 0..1 and cheaper on mobile GPUs
                float alongLine : TEXCOORD2;  // world units from the start of the line (float: it can pass 10)
                half4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                // The raw V (0..1 across the line's width), before tiling/offset.
                output.acrossWidth = input.uv.y;
                // The LineRenderer's Tile mode repeats the texture once per world unit, so the raw U
                // is the distance along the line.
                output.alongLine = input.uv.x;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor * input.color;

                // 0 at the center of the width, 1 at either edge
                half distanceFromCenter = abs(input.acrossWidth - 0.5h) * 2.0h;
                color.a *= 1.0 - smoothstep(_SolidCenterWidth, 1.0, distanceFromCenter);

                // Pointed head: over the last _TipLength the visible half width shrinks evenly to 0,
                // centered on the line. tipEdge is 1 where the tip starts, 0 at its point, and above 1
                // everywhere before the tip, so the rest of the line is untouched.
                half tipEdge = (_TrailLength - input.alongLine) / max(_TipLength, 0.0001);
                color.a *= saturate((tipEdge - distanceFromCenter) / _TipSoftness);

                return color;
            }
            ENDHLSL
        }
    }
}
