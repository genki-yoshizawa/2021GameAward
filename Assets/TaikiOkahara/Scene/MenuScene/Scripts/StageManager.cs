using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    

    private int _ChidCount;

    private float _Radius = 3.0f;

    [SerializeField]
    private GameObject _ChoiceStage;

    [SerializeField]
    private GameObject _DetailUI;

    
    private bool _Move = false;

    private float _MoveSpeed = 5.0f;//移動スピード

    private float _MoveTime = 0.0f;//移動時間総数

    private Vector3[] _StartPos;
    private Vector3[] _EndPos;

    [SerializeField]
    private Animator _Fade;

    [SerializeField]
    private AudioClip _ButtonCursorSound;//カーソル移動音
    [SerializeField]
    private AudioClip _ButtonDecisionSound;//決定音

    public static int _ChoiceStageNumber = 0;
    public static int _MaxTurn = 0;
    public static int _3Star = 0;
    public static int _2Star = 0;
    public static int _1Star = 0;
    public static string _StageName;
    public static string _StageModelName;
    public static bool _NextStage = false;

    private bool firstMenu = true;

    void Start()
    {
        _ChidCount = this.transform.childCount - 1;
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
        //_DetailUI.gameObject.GetComponent<DetailUINumber>().SetScore(_ChoiceStage.GetComponent<Stage>().GetClearParsentage());

    }

    void Update()
    {
      

        string curStageName = SceneManager.GetActiveScene().name;
        if(curStageName == "MenuScene")
        {

            if(firstMenu)
            {
                _DetailUI.gameObject.GetComponent<DetailUI>().DetailUIAnimation();
                firstMenu = false;
            }

            for (int i = 0; i < _ChidCount; i++)
            {

                //if (transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Renderer>() != null)
                //    transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = true;

                transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(true);

                if (transform.GetChild(i).gameObject.GetComponent<Stage>() != null)
                    transform.GetChild(i).gameObject.GetComponent<Stage>().enabled = true;
               
            }

            transform.GetChild(_ChidCount).gameObject.SetActive(true);

            //_DetailUI.gameObject.GetComponent<DetailUINumber>().SetScore(_ChoiceStage.GetComponent<Stage>().GetClearParsentage());
        }
        else
        {
            firstMenu = true;

            for (int i = 0; i < _ChidCount; i++)
            {
                //if (transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Renderer>() != null)
                //    transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;

                transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(false);

                if (transform.GetChild(i).gameObject.GetComponent<Stage>() != null)
                    transform.GetChild(i).gameObject.GetComponent<Stage>().enabled = false;

            }

            transform.GetChild(_ChidCount).gameObject.SetActive(false);

            return;
        }


        float dph = Input.GetAxis("Controller_D_Pad_Horizontal");
        float lsh = Input.GetAxis("Controller_L_Stick_Horizontal");

        if (dph > 0 || lsh > 0.5f || Input.GetKeyDown(KeyCode.RightArrow))
            InputRightButton();
        else if (dph < 0 || lsh < -0.5f || Input.GetKeyDown(KeyCode.LeftArrow))
            InputLeftButton();
        else if ((/*Input.GetKeyDown("joystick button 1")*/Input.GetButtonDown("Controller_B") || (Input.GetKeyDown(KeyCode.Return))) && !_Move)
        {
            this.GetComponent<AudioSource>().volume = 0.1f;
            this.GetComponent<AudioSource>().PlayOneShot(_ButtonDecisionSound);
            GameStart();
        }



        for (int i = 0; i < _ChidCount; i++)
        {
            if(_ChoiceStage.transform.position.z > transform.GetChild(i).transform.position.z)
            {
                _ChoiceStage = transform.GetChild(i).gameObject;
            }
        }

        for(int i = 0; i < _ChidCount; i++)
        {
            if (_ChoiceStage.transform == transform.GetChild(i))
            {

                //transform.GetChild(i).GetChild(0).GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            else
            {
                //transform.GetChild(i).GetChild(0).GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f, 1);
            }
        }


        _ChoiceStageNumber = _ChoiceStage.GetComponent<Stage>().GetStageNumber();
        _DetailUI.transform.GetChild(0).GetComponent<MenuStageNumber>().SetScore(_ChoiceStageNumber);


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
            //_DetailUI.gameObject.GetComponent<DetailUINumber>().SetScore(_ChoiceStage.GetComponent<Stage>().GetClearParsentage());
            _MoveTime = 0.0f;
            _Move = false;
        }

        float dph = Input.GetAxis("Controller_D_Pad_Horizontal");
        float lsh = Input.GetAxis("Controller_L_Stick_Horizontal");
        if (dph != 0 || (lsh < 0.5f && lsh > 0.5f) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _DetailUI.gameObject.GetComponent<DetailUI>().AnimationReset();
        }



        _MoveTime += _MoveSpeed * Time.deltaTime;
    }

    
    void ChageNextStage()
    {
        //クリーン度を100にする
        //_ChoiceStage.GetComponent<Stage>().SetClearParsentage(100);
        _ChoiceStage.GetComponent<Stage>().Clear();

        bool allStageClear = true;
        //全てのステージが100ならムービーを流してメニューに戻る
        for (int i = 0; i < _ChidCount; i++)
        {
            GameObject obj = this.gameObject.transform.GetChild(i).gameObject;
            if (obj.GetComponent<Stage>().GetClear() == false)
            {
                allStageClear = false;
            }
        }

        if (allStageClear)
        {
            //ムービー流す
            Debug.Log("Movie流すよ");
            SceneManager.LoadScene("MovieScene");
            return;
        }


        int idx = _ChoiceStage.transform.GetSiblingIndex();
        idx++;
        _ChoiceStage = this.transform.GetChild(idx).gameObject;

        GameStart();


        return;
    }

    public void InputLeftButton()
    {
        if (_Move)
            return;

        this.GetComponent<AudioSource>().volume = 0.05f;
        this.GetComponent<AudioSource>().PlayOneShot(_ButtonCursorSound);

        for (int i = 0; i < _ChidCount; i++)
        {
            _StartPos[i] = transform.GetChild(i).position;
        }

        for (int i = 0; i < _ChidCount - 1; i++)
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

        this.GetComponent<AudioSource>().volume = 0.05f;
        this.GetComponent<AudioSource>().PlayOneShot(_ButtonCursorSound);

        for (int i = 0; i < _ChidCount; i++)
        {
            _StartPos[i] = transform.GetChild(i).position;
        }

        for (int i = _ChidCount-1; i > 0; i--)
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
        _MaxTurn = _ChoiceStage.GetComponent<Stage>()._MaxTurn;

        GameObject obj = _ChoiceStage.transform.GetChild(0).gameObject;
        _StageModelName = obj.name;


        SceneManager.LoadScene("FadeScene");

        foreach (Transform child in transform)
        {
            //if (child.gameObject.transform.GetChild(0).GetComponent<Renderer>() != null)
            //    child.gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;

            child.gameObject.transform.GetChild(0).gameObject.SetActive(false);

            if (child.gameObject.GetComponent<Stage>() != null)
                child.gameObject.GetComponent<Stage>().enabled = false;

            if(GameObject.Find("MenuCanvas") != null)
            {
                GameObject canv = GameObject.Find("MenuCanvas");
                canv.gameObject.SetActive(false);
            }
        }
    }

    public GameObject GetChoiceStageObject()
    {
        return _ChoiceStage;
    }

    public void NextStage()
    {
      

        ChageNextStage();
      
    }

    public void UpdateUI()
    {
        //_DetailUI.gameObject.GetComponent<DetailUINumber>().SetScore(_ChoiceStage.GetComponent<Stage>().GetClearParsentage());
        _DetailUI.gameObject.GetComponent<DetailUI>().DetailUIAnimation();

        _ChoiceStageNumber = _ChoiceStage.GetComponent<Stage>().GetStageNumber();
        _DetailUI.transform.GetChild(0).GetComponent<MenuStageNumber>().SetScore(_ChoiceStageNumber);

        return;
    }



    int Digit(int num)
    {
        // Mathf.Log10(0)はNegativeInfinityを返すため、別途処理する。
        return (num == 0) ? 1 : ((int)Mathf.Log10(num) + 1);
    }
}
