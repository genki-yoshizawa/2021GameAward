using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FukidasiAnimationUI : MonoBehaviour
{
    //座標としてプレイヤー追従用 子クラスにしてもよい？
    private GameObject _Player;

    private Animator _Animator;

    public void Start()
    {
        //初手FindWithTag
        var GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Player = GameManagerScript.GetPlayer();

        _Animator = GetComponent<Animator>();

    }

    public void Update()
    {
        //ビルボード処理
        //Vector3 p = Camera.main.transform.position;
        //p.x = transform.position.x;
        //p.y = transform.position.y;
        //p.z = transform.position.z;
        //transform.LookAt(p);
    }

    public void SetCount(int num) { _Animator.SetInteger("_ActionCount", num); }

    public void ResetCount(){ _Animator.SetInteger("_ActionCount", 0); }
}

