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
    private Vector2Int _StartDirection;

    [Header("吹き出し")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("選択アイコン")]
    [SerializeField] private GameObject _IconObj;

    [Header("歩く時間")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("行動終了後の待機時間")]
    [SerializeField] private float _ActionTime = 0.01f;

    //アニメーションスタートからの経過時間
    private float _PassedTime;

    //歩き始めの座標
    private Vector3 _WalkStartPosition;

    //目的地の座標
    private Vector3 _WalkTargetPosition;

    //プレイヤーのアニメーター
    private Animator _Animator;

    //プレイヤーの配列座標
    private Vector2Int _LocalPosition;
    private Vector2Int _StartPostion;

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

    //ふきだしのUIを管理する
    private FukidasiAnimationUI _FukidasiScript;

    private int _CommandSelect = 0;

    public void Start()
    {
        //初手FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Animator = GetComponent<Animator>();

        _IsExist = true;


        _FukidasiScript = _FukidasiObj.GetComponent<FukidasiAnimationUI>();

        _StartPostion = _LocalPosition;
        _StartDirection = _Direction;
        _IsExist = true;

        _WalkStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;
    }

    public void Update()
    {
        // 歩くアニメーション
        if (_Animator.GetBool("Walk"))
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                _PassedTime = _WalkTime;
                _Animator.SetBool("Walk", false);
            }

            transform.position = _WalkStartPosition + (_WalkTargetPosition - _WalkStartPosition) * (_PassedTime / _WalkTime);

            if (!_Animator.GetBool("Walk"))
                _PassedTime = 0.0f;
        }

        //アクションアニメーション
        if(_Animator.GetBool("Action"))
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _ActionTime)
            {
                _Animator.SetBool("Action", false);
                _PassedTime = 0.0f;
            }
        }

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

    public void PlayerInit()
    {
        _IsExist = true;
        _LocalPosition = _StartPostion;
        _Direction = _StartDirection;
        _WalkStartPosition  = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;

        if (_Animator.GetBool("Tired"))
            SetTired(false);
        if (_Animator.GetBool("GameOver"))
            SetDead(false);
    }

    private void Move(Vector2Int direction)
    {
        //前のブロック取得
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //ゆっくり歩くために現在地と目的地を取得
        _WalkStartPosition = transform.position;
        _WalkTargetPosition = block.transform.position;

        //Walkのアニメーション開始
        _Animator.SetBool("Walk", true);

        //親を切り替える
        transform.parent = block.transform.GetChild(0).transform;

        //移動
        //transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
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
        _Animator.SetBool("Action", true);
        blockScript.Rotate(_IsFront, 90);
        SetFrontBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        _Animator.SetBool("Action", true);
        blockScript.Swap(_IsFront);
        SetFrontBlock();
    }

    private void PlayerTurnOver(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.TurnOver(_IsFront, _Direction);
        _Animator.SetBool("Action", true);
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

            _FukidasiScript.ResetPanel();
            _FukidasiScript.ResetCount();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateMySelf(_LocalPosition, _IsFront ? -90.0f : 90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
            SetFrontBlock();

            _FukidasiScript.ResetPanel();
            _FukidasiScript.ResetCount();
        }

        //上下矢印でコマンド選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_CommandSelect > 0)
            {
                _CommandSelect--;
                _IconObj.transform.Translate(0.0f, 0.3f, 0.0f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_CommandSelect < _CanActionList.Count)
            {
                _CommandSelect++;
                _IconObj.transform.Translate(0.0f, -0.3f, 0.0f);
            }
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
                        break;
                    case 1:
                        PlayerMove();
                        break;
                    case 2:
                        PlayerTurnOver(_FrontBlock);
                        break;
                    case 3:
                        PlayerSwap(_FrontBlock);
                        break;
                }
                break;
            }

            count++;
        }
    }

    public void SetTired(bool flag)
    {
        _Animator.SetBool("Tired", flag);
    }

    public void SetDead(bool flag)
    {
        _Animator.SetBool("GameOver", flag);
    }

    GameObject CheckEnemy(Vector2Int position)
    {
        var enemys = _GameManagerScript.GetEnemys();

        EnemyControl enemyScript;
        foreach (var enemy in enemys)
        {
            enemyScript = enemy.GetComponent<EnemyControl>();
            if (position == enemyScript.GetLocalPosition())
                return enemy;
        }

        return null;
    }

}

