using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelMarkerScript : MonoBehaviour
{
    // パネルからセット関数を通じて設定されるサイクル時間
    private float _CycleTime = 0.0f;
    // パネルからセット関数を通じて設定される最大サイズ倍率
    private float _MaxScale = 1.0f;
    // パネルからセット関数を通じて設定されるsin波フラグ(falseはのこぎり波)
    private bool  _isSinPulse = false;
    // 経過時間
    private float _PassedTime = 0.0f;

    private Vector3 _SaveScale;

    // Start is called before the first frame update
    void Start()
    {
        _PassedTime = 0.0f;
        _SaveScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        _PassedTime += Time.deltaTime;


        Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        if (_isSinPulse)
        {
            scale.x = _SaveScale.x + Mathf.Abs((1.0f - _SaveScale.x) * Mathf.Sin(180.0f * _PassedTime / _CycleTime));
            //scale.y = _SaveScale.y + Mathf.Abs((1.0f - _SaveScale.y) * Mathf.Sin(180.0f * _PassedTime / _CycleTime));
            scale.z = _SaveScale.z + Mathf.Abs((1.0f - _SaveScale.z) * Mathf.Sin(180.0f * _PassedTime / _CycleTime));
        }
        else
        {
            if (_PassedTime < _CycleTime * 0.5f)
            {
                scale.x = _SaveScale.x + (1.0f - _SaveScale.x) / (_CycleTime * 0.5f) * _PassedTime;
                //scale.y = _SaveScale.y + (1.0f - _SaveScale.y) / (_CycleTime * 0.5f) * _PassedTime;
                scale.z = _SaveScale.z + (1.0f - _SaveScale.z) / (_CycleTime * 0.5f) * _PassedTime;
            }
            else
            {
                scale.x = 1.0f - (1.0f - _SaveScale.x) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                //scale.y = 1.0f - (1.0f - _SaveScale.y) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                scale.z = 1.0f - (1.0f - _SaveScale.z) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
            }
        }

    }

    public void SetCycleTime(float time) { _CycleTime = time; }
    public void SetMaxScale(float scale) { _MaxScale= scale; }
    public void SetisSinPulse(bool isSin) { _isSinPulse = isSin; }
}
