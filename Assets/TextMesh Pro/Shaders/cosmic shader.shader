Shader "Unlit/UnlitSpriteWithNeonGlow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GalaxyTex ("Galaxy Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GalaxyIntensity ("Galaxy Intensity", Range(0, 2)) = 0.5
        _BlendMode ("Blend Mode", Range(0, 1)) = 0.5
        _ScrollSpeed ("Scroll Speed", Vector) = (1, 0, 0, 0) // Adjust speed and direction of scrolling
        _IntensityMultiplier ("Intensity Multiplier", Range(0, 10)) = 1.0 // Adjust intensity of the effect
        _Contrast ("Contrast", Range(0, 10)) = 5.0 // Adjust contrast of the effect
        _SwirlSpeed ("Swirl Speed", Range(0, 10)) = 2.0 // Adjust speed of the swirling effect
        _SwirlAmount ("Swirl Amount", Range(0, 1)) = 0.5 // Adjust amount of swirling effect
        _PulsateSpeed ("Pulsate Speed", Range(0, 10)) = 2.0 // Adjust the speed of pulsation
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 5.0 // Adjust intensity of the glow effect
        _GlowColor ("Glow Color", Color) = (1,1,1,1) // Color of the glow
        _ExcludedColors ("Excluded Colors", Color) = (0,0,0,0) // Colors to exclude
        _ExcludedColor2 ("Excluded Color 2", Color) = (0,0,0,0) // Second color to exclude
        _ExcludedColor3 ("Excluded Color 3", Color) = (0,0,0,0) // Third color to exclude
        _ExcludedColor4 ("Excluded Color 4", Color) = (0,0,0,0) // Fourth color to exclude
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 mainTexUV : TEXCOORD0;
                float2 galaxyTexUV : TEXCOORD1;
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
            half _PulsateSpeed;
            half _GlowIntensity;
            float4 _GlowColor;
            float4 _ExcludedColors[4]; // Adjusted to hold 4 elements
            float4 _ExcludedColor2;
            float4 _ExcludedColor3;
            float4 _ExcludedColor4; // New excluded color
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.mainTexUV = v.uv;
                
                // Apply swirl effect to UV coordinates for galaxy texture
                float angle = _Time.y * _SwirlSpeed;
                float s = sin(angle);
                float c = cos(angle);
                float2 offset = float2(v.uv.x - 0.5, v.uv.y - 0.5);
                o.galaxyTexUV.x = offset.x * c - offset.y * s;
                o.galaxyTexUV.y = offset.x * s + offset.y * c;
                o.galaxyTexUV += float2(0.5, 0.5);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.mainTexUV);
                fixed4 galaxyColor = tex2D(_GalaxyTex, i.galaxyTexUV);
                
                // Check if the pixel color matches any excluded color on the sprite
                bool excludePixel = false;
                for (int j = 0; j < 4; j++) {
                    if (distance(spriteColor.rgb, _ExcludedColors[j].rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor2.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor3.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor4.rgb) < 0.05) {
                        excludePixel = true;
                        break;
                    }
                }
                
                // If excluded, keep the pixel for the sprite and discard the galaxy texture pixel
                if (excludePixel)
                    return spriteColor * _Color;
                
                // Apply effects to galaxy texture pixel
                galaxyColor.rgb = (galaxyColor.rgb - 0.5) * _Contrast + 0.5;
                galaxyColor.rgb *= _IntensityMultiplier;
                float pulsateFactor = 0.5 + 0.5 * sin(_Time.y * _PulsateSpeed);
                galaxyColor.rgb *= pulsateFactor;
                float glowFactor = _GlowIntensity * (1.0 - spriteColor.a);
                fixed4 glow = _GlowColor * glowFactor;
                galaxyColor.rgb += glow.rgb;
                
                fixed4 finalColor;
                if (_BlendMode == 0) {
                    finalColor.rgb = lerp(spriteColor.rgb, galaxyColor.rgb, _GalaxyIntensity);
                    finalColor.a = spriteColor.a;
                } else {
                    finalColor.rgb = spriteColor.rgb + galaxyColor.rgb * _GalaxyIntensity;
                    finalColor.a = spriteColor.a;
                }
                
                return finalColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
