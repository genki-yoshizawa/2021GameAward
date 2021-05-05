using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    Vector2Int _PlayrPosition;
    GameObject _FrontBlock;
    GameObject _BackBlock;
    Vector3 _MoveDirection;

    bool _IsMove;
    int _Count;

    void Start()
    {
        _FrontBlock = GameObject.Find("panel01");
        _BackBlock = GameObject.Find("panel02");

        _IsMove = false;
        _Count = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("右が押されたよ");
            _IsMove = true;

            transform.parent = _FrontBlock.transform;
            _MoveDirection = transform.localPosition * 0.01f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("左が押されたよ");
            _IsMove = true;

            transform.parent = _BackBlock.transform;
            _MoveDirection = transform.localPosition * 0.01f;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var foward = transform.forward;
            Debug.Log(foward);
            transform.Translate(foward * 0.1f);
        }

        if(_IsMove)
            PlayerMove();

        //箱コンこれで動くと思う
        //if (Input.GetKeyDown("joystick button 1"))  //B
        //{

        //}

        //if (Input.GetKeyDown("joystick button 0"))  //A
        //{

        //}

        //if (Input.GetKeyDown("joystick button 2"))  //X
        //{

        //}

        //if (Input.GetKeyDown("joystick button 3"))  //Y
        //{

        //}

    }

    private void PlayerMove()
    {
        Vector3 nowpos = transform.localPosition;
        //transform.localPosition = new Vector3(nowpos.x - _MoveDirection.x, nowpos.y - _MoveDirection.y, nowpos.z - _MoveDirection.z);
        transform.localPosition = nowpos - _MoveDirection;

        _Count++;
        if(_Count >= 100)
        {

            _Count = 0;
            _IsMove = false;
        }
    }

    //終了時の確認
    public void TurnEndUpdate()
    {

    }
}
