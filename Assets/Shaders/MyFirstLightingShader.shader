
Shader "Custom/My First Lighting Shader" 
{
	Properties 
	{
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
	}

	SubShader 
	{
		Pass 
		{
			Tags {
					"LightMode" = "ForwardBase"
				 }


			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			
			#include "UnityStandardBRDF.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct Interpolators 
			{
				float4 position : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD2;
			};

			struct VertexData {
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};


			Interpolators  MyVertexProgram (VertexData v)
			{
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				i.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = v.uv*_MainTex_ST.xy;
				i.normal = v.normal;
				return i;
			}

			float4  MyFragmentProgram (Interpolators i) : SV_TARGET 
			{			
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
				float3 diffuse = albedo * lightColor * DotClamped(lightDir, i.normal);
				return float4(diffuse, 1);
			}

			ENDCG
		}
	}
}