// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Death"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	    _Fade ("Fade", Color) = (1, 1, 1, 1)
        _DeathTime ("Death time", Float) = 0.0
        _FadeTime ("Fade time", Float) = 0.1
        _FadeWidth ("Fade width", Float) = 0.1
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
                float2 world : TEXCOORD1;
};
			
			fixed4 _Color;
		
			v2f vert(appdata_t IN)
			{
				v2f OUT;

				OUT.vertex = mul(unity_MatrixMVP, IN.vertex);
				OUT.texcoord = float2(IN.texcoord);
				OUT.color = IN.color * _Color;
                OUT.world = mul(unity_ObjectToWorld, IN.vertex).xy;
			    
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
		    fixed4 _Fade;
            float _DeathTime;
            float _FadeTime;
            float _FadeWidth;
            //float4 _Time;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord);
			    c *= _Color; 
			    c += (_Fade * _Fade.a) * c.a;
			    c.rgb *= c.a;
    
                //c.rgb *= ;
                float noise = tex2D(_NoiseTex, IN.world).x - (_Time.y - _DeathTime) / _FadeTime;
                if (noise < _FadeWidth)
                    c.rgb = _Fade.rgb * c.a;
                //noise = saturate(noise);
                clip(noise);
                //c.rgb = noise;
                //c.a = saturate(c.a - noise);
				return c;
			}
		ENDCG
		}
	}
}