Shader "Unlit/DynHoleUnlit"
{
	Properties
	{
		_BaseColor ("BaseColor", Color) = (1, 0, 0, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			float4 _BaseColor;
			float4 _HoleMakerPosition;
			float _HoleMakerRadius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _BaseColor;

				if (distance(i.worldPos, _HoleMakerPosition) < _HoleMakerRadius)
				{
					col = float4(0.0, 1.0, 0.0, 1.0);
					col.a = 0.0;
					discard;
				}


				return col;
			}
			ENDCG
		}
	}
}
