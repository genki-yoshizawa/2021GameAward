using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelMarkerScript : MonoBehaviour
{
    // �p�l������Z�b�g�֐���ʂ��Đݒ肳���T�C�N������
    private float _CycleTime = 0.0f;
    // �p�l������Z�b�g�֐���ʂ��Đݒ肳���ő�T�C�Y�{��
    private float _MaxScale = 0.1f;
    // �p�l������Z�b�g�֐���ʂ��Đݒ肳���sin�g�t���O(false�͂̂�����g)
    private bool  _isSinPulse = false;
    // �o�ߎ���
    private float _PassedTime = 0.0f;

    private Vector3 _SaveScale;

    // Start is called before the first frame update
    void Start()
    {
        _PassedTime = 0.0f;
        _SaveScale = transform.localScale;
        //_SaveScale.x = transform.localScale.x;
        //_SaveScale.y = transform.localScale.y;
        //_SaveScale.z = transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        _PassedTime += Time.deltaTime;
        while (true)
        {
            if (_PassedTime > _CycleTime)
                _PassedTime -= _CycleTime;
            else
                break;
        }


        Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        if (_isSinPulse)
        {
            scale.x = _SaveScale.x + Mathf.Abs((_MaxScale - _SaveScale.x) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
            //scale.y = _SaveScale.y + Mathf.Abs((_MaxScale - _SaveScale.y) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
            scale.z = _SaveScale.z + Mathf.Abs((_MaxScale - _SaveScale.z) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
        }
        else
        {
            if (_PassedTime < _CycleTime * 0.5f)
            {
                scale.x = _SaveScale.x + (_MaxScale - _SaveScale.x) / (_CycleTime * 0.5f) * _PassedTime;
                //scale.y = _SaveScale.y + (_MaxScale - _SaveScale.y) / (_CycleTime * 0.5f) * _PassedTime;
                scale.z = _SaveScale.z + (_MaxScale - _SaveScale.z) / (_CycleTime * 0.5f) * _PassedTime;
            }
            else
            {
                scale.x = _MaxScale - (_MaxScale - _SaveScale.x) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                //scale.y = _MaxScale - (_MaxScale - _SaveScale.y) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                scale.z = _MaxScale - (_MaxScale - _SaveScale.z) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
            }
        }

        transform.localScale = scale;
    }

    public void SetCycleTime(float time) { _CycleTime = time; }
    public void SetMaxScale(float scale) { _MaxScale= scale; }
    public void SetisSinPulse(bool isSin) { _isSinPulse = isSin; }
}
