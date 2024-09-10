Shader "Unlit/Cloud Crawl 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {} // Distortion
        _ScrollSpeed ("ScrollSpeed", Range(0,1)) = 0.2
        _DistortionLevel ("DistortionLevel", Range(0,1)) = 0.2
        _CloudOpacity ("Cloud Opacity", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"}
        Tags {"Queue"="Transparent"}
        //potentially set the queue type to transparent
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _ScrollSpeed;
            float _DistortionLevel;
            float _CloudOpacity;


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


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 noiseUV = i.uv + float2(_ScrollSpeed * _Time.y, _ScrollSpeed * _Time.y);
                float noise = tex2D(_NoiseTex, noiseUV).r;

                float2 distortedUV = i.uv + (noise - 0.5) * _DistortionLevel;

                fixed4 cloudColour = tex2D(_MainTex, distortedUV);

                cloudColour.a *= _CloudOpacity;

                return cloudColour;
                
            }
            ENDCG
        }
    }
}
