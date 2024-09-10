Shader "Unlit/Cloud Crawl 2"
{

    // potential methods to try - use an offset texture of noise to show clouds and shadows. Run a pass over the texture using time to make it fluffier. 
    // run the clouds at a higher spec and then pixelate them? 

    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0, 1)) = 0.1
        _Brightness ("Brightness", Range(0, 2)) = 0.1
        _Threshold ("Threshold", Range(0, 2)) = 0.1
        _Fluffiness ("Fluffiness", Range(0, 2)) = 0.1
        _IsSoft ("Is Soft", Float) = 0
        _PixelSize("Pixel Size", Range(0.001, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent"} 
        Tags {"Queue"="Transparent" }

        LOD 200

        Pass
        {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float _Speed;
            float _Brightness;
            float _Threshold;
            float _Fluffiness;
            float _IsSoft;
            float _PixelSize;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 pixelatedUV = floor(i.uv / _PixelSize) * _PixelSize;

                // Time offset for animation
                float timeOffset1 = _Time.y * 2.5 * _Speed / 10.0;
                float timeOffset2 = _Time.y * (2.5 + _Fluffiness * 3.0) * _Speed / 10.0;

                // Fetching two noise textures with time offsets
                float4 noise1 = tex2D(_NoiseTex, frac(pixelatedUV + timeOffset1));
                float4 noise2 = tex2D(_NoiseTex, frac(pixelatedUV + timeOffset2));

                // Combine both noise textures
                float4 combinedNoise = noise1 + noise2;

                float4 resultColor;

                if (_IsSoft > 0.5)
                {
                    // Smooth transitions (soft)
                    resultColor.rgb = smoothstep(_Threshold - 0.3, _Threshold + 0.3, combinedNoise.rgb) * _Brightness;
                    resultColor.a = smoothstep(_Threshold - 0.3, _Threshold + 0.3, combinedNoise.r);
                }
                else
                {
                    // Hard transitions (not soft)
                    resultColor.rgb = combinedNoise.rgb * _Brightness;
                    resultColor.a = combinedNoise.r < _Threshold ? 0.0 : 1.0;
                }

                return resultColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
