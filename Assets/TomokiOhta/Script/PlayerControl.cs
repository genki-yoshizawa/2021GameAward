using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //プレイヤーのアニメーション
    private FukidasiAnimationUI _FukidasiAnimationUI;

    //表裏
    private bool _IsFront;

    //生存判定
    private bool _IsExist;

    //前のブロック情報
    private GameObject _FrontBlock;

    //どの行動が可能でど7の文字を格納するかを管理する
    private List<int> _CanActionList = new List<int>();

    void Start()
    {
        //初手FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _FukidasiAnimationUI = GameObject.Find("fukidasi_0").GetComponent<FukidasiAnimationUI>();
        _IsExist = true;
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

    public void RotateMySelf(Vector2Int position, float angle)
    {
        //Rotate時に呼び出される関数、自分の方向を変えるときにも自分で呼ぶ
        if (position != _LocalPosition)
            return;

        Vector3 direction = new Vector3(_Direction.x, 0f, _Direction.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

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

    public void TurnOverMySelf(Vector2Int position/*, Vector3 axis*/)
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
        RotateMySelf(_LocalPosition, 180.0f);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        SetFromtBlock();
    }

    private void PlayerRotate(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Rotate(_IsFront, 90);
        SetFromtBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Swap(_IsFront);
        SetFromtBlock();
    }

    private void PlayerTurnOver(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.TurnOver(_IsFront);
        SetFromtBlock();
    }

    public bool PlayerTurn()
    {
        if (_FrontBlock == null)
            SetFromtBlock();

        bool turnEnd = false;

        //プレイヤー左右回転
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateMySelf(_LocalPosition, 90.0f);
            transform.Rotate(0.0f, 90.0f, 0.0f);
            SetFromtBlock();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateMySelf(_LocalPosition, -90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
            SetFromtBlock();
        }

        //上下矢印でコマンド選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _FukidasiAnimationUI.SetCount(_CanActionList.Count);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _FukidasiAnimationUI.SetCount(0);
        }

        //Enterキーで行動 ターンを進める
        if (Input.GetKeyDown(KeyCode.Return))
        {

            turnEnd = true;
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }

    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }

    //前のブロックの情報取得
    private void SetFromtBlock()
    {
        _FrontBlock = _GameManagerScript.GetBlock(_LocalPosition + _Direction);
        if (_FrontBlock == null)
            return;

        var blockScript = _FrontBlock.GetComponent<BlockConfig>();

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

}

