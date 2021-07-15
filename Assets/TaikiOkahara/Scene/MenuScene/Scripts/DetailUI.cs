using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailUI : MonoBehaviour
{

    [SerializeField]
    private Animator _Animator;

    //[SerializeField]
    //private GameObject _Comment;

    //[SerializeField]
    //private Sprite[] _CommentSprites;

    //private int _StageClearPercentage;

    void Update()
    {

        float dph = Input.GetAxis("Controller_D_Pad_Horizontal");
        float lsh = Input.GetAxis("Controller_L_Stick_Horizontal");

        if (dph != 0 || (lsh < 0.5f && lsh > 0.5f) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            AnimationReset();


        

        //switch(_StageClearPercentage)
        //{
        //    case 0:
        //    case 10:
        //    case 20:
        //    case 30:
        //        _Comment.GetComponent<Image>().sprite = _CommentSprites[0];
        //        break;

        //    case 40:
        //    case 50:
        //    case 60:
        //        _Comment.GetComponent<Image>().sprite = _CommentSprites[1];
        //        break;

        //    case 70:
        //    case 80:
        //    case 90:
        //        _Comment.GetComponent<Image>().sprite = _CommentSprites[2];
        //        break;
        //    case 100:
        //        _Comment.GetComponent<Image>().sprite = _CommentSprites[3];
        //        break;
        //    default:
        //        break;
        //}
    }

    public void DetailUIAnimation()
    {
        _Animator.SetBool("isAnimation", true);
        //_StageClearPercentage = StageManager.Instance.GetChoiceStageObject().GetComponent<Stage>().GetClearParsentage();
    }

    public void AnimationReset()
    {
        _Animator.SetBool("isAnimation", false);
        //_StageClearPercentage = StageManager.Instance.GetChoiceStageObject().GetComponent<Stage>().GetClearParsentage();
    }

    public void AnimationInitialize()
    {
        //Reset
        Debug.Log("Reset");
        _Animator.Play("None");
    }
}
