Shader "PortalGate/PortalGate"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float4x4 _MainCameraViewProj;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 sposOnMain : TEXCOORD0;
				//half2 uv : TEXCOORD0;
			};

			v2f vert( float3 v : POSITION)
			{
				v2f o;

				float3 posWorld = mul(unity_ObjectToWorld, float4(v, 1.0)).xyz;
				float4 clipPos = mul(UNITY_MATRIX_VP, float4(posWorld, 1));
				float4 clipPosOnMain = mul(_MainCameraViewProj, float4(posWorld, 1));
				//float3 posView = mul(UNITY_MATRIX_V, float4(posWorld, 1.0)).xyz;

				//float4 clipPos = mul(UNITY_MATRIX_P, float4(posView, 1.0));
				//float4 clipPosOnMain = mul(_MainCameraProj, float4(posView, 1.0));

				o.pos = clipPos;
				o.sposOnMain = ComputeScreenPos(clipPosOnMain);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 sUv = i.sposOnMain.xy / i.sposOnMain.w;
				//float4 col = float4(sUv, 0, 1);
				float4 col = tex2D(_MainTex, sUv);

				return col;
			}
			ENDCG
		}
	}
}
