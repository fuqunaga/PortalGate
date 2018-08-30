Shader "Custom/PortalGate"
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
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 spos : TEXCOORD0;
				//half2 uv : TEXCOORD0;
			};

			v2f vert( appdata_img v )
			{
				v2f o;

				o.pos = UnityObjectToClipPos (v.vertex);
				o.spos = ComputeScreenPos(o.pos);
				//o.uv = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 sUv = i.spos.xy / i.spos.w;
				//float4 col = float4(sUv, 0, 1);
				float4 col = tex2D(_MainTex, sUv);

				return col;
			}
			ENDCG
		}
	}
}
