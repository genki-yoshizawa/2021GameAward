using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootSmoke : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _FootSmoke;

    private Vector3 _PrePosition;
    private Vector3 _CurPos;

    void Start()
    {
        _PrePosition = _CurPos =this.transform.position;
        _PrePosition.y = 0;
    }


    void Update()
    {
        _CurPos = this.transform.position;
        _CurPos.y = 0;

        float distance = Vector3.Distance(_CurPos, _PrePosition);

        Debug.Log(distance);


        // 速度が0以外なら
        if (distance != 0)
        {
            Debug.Log("ケムリ再生");
            // 再生
            if (!_FootSmoke.isEmitting)
            {
                _FootSmoke.Play();
            }
        }
        else
        {
            Debug.Log("ケムリ停止");
            // 停止
            if (_FootSmoke.isEmitting)
            {
                _FootSmoke.Stop();
            }
        }

        _PrePosition = _CurPos;
        _PrePosition.y = 0;
    }
}
