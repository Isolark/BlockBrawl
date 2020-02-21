// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

Shader "Screen Transitions Pro/Transition Single Focus Noise"
{
	Properties
	{
		_Color("Background Color", Color) = (1,1,1,1)
		_Texture("Background Texture", 2D) = "black" {}
		_Cutoff("Cutoff", Range(0, 1.5)) = 0
		_Falloff("Noise Falloff", Range(0, 1)) = 0
		_NoiseScale("Noise Scale", float) = 1
		_NoiseSpeedX("Noise Speed X", float) = 0
		_NoiseSpeedY("Noise Speed Y", float) = 0
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
			half _Cutoff, _Falloff;
			float _FocusX, _FocusY, _NoiseScale, _NoiseSpeedX, _NoiseSpeedY;

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
				
				fixed4 tex = tex2D(_MainTex, i.uv);

				float n = Noise(uv, _NoiseScale, _NoiseSpeedX, _NoiseSpeedY);
				float l = length(uv - float2(_FocusX, _FocusY));

				float t = 1 - (l - 2.5 + _Cutoff + _Falloff) / _Falloff;
				float s = smoothstep(0, 1, t);	
				float ler = s < 0.5 ? lerp(0, n, s * 2) : lerp(n, 1, s * 2 - 1);
				float ns = step(0.5, ler);

				return lerp(bg, tex, ns);
			}
			ENDCG
		}
	}
}
