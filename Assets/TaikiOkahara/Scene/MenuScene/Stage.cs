using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{
    [SerializeField]
    string _SceneName;

    [SerializeField]
    private float _Range;

    [SerializeField]
    private float _Speed;

    [SerializeField]
    private int _ClearPercentage;

    private float _StartPos;

    private float _StartY;

    void Start()
    {
        _StartPos = Random.Range(-_Range, _Range);
        _StartY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        float sin = Mathf.Sin((Time.time + _StartPos)* _Speed) * _Range;
        Vector3 pos = transform.position;
        pos.y = sin + _StartY;
        transform.position = pos;


        transform.Rotate(0.01f, 0.01f, 0.0f);
    }

    public string GetSceneName()
    {
        return _SceneName;
    }

    public int GetClearParsentage()
    {
        return _ClearPercentage;
    }
}
