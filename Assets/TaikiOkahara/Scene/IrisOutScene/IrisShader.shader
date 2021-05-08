Shader"Custom/IrisShader"{
    Properties{
        _MainTex("None",2D) = "white"{}
        _Length("Length",Range(0,0.5)) = 0.5
        _Radius("Radius",Range(0,2)) = 2

    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            Pass{
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex:POSITION;
                    float2 uv:TEXCOORD0;
                };

                struct v2f {
                    float4 vertex:SV_POSITION;
                    float2 uv:TEXCOORD0;
                };

                sampler2D _MainTex;
                float _Length;
                float _Radius;
                float2 _PlayerPosition;
                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) :COLOR{
                    //�����ŃJ�����Ɏʂ��Ă���F�����擾����
                    fixed4 c = tex2D(_MainTex,i.uv);
                    //uv���W�̂�����0.5���������l�̐�Βl�i�����ł͉�ʂ̐^�񒆂�(0.5,0.5)�Ƃ��Ă���j
                    //fixed l = abs(i.uv.y - 0.5);
                    //l�̒l���QLength�ȉ��Ȃ炻�̂܂ܕ`�悷��B�Ⴄ�̂Ȃ獕���`��


                    _PlayerPosition -= fixed2(0.5, 0.5);//�ʒu���W���������璆���ֈړ�
                    _PlayerPosition.x *= 16.0 / 9.0;//��ʃA�X�y�N�g��

                    i.uv -= fixed2(0.5, 0.5);//�ʒu���W���������璆���ֈړ�
                    i.uv.x *= 16.0 / 9.0;//��ʃA�X�y�N�g��

                    fixed4 color = (distance(i.uv, _PlayerPosition) < _Radius) ? c : fixed4(0, 0, 0, 1);

                return color;
                }
                ENDCG
            }
        }
}