Shader "Custom/GrassShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveFrequency ("Wave Frequency", Float) = 2.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.05
        _WaveSpeed ("Wave Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend One OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _WaveSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate wave offset based on time and UV coordinates
                float waveOffset = _Time.y * _WaveSpeed;

                // Calculate vertex displacement based on sine wave
                float wave = sin((i.uv.x * _WaveFrequency + waveOffset) * 2.0 * 3.14159265) * _WaveAmplitude;

                // Apply the wave effect to the Y coordinate of the UV
                i.uv.y += wave;

                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
