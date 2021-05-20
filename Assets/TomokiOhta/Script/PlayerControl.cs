using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerControl : MonoBehaviour
{
    [Header("使いたい音声ファイルを入れてください。")]
    [SerializeField] private AudioClip audioClip;

    [Header("プレイヤーの向きを入れてください。")]
    [SerializeField]private Vector2Int _Direction;

    [Header("吹き出し")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("コマンド")]
    [SerializeField] private GameObject _ActObj;

    //プレイヤーの配列座標
    private Vector2Int _LocalPosition;

    //他のオブジェクトに触れるときの仲介役
    private GameManagerScript _GameManagerScript;

    //表裏
    private bool _IsFront;

    //生存判定
    private bool _IsExist;

    //前のブロック情報
    private GameObject _FrontBlock;

    //どの行動が可能でど7の文字を格納するかを管理する
    private List<int> _CanActionList = new List<int>();

    //
    private FukidasiAnimationUI _FukidasiScript;

    private int _CommandSelect = 0;

    void Start()
    {
        //初手FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
<<<<<<< HEAD
        //_FukidasiAnimationUI = GameObject.Find("fukidasi_0").GetComponent<FukidasiAnimationUI>();
=======
>>>>>>> 782f95e958e8120805bb300657cd53862ba20222
        _IsExist = true;

        _FukidasiScript = _FukidasiObj.GetComponent<FukidasiAnimationUI>();
    }

    void Update()
    {
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

    private void Move(Vector2Int direction)
    {
        //前のブロック取得
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //親を切り替える
        transform.parent = block.transform.GetChild(0).transform;

        //移動
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _LocalPosition += _Direction;
    }

    public void RotateMySelf(Vector2Int position, float angle, float axisX = 0.0f, float axisY =1.0f, float axisZ = 0.0f)
    {
        Vector3 axis = new Vector3(axisX, axisY, axisZ);

        //Rotate時に呼び出される関数、自分の方向を変えるときにも自分で呼ぶ
        if (position != _LocalPosition)
            return;

        Vector3 direction = new Vector3(_Direction.x, 0f, _Direction.y);
        direction = Quaternion.Euler(axis * angle) * direction;

        Vector2 tmp = new Vector2(direction.x, direction.z);
        //四捨五入して代入することでVector2Intにも無理やり代入させる
        _Direction = new Vector2Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y));
    }

    public void SwapMySelf(List<Vector2Int> position)
    {
        //Swap時に呼び出される関数、親オブジェクトであるブロックの移動についていくだけ
        foreach (Vector2Int pos in position)
        {
            if (pos == _LocalPosition)
            {
                var blockConfig = transform.parent.parent.GetComponent<BlockConfig>();
                _LocalPosition = blockConfig.GetBlockLocalPosition();
                return;
            }
        }

    }

    public void TurnOverMySelf(Vector2Int position, Vector3 axis)
    {
        //TurnOver時に呼び出される関数
        if (position != _LocalPosition)
            return;

        //ひっくり返す
        if (_IsFront)
            _IsFront = false;
        else
            _IsFront = true;

        //向きを反転
        //_Direction *= -1;
        //モデルを反転
        RotateMySelf(_LocalPosition, 180.0f, axis.x, axis.y, axis.z);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        SetFrontBlock();
    }

    private void PlayerRotate(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Rotate(_IsFront, 90);
        SetFrontBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Swap(_IsFront);
        SetFrontBlock();
    }

    private void PlayerTurnOver(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.TurnOver(_IsFront, _Direction);
        SetFrontBlock();
    }

    public bool PlayerTurn()
    {
        bool turnEnd = false;

        //Startで取得するのでターン開始時に手動で取得
        if (_FrontBlock == null)
            SetFrontBlock();

        //吹き出しのアニメーション終了を確認したら生成する
        if (_FukidasiScript.GetCount() == 0)
        {
            _FukidasiScript.SetCount(_CanActionList.Count);
            _FukidasiScript.SetPanel(_CanActionList);
        }

        //プレイヤー左右回転
        //きょろきょろしすぎるとコマンドが消えたまま出てこなくなるので注意
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateMySelf(_LocalPosition, _IsFront ? 90.0f : -90.0f);
            transform.Rotate(0.0f, 90.0f, 0.0f);
            SetFrontBlock();

            _FukidasiScript.ResetCount();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateMySelf(_LocalPosition, _IsFront ? -90.0f : 90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
            SetFrontBlock();

            _FukidasiScript.ResetCount();
        }

        //上下矢印でコマンド選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(_CommandSelect > 0)
            _CommandSelect--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(_CommandSelect < _CanActionList.Count)
            _CommandSelect++;
        }

        //Enterキーで行動 ターンを進める
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CommandAction(_CommandSelect);
            _CommandSelect = 0;
            _FukidasiScript.ResetCount();
            turnEnd = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PlayerTurnOver(_FrontBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            PlayerRotate(_FrontBlock);
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }
    public bool GetIsExist() { return _IsExist; }


    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
    public void SetIsExist(bool isExist) { _IsExist = isExist; }


    //前のブロックの情報取得
    private void SetFrontBlock()
    {
        BlockConfig blockScript;

        if (_FrontBlock)
        {
            blockScript = _FrontBlock.GetComponent<BlockConfig>();
            blockScript.PanelRemoveAttention(_IsFront);
        }

        _FrontBlock = _GameManagerScript.GetBlock(_LocalPosition + _Direction);
        if (_FrontBlock == null)
            return;

        blockScript = _FrontBlock.GetComponent<BlockConfig>();
        blockScript.PanelAttention(_IsFront);

        //中身をリセットして新たに情報を渡す
        _CanActionList = new List<int>();

        //回転可能なら0を入れる
        if (blockScript.CheckPanelRotate(_IsFront))
            _CanActionList.Add(0);

        //移動可能なら1を入れる
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            _CanActionList.Add(1);

        //反転可能なら2を入れる
        if (blockScript.CheckPanelTurnOver(_IsFront))
            _CanActionList.Add(2);

        //入替可能なら3を入れる
        if (blockScript.CheckPanelSwap(_IsFront))
            _CanActionList.Add(3);
    }

    private void CommandAction(int num)
    {
        int count = 0;
        foreach (var act in _CanActionList)
        {
            if (num == count)
            {
                switch (act)
                {
                    case 0:
                        PlayerRotate(_FrontBlock);
                        Debug.Log("Rotate");
                        break;
                    case 1:
                        PlayerMove();
                        Debug.Log("Move");
                        break;
                    case 2:
                        PlayerTurnOver(_FrontBlock);
                        Debug.Log("TurnOver");
                        break;
                    case 3:
                        PlayerSwap(_FrontBlock);
                        Debug.Log("Swap");
                        break;
                }
                break;
            }

            count++;
        }
    }

}

