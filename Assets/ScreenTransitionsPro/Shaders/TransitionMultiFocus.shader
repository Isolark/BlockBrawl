// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

Shader "Screen Transitions Pro/Transition Multi Focus"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 1.5)) = 0
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		[HideInInspector]_Focus1("Use Focus 1", int) = 0
		[HideInInspector]_FocusX1("Focus Position X 1", float) = 0
		[HideInInspector]_FocusY1("Focus Position Y 1", float) = 0
		[HideInInspector]_Focus2("Use Focus 2", int) = 0
		[HideInInspector]_FocusX2("Focus Position X 2", float) = 0
		[HideInInspector]_FocusY2("Focus Position Y 2", float) = 0
		[HideInInspector]_Focus3("Use Focus 3", int) = 0
		[HideInInspector]_FocusX3("Focus Position X 3", float) = 0
		[HideInInspector]_FocusY3("Focus Position Y 3", float) = 0
		[HideInInspector]_Focus4("Use Focus 4", int) = 0
		[HideInInspector]_FocusX4("Focus Position X 4", float) = 0
		[HideInInspector]_FocusY4("Focus Position Y 4", float) = 0
		[HideInInspector]_Focus5("Use Focus 5", int) = 0
		[HideInInspector]_FocusX5("Focus Position X 5", float) = 0
		[HideInInspector]_FocusY5("Focus Position Y 5", float) = 0
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
			int _Focus1, _Focus2, _Focus3, _Focus4, _Focus5;
			float _FocusX1, _FocusX2, _FocusX3, _FocusX4, _FocusX5,
				  _FocusY1, _FocusY2, _FocusY3, _FocusY4, _FocusY5;

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
					_FocusX1 *= r;
					_FocusX2 *= r;
					_FocusX3 *= r;
					_FocusX4 *= r;
					_FocusX5 *= r;
				}
				else
				{
					float r = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
					uv.y *= r;
					_FocusY1 *= r;
					_FocusY2 *= r;
					_FocusY3 *= r;
					_FocusY4 *= r;
					_FocusY5 *= r;
				}
				
				fixed4 bg = _Color;

				#if USE_TEXTURE
					bg = tex2D(_Texture, i.uv);
				#endif
				
				if (_Focus1 == 1)
					if (length(uv - float2(_FocusX1, _FocusY1)) < 1.5 - _Cutoff)
						bg = tex2D(_MainTex, i.uv);

				if (_Focus2 == 1)
					if (length(uv - float2(_FocusX2, _FocusY2)) < 1.5 - _Cutoff)
						bg = tex2D(_MainTex, i.uv);

				if (_Focus3 == 1)
					if (length(uv - float2(_FocusX3, _FocusY3)) < 1.5 - _Cutoff)
						bg = tex2D(_MainTex, i.uv);

				if (_Focus4 == 1)
					if (length(uv - float2(_FocusX4, _FocusY4)) < 1.5 - _Cutoff)
						bg = tex2D(_MainTex, i.uv);

				if (_Focus5 == 1)
					if (length(uv - float2(_FocusX5, _FocusY5)) < 1.5 - _Cutoff)
						bg = tex2D(_MainTex, i.uv);

				return bg;
			}
			ENDCG
		}
	}
}
