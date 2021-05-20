using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailUI : MonoBehaviour
{

    [SerializeField]
    private Animator _Animator;

    public void DetailUIAnimation()
    {
        _Animator.SetBool("isAnimation", true);
    }

    public void AnimationReset()
    {
        _Animator.SetBool("isAnimation", false);
    }
}
