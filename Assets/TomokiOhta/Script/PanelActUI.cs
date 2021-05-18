using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActUI : MonoBehaviour
{
    private Animator _Animator;

    private int _ActType = 0;

    public void Start()
    {
        _Animator = GetComponent<Animator>();
    }


    public void Update()
    {
        _Animator.SetInteger("_ActType",_ActType);
    }

    public void SetActType(int num) { _ActType = num; }

    public void ResetActType() { _ActType = 0; }

}
