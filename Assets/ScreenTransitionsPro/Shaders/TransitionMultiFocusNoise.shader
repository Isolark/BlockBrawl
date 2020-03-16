// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

Shader "Screen Transitions Pro/Transition Multi Focus Noise"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 2.5)) = 0
		_Falloff("Noise Falloff", Range(0, 1)) = 0
		_NoiseScale("Noise Scale", float) = 1
		_NoiseSpeedX("Noise Speed X", float) = 0
		_NoiseSpeedY("Noise Speed Y", float) = 0
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
			half _Cutoff, _Falloff;
			int _Focus1, _Focus2, _Focus3, _Focus4, _Focus5;
			float _FocusX1, _FocusX2, _FocusX3, _FocusX4, _FocusX5,
				  _FocusY1, _FocusY2, _FocusY3, _FocusY4, _FocusY5,
				  _NoiseScale, _NoiseSpeedX, _NoiseSpeedY;

			float noise_randomValue (float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
			}

			float noise_interpolate (float a, float b, float t)
			{
				return (1.0 - t) * a + (t * b);
			}

			float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac(uv);
				f = f * f * (3.0 - 2.0 * f);

				uv = abs(frac(uv) - 0.5);
				float2 c0 = i + float2(0.0, 0.0);
				float2 c1 = i + float2(1.0, 0.0);
				float2 c2 = i + float2(0.0, 1.0);
				float2 c3 = i + float2(1.0, 1.0);
				float r0 = noise_randomValue(c0);
				float r1 = noise_randomValue(c1);
				float r2 = noise_randomValue(c2);
				float r3 = noise_randomValue(c3);

				float bottomOfGrid = noise_interpolate(r0, r1, f.x);
				float topOfGrid = noise_interpolate(r2, r3, f.x);
				float t = noise_interpolate(bottomOfGrid, topOfGrid, f.y);
				return t;
			}

			float Noise(float2 UV, float Scale, float SpeedX, float SpeedY)
			{
				float t = 0.0;
				for(int i = 0; i < 3; i++)
				{
					float freq = pow(2.0, float(i));
					float amp = pow(0.5, float(3 - i));
					t += valueNoise(float2(UV.x * Scale/freq + _Time.y * SpeedX, UV.y * Scale/freq + _Time.y * SpeedY)) * amp;
				}

				return t;
			}

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

				fixed4 tex = tex2D(_MainTex, i.uv);
				float n = Noise(uv, _NoiseScale, _NoiseSpeedX, _NoiseSpeedY);
				float l, t, tMin = 1;

				if (_Focus1 == 1)
				{
					l = length(uv - float2(_FocusX1, _FocusY1));
					t = (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
					tMin = min(tMin, t);
				}

				if (_Focus2 == 1)
				{
					l = length(uv - float2(_FocusX2, _FocusY2));
					t = (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
					tMin = min(tMin, t);
				}

				if (_Focus3 == 1)
				{
					l = length(uv - float2(_FocusX3, _FocusY3));
					t = (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
					tMin = min(tMin, t);
				}

				if (_Focus4 == 1)
				{
					l = length(uv - float2(_FocusX4, _FocusY4));
					t = (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
					tMin = min(tMin, t);
				}

				if (_Focus5 == 1)
				{
					l = length(uv - float2(_FocusX5, _FocusY5));
					t = (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
					tMin = min(tMin, t);
				}

				float s = smoothstep(0, 1, tMin);
				float ler = s < 0.5 ? lerp(0, n, s * 2) : lerp(n, 1, s * 2 - 1);
				float ns = step(0.5, ler);

				return lerp(tex, bg, ns);
			}
			ENDCG
		}
	}
}
