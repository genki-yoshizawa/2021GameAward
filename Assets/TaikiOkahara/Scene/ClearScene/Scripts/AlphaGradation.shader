Shader "Custom/AlphaGradation"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Alpha("Alpha",Range(0,1.5)) = 0.0
	}

		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				half2 texcoord  : TEXCOORD0;
			};

			fixed _Alpha;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			// 頂点シェーダーの基本
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			// 通常のフラグメントシェーダー
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, IN.texcoord);

				half alpha = 0;
				if (_Alpha < IN.texcoord.x)
				{
					alpha = 0;
				}
				else
				{
					alpha = (_Alpha - IN.texcoord.x <= 0.5) ? ((_Alpha - IN.texcoord))/0.5 * color.a : color.a;
				}

				return fixed4(color.r, color.g, color.b,alpha);

			}
			ENDCG
		}
	}
	FallBack "UI/Default"
}