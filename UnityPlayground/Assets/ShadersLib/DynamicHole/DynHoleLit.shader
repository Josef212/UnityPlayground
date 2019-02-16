Shader "Custom/DynHoleLit" 
{
	Properties 
	{
		_BaseColor("BaseColor", Color) = (1, 0, 0, 1)
		_MainTex("Albedo", 2D) = "white" {}
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.1
	}
	SubShader 
	{

		Pass
		{
			Tags
			{
				"RenderType" = "Transparent"
				"RenderQueue" = "Transparent"
				"LightMode" = "ForwardBase"
			}
			
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityPBSLighting.cginc"

			#pragma target 3.0

			float4 _BaseColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _HoleMakerPosition;
			float _HoleMakerRadius;

			float _Metallic;
			float _Smoothness;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			Interpolators vert(appdata v)
			{
				Interpolators i;

				i.vertex = UnityObjectToClipPos(v.vertex);
				i.worldPos = mul(unity_ObjectToWorld, v.vertex);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return i;
			}

			float4 frag(Interpolators i) : SV_TARGET
			{
				if (distance(i.worldPos, _HoleMakerPosition) < _HoleMakerRadius)
				{
					//discard;
					return float4(0.0, 0.0, 0.0, 0.0);
				}

				i.normal = normalize(i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _BaseColor.rgb;

				float3 specularTint;
				float oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);

				UnityLight light;
				light.color = lightColor;
				light.dir = lightDir;
				light.ndotl = DotClamped(i.normal, lightDir);

				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

				float4 color = UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,
					light, indirectLight
				);

				return color;
			}

			ENDCG
		}

	}
}
