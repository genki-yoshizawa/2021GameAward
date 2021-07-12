Shader "Custom/PanelBlightShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            float blightCol =((_SinTime.x * 10.0 - 10) + 10.0) * 0.05;
            o.Albedo = c.rgb + float3(blightCol, blightCol, blightCol);// (float3((1 + 1.0) * .5, (1 + 1.0) * .5, (1 + 1.0) * .5));

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}


//Shader "PanelBlightShader" {
//	Properties{
//		_Color("Main Color", Color) = (1,1,1,1)
//		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
//	}
//
//		SubShader{
//			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
//			LOD 200
//
//		CGPROGRAM
//		#pragma surface surf Lambert alpha
//
//		sampler2D _MainTex;
//		fixed4 _Color;
//
//		struct Input {
//			float2 uv_MainTex;
//		};
//
//		void surf(Input IN, inout SurfaceOutput o) {
//			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;// float4((_SinTime.x + 1.0) * .5, (_SinTime.x + 1.0) * .5, (_SinTime.x + 1.0) * .5, 1.0);
//			o.Alpha = c.a;
//		}
//		ENDCG
//	}
//
//		Fallback "Transparent/VertexLit"
//}