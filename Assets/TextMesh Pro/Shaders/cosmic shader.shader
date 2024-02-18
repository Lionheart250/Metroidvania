Shader "CosmicShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GalaxyTex ("Galaxy Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GalaxyIntensity ("Galaxy Intensity", Range(0, 2)) = 0.5
        _BlendMode ("Blend Mode", Range(0, 1)) = 0.5
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
        _ExcludedColor5 ("Excluded Color 5", Color) = (0,0,0,0) // Fifth color to exclude
        _ExcludedColor6 ("Excluded Color 6", Color) = (0,0,0,0) // Sixth color to exclude
        _ExcludedColor7 ("Excluded Color 7", Color) = (0,0,0,0) // Seventh color to exclude
        _ExcludedColor8 ("Excluded Color 8", Color) = (0,0,0,0) // Eighth color to exclude
        _ExcludedColor9 ("Excluded Color 9", Color) = (0,0,0,0) // Ninth color to exclude
        _ExcludedColor10 ("Excluded Color 10", Color) = (0,0,0,0) // Tenth color to exclude
        _ExcludedColor11 ("Excluded Color 11", Color) = (0,0,0,0) // Eleventh color to exclude
        _ExcludedColor12 ("Excluded Color 12", Color) = (0,0,0,0) // Twelfth color to exclude
        _ExcludedColor13 ("Excluded Color 13", Color) = (0,0,0,0) // Thirteenth color to exclude
        _ExcludedColor14 ("Excluded Color 14", Color) = (0,0,0,0) // Fourteenth color to exclude
        _ExcludedColor15 ("Excluded Color 15", Color) = (0,0,0,0) // Fifteenth color to exclude
        _ExcludedColor16 ("Excluded Color 16", Color) = (0,0,0,0) // Sixteenth color to exclude
        _ExcludedColor17 ("Excluded Color 17", Color) = (0,0,0,0) // Seventeenth color to exclude
        _ExcludedColor18 ("Excluded Color 18", Color) = (0,0,0,0) // Eighteenth color to exclude
        _ExcludedColor19 ("Excluded Color 19", Color) = (0,0,0,0) // Nineteenth color to exclude
        _ExcludedColor20 ("Excluded Color 20", Color) = (0,0,0,0) // Twentieth color to exclude
        _EnableEffects ("Enable Effects", Float) = 1.0 // Toggle to enable/disable effects
        _PatternPulsateSpeed ("Pattern Pulsate Speed", Range(0, 10)) = 2.0 // Adjust the speed of the pulsating pattern
        _PatternPulsateAmount ("Pattern Pulsate Amount", Range(0, 1)) = 0.5 // Adjust the amount of pulsating pattern effect
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
            half _IntensityMultiplier;
            half _Contrast;
            half _SwirlSpeed;
            half _SwirlAmount;
            half _PulsateSpeed;
            half _GlowIntensity;
            float4 _GlowColor;
            float4 _ExcludedColors[20]; // Adjusted to hold 20 elements
            float4 _ExcludedColor2;
            float4 _ExcludedColor3;
            float4 _ExcludedColor4;
            float4 _ExcludedColor5;
            float4 _ExcludedColor6;
            float4 _ExcludedColor7;
            float4 _ExcludedColor8;
            float4 _ExcludedColor9;
            float4 _ExcludedColor10;
            float4 _ExcludedColor11;
            float4 _ExcludedColor12;
            float4 _ExcludedColor13;
            float4 _ExcludedColor14;
            float4 _ExcludedColor15;
            float4 _ExcludedColor16;
            float4 _ExcludedColor17;
            float4 _ExcludedColor18;
            float4 _ExcludedColor19;
            float4 _ExcludedColor20;
            float _EnableEffects;
            half _PatternPulsateSpeed;
            half _PatternPulsateAmount;
            
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
                
                if (_EnableEffects == 0.0) {
                    // If effects are disabled, return the sprite color without any modifications
                    return spriteColor * _Color;
                }
                
                // Calculate the pulsating pattern effect
                float pulsateFactor = 0.5 + 0.5 * sin(_Time.y * _PatternPulsateSpeed);
                float pulsateOffset = (_PatternPulsateAmount / 2.0) * pulsateFactor;

                // Apply pulsating pattern to UV coordinates
                float2 pulsatingUV = i.galaxyTexUV;
                pulsatingUV.y += pulsateOffset;

                // Sample the galaxy texture with pulsating UV coordinates
                fixed4 pulsatingGalaxyColor = tex2D(_GalaxyTex, pulsatingUV);

                // Check if the pixel color matches any excluded color on the sprite
                bool excludePixel = false;
                for (int j = 0; j < 20; j++) {
                    if (distance(spriteColor.rgb, _ExcludedColors[j].rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor2.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor3.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor4.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor5.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor6.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor7.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor8.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor9.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor10.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor11.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor12.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor13.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor14.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor15.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor16.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor17.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor18.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor19.rgb) < 0.05 ||
                        distance(spriteColor.rgb, _ExcludedColor20.rgb) < 0.05) {
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
                float pulsatePatternFactor = 0.5 + 0.5 * sin(_Time.y * _PulsateSpeed);
                galaxyColor.rgb *= pulsatePatternFactor;
                float glowFactor = _GlowIntensity * (1.0 - spriteColor.a);
                fixed4 glow = _GlowColor * glowFactor;
                galaxyColor.rgb += glow.rgb;
                
                fixed4 finalColor;
                if (_BlendMode == 0) {
                    finalColor.rgb = lerp(spriteColor.rgb, pulsatingGalaxyColor.rgb, _GalaxyIntensity);
                    finalColor.a = spriteColor.a;
                } else {
                    finalColor.rgb = spriteColor.rgb + pulsatingGalaxyColor.rgb * _GalaxyIntensity;
                    finalColor.a = spriteColor.a;
                }
                
                return finalColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
