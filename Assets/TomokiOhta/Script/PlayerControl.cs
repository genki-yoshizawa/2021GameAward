using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("使いたい音声ファイルを入れてください。")]
    [SerializeField] private AudioClip[] _AudioClip;

    [Header("プレイヤーの向きを入れてください。")]
    [SerializeField]private Vector2Int _Direction;
    private Vector2Int _StartDirection;

    [Header("吹き出し")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("歩く時間")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("方向転換にかかる時間")]
    [SerializeField] private float _RotateTime = 0.5f;

    [Header("クリア画面"), SerializeField] private GameObject _ClearScreen;
    [Header("ゲームオーバー画面"), SerializeField] private GameObject _GameOverScreen;

    private bool _NowWalkAnim = false;

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

    //キー入力で右を押したのか？
    private bool _IsRight = false;

    //クリア及びGameOver判定
    private bool _IsClear;
    private bool _IsGameOver;


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

        _CorsorStartPosition = _FukidasiObj.transform.GetChild(_AnimMax).localPosition;

        _TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();

        AudioManager.Instance.PlayGameBGM(_AudioClip[0], _AudioClip[1]);

        _IsClear = false;
        _IsGameOver = false;
    }

    public void Update()
    {
        // 歩くアニメーション
        if (_Animator.GetBool("Walk") && _NowWalkAnim == false)
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
        if (_Animator.GetBool("Walk") && _NowWalkAnim == true)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _RotateTime)
            {
                _Animator.SetBool("Walk", false);
            }

            transform.Rotate(0.0f, 90.0f * (time / _RotateTime) * (_IsRight ? 1.0f : -1.0f), 0.0f);

            if (!_Animator.GetBool("Walk"))
            {
                _PassedTime = 0.0f;
                _NowWalkAnim = false;
            }
        }

        //現在のアニメーション情報を取得
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

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
        _GameManagerScript.GetCamera().transform.GetComponent<CameraWork>().PlayerMoveCameraWork(_LocalPosition + direction);

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
        //カメラ位置の更新
        _GameManagerScript.GetCamera().GetComponent<CameraWork>().PlayerSwapCameraWork();

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
        var camera = _GameManagerScript.GetCamera().GetComponent<CameraWork>();
        if (camera != null)
            camera.PlayerTurnCameraWork();
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
        bool turnEnd = false;

        //アニメーション遷移中だったら動かなくする
        if (_Animator.IsInTransition(0))
            return turnEnd;

        //現在のアニメーション情報を取得
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        //アニメーションが再生中はコマンド操作を受け付けない
        if (clipInfo.clip.name == "Walk" || clipInfo.clip.name == "PanelAction" || clipInfo.clip.name == "Capture")
            return turnEnd;

        //死ぬ
        if (clipInfo.clip.name == "GameOver")
            return turnEnd;

        //Startで取得するのでターン開始時に手動で取得
        if (_FrontBlock == null)
            SetFrontBlock();

        //吹き出しのアニメーション終了を確認したら生成する
        if (_FukidasiScript.GetAnimPattern() == -1 && _IsExist)
        {
            var enemys = _GameManagerScript.GetEnemys();

            //バグ 移動と捕獲が同時に出る
            if (!(_FrontBlock == null || enemys.Count == 0))
            {
                //前のパネルが何かできる
                if (_CanActionList.Count != 0)
                {
                    var icon = _FukidasiObj.transform.GetChild(_AnimMax).GetComponent<RectTransform>();

                    if (CheckEnemy(_LocalPosition + _Direction) != null)
                    {
                        //前に敵がいたのでそれ用の画像を出す
                        _FukidasiScript.SetAnimPattern(1);
                        _FukidasiScript.SetActPattern(_CanActionList, true);

                        //カーソル位置の設定
                        _CommandSelect = 0;
                    }
                    else
                    {
                        _FukidasiScript.SetAnimPattern(_CanActionList.Count);
                        _FukidasiScript.SetActPattern(_CanActionList);

                        //カーソル位置の設定
                        _CommandSelect = _CanActionList.Count - 1;
                    }

                    AudioManager.Instance.PlaySE(_AudioClip[3]);

                    //カーソルを一番上に設定
                    icon.anchoredPosition =
                        new Vector3(icon.localPosition.x, _CorsorStartPosition.y + (20.0f * _CommandSelect), _CorsorStartPosition.z);
                }
            }
        }

        //プレイヤー左右回転
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Controller_L_Stick_Horizontal") > 0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") > 0.5f)
        {
            _Animator.SetBool("Walk", true);
            _NowWalkAnim = true;
            _IsRight = true;

            RotateMySelf(_LocalPosition, _IsFront ? 90.0f : -90.0f);
            
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Controller_L_Stick_Horizontal") < -0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") < -0.5f)
        {
            //向き変更時に歩行アニメーション再生
            _Animator.SetBool("Walk", true);
            _NowWalkAnim = true;
            _IsRight = false;

            RotateMySelf(_LocalPosition, _IsFront ? -90.0f : 90.0f);
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }

        //上下矢印でコマンド選択
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Controller_L_Stick_Vertical") > 0.5f || Input.GetAxis("Controller_D_Pad_Vertical") > 0.5f)
        {
            if (_CommandSelect < _CanActionList.Count - 1 && CheckEnemy(_LocalPosition + _Direction) == null)
            {
                _CommandSelect++;

                //アイコンのtransform取得
                var icon = _FukidasiObj.transform.GetChild(_AnimMax).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y + 20.0f, icon.localPosition.z);

                //文字のsprite変更
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Controller_L_Stick_Vertical") < -0.5f || Input.GetAxis("Controller_D_Pad_Vertical") < -0.5f)
        {
            if (_CommandSelect > 0 && CheckEnemy(_LocalPosition + _Direction) == null)
            {
                _CommandSelect--;

                //アイコンのtransform取得
                var icon = _FukidasiObj.transform.GetChild(_AnimMax).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y - 20.0f, icon.localPosition.z);

                //文字のsprite変更
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }

        //Enterキーで行動 ターンを進める
        if ( Input.GetButtonDown("Controller_B")|| Input.GetKeyDown(KeyCode.Return))
        {
            if (_FrontBlock == null)
                return turnEnd;

            var enemy = CheckEnemy(_LocalPosition + _Direction);
            if (enemy != null)
            {
                AudioManager.Instance.PlaySE(_AudioClip[5]);

                _Animator.SetTrigger("Capture");
                _GameManagerScript.KillEnemy(enemy);
                var remainEnemy = _GameManagerScript.GetEnemys();

                //敵がいなくなったことを確認したらゲームを終わらせに行く
                if (remainEnemy.Count <= 0)
                {
                    _Animator.SetBool("Clear", true);
                }
                _FukidasiScript.ResetAnimPattern();
                turnEnd = true;
            }
            else if(_CanActionList.Count != 0)
            {
                AudioManager.Instance.PlaySE(_AudioClip[4]);

                CommandAction(_CommandSelect);
                _FukidasiScript.ResetAnimPattern();
                turnEnd = true;
            }
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
        if (!isExist)
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

        //回転可能なら1を入れる
        if (blockScript.CheckPanelRotate(_IsFront))
            _CanActionList.Add(1);

        //移動可能なら0を入れる
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
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
                        PlayerMove();
                        break;
                    case 1:
                        PlayerRotate(_FrontBlock);
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

