using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    

    private int _ChidCount;

    [SerializeField]
    private float _Radius  =1;

    [SerializeField]
    private GameObject _ChoiceStage;

    [SerializeField]
    private GameObject _DetailUI;

    
    private bool _Move = false;
    [SerializeField]
    private float _MoveSpeed;//�ړ��X�s�[�h

    private float _MoveTime = 0.0f;//�ړ����ԑ���

    private Vector3[] _StartPos;
    private Vector3[] _EndPos;




    public static int _ChoiceStageNumber = 0;
    public static int _MaxTurn = 0;
    public static int _3Star = 0;
    public static int _2Star = 0;
    public static int _1Star = 0;
    public static string _StageName;
    public static string _StageModelName;

    void Start()
    {
        _ChidCount = this.transform.childCount;
        _ChoiceStage = transform.GetChild(0).gameObject;
        _EndPos = new Vector3[_ChidCount];
        _StartPos = new Vector3[_ChidCount];

        for (int i = 0; i < _ChidCount; i++)
        {
            _StartPos[i] = transform.GetChild(i).position;

            Vector3 pos = this.transform.position;
            pos.z += -_Radius * Mathf.Cos(2 * Mathf.PI * i / _ChidCount);
            pos.x += _Radius * Mathf.Sin(2 * Mathf.PI * i / _ChidCount);
            pos.y = transform.GetChild(i).position.y;

            transform.GetChild(i).position = pos;
        }

        _DetailUI.gameObject.GetComponent<DetailUI>().DetailUIAnimation();
        _DetailUI.gameObject.GetComponent<DetailUINumber>().SetScore(_ChoiceStage.GetComponent<Stage>().GetClearParsentage());

    }

    void Update()
    {
        float dph = Input.GetAxis("D Pad Horizontal");

        if (dph > 0)
            InputRightButton();
        else if (dph < 0)
            InputLeftButton();
        else if (Input.GetKeyDown("joystick button 0") && !_Move)
            GameStart();


        foreach(Transform child in transform)
        {
            if(_ChoiceStage.transform.position.z > child.transform.position.z)
            {
                _ChoiceStage = child.gameObject;
            }
        }

        foreach (Transform child in transform)
        {
            if (_ChoiceStage.transform == child.transform)
            {

                child.GetChild(0).GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            else
            {
                child.GetChild(0).GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f, 1);
            }
        }


        StageMove();

       
    }


    void StageMove()
    {
        if (!_Move)
            return;

       
        for(int i = 0;i<_ChidCount;i++)
        {

            Vector3 pos = _StartPos[i];
            Vector3 nexPos = _EndPos[i];
            nexPos.y = transform.GetChild(i).position.y;

            Vector3 move = Vector3.Slerp(pos, nexPos, _MoveTime);
            transform.GetChild(i).position = move;

        }


        if (_MoveTime >= 1.0f)
        {
            _DetailUI.gameObject.GetComponent<DetailUI>().DetailUIAnimation();
            _DetailUI.gameObject.GetComponent<DetailUINumber>().SetScore(_ChoiceStage.GetComponent<Stage>().GetClearParsentage());
            _MoveTime = 0.0f;
            _Move = false;
        }

        _MoveTime += _MoveSpeed * Time.deltaTime;
    }

    


    public void InputLeftButton()
    {
        if (_Move)
            return;


        for (int i = 0; i < _ChidCount; i++)
        {
            _StartPos[i] = transform.GetChild(i).position;
        }

        for (int i = 0; i < _ChidCount-1; i++)
        {
            _EndPos[i] = transform.GetChild(i+1).position;
        }
        _EndPos[_ChidCount - 1] = transform.GetChild(0).position;



        _Move = true;
    }

    public void InputRightButton()
    {
        if (_Move)
            return;


        for (int i = 0; i < _ChidCount; i++)
        {
            _StartPos[i] = transform.GetChild(i).position;
        }

        for (int i = _ChidCount - 1; i > 0; i--)
        {
            _EndPos[i] = transform.GetChild(i-1).position;
        }
        _EndPos[0] = transform.GetChild(_ChidCount-1).position;



        _Move = true;
    }

    public void GameStart()
    {
        _StageName = _ChoiceStage.GetComponent<Stage>().GetSceneName();

        _ChoiceStageNumber = _ChoiceStage.GetComponent<Stage>().GetStageNumber();
        _1Star = _ChoiceStage.GetComponent<Stage>()._1Star;
        _2Star = _ChoiceStage.GetComponent<Stage>()._2Star;
        _3Star = _ChoiceStage.GetComponent<Stage>()._3Star;


        GameObject obj = _ChoiceStage.transform.GetChild(0).gameObject;
        _StageModelName = obj.name;

        SceneManager.LoadScene("FadeScene");
    }

    public GameObject GetChoiceStageObject()
    {
        return _ChoiceStage;
    }

}
