using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogoSmoke : MonoBehaviour
{
    [SerializeField]
    private GameObject _SmokeLeft, _SmokeRight;

    [SerializeField]
    private GameObject  _StartButton;

    bool _IsStart = false;

    float _Distance = 6.0f, _Hight = 0.0f;

    float _Time = 0.5f;

    float _TimeCount = 0;

    float _MaxSize = 3.5f;


    private Vector3 _LeftStartPos, _RightStartPos;

    void Start()
    {
        _LeftStartPos = _SmokeLeft.transform.position;
        _RightStartPos = _SmokeRight.transform.position;

        _SmokeLeft.GetComponent<ParticleSystem>().Stop();
        _SmokeRight.GetComponent<ParticleSystem>().Stop();
        _StartButton.GetComponent<Animator>().SetBool("start", false);


    }

    void FixedUpdate()
    {
        Effect();
    }

    void Effect()
    {
        if (!_IsStart) return;

        _TimeCount += Time.deltaTime;


        if (_TimeCount <= _Time)
        {
            Vector3 pos;
            Vector3 size;

            float s = (_TimeCount / _Time) * _MaxSize;
            size = new Vector3(s, s, s);


            pos = Vector3.Slerp(_RightStartPos, _RightStartPos + new Vector3(_Distance,_Hight,0), (_TimeCount / _Time));



            _SmokeRight.transform.position = pos;
            _SmokeRight.transform.localScale = size;

            pos = Vector3.Slerp(_LeftStartPos, _LeftStartPos + new Vector3(-_Distance,_Hight,0), (_TimeCount / _Time));
            _SmokeLeft.transform.position = pos;
            _SmokeLeft.transform.localScale = size;
            return;
        }

        _IsStart = false;
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "TitleLogo") //Objectタグの付いたゲームオブジェクトと衝突したか判別
        {
            _SmokeLeft.GetComponent<ParticleSystem>().Play();
            _SmokeRight.GetComponent<ParticleSystem>().Play();
            _IsStart = true;
            _StartButton.GetComponent<Animator>().SetBool("start",true);
        }

    }
}
