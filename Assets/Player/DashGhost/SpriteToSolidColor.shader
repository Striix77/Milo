Shader "Custom/SpriteToSolidColor"
{
   Properties
   {
      _MainTex ("Base (RGB)", 2D) = "white" {}
    //   _OriginalColor ("Original Color", Color) = (1,1,1,1)
      _TargetColor ("Target Color", Color) = (1,1,1,1)
      _Opacity ("Opacity", Range(0, 1)) = 1.0
    //   _Tolerance("Tolerance", Range(0, 0.01)) = 0.001
   }

   SubShader
   {
    Tags {"RenderType"="Transparency"}// Treat object as transparent so we can have transparent pixels
    Blend SrcAlpha OneMinusSrcAlpha // Blending modes set to handle transparency
    ZWrite Off // Depth Writing off

    Cull Off // Disable culling
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
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            // float4 _OriginalColor;
            float4 _TargetColor;
            float _Opacity; 
            // float _Tolerance;
 
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
 
                if (col.a == 0)
                {
                    return half4(0, 0, 0, 0);
                }
 
                // if (length(col - _OriginalColor) < _Tolerance)
                // {
                    return half4(_TargetColor.rgb,_Opacity);
                // }
 
                return col;
            }
 

        ENDCG
    }
   }
}
