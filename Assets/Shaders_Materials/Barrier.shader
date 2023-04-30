// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CloverSwatch/BinstonBarrier"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,0)
	}

	SubShader
	{
		Blend One One
		ZWrite Off
		Cull Off

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewDir : TEXCOORD1;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}
			
			sampler2D _CameraDepthNormalsTexture;
			fixed4 _Color;

			float triWave(float t, float offset, float yOffset)
			{
				return saturate(abs(frac(offset + t) * 2 - 1) + yOffset);
			}

			fixed4 texColor(v2f i, float rim)
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				return mainTex.r * _Color + mainTex.g * _Color;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * 2;

				fixed4 glowColor = fixed4(lerp(_Color.rgb, fixed3(1, 1, 1), pow(rim, 4)), 1);
				fixed4 hexes = texColor(i, rim);

				fixed4 col = _Color * _Color.a + glowColor * rim + hexes;
				return col;
			}
			ENDCG
		}
	}
}
