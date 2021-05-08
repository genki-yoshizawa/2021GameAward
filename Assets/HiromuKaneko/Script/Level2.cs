//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Level2 : EnemyControl
//{
//    void Start()
//    {

//    }
//    public override void ChangeState()
//    {
//        // プレイヤーのいるブロックを朱徳して
//        // プレイヤーから一番遠いブロックへ逃げる
//        Vector3 playerpos = _Player.transform.position;
//        GameObject obj = new GameObject();
//        float distance = 0.0f;
//        float tmp = 0.0f;

//        if (_Up != null)
//        {
//            tmp = Vector3.Distance(playerpos, _Up.transform.position);
//            if (tmp > distance)
//            {
//                obj = _Up;
//                distance = tmp;
//            }
//        }
//        if (_Down != null)
//        {
//            tmp = Vector3.Distance(playerpos, _Down.transform.position);
//            if (tmp > distance)
//            {
//                obj = _Down;
//                distance = tmp;
//            }
//        }

//        if (_Left != null)
//        {
//            tmp = Vector3.Distance(playerpos, _Left.transform.position);
//            if (tmp > distance)
//            {
//                obj = _Left;
//                distance = tmp;
//            }
//        }
//        if (_Right != null)
//        {
//            tmp = Vector3.Distance(playerpos, _Right.transform.position);
//            if (tmp > distance)
//            {
//                obj = _Right;
//                distance = tmp;
//            }
//        }


//        _NextBlock = obj;
//        _EnemyState = EnemyState.MOVE;
//    }
//}
