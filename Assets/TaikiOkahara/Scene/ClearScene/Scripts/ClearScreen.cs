using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class ClearScreen : MonoBehaviour
{

    [SerializeField]
    Gauss _Gauss;

    [SerializeField]
    private Animator _StarAnimator;

    [SerializeField]
    private Animator _ClearScreenAnimator;

    [SerializeField]
    private Animator _CommentAnimator;

    bool _PauseFlag = false;
    float _Intencity = 0;

    float _GaussTime = 1.0f;


    float _StarStartCount = 0;
    float _StarStartTime = 1.0f;

    float _ClearTextStartCount = 0;
    float _ClearTextStartTime = 3.0f;

    float _CommentStartCount = 0;
    float _CommentStartTime = 4.0f;

    void Start()
    {

        DisplayClearScreen();
    }

    void Update()
    {
        GaussFilter();
        StarAnim();
        ClearScreenAnim();
        CommentAnim();

        if(Input.GetKeyDown(KeyCode.Return))
            SceneManager.LoadScene("MenuScene");

    }

    void StarAnim()
    {
        if (!_PauseFlag) return;


        if (_StarStartCount< _StarStartTime){
            _StarStartCount += Time.deltaTime;
            return;
        }

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

        _CommentAnimator.SetBool("Display", true);

        return;
    }


    public void DisplayClearScreen()
    {
        _PauseFlag = true;
    }
}