using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoseManager : MonoBehaviour
{

    [SerializeField]
    private float _Power = 0.6f;//透明度


    [SerializeField]
    private float _FadeInTime = 0.5f;//フェード時間
    [SerializeField]
    private float _FadeOutTime = 1.0f;//フェード時間

    private float _FadeCount = 0;


    [SerializeField]
    private GameObject _Image;

    [SerializeField]
    private Animator _StageModelAnimator;

    [SerializeField]
    private Animator _DetailUIAnimator;


    private enum FadeType
    {
        NONE,
        IN,
        OUT,
        DO//ポーズ実行中
    }

    private FadeType _FadeType;

    void Start()
    {
        //_Image = this.gameObject.GetComponent<Image>();
        _FadeType = FadeType.NONE;
    }

    void Update()
    {
        // 画面サイズに変更する
        var targetSize = new Vector2(Screen.width, Screen.height);
        _Image.GetComponent<RectTransform>().sizeDelta = targetSize;


 

        Pose();

    }

    public void Pose()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch(_FadeType)
            {
                case FadeType.NONE:
                    _FadeType = FadeType.IN;
                    PoseInAnimation();
                    break;

                case FadeType.DO:
                    _FadeType = FadeType.OUT;
                    PoseOutAnimation();
                    break;

                default:
                    break;
            }

        }
          

        FadeIn();
        FadeOut();
    }

    void FadeIn()
    {



        if (_FadeType != FadeType.IN) return;

        _FadeCount += Time.deltaTime;

        if(_FadeCount <= _FadeInTime)
        {
            _Image.GetComponent<Image>().color = new Color(0, 0, 0, _Power * (_FadeCount / _FadeInTime));
            return;
        }

        _FadeCount = 0;
        _FadeType = FadeType.DO;
        return;
    }

    void FadeOut()
    {
        if (_FadeType != FadeType.OUT) return;


        _FadeCount += Time.deltaTime;


        if (_FadeCount <= _FadeOutTime)
        {
            _Image.GetComponent<Image>().color = new Color(0, 0, 0, _Power * ((_FadeOutTime - _FadeCount) / _FadeOutTime));
            return;
        }


        _Image.GetComponent<Image>().color = new Color(0, 0, 0,0);
        _FadeCount = 0;
        _FadeType = FadeType.NONE;
        return;
    }

    void PoseInAnimation()
    {
        _StageModelAnimator.SetBool("Pose", true);
        _DetailUIAnimator.SetBool("Pose", true);

    }

    void PoseOutAnimation()
    {
        _StageModelAnimator.SetBool("Pose", false);
        _DetailUIAnimator.SetBool("Pose", false);

    }

}
