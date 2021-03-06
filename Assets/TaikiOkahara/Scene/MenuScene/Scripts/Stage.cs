using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{
    [SerializeField]
    string _SceneName;

    //[SerializeField]
    //private int _ClearPercentage;

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

    float _YPos = -0.3f;

    private float _Time = 0;

    bool _Clear = false;

    [SerializeField]
    bool _Lock = true;


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
        pos.y = sin + _YPos;
        transform.position = pos;


        transform.Rotate(0.01f, 0.01f, 0.0f);


        if (_Lock)
        {
            //this.transform.GetChild(0).transform.GetComponent<Material>().SetColor("_BaseColor", new Color(0, 0, 0));
            this.transform.GetChild(0).transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        }
        else
        {
            //this.transform.GetChild(0).transform.;
            this.transform.GetChild(0).transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }

    public string GetSceneName(){ return _SceneName;}
    //public int GetClearParsentage(){ return _ClearPercentage;}
    //public void SetClearParsentage(int clean){ _ClearPercentage = clean;}
    public int GetStageNumber(){ return _StageNumber;}

    public void Clear() {
        
        _Clear = true;
        _Lock = false;
    }
    public bool GetClear() { return _Clear; }

    public void SetLock(bool bLock)
    {
        _Lock = bLock;

        return;
    }

    public bool GetLock() { return _Lock; }
}
