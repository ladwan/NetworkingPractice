Shader "Unlit/Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normals : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float3 interpolatorNormal : TEXCOORD1;
                float2 uv : TEXCOORD0;    
            };

            Interpolators vert (MeshData meshData)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(meshData.vertex);
                o.interpolatorNormal = UnityObjectToWorldNormal(meshData.normals);
                o.uv = TRANSFORM_TEX(meshData.uv, _MainTex);             
                
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target 
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float offset = cos(i.uv.x * 6.8 * 8) * 0.01;
                float t = cos( (i.uv.y + offset - _Time.y * 0.1) * 6.8 * 5) * 0.5 + 0.5;
                return t;
            }
            ENDCG
        }
    }
}
