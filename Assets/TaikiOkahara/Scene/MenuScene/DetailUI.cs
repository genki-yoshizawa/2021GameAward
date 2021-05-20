using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailUI : MonoBehaviour
{

    [SerializeField]
    private Animator _Animator;

    [SerializeField]
    private GameObject _ClearTurn;


    private int _StageClearPercentage;

    void Update()
    {
        _StageClearPercentage = StageManager.Instance.GetChoiceStageObject().GetComponent<Stage>().GetClearParsentage();

        _ClearTurn.GetComponent<Text>().text = _StageClearPercentage.ToString();
    }

    public void DetailUIAnimation()
    {
        _Animator.SetBool("isAnimation", true);
    }

    public void AnimationReset()
    {
        _Animator.SetBool("isAnimation", false);
    }
}
