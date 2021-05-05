using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl : GimmicControl
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public override void TrunEndUpdate()
    //{

    //}

    public override void TurnOver()
    {

    }

    public override void Rotate(float angle)
    {
        WallConfig config = transform.GetComponent<WallConfig>();

        Vector3 direction = new Vector3(config.GetDirection().x, 0f, config.GetDirection().y);
        
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        config.SetDirection(new Vector2(direction.x, direction.z));

    }

    public override bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // 並行条件 a1b2 - a2b1 = 0
        if ((wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//自身の向きとオブジェクトの向きが平行でないなら
            return true;

        // 自分のパネル位置とオブジェクトパネル位置が一緒かどうかで分岐
        if (objectPosition == panelPosition)//自分のパネル位置とオブジェクトパネル位置が一緒
        {
            if (wallDirection == direction) return false; //オブジェクト向きと壁向きが一緒
            else return true;                             //オブジェクト向きと壁向きが異なる

        }
        else//自分のパネル位置とオブジェクトパネル位置が異なる
        {
            if (wallDirection == direction) return true; //オブジェクト向きと壁向きが一緒
            else return false;                           //オブジェクト向きと壁向きが異なる
        }
    }
}
