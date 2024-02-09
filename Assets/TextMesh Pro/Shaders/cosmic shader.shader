Shader "Unlit/UnlitSpriteWithGalaxy"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GalaxyTex ("Galaxy Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GalaxyIntensity ("Galaxy Intensity", Range(0, 1)) = 0.5
        _BlendMode ("Blend Mode", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _GalaxyTex;
            fixed4 _Color;
            half _GalaxyIntensity;
            half _BlendMode;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.uv) * _Color;
                fixed4 galaxyColor = tex2D(_GalaxyTex, i.uv);
                
                // Apply blending mode
                fixed4 finalColor;
                if (_BlendMode == 0) {
                    finalColor = lerp(spriteColor, galaxyColor, _GalaxyIntensity);
                } else {
                    finalColor = spriteColor + galaxyColor * _GalaxyIntensity;
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
}
