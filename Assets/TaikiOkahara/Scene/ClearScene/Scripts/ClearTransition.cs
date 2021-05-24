using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClearTransition : MonoBehaviour
{
    [SerializeField]
    private Material _TransitionIn;

    [SerializeField]
    private GameObject _ScoreText;

    [SerializeField]
    private float _Time;

    private float _Current = 0;

    private bool _Transition= false;

    void Start()
    {
        this.GetComponent<Image>().enabled = false;
        _TransitionIn.SetFloat("_Alpha", 0);
    }

    void Update()
    {
        if (!_Transition)
            return;

        while (_Current < _Time)
        {
            _TransitionIn.SetFloat("_Alpha", (_Current / _Time) * 1.5f);
            _Current += Time.deltaTime;
            return;
        }

        _ScoreText.GetComponent<Animator>().SetBool("Display", true);
        _TransitionIn.SetFloat("_Alpha", 1.5f);
        _Transition = false;
        return;
    }
 
    


    public void BeginTransition()
    {
        _Transition = true;
        this.GetComponent<Image>().enabled = true;

        return;
    }
}
