using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("使いたい音声ファイルを入れてください。")]
    [SerializeField] private AudioClip[] _AudioClip;

    [Header("プレイヤーの向きを入れてください。")]
    [SerializeField] private Vector2Int _Direction;
    private Vector2Int _StartDirection;

    [Header("歩く時間")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("方向転換にかかる時間")]
    [SerializeField] private float _RotateTime90;

    [Header("方向転換にかかる時間")]
    [SerializeField] private float _RotateTime180;

    [Header("クリア画面"), SerializeField] private GameObject _ClearScreen;
    [Header("ゲームオーバー画面"), SerializeField] private GameObject _GameOverScreen;

    private bool _IsWalk = false;

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

    //コマンド選択で上を選んでいるか
    private bool _CommandTop = true;  

    //ターンマネージャー
    private TurnManager _TurnManager;

    //クリア及びGameOver判定
    private bool _IsClear;
    private bool _IsGameOver;

    //関数ポインタのようなもの
    private System.Action<GameObject> _CommandAction;

    //コマンド用のスクリプト
    private CommandUI _CommandScript;

    private bool _SelectDirection = false;
    private bool _SelectCommand = false;

    //これから向く方向
    private float _TurnAngle = 0.0f;

    //正面パネルに移動できるか
    private bool _CanMove = false;

    public void Start()
    {
        //初手FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Animator = GetComponent<Animator>();

        //commandScript取得
        _CommandScript = GameObject.FindGameObjectWithTag("Command").GetComponent<CommandUI>();


        _StartPostion = _LocalPosition;
        _StartDirection = _Direction;
        _IsExist = true;

        //アニメーション用の変数
        _WalkStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;


        _TurnManager = _GameManagerScript.GetTurnManager().GetComponent<TurnManager>();

        AudioManager.Instance.PlayGameBGM(_AudioClip[0], _AudioClip[1]);

        _IsClear = false;
        _IsGameOver = false;
    }

    public void Update()
    {
        //現在のアニメーション情報を取得
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        // 歩くアニメーション
        if (_Animator.GetBool("Walk") && _IsWalk == false)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                _PassedTime = _WalkTime;
                _Animator.SetBool("Walk", false);
            }

            transform.position = _WalkStartPosition + (_WalkTargetPosition - _WalkStartPosition) * (_PassedTime / _WalkTime);

            if (!_Animator.GetBool("Walk"))
            {
                _PassedTime = 0.0f;
                _GameManagerScript.GetCamera().transform.GetComponent<MainCameraScript>().SetIsPlayerMove(false);
            }
        }


        //向き変更
        if (_Animator.GetBool("Walk") && _IsWalk == true)
        {
            float time = Time.deltaTime;

            if (_TurnAngle == -180.0f)
            {
                if ((_PassedTime += time) > _RotateTime180)
                    _Animator.SetBool("Walk", false);

                transform.Rotate(0.0f, Mathf.Abs(_TurnAngle) * (time / _RotateTime180) * (_TurnAngle > 0.0f ? 1.0f : -1.0f), 0.0f);
            }
            else
            {
                if ((_PassedTime += time) > _RotateTime90)
                    _Animator.SetBool("Walk", false);

                transform.Rotate(0.0f, Mathf.Abs(_TurnAngle) * (time / _RotateTime90) * (_TurnAngle > 0.0f ? 1.0f : -1.0f), 0.0f);
            }

            //回転
            //transform.Rotate(0.0f, Mathf.Abs(_TurnAngle) * (time / _RotateTime90) * (_TurnAngle > 0.0f ? 1.0f : -1.0f), 0.0f);

            if (!_Animator.GetBool("Walk"))
            {
                //向きの調整
                if(_Direction == Vector2Int.up)
                    transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                else if(_Direction == Vector2Int.down)
                    transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                else if(_Direction == Vector2Int.right)
                    transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                else if(_Direction == Vector2Int.left)
                    transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);

                _PassedTime = 0.0f;
                _TurnAngle = 90.0f;
                _IsWalk = false;
            }
        }

        //クリア確認
        if (clipInfo.clip.name == "Clear")
        {
            if (!_IsClear)
            {
                var clearScreenScript = _ClearScreen.GetComponent<ClearScreen>();
                clearScreenScript.DisplayClearScreen(_TurnManager.GetTurnCount());
                _IsClear = true;
            }
        }

        //ゲームオーバー確認
        if (clipInfo.clip.name == "GameOvered")
        {
            if (!_IsGameOver)
            {
                var gameOverScript = _GameOverScreen.GetComponent<GameOverScreen>();
                gameOverScript.DisplayGameOverScreen();
                _IsGameOver = true;
            }
        }

    }

    private void Move(Vector2Int direction)
    {
        //前のブロック取得
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        var camera = _GameManagerScript.GetCamera();

        if (camera)
            camera.GetComponent<MainCameraScript>().SetIsPlayerMove(true);

        //ゆっくり歩くために現在地と目的地を取得
        _WalkStartPosition = transform.position;
        _WalkTargetPosition = block.transform.position;
        _WalkTargetPosition.y = transform.position.y;

        //Walkのアニメーション開始
        _Animator.SetBool("Walk", true);

        //親を切り替える
        transform.parent = block.transform.GetChild(0).transform;

        //移動
        _LocalPosition += _Direction;

    }

    public void RotateMySelf(Vector2Int position, float angle, float axisX = 0.0f, float axisY = 1.0f, float axisZ = 0.0f)
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
        //カメラ位置の更新
        var camera = _GameManagerScript.GetCamera();
        if (camera)
            camera.GetComponent<MainCameraScript>().SetIsPlayerSwap();

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

        //カメラ位置の更新
        var camera = _GameManagerScript.GetCamera().GetComponent<MainCameraScript>();
        if (camera)
            camera.SetIsPlayerTurnOver();
        else
            Debug.Log("かめらないよ！！！！");

        //ひっくり返す
        _IsFront = !_IsFront;

        //モデルを反転
        RotateMySelf(_LocalPosition, 180.0f, axis.x, axis.y, axis.z);

        AudioManager.Instance.ReverseBGM(_IsFront);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        AudioManager.Instance.PlaySE(_AudioClip[2]);
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
        block.GetComponent<BlockConfig>().PanelRemoveAttention(_IsFront);
        blockScript.TurnOver(_IsFront, _Direction);
        _Animator.SetBool("Action", true);
        SetFrontBlock();
    }

    public bool PlayerTurn()
    {
        //アニメーション遷移中だったら動かなくする
        if (_Animator.IsInTransition(0))
            return false;

        //そもそも死んでいたらターンは来ない
        if (!_IsExist)
            return false;


        //現在のアニメーション情報を取得
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        //アニメーションが再生中はコマンド操作を受け付けない
        if (clipInfo.clip.name == "Walk" || clipInfo.clip.name == "PanelAction" || clipInfo.clip.name == "Capture" || clipInfo.clip.name == "GameOver")
            return false;

        //ターン開始時に手動で取得
        if (_FrontBlock == null)
            SetFrontBlock();

        //コマンドの描画
        //Underも設定しなければいけないので要変更
        //前にブロックはあるが何もできないときも描画したくない
        if (!_CommandScript.IsDraw() && _FrontBlock != null)
        {
            var enemy = CheckEnemy(_LocalPosition + _Direction);

            bool isUnder = (enemy != null || _CommandAction == null);
            
            var blockScript = _FrontBlock.GetComponent<BlockConfig>();

            _CommandScript.SetUnder(!isUnder);
            _CommandScript.SetActPattern(blockScript, enemy, _IsFront, _CanMove);
            _CommandScript.SetDraw(true);
        }


        //向きが決まっていなければ決める
        if (!_SelectDirection)
        {
            //プレイヤー左右回転 WASDで向きが変わるようにする
            if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("Controller_L_Stick_Horizontal") > 0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") > 0.5f)
                SelectDirection(Vector2Int.right);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("Controller_L_Stick_Horizontal") < -0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") < -0.5f)
                SelectDirection(Vector2Int.left);
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("Controller_L_Stick_Vertical") > 0.5f || Input.GetAxis("Controller_D_Pad_Vertical") > 0.5f)
                SelectDirection(Vector2Int.up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("Controller_L_Stick_Vertical") < -0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") < -0.5f)
                SelectDirection(Vector2Int.down);

            //決定を押してコマンド選びの項目へ行く
            if (Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return))
            {
                //前にパネルがなければコマンドを表示しない
                if (!_FrontBlock)
                    return false;

                //前のパネルが何もできないなら
                if (!_CanMove)
                {
                    if (_CommandAction == null)
                        return false;

                    _CommandTop = false;
                }

                var enemy = CheckEnemy(_LocalPosition + _Direction);

                _CommandScript.ActiveSelectCommand(_CanMove, enemy);
                _SelectDirection = true;
            }
        }
        //決まっていればコマンド決め
        else if (!_SelectCommand)
        {
            //上下矢印でコマンド選択
            if (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("Controller_L_Stick_Vertical") > 0.5f || Input.GetAxis("Controller_D_Pad_Vertical") > 0.5f)
            {
                if (_CanMove && CheckEnemy(_LocalPosition + _Direction) == null)
                {
                    _CommandTop = true;
                    _CommandScript.CommandSelect(_CommandTop);
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("Controller_L_Stick_Vertical") < -0.5f || Input.GetAxis("Controller_D_Pad_Vertical") < -0.5f)
            {
                if (_CommandAction != null && CheckEnemy(_LocalPosition + _Direction) == null)
                {
                    _CommandTop = false;
                    _CommandScript.CommandSelect(_CommandTop);
                }
            }

            //決定を押してターンを進める
            if (Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return))
            {
                var enemy = CheckEnemy(_LocalPosition + _Direction);

                //前に敵がいたら捕まえる
                if (enemy)
                {
                    CaptureEnemy(enemy);
                    return true;
                }

                //上部なら移動、下部ならactを行う
                if (_CommandTop)
                    PlayerMove();
                else
                    _CommandAction(_FrontBlock);

                _CommandTop = true;
                _CommandScript.SetDraw(false);
                _SelectDirection = false;
                return true;
            }

            //方向を決め直す
            else if (Input.GetButtonDown("Controller_A") || Input.GetKeyDown(KeyCode.Backspace))
            {
                _SelectDirection = false;

                var enemy = CheckEnemy(_LocalPosition + _Direction);

                _CommandScript.UnactiveCommand(_CommandTop, enemy);
            }
        }

        return false;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }
    public bool GetIsExist() { return _IsExist; }


    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront) { _IsFront = isFront; }
    public void SetIsExist(bool isExist)
    {
        if (!isExist)
            SetDead();
        _IsExist = isExist;
    }


    private bool SetFrontBlock()
    {
        BlockConfig blockScript;

        _CanMove = false;

        //既に取得しているブロック情報は削除しておく
        if (_FrontBlock)
        {
            blockScript = _FrontBlock.GetComponent<BlockConfig>();
            blockScript.PanelRemoveAttention(_IsFront);
        }

        //新しいブロック情報を取得
        _FrontBlock = _GameManagerScript.GetBlock(_LocalPosition + _Direction);
        if (_FrontBlock == null)
            return false;

        blockScript = _FrontBlock.GetComponent<BlockConfig>();
        blockScript.PanelAttention(_IsFront);

        //移動
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            _CanMove = true;
        //回転
        if (blockScript.CheckPanelRotate(_IsFront))
        {
            _CommandAction = PlayerRotate;
            return true;
        }
        //入替
        if (blockScript.CheckPanelSwap(_IsFront))
        {
            _CommandAction = PlayerSwap;
            return true;
        }
        //反転
        if (blockScript.CheckPanelTurnOver(_IsFront))
        {
            _CommandAction = PlayerTurnOver;
            return true;
        }

        //どれもできなければnullを入れておく
        _CommandAction = null;

        return false;
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
        if (_FrontBlock == null)
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

    private void SelectDirection(Vector2Int dir)
    {
        //自分が今向いてる方向から押した方向に向きを変える

        //今の向きとこれから向く向きが同じなら何もしない
        if (dir == _Direction)
            return;

        _CommandScript.SetDraw(false);

        _Animator.SetBool("Walk", true);
        _IsWalk = true;

        //内積を使って計算で向く方向と角度を決める
        _TurnAngle = Vector2.SignedAngle(_Direction, dir) * -1;

        RotateMySelf(_LocalPosition, _IsFront ? _TurnAngle : -_TurnAngle);

        SetFrontBlock();
    }

    private void CaptureEnemy(GameObject enemy)
    {
        //エネミーが正面にいれば捕まえる
        if (enemy != null)
        {
            AudioManager.Instance.PlaySE(_AudioClip[5]);

            _Animator.SetTrigger("Capture");
            _GameManagerScript.KillEnemy(enemy);
            var remainEnemy = _GameManagerScript.GetEnemys();

            //敵がいなくなったことを確認したらゲームを終わらせに行く
            if (remainEnemy.Count <= 0)
                _Animator.SetBool("Clear", true);
        }
    }

}

