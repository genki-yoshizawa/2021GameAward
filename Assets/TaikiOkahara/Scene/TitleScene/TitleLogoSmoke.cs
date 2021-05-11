using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogoSmoke : MonoBehaviour
{
    [SerializeField]
    private GameObject _SmokeLeft, _SmokeRight;

    bool _IsStart = false;

    [SerializeField]
    float _Distance, _Hight;

    [SerializeField]
    float _Time;

    float _TimeCount = 0;

    [SerializeField]
    float _MaxSize;


    private Vector3 _LeftStartPos, _RightStartPos;

    void Start()
    {
        _LeftStartPos = _SmokeLeft.transform.position;
        _RightStartPos = _SmokeRight.transform.position;

        _SmokeLeft.GetComponent<ParticleSystem>().Stop();
        _SmokeRight.GetComponent<ParticleSystem>().Stop();
    }

    void Update()
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


            pos = _RightStartPos + new Vector3((_TimeCount / _Time) * _Distance, (_TimeCount / _Time) * _Hight, 0);
            _SmokeRight.transform.position = pos;
            _SmokeRight.transform.localScale = size;

            pos = _LeftStartPos + new Vector3((_TimeCount / _Time) * -_Distance, (_TimeCount / _Time) * _Hight, 0);
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
            Debug.Log("Hit!!");
            _IsStart = true;
            //Destroy(this.gameObject); //衝突したゲームオブジェクトを削除
        }

    }
}
