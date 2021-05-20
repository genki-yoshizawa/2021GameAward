Shader "Custom/AlphaGradation"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		//_FadeTex("Fade Texture", 2D) = "white" {}
		//_Color("Tint", Color) = (1,1,1,1)
		_Alpha("Alpha",Range(0,1)) = 0
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

			//fixed4 _Color;
			fixed _Alpha;
			sampler2D _MainTex;
			//sampler2D _FadeTex;
			float4 _MainTex_ST;

			// ���_�V�F�[�_�[�̊�{
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
//#ifdef UNITY_HALF_TEXEL_OFFSET
//				OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1,1);
//#endif
				//OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				return OUT;
			}

			// �ʏ�̃t���O�����g�V�F�[�_�[
			fixed4 frag(v2f IN) : SV_Target
			{
				//half alpha = tex2D(_MainTex, IN.texcoord).a;
				//alpha = saturate(1 - alpha - (_Alpha * 2 - 1));
				fixed4 color = tex2D(_MainTex, IN.texcoord);
				//half fadeAlpha = tex2D(_FadeTex, IN.texcoord).a;
				//half alpha = (fadeAlpha > 0.5) ? color.a : 0;
				half alpha = (_Alpha > IN.texcoord.x)? color.a : ( 1 - IN.texcoord.x/(_Alpha)) * color.a;
				return fixed4(color.r, color.g, color.b,alpha);
				//return color;

			}
			ENDCG
		}
	}
	FallBack "UI/Default"
}