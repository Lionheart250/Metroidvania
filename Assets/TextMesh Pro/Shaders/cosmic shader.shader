Shader "Unlit/UnlitSpriteWithBlackHoleSwirlPulsating"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GalaxyTex ("Galaxy Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GalaxyIntensity ("Galaxy Intensity", Range(0, 1)) = 0.5
        _BlendMode ("Blend Mode", Range(0, 1)) = 0.5
        _ScrollSpeed ("Scroll Speed", Vector) = (1, 0, 0, 0) // Adjust speed and direction of scrolling
        _IntensityMultiplier ("Intensity Multiplier", Range(0, 10)) = 1.0 // Adjust intensity of the effect
        _Contrast ("Contrast", Range(0, 10)) = 5.0 // Adjust contrast of the effect
        _SwirlSpeed ("Swirl Speed", Range(0, 10)) = 2.0 // Adjust speed of the swirling effect
        _SwirlAmount ("Swirl Amount", Range(0, 1)) = 0.5 // Adjust amount of swirling effect
        _HeadSize ("Head Size", Range(0, 0.5)) = 0.1 // Size of the square representing the player's head
        _HeadPosition ("Head Position", Vector) = (0.5, 0.5, 0, 0) // Position of the head center in UV space
        _PulsateSpeed ("Pulsate Speed", Range(0, 10)) = 2.0 // Adjust the speed of pulsation
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
                float2 mainTexUV : TEXCOORD0;
                float2 galaxyTexUV : TEXCOORD1;
                float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _GalaxyTex;
            fixed4 _Color;
            half _GalaxyIntensity;
            half _BlendMode;
            float4 _ScrollSpeed;
            half _IntensityMultiplier;
            half _Contrast;
            half _SwirlSpeed;
            half _SwirlAmount;
            half _HeadSize;
            float2 _HeadPosition;
            half _PulsateSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = _Color;
                
                // Pass through the UV coordinates for the sprite texture
                o.mainTexUV = v.uv;
                
                // Apply swirl effect to UV coordinates for galaxy texture
                float angle = _Time.y * _SwirlSpeed;
                float s = sin(angle);
                float c = cos(angle);
                float2 offset = float2(v.uv.x - 0.5, v.uv.y - 0.5);
                o.galaxyTexUV.x = offset.x * c - offset.y * s;
                o.galaxyTexUV.y = offset.x * s + offset.y * c;
                o.galaxyTexUV += float2(0.5, 0.5);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample sprite texture with alpha
                fixed4 spriteColor = tex2D(_MainTex, i.mainTexUV);
                
                // Calculate distance from the head position
                float2 headMin = _HeadPosition - float2(_HeadSize / 2, _HeadSize / 2);
                float2 headMax = _HeadPosition + float2(_HeadSize / 2, _HeadSize / 2);
                float2 closestPoint = clamp(i.mainTexUV, headMin, headMax);
                float distanceFromHead = distance(i.mainTexUV, closestPoint);
                
                // Sample galaxy texture with alpha using the modified UV coordinates
                fixed4 galaxyColor = tex2D(_GalaxyTex, i.galaxyTexUV);
                
                // Exclude square area around the head position
                if (distanceFromHead < _HeadSize / 2) {
                    galaxyColor = fixed4(0, 0, 0, 0); // Set galaxy color to transparent
                }
                
                // Apply contrast to galaxy texture
                galaxyColor.rgb = (galaxyColor.rgb - 0.5) * _Contrast + 0.5;
                
                // Apply intensity multiplier to galaxy texture
                galaxyColor.rgb *= _IntensityMultiplier;
                
                // Apply pulsating effect to galaxy texture intensity
                float pulsateFactor = 0.5 + 0.5 * sin(_Time.y * _PulsateSpeed);
                galaxyColor.rgb *= pulsateFactor;
                
                // Apply blending mode with alpha blending
                fixed4 finalColor;
                if (_BlendMode == 0) {
                    finalColor.rgb = lerp(spriteColor.rgb, galaxyColor.rgb, _GalaxyIntensity);
                    finalColor.a = spriteColor.a;
                } else {
                    finalColor.rgb = spriteColor.rgb + galaxyColor.rgb * _GalaxyIntensity;
                    finalColor.a = spriteColor.a;
                }
                
                return finalColor * i.color;
            }
            ENDCG
        }
    }
}
