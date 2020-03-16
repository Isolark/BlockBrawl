// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

Shader "Screen Transitions Pro/Transition Fade"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0
		[MaterialToggle]_Additive("Additive Color", int) = 0
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USE_TEXTURE

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex, _Texture;
			fixed4 _Color;
			half _Cutoff;
			int _Additive;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex = tex2D(_MainTex, i.uv);

				fixed4 bg = _Color;

				#if USE_TEXTURE
					bg = tex2D(_Texture, i.uv);
				#endif

				return lerp(tex + bg * _Cutoff * _Additive, bg, _Cutoff);
			}
			ENDCG
		}
	}
}
