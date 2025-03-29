Shader "Custom/SpriteToSolidColor_URP_Bloom"
{
   Properties
   {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _TargetColor ("Target Color", Color) = (1,1,1,1)
      _Opacity ("Opacity", Range(0, 1)) = 1.0
      _EmissionStrength ("Emission Strength", Range(0, 10)) = 1.0
   }

   SubShader
   {
      Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline" }
      Blend SrcAlpha OneMinusSrcAlpha
      ZWrite Off
      Cull Off

      Pass
      {
         HLSLPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

         struct appdata
         {
             float4 vertex : POSITION;
             float2 uv : TEXCOORD0;
         };

         struct v2f
         {
             float2 uv : TEXCOORD0;
             float4 vertex : SV_POSITION;
         };

         sampler2D _MainTex;
         float4 _TargetColor;
         float _Opacity;
         float _EmissionStrength;

         v2f vert(appdata v)
         {
             v2f o;
             o.vertex = TransformObjectToHClip(v.vertex); // Proper URP transformation
             o.uv = v.uv;
             return o;
         }

         half4 frag(v2f i) : SV_Target
         {
             half4 col = tex2D(_MainTex, i.uv);

             if (col.a == 0) // Discard fully transparent pixels
             {
                 return half4(0, 0, 0, 0);
             }

             // Apply the target color
             half3 finalColor = _TargetColor.rgb * col.a;
             
             // HDR Emission for Bloom
             half3 emission = finalColor * _EmissionStrength;

             // Output the color with emission
             return half4(emission, col.a * _Opacity);
         }

         ENDHLSL
      }
   }
}
