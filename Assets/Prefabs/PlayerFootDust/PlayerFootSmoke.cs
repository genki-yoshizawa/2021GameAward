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

        //Debug.Log(distance);


        // ���x��0�ȊO�Ȃ�
        if (distance != 0)
        {
            // �Đ�
            if (!_FootSmoke.isEmitting)
            {
                _FootSmoke.Play();
            }
        }
        else
        {
            // ��~
            if (_FootSmoke.isEmitting)
            {
                _FootSmoke.Stop();
            }
        }

        _PrePosition = _CurPos;
        _PrePosition.y = 0;
    }
}
