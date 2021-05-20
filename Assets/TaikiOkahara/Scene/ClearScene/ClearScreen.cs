using UnityEngine;
using System.Collections;

public class ClearScreen : MonoBehaviour
{

    [SerializeField]
    Gauss _Gauss;

    [SerializeField]
    private Animator _ClearScreenAnimator;

    bool _PauseFlag = false;
    float _Intencity = 0;

    float _PrevTime;

    void Start()
    {

    }

    void Update()
    {
        var deltaTime = Time.realtimeSinceStartup - _PrevTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _PauseFlag = !_PauseFlag;
            bool animBool = _ClearScreenAnimator.GetBool("Clear");
            _ClearScreenAnimator.SetBool("Clear", !animBool);
        }
        if (_PauseFlag)
        {
            _Intencity += deltaTime * 8;
            Time.timeScale = 0;
            
        }
        else
        {
            _Intencity -= deltaTime * 8;
            Time.timeScale = 1;
        }
        _Intencity = Mathf.Clamp01(_Intencity);
        _Gauss.Resolution = (int)(_Intencity * 10);

        _PrevTime = Time.realtimeSinceStartup;
    }
}