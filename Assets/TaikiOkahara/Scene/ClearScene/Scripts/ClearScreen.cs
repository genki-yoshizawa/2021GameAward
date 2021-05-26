using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class ClearScreen : MonoBehaviour
{

    [SerializeField]
    Gauss _Gauss;

    [SerializeField]
    GameObject _Pause = null;

    [SerializeField]
    private Animator _StarAnimator;

    [SerializeField]
    private Animator _ClearScreenAnimator;

    [SerializeField]
    private GameObject _Comment;

    [SerializeField]
    private GameObject _TurnNumber;

    [SerializeField]
    private GameObject _TurnScoreNumber;

    [SerializeField]
    private Animator _StarParticle;

    [SerializeField]
    private Sprite[] _Comments;


    bool _PauseFlag = false;
    float _Intencity = 0;

    float _GaussTime = 1.0f;


    float _StarStartCount = 0;
    float _StarStartTime = 1.0f;

    float _ClearTextStartCount = 0;
    float _ClearTextStartTime = 3.0f;

    float _CommentStartCount = 0;
    float _CommentStartTime = 4.0f;

    float _TurnNumberStartCount = 0;
    float _TurnNumberStartTime = 1.0f;

    [SerializeField]
    float _ChangeMenuSceneCount = 0;
    float _ChangeMenuSceneTime = 5.0f;


    private int _Score;

    void Start()
    {
        //DisplayClearScreen();
    }

    void Update()
    {
       

        GaussFilter();
        StarAnim();
        ClearScreenAnim();
        CommentAnim();
        TurnNumber();
        ChangeMenuScene();
    }

    void StarAnim()
    {
        if (!_PauseFlag) return;


        if (_StarStartCount< _StarStartTime){
            _StarStartCount += Time.deltaTime;
            return;
        }

        //スコアに応じてアニメーションを分岐
        if(_Score <= StageManager._3Star)
            _StarAnimator.SetTrigger("ThreeStar");
        else if(_Score <= StageManager._2Star)
            _StarAnimator.SetTrigger("TwoStar");
        else
            _StarAnimator.SetTrigger("OneStar");


        return;
    }

    void GaussFilter()
    {
        if (!_PauseFlag) return;

        if(_Intencity > _GaussTime)
        {
            //_PauseFlag = false;
            
            return;
        }
         
        _Intencity += Time.deltaTime;

        _Gauss.Resolution = (int)((_Intencity/_GaussTime) * 20);
    }

    void ClearScreenAnim()
    {
        if (!_PauseFlag) return;

        if (_ClearTextStartCount < _ClearTextStartTime)
        {
            _ClearTextStartCount += Time.deltaTime;
            return;
        }

        _ClearScreenAnimator.SetBool("Clear", true);
        _StarParticle.SetBool("StarParticle", true);
        
        return;
    }

    void CommentAnim()
    {
        if (!_PauseFlag) return;

        if (_CommentStartCount < _CommentStartTime)
        {
            _CommentStartCount += Time.deltaTime;
            return;
        }

        _Comment.GetComponent<Animator>().SetBool("Display", true);
        return;
    }

    void TurnNumber()
    {
        if (!_PauseFlag) return;

        if (_TurnNumberStartCount < _TurnNumberStartTime)
        {
            _TurnNumberStartCount += Time.deltaTime;
            return;
        }

        _TurnNumber.GetComponent<ClearTransition>().BeginTransition();


        return;
    }

    void ChangeMenuScene()
    {
        if (!_PauseFlag) return;

        if(_ChangeMenuSceneCount < _ChangeMenuSceneTime)
        {
            _ChangeMenuSceneCount += Time.deltaTime;
            return;
        }


        if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.Return))
        {
            this.gameObject.SetActive(false);
            StageManager.Instance.NextStage();
        }
            
    }

    public void DisplayClearScreen(int score)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Manager");
        obj.GetComponent<GameManagerScript>().SetClear();

        _Score = score;
        _TurnScoreNumber.GetComponent<TurnScore>().SetScore(score);

        //スコアに応じてアニメーションを分岐
        if (_Score <= StageManager._3Star)
            _Comment.GetComponent<Image>().sprite = _Comments[2];
        else if (_Score <= StageManager._2Star)
            _Comment.GetComponent<Image>().sprite = _Comments[1];
        else
            _Comment.GetComponent<Image>().sprite = _Comments[0];


        //クリーン度を100にする
        StageManager.Instance.GetChoiceStageObject().GetComponent<Stage>().SetClearParsentage(100);


        _PauseFlag = true;
        if(_Pause != null)
            _Pause.SetActive(false);
    }
}