using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl : GimmicControl
{
    // 破壊アニメーションに必要な変数
    [Header("ブロック破壊アニメーションにかける秒数")]
    [SerializeField] private float _BreakAnimTime = 1.0f;
    private bool _isBreakAnim = false;

    // 再建アニメーションに必要な変数
    [Header("ブロック復活アニメーションにかける秒数")]
    [SerializeField] private float _RebornAnimTime = 1.0f;
    private bool _isRebornAnim = false;

    private Vector3 _StartLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _StartGlobalPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _isBreakAnim = false;
        _isRebornAnim = false;

        _StartLocalScale = transform.localScale;
        _StartGlobalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBreakAnim)
        {
            _StartGlobalPosition = new Vector3(transform.localPosition.x, _StartGlobalPosition.y, transform.localPosition.z);
            float time = Time.deltaTime;
            if ( (_PassedTime += time) > _BreakAnimTime)
            {
                _PassedTime = _BreakAnimTime;
                _isBreakAnim = false;
            }

            // y座標を下げながらyサイズを0に向かわせる
            transform.localPosition = _StartGlobalPosition + (new Vector3(_StartGlobalPosition.x ,0.0f,_StartGlobalPosition.z) - _StartGlobalPosition) * (_PassedTime / _BreakAnimTime);

            transform.localScale = _StartLocalScale + (new Vector3(_StartLocalScale.x, 0.0f, _StartLocalScale.z) - _StartLocalScale) * (_PassedTime / _BreakAnimTime);

            if (!_isBreakAnim)
                _PassedTime = 0.0f;
        }

        if (_isRebornAnim)
        {
            _StartGlobalPosition = new Vector3(transform.localPosition.x, _StartGlobalPosition.y, transform.localPosition.z);
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _RebornAnimTime)
            {
                _PassedTime = _RebornAnimTime;
                _isRebornAnim = false;
            }

            // y座標を上げながらyサイズを元のサイズにに向かわせる
            //float move = (0.0f - _StartGlobalPosition.y) * (time / _BreakAnimTime);
            //transform.position = new Vector3(transform.position.x, transform.position.y - move, transform.position.z);
            Vector3 beforePos = new Vector3(_StartGlobalPosition.x, 0.0f, _StartGlobalPosition.z);
            transform.localPosition = beforePos + (_StartGlobalPosition - beforePos) * (_PassedTime / _RebornAnimTime);

            Vector3 beforeScale = new Vector3(_StartLocalScale.x, 0.0f, _StartLocalScale.z);
            transform.localScale = beforeScale + (_StartLocalScale - beforeScale) * (_PassedTime / _RebornAnimTime);

            if (!_isRebornAnim)
                _PassedTime = 0.0f;
        }
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
        if (transform.GetComponent<WallConfig>().GetIsBreak() || //壊れているor
            (Mathf.RoundToInt(wallDirection.x) * Mathf.RoundToInt(direction.y) - Mathf.RoundToInt(wallDirection.y) * Mathf.RoundToInt(direction.x)) != 0)//自身の向きとオブジェクトの向きが平行でないなら
        {
            return true;
        }
        // 自分のパネル位置とオブジェクトパネル位置が一緒かどうかで分岐
        if (objectPosition == panelPosition)//自分のパネル位置とオブジェクトパネル位置が一緒
        {
            if (wallDirection == direction * (transform.parent == transform.parent.parent.GetChild(0) ? 1 : -1) )//オブジェクト向きと壁向きが一緒 //裏世界のときは向き反転
                return false;
        }
        else//自分のパネル位置とオブジェクトパネル位置が異なる
        {
            if (wallDirection != direction * (transform.parent == transform.parent.parent.GetChild(0) ? 1 : -1))//オブジェクト向きと壁向きが一緒 //裏世界のときは向き反転
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
        if (transform.GetComponent<WallConfig>().GetIsBreak() || Mathf.RoundToInt(wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//壊れているor自身の向きとオブジェクトの向きが平行でないなら
        {
            return 0;
        }
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

    public void SetisBreakAnim()
    {
        _isBreakAnim = true;
    }
    public void SetisRebornAnim()
    {
        _isRebornAnim = true;
    }
}
