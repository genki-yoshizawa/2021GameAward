using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("使いたい音声ファイルを入れてください。")]
    [SerializeField] private AudioClip audioClip;

    [Header("プレイヤーの向きを入れてください。")]
    [SerializeField]private Vector2Int _Direction;
    private Vector2Int _StartDirection;

    [Header("吹き出し")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("歩く時間")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("行動終了後の待機時間")]
    [SerializeField] private float _ActionTime = 0.01f;

    [Header("クリア画面"), SerializeField] private GameObject _ClearScreen;

    [Header("向き変更時に歩行アニメーションを再生するか"), SerializeField] private bool IsWalkAnim;
    private bool NowWalkAnim = false;

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

    //どの行動が可能でどの文字を格納するかを管理する
    private List<int> _CanActionList = new List<int>();

    //ふきだしのUIを管理する
    private FukidasiAnimationUI _FukidasiScript;
    private Vector3 _CorsorStartPosition;

    //コマンド選択時に上から何番目にいるか
    private int _CommandSelect = 3;

    //行動コマンドの種類数
    private readonly int _AnimMax = 4;

    //ターンマネージャー
    private TurnManager _TurnManager;

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

        //アニメーション用の変数
        _WalkStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;

        _CorsorStartPosition = _FukidasiObj.transform.GetChild(4).localPosition;

        _TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();
    }

    public void Update()
    {
        // 歩くアニメーション
        if (_Animator.GetBool("Walk") && NowWalkAnim == false)
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


        //向き変更
        if (_Animator.GetBool("Walk") && NowWalkAnim == true)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                //_PassedTime = _WalkTime;

                _Animator.SetBool("Walk", false);
            }

            transform.Rotate(0.0f, 90.0f * 0.01f, 0.0f);

            if (!_Animator.GetBool("Walk"))
            {
                _PassedTime = 0.0f;
                NowWalkAnim = false;
            }
        }

        //現在のアニメーション情報を取得
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        //クリア確認
        if (clipInfo.clip.name == "Clear")
        {
            var clearScreenScript = _ClearScreen.GetComponent<ClearScreen>();
            clearScreenScript.DisplayClearScreen(_TurnManager.GetTurnCount());
        }

        //ゲームオーバー確認
        if (clipInfo.clip.name == "GameOvered")
        {
            var gameOverScript = _ClearScreen.GetComponent<GameOverScreen>();
            gameOverScript.DisplayGameOverScreen();
        }

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
    }

    private void Move(Vector2Int direction)
    {
        //前のブロック取得
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);
        _GameManagerScript.GetCamera().transform.GetComponent<CameraWork>().PlayerMoveCameraWork(_LocalPosition + direction);

        //ゆっくり歩くために現在地と目的地を取得
        _WalkStartPosition = transform.position;
        _WalkTargetPosition = block.transform.position;

        //Walkのアニメーション開始
        _Animator.SetBool("Walk", true);

        //親を切り替える
        transform.parent = block.transform.GetChild(0).transform;

        //移動
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
        _Animator.SetTrigger("Action");
        blockScript.Rotate(_IsFront, 90);
        SetFrontBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        _Animator.SetTrigger("Action");
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
        if (_FukidasiScript.GetAnimPattern() == -1)
        {
            _FukidasiScript.SetAnimPattern(_CanActionList.Count);
            if (CheckEnemy(_LocalPosition + _Direction) != null)
            {
                //前に敵がいたのでそれ用の画像を出す
                _FukidasiScript.SetActPattern(_CanActionList, true);
            }
            else
                _FukidasiScript.SetActPattern(_CanActionList);

            //カーソルを一番上に設定
            _CommandSelect = _CanActionList.Count - 1;
            var icon = _FukidasiObj.transform.GetChild(4).GetComponent<RectTransform>();
            icon.anchoredPosition = 
                new Vector3(icon.localPosition.x, _CorsorStartPosition.y + (20.0f * _CommandSelect), _CorsorStartPosition.z);
        }

        //プレイヤー左右回転
        //きょろきょろしすぎるとコマンドが消えたまま出てこなくなるので注意
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //向き変更時に歩行アニメーション再生
            if (IsWalkAnim)
            {
                _Animator.SetBool("Walk", true);
                NowWalkAnim = true;
            }
            else
                transform.Rotate(0.0f, 90.0f, 0.0f);

            RotateMySelf(_LocalPosition, _IsFront ? 90.0f : -90.0f);
            
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //向き変更時に歩行アニメーション再生
            if (IsWalkAnim)
            {
                _Animator.SetBool("Walk", true);
                NowWalkAnim = true;
            }
            else
                transform.Rotate(0.0f, -90.0f, 0.0f);

            RotateMySelf(_LocalPosition, _IsFront ? -90.0f : 90.0f);
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }

        //上下矢印でコマンド選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_CommandSelect < _CanActionList.Count - 1)
            {
                _CommandSelect++;

                //アイコンのtransform取得
                var icon = _FukidasiObj.transform.GetChild(4).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y + 20.0f, icon.localPosition.z);

                //文字のsprite変更
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_CommandSelect > 0)
            {
                _CommandSelect--;

                //アイコンのtransform取得
                var icon = _FukidasiObj.transform.GetChild(4).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y - 20.0f, icon.localPosition.z);

                //文字のsprite変更
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }

        //Enterキーで行動 ターンを進める
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var enemy = CheckEnemy(_LocalPosition + _Direction);
            if (enemy != null)
            {
                _Animator.SetTrigger("Capture");
                _GameManagerScript.KillEnemy(enemy);
                var remainEnemy = _GameManagerScript.GetEnemys();

                //敵がいなくなったことを確認したらゲームを終わらせに行く
                if (remainEnemy.Count <= 0)
                    _Animator.SetBool("Clear", true);
            }
            else
            {
                CommandAction(_CommandSelect);
            }

            //_CommandSelect = 3;   存在意義が分からないけど一応残しておく
            _FukidasiScript.ResetAnimPattern();
            turnEnd = true;
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }
    public bool GetIsExist() { return _IsExist; }


    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
    public void SetIsExist(bool isExist)
    {
        if (isExist)
            SetDead();
        _IsExist = isExist;
    }


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

        //入替可能なら3を入れる
        if (blockScript.CheckPanelSwap(_IsFront))
            _CanActionList.Add(3);

        //反転可能なら2を入れる
        if (blockScript.CheckPanelTurnOver(_IsFront))
            _CanActionList.Add(2);

        //移動可能なら1を入れる
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            _CanActionList.Add(1);

        //回転可能なら0を入れる
        if (blockScript.CheckPanelRotate(_IsFront))
            _CanActionList.Add(0);
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

    public void SetDead()
    {
        _Animator.SetTrigger("GameOver");
    }

    private GameObject CheckEnemy(Vector2Int position)
    {
        //前に壁が存在しているかを調べる
        if(_FrontBlock == null)
            return null;

        var frontBlockScript = _FrontBlock.GetComponent<BlockConfig>();

        bool noWall = frontBlockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction);
        if (!noWall)
            return null;

        //正面にenemyがいるかを調べる
        var enemys = _GameManagerScript.GetEnemys();

        EnemyControl enemyScript;
        foreach (var enemy in enemys)
        {
            enemyScript = enemy.GetComponent<EnemyControl>();
            if (position == enemyScript.GetLocalPosition() && _IsFront && enemyScript.GetIsFront())
                return enemy;
        }

        return null;
    }

    public Vector3 GetTargetPosition() { return _WalkTargetPosition; }

}

