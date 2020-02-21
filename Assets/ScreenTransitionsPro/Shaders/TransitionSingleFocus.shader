// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

Shader "Screen Transitions Pro/Transition Single Focus"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 1.5)) = 0
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		[HideInInspector]_FocusX("Focus Position X", float) = 0
		[HideInInspector]_FocusY("Focus Position Y", float) = 0
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

			sampler2D _MainTex, _Texture;
			half4 _MainTex_TexelSize;
			fixed4 _Color;
			half _Cutoff;
			float _FocusX, _FocusY;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv.y = 1 - o.uv.y;
				#endif

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;

				if (_MainTex_TexelSize.x > _MainTex_TexelSize.y)
				{
					float r = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
					uv.x *= r;
					_FocusX *= r;
				}
				else
				{
					float r = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
					uv.y *= r;
					_FocusY *= r;
				}

				fixed4 bg = _Color;

				#if USE_TEXTURE
					bg = tex2D(_Texture, i.uv);
				#endif

				if (length(uv - float2(_FocusX, _FocusY)) > (1.5 - _Cutoff))
					return bg;

				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
