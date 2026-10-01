// The movement guide line's endpoint marker: a ring that starts at the center of the quad,
// travels out to its edge and fades as it goes, then starts again. Everything off the ring is
// fully transparent. Lives in Resources so it's always in builds, since MovementGuideLine
// creates its material in code.
//
// Done in the shader on the existing quad instead of scaling a ring mesh: the ring has to fade
// out either way (so it's transparent either way), and this needs no extra mesh, no per-frame
// transform updates, and only a few pixels of cheap math since the marker is small on screen.
Shader "ForeverFight/Guide Line Endpoint Wave"
{
    Properties
    {
        _BaseColor ("Color", Color) = (1, 1, 1, 1)
        _WaveSpeed ("Waves Per Second", Float) = 1.2
        _RingWidth ("Ring Width", Range(0.01, 0.5)) = 0.08
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

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half _WaveSpeed;
                half _RingWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half2 centeredUv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                // -1..1 with 0 at the quad's center, so length() is 0 at the center and 1 at the edge midpoints
                output.centeredUv = input.uv * 2.0 - 1.0;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half distanceFromCenter = length(input.centeredUv);

                // 0..1 progress of the current wave; the ring's radius grows with it
                half wave = frac(_Time.y * _WaveSpeed);
                half ringRadius = wave;

                // Soft-edged ring: 1 on the ring, easing to 0 _RingWidth away on either side
                half ring = 1.0 - smoothstep(0.0, _RingWidth, abs(distanceFromCenter - ringRadius));

                // Fade in quickly as it leaves the center, then fade out as it reaches the edge
                half fade = smoothstep(0.0, 0.1, wave) * (1.0 - wave);

                half4 color = _BaseColor;
                color.a *= ring * fade;
                return color;
            }
            ENDHLSL
        }
    }
}
