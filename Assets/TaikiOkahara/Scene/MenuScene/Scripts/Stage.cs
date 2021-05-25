using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{
    [SerializeField]
    string _SceneName;

    [SerializeField]
    private int _ClearPercentage;

    [SerializeField]
    private Image _StageName;

    [SerializeField]
    private int _StageNumber = 0;

    [SerializeField]
    public int _MaxTurn = 0;
    [SerializeField]
    public int _3Star = 0;
    [SerializeField]
    public int _2Star = 0;
    [SerializeField]
    public int _1Star = 0;


    private float _Range = 0.025f;
    private float _StartPos;
    private float _Speed = 1.0f;


    private float _Time = 0;

    void Start()
    {
        _StartPos = Random.Range(0, 2 * Mathf.PI);
        _Time = _StartPos;
    }

    // Update is called once per frame
    void Update()
    {
        _Time += Time.deltaTime;
        float sin = Mathf.Sin(_Time * _Speed) * _Range;
        Vector3 pos = transform.position;
        pos.y = sin;// + _StartY;
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

    public int GetMatTurn()
    {
        return _MaxTurn;
    }

    public int GetStageNumber()
    {
        return _StageNumber;
    }
}
