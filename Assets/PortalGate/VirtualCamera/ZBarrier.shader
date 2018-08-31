Shader "PortalGate/ZBarrier"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry-1"}
		ZTest Always
		ColorMask 0

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 vert (float4 v : POSITION) : SV_POSITION
			{
				 return UnityObjectToClipPos(v);
			}
			
			void frag ()
			{
			}
			ENDCG
		}
	}
}
