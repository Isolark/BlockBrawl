// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

Shader "Screen Transitions Pro/Transition Displacement"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0
		[MaterialToggle]_FlipH("Flip horizontally", int) = 0
		[MaterialToggle]_FlipV("Flip vertically", int) = 0
		[NoScaleOffset]_Transition("Transition Texture", 2D) = "white" {}
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
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex, _Transition, _Texture;
			half4 _MainTex_TexelSize;
			fixed4 _Color;
			half _Cutoff;
			int _Fit, _FlipH, _FlipV;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv2 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv.y = 1 - o.uv.y;
				#endif

				if (_FlipH)
					o.uv2.x = 1 - o.uv2.x;

				if (_FlipV)
					o.uv2.y = 1 - o.uv2.y;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{				
				float2 uv = i.uv2;			
				fixed4 t = tex2D(_Transition, uv);

				#if !UNITY_COLORSPACE_GAMMA
					t.rgb = LinearToGammaSpace(t.rgb);
				#endif

				t.r = (t.r - 0.5) * 4;
				t.g = (t.g - 0.5) * 4;

				if (abs(t.r) < 0.01)
					t.r = 0;

				if (abs(t.g) < 0.01)
					t.g = 0;

				fixed4 bg = _Color;

				#if USE_TEXTURE
					bg = tex2D(_Texture, i.uv);
				#endif

				if (t.b < _Cutoff)
					return bg;

				float offsetX = _FlipH == 0 ? -t.r : t.r;
				float offsetY = _FlipV == 0 ? -t.g : t.g;

				float2 d = float2(offsetX, offsetY);
				return tex2D(_MainTex, i.uv + saturate(_Cutoff - t.a) * d);
			}
			ENDCG
		}
	}
}
