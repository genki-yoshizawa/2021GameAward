using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailUI : MonoBehaviour
{

    [SerializeField]
    private Animator _Animator;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }

    public void DetailUIAnimation()
    {
        Debug.Log("アニメ開始");
        //_Animator.SetTrigger("IsCenter");
        _Animator.SetBool("isAnimation", true);
    }

    public void AnimationReset()
    {
        Debug.Log("アニメ終了");
        _Animator.SetBool("isAnimation", false);
    }
}
