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

    public override void TurnEndUpdate()
    {
        WallConfig config = transform.GetComponent<WallConfig>();

        if (config.GetIsBreak()) config.AddBreakCount();
    }

    public override void Rotate(float angle)
    {
        // 向きベクトルの回転
        WallConfig config = transform.GetComponent<WallConfig>();

        Vector3 direction = new Vector3(config.GetDirection().x, 0f, config.GetDirection().y);
        
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        config.SetDirection(new Vector2(direction.x, direction.z));

    }

    public override void TurnOver(Vector3 rotAxis)
    {
        // 向きベクトルの回転
        WallConfig config = transform.GetComponent<WallConfig>();

        Vector3 direction = new Vector3(config.GetDirection().x, 0f, config.GetDirection().y);

        direction = Quaternion.Euler(rotAxis * 180) * direction;

        config.SetDirection(new Vector2(direction.x, direction.z));

    }

    public override bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // 並行条件 a1b2 - a2b1 = 0
        if (transform.GetComponent<WallConfig>().GetIsBreak() || (wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//壊れているor自身の向きとオブジェクトの向きが平行でないなら
            return true;

        // 自分のパネル位置とオブジェクトパネル位置が一緒かどうかで分岐
        if (objectPosition == panelPosition)//自分のパネル位置とオブジェクトパネル位置が一緒
        {
            if (wallDirection == direction)//オブジェクト向きと壁向きが一緒
                return false;
        }
        else//自分のパネル位置とオブジェクトパネル位置が異なる
        {
            if (wallDirection != direction)//オブジェクト向きと壁向きが一緒
                return false;
        }

        return true;
    }

    public override int CheckWallLevel(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // 並行条件 a1b2 - a2b1 = 0
        if (transform.GetComponent<WallConfig>().GetIsBreak() || (wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//壊れているor自身の向きとオブジェクトの向きが平行でないなら
            return 0;

        // 自分のパネル位置とオブジェクトパネル位置が一緒かどうかで分岐
        if (objectPosition == panelPosition)//自分のパネル位置とオブジェクトパネル位置が一緒
        {
            if (wallDirection == direction)//オブジェクト向きと壁向きが一緒
                return transform.GetComponent<WallConfig>().GetWallLevel();
        }
        else//自分のパネル位置とオブジェクトパネル位置が異なる
        {
            if (wallDirection != direction)//オブジェクト向きと壁向きが一緒
                return transform.GetComponent<WallConfig>().GetWallLevel();
        }

        return 0;
    }

    // 壁を壊したら2,壊せなかったら1,存在しないなら0を返す
    // enumとかにした方がいい？やり方が・・・
    public override int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // 並行条件 a1b2 - a2b1 = 0
        if (transform.GetComponent<WallConfig>().GetIsBreak() || (wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//壊れているor自身の向きとオブジェクトの向きが平行でないなら
            return 0;

        // 自分のパネル位置とオブジェクトパネル位置が一緒かどうかで分岐
        if (objectPosition == panelPosition)//自分のパネル位置とオブジェクトパネル位置が一緒
        {
            if (wallDirection == direction)//オブジェクト向きと壁向きが一緒
            {
                if(transform.GetComponent<WallConfig>().GetWallLevel() > lv)
                {// 壊せない
                    return 1;
                }
                else
                {// 壊せる
                    transform.GetComponent<WallConfig>().BreakWall();
                    return 2;
                }
            }
        }
        else//自分のパネル位置とオブジェクトパネル位置が異なる
        {
            if (wallDirection != direction)//オブジェクト向きと壁向きが一緒
            {
                if (transform.GetComponent<WallConfig>().GetWallLevel() > lv)
                {// 壊せない
                    return 1;
                }
                else
                {// 壊せる
                    transform.GetComponent<WallConfig>().BreakWall();
                    return 2;
                }
            }
        }

        return 0;
    }
}
