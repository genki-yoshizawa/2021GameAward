using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{

    protected enum EnemyState
    {
        IDLE,   // 相手のターン中
        STAY,   // 自分のターンに何もしない
        MOVE,   // 移動
        BREAK   // 壁を壊す
    }

    protected enum EnemyLevel
    {
        LEVEL1,     // 行動しない。待機モーションのみ
        LEVEL2,     // 2ターンに一度行動する。かじることはしない
        LEVEL3,     // 2ターンに一度行動、たまに1ターンに一度行動する。かじることはしない
        LEVEL4,     // 2ターンに一度行動、たまに1ターンに一度行動する。レベル１の硬さのオブジェクトをかじる
        LEVEL5,     // 毎ターン行動する。レベル1の硬さのオブジェクトをかじる
        LEVEL6,     // 毎ターン行動する。レベル２の硬さのオブジェクトをかじる
        LEVEL7      // 毎ターン行動する。レベル3の硬さのオブジェクトをかじる
    }

    protected enum BreakTurn
    {
        RANDOM,
        EVERYTURN
    }


    // 必要そうな変数をとりあえず用意
    protected EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] protected EnemyLevel _EnemyLevel = EnemyLevel.LEVEL1;
    protected EnemyState _NextState = EnemyState.IDLE;


    [Header("１ターンの行動回数")]
    [SerializeField, TooltipAttribute("１ターンの行動回数"), Range(0, 2)] protected int _ActCount;           // 敵の行動回数    （１～２？）

    [Header("壁をかじるレベル")]
    [SerializeField, TooltipAttribute("かじれる壁のレベル"), Range(0, 3)] protected int _BreakLevel;       // かじるレベル    （０～３？）

    [Header("ランダムorターン毎")]
    [SerializeField] protected BreakTurn _BreakTurn = BreakTurn.RANDOM;

    [Header("嗅覚範囲")]
    [SerializeField] private int _CheeseSearchRange = 0;

    [Header("デバッグカラーの設定")]
    [SerializeField] private Color _DebugColor = new Color(1f, 1f, 0, 0.5f);

    [Header("何秒かけて移動するか")]
    [SerializeField] protected float _WalkTime = 1.0f;

    [Header("何秒かけてかじるか")]
    [SerializeField] protected float _BiteTime = 1.0f;

    [Header("何秒間パニックするか")]
    [SerializeField] protected float _PanicTime = 1.0f;

    [Header("何秒間後にCapturedを再生するか")]
    [SerializeField] private float _CapturedDelayTime = 1.0f;

    [Header("表世界時のテクスチャ")]
    [SerializeField] Texture _FrontTexture;
    [Header("裏世界時のテクスチャ")]
    [SerializeField] Texture _BackTexture;
    [Header("ネズミに使ってるマテリアルを入れる")]

    public Material TargetMaterial;
    [Header("↓デバッグ用")]


    // それぞれにGet・Setを作成？
    [SerializeField] private Vector2Int _EnemyLocalPosition;      // ネズミのいるブロックの座標
    private Vector2Int _EnemyDirection;          // ネズミの向いてる方向
    private bool _StartEnemyTurn;                // エネミーターンが始まった最初に処理する用

    private GameObject _GameManager;                              // ゲームマネージャーを保持
    protected GameObject _Player;                                 // プレイヤーを保持
    private GameObject _Cheese;                                   // チーズを保持

    // デバッグ用に表示させてだけなので後々SerializeFieldは消す予定
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // 移動可能ブロックの保持、進む先のブロックを保持

    private int _TurnCount;
    private int _Count;
    private float _PosY = 0.075f;    // Y座標固定用

    private Animator _EnemyAnimation;
    private Vector3 _StartPoint;
    private Vector3 _TargetPoint;
    private Vector3 _UpdatePosition;
    private float _PassedTime;
    private bool _CheeseBite;
    private bool _PlayerBite;
    private bool _IsExist;
    private bool _IsFront;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        GameObject parent = transform.root.gameObject;
        _EnemyLocalPosition = parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
        _IsFront = (transform.parent == transform.parent.parent.GetChild(0));
        _EnemyAnimation = gameObject.GetComponent<Animator>();
        _CheeseBite = false;
        _IsExist = false;
        _Count = 0;
        _PassedTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {

        if (_EnemyState == EnemyState.IDLE)
        {
            Idle();
        }

        // パニックアニメーション
        if (_EnemyAnimation.GetBool("Panic"))
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _PanicTime)
            {
                _EnemyAnimation.SetBool("Panic", false);
                _PassedTime = 0.0f;

            }

        }

        // 歩くアニメーション
        if (_EnemyAnimation.GetBool("Walk"))
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                _PassedTime = _WalkTime;
                _EnemyAnimation.SetBool("Walk", false);
            }

            Rotate();
            transform.position = _StartPoint + (_TargetPoint - _StartPoint) * (_PassedTime / _WalkTime);

            if (!_EnemyAnimation.GetBool("Walk"))
                _PassedTime = 0.0f;

        }

        // かじるアニメーション
        if (_EnemyAnimation.GetBool("Bite"))
        {
            // チーズをかじるときのアニメーション　かじりながら移動
            if (_CheeseBite)
            {
                _Cheese.gameObject.GetComponent<CheeseControl>().Eaten();
                float time = Time.deltaTime;
                if ((_PassedTime += time) > _WalkTime)
                {
                    _PassedTime = _WalkTime;
                    _EnemyAnimation.SetBool("Bite", false);
                }

                Rotate();
                transform.position = _StartPoint + (_TargetPoint - _StartPoint) * (_PassedTime / _WalkTime);

                if (!_EnemyAnimation.GetBool("Bite"))
                {
                    _PassedTime = 0.0f;
                    _CheeseBite = false;
                }

            }
            else
            {
                float time = Time.deltaTime;
                Rotate();
                if ((_PassedTime += time) > _BiteTime)
                {
                    _EnemyAnimation.SetBool("Bite", false);
                    _PassedTime = 0.0f;

                }
            }

        }

        var clipInfo = _EnemyAnimation.GetCurrentAnimatorClipInfo(0)[0];   // 引数はLayer番号、配列の0番目

        // 現在のアニメーションがDeadだったらオブジェクトを削除する
        if (clipInfo.clip.name == "Dead")
        {
            Destroy(this.gameObject);
        }

    }

    void Wait()
    {
        Vector2Int pos = _EnemyLocalPosition;

        // 前後左右にブロックがあるか
        if (_IsFront)
        {

            // 上のブロック
            _EnemyDirection = new Vector2Int(0, 1);
            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // 下のブロック
            _EnemyDirection = new Vector2Int(0, -1);
            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // 左のブロック
            _EnemyDirection = new Vector2Int(-1, 0);
            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // 右のブロック
            _EnemyDirection = new Vector2Int(1, 0);
            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));


        }
        else
        {
            // 上のブロック
            _EnemyDirection = new Vector2Int(0, 1);
            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // 下のブロック
            _EnemyDirection = new Vector2Int(0, -1);
            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // 左のブロック
            _EnemyDirection = new Vector2Int(-1, 0);
            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // 右のブロック
            _EnemyDirection = new Vector2Int(1, 0);
            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));
        }


        // ここで壁があるパネルを取得して、何枚以上あったらかじるようにする？
        // 取得したブロックに壁があるかを判定する
        // ChangeStateで壁を壊すか、動く
        // Moveする前にそのブロックにプレイヤーがいるかいないかを判定させていなければ進めるように処理を変える
        // 


        //List<GameObject> moveblocks = new List<GameObject>();

        //// 移動できるブロックをいれる
        //if (_Up != null)
        //{
        //    moveblocks.Add(_Up);
        //}
        //if (_Down != null)
        //{
        //    moveblocks.Add(_Down);
        //}
        //if (_Left != null)
        //{
        //    moveblocks.Add(_Left);
        //}
        //if (_Right != null)
        //{
        //    moveblocks.Add(_Right);
        //}


    }

    // 待機関数
    void Idle()
    {
        // _EnemyAnimation.SetBool("Wait", true);
        if (_Count > 0)
        {
            EnemyTurn();
        }

    }

    // 移動もかじるもしない待機モーションのみでターン終了
    void Stay()
    {
        _EnemyAnimation.SetBool("Panic", true);
        _Count--;
        _EnemyState = EnemyState.IDLE;
    }

    // 移動関数
    void Move()
    {

        if (_IsFront)
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(0).gameObject;
            transform.parent = o.transform;

            _TargetPoint = o.transform.position;
            _StartPoint = this.transform.position;
            _TargetPoint.y = _PosY;

            // ネズミのローカルポジションを更新
            _EnemyLocalPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();

            if (_Cheese != null)
            {
                Vector2Int cheeselocalposition = _Cheese.gameObject.GetComponent<CheeseConfig>().GetCheeseLocalPosition();

                if (cheeselocalposition == _EnemyLocalPosition)
                {
                    _CheeseBite = true;
                    _EnemyAnimation.SetBool("Bite", true);
                }
                else
                {
                    _EnemyAnimation.SetBool("Walk", true);
                }
            }
            else
            {
                _EnemyAnimation.SetBool("Walk", true);

            }
        }
        else
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(1).gameObject;
            transform.parent = o.transform;

            _TargetPoint = o.transform.position;
            _StartPoint = this.transform.position;
            _TargetPoint.y = -_PosY;

            // ネズミのローカルポジションを更新
            _EnemyLocalPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();

            if (_Cheese != null)
            {
                Vector2Int cheeselocalposition = _Cheese.gameObject.GetComponent<CheeseConfig>().GetCheeseLocalPosition();

                if (cheeselocalposition == _EnemyLocalPosition)
                {
                    _CheeseBite = true;
                    _EnemyAnimation.SetBool("Bite", true);
                }
                else
                {
                    _EnemyAnimation.SetBool("Walk", true);
                }
            }
            else
            {
                _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();

                if (!_Player.gameObject.GetComponent<PlayerControl>().GetIsFront())
                {
                    if (_Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition() == _EnemyLocalPosition)
                    {
                        PlayerKill();

                    }
                    else
                        _EnemyAnimation.SetBool("Walk", true);
                }

                _EnemyAnimation.SetBool("Walk", true);

            }

        }
        _Count--;
        _NextBlock = null;
        // ステートをIDLEに移行する
        _EnemyState = EnemyState.IDLE;

        // ネズミのターンを終了する

    }

    // 壁をかじる関数
    void Break()
    {
        Rotate();

        _EnemyAnimation.SetBool("Bite", true);

        _NextBlock.gameObject.GetComponent<BlockControl>().BreakWall(_IsFront, _EnemyLocalPosition, _EnemyDirection, _BreakLevel);

        if (_BreakTurn == BreakTurn.RANDOM)
        {

        }

        _Count--;

        _EnemyState = EnemyState.IDLE;

    }


    public void EnemyTurn()
    {
        // 生きていれば動ける
        if (_IsExist == false)
        {
            _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
            if (_Player.GetComponent<PlayerControl>().GetLocalPosition() != _EnemyLocalPosition)
            {
                if (_StartEnemyTurn)
                {
                    _Count = _ActCount;
                    _StartEnemyTurn = false;
                }

                Wait();
                // エネミーステートを変更する関数を呼ぶ


                // 右Shiftで裏に行く（戻れない）
                //if (Input.GetKeyDown(KeyCode.RightShift))
                //{
                //    _IsFront = false;
                //    this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

                //    GameObject ob;
                //    GameObject parent = transform.root.gameObject;
                //    ob = parent.transform.GetChild(1).gameObject;
                //    transform.parent = ob.transform;

                //}

                // ここでレベル別に処理を書く？
                // ChangeState();
                switch (_EnemyLevel)
                {
                    case EnemyLevel.LEVEL1:
                        Level1();
                        break;
                    case EnemyLevel.LEVEL2:
                        Level2();
                        break;
                    case EnemyLevel.LEVEL3:
                        Level3();
                        break;
                    case EnemyLevel.LEVEL4:
                        Level4();
                        break;
                    case EnemyLevel.LEVEL5:
                        Level5();
                        break;
                    case EnemyLevel.LEVEL6:
                        Level6();
                        break;
                    case EnemyLevel.LEVEL7:
                        Level7();
                        break;
                }


                switch (_EnemyState)
                {
                    case EnemyState.IDLE:
                        Idle();
                        break;
                    case EnemyState.STAY:
                        Stay();
                        break;
                    case EnemyState.MOVE:
                        Move();
                        break;
                    case EnemyState.BREAK:
                        Break();
                        break;

                }
            }
            else
            {
                if (!_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() && !_IsFront)
                {
                    PlayerKill();

                }

            }

        }



    }

    // 何も行動しない　待機モーションのみ
    public void Level1()
    {
        _EnemyState = EnemyState.STAY;
    }

    // ２ターンに１度行動する　かじることはしない
    public void Level2()
    {
        // プレイヤーのいるブロックを取得して
        // プレイヤーから一番遠いブロックへ逃げる
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        Vector2Int movedirection = new Vector2Int();

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;


        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;
        }
        else
        {

            // チーズみつけてる
            if (_Cheese != null)
            {
                // 表
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }
                // 裏
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }

                // チーズとネズミのいる位置を比較
                tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
                if (moveobj != null)
                {
                    // チーズとムーブ先のパネルの位置を比較
                    tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }
                }
                else
                {
                    _EnemyState = EnemyState.STAY;

                }
            }
            // チーズ見つけてない
            else
            {
                // 表
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);


                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);

                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);

                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                        }
                    }

                    // プレイヤーとネズミの距離を取得
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // プレイヤーとムーブ先のパネルの距離を取得
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                        _EnemyState = EnemyState.STAY;

                }
                // 裏
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }

                    // プレイヤーとネズミの距離を取得
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // プレイヤーとムーブ先のパネルの距離を取得
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                        _EnemyState = EnemyState.STAY;

                }
            }
            _TurnCount = 0;
        }
    }

    // ２ターンに１度行動する　たまに１ターンに１度行動（現状30%位）　かじることはしない
    public void Level3()
    {
        // プレイヤーのいるブロックを取得して
        // プレイヤーから一番遠いブロックへ逃げる
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        Vector2Int movedirection = new Vector2Int();

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;

        random = Random.value;

        if (random < 0.3)
        {
            _TurnCount++;
        }

        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;
        }
        else
        {

            // チーズみつけてる
            if (_Cheese != null)
            {
                // 表
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }
                // 裏
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }

                // チーズとネズミのいる位置を比較
                tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
                if (moveobj != null)
                {
                    // チーズとムーブ先のパネルの位置を比較
                    tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }
                }
                else
                {
                    _EnemyState = EnemyState.STAY;

                }
            }
            // チーズ見つけてない
            else
            {
                // 表
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);


                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);

                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);

                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                        }
                    }

                    // プレイヤーとネズミの距離を取得
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // プレイヤーとムーブ先のパネルの距離を取得
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                         _EnemyState = EnemyState.STAY;

                }
                // 裏
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }

                    // プレイヤーとネズミの距離を取得
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // プレイヤーとムーブ先のパネルの距離を取得
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                        _EnemyState = EnemyState.STAY;

                }
            }
            _TurnCount = 0;
        }
    }


    // ２ターンに１度行動する　たまに１ターンに１度行動（現状50%位）　レベル１の壁をかじる
    public void Level4()
    {
        // プレイヤーのいるブロックを取得して
        // プレイヤーから一番遠いブロックへ逃げる
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        GameObject breakobj = null;

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float breakdistance = 0.0f;
        float breakdistance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;
        Vector2Int movedirection = new Vector2Int();
        Vector2Int breakdirection = new Vector2Int();


        random = Random.value;

        if (random < 0.5)
        {
            _TurnCount++;
        }

        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;

        }
        else
        {


            // チーズみつけてる
            if (_Cheese != null)
            {
                // 表
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }
                }
                // 裏
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }
                }

                // チーズとネズミのいる位置を比較
                tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
                if (moveobj != null)
                {
                    // チーズとムーブ先のパネルの位置を比較
                    tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                    if (breakobj != null)
                    {
                        // チーズと壊す壁の位置を比較
                        float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            if (tmp3 < tmp)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }

                        }

                    }
                    else
                    {
                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }

                    }
                }
                else
                {
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                        if (tmp3 < tmp2)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }

                }
            }
            // チーズ見つけてない
            else
            {
                // 表
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);


                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance = tmp;
                                }

                            }
                        }

                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);

                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance = tmp;
                                    }


                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);

                        if (tmp == distance)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }

                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp2 < tmp)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                if (tmp < tmp3)
                                {
                                    _EnemyDirection = breakdirection;
                                    _NextBlock = breakobj;
                                    _EnemyState = EnemyState.BREAK;
                                }
                                else
                                {
                                    _EnemyState = EnemyState.STAY;
                                }
                            }

                        }
                        else
                        {
                            if (tmp2 < tmp)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }
                    }
                    else
                    {
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp2 < tmp3)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }

                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
                // 裏
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    // プレイヤーとネズミの位置の距離を取得
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp < tmp2)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                if (tmp3 < tmp)
                                {
                                    _EnemyDirection = breakdirection;
                                    _NextBlock = breakobj;
                                    _EnemyState = EnemyState.BREAK;
                                }
                                else
                                {
                                    _EnemyState = EnemyState.STAY;
                                }
                            }

                        }
                        else
                        {
                            if (tmp < tmp2)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }
                    }
                    else
                    {
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp3 < tmp2)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }

                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
            }
            _TurnCount = 0;
        }
    }

    // 毎ターン行動する。レベル1の硬さのオブジェクトをかじる
    public void Level5()
    {


        // プレイヤーのいるブロックを取得して
        // プレイヤーから一番遠いブロックへ逃げる
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        GameObject breakobj = null;

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float breakdistance = 0.0f;
        float breakdistance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;
        Vector2Int movedirection = new Vector2Int();
        Vector2Int breakdirection = new Vector2Int();


        // チーズみつけてる
        if (_Cheese != null)
        {
            // 表
            if (_IsFront)
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }
            }
            // 裏
            else
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }
            }

            // チーズとネズミのいる位置を比較
            tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
            if (moveobj != null)
            {
                // チーズとムーブ先のパネルの位置を比較
                tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                if (breakobj != null)
                {
                    // チーズと壊す壁の位置を比較
                    float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }
                    else
                    {
                        if (tmp3 < tmp)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }

                }
                else
                {
                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }

                }
            }
            else
            {
                if (breakobj != null)
                {
                    float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                    if (tmp3 < tmp2)
                    {
                        _EnemyDirection = breakdirection;
                        _NextBlock = breakobj;
                        _EnemyState = EnemyState.BREAK;
                    }

                }

            }


        }
        // チーズ見つけてない
        else
        {
            // 表
            if (_IsFront)
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(playerpos, _Up.transform.position);
                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);


                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance = tmp;
                            }

                        }
                    }

                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance = tmp;
                                }


                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance = tmp;
                            }

                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance = tmp;
                            }

                        }
                    }
                }

                tmp2 = Vector3.Distance(playerpos, this.transform.position);
                if (moveobj != null)
                {
                    tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            if (tmp < tmp3)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }

                    }
                    else
                    {
                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
                else
                {
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp2 < tmp3)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }
                    else
                    {
                        _EnemyState = EnemyState.STAY;
                    }
                }
            }
            // 裏
            else
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(playerpos, _Up.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                // プレイヤーとネズミの位置の距離を取得
                tmp2 = Vector3.Distance(playerpos, this.transform.position);
                if (moveobj != null)
                {
                    tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            if (tmp3 < tmp)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }

                    }
                    else
                    {
                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
                else
                {
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp3 < tmp2)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }
                    else
                    {
                        _EnemyState = EnemyState.STAY;
                    }
                }
            }





        }


    }

    // 毎ターン行動する。レベル２の硬さのオブジェクトをかじる
    public void Level6()
    {

    }

    // 毎ターン行動する。レベル３の硬さのオブジェクトをかじる
    public void Level7()
    {

    }

    public void PlayerKill()
    {
        _EnemyAnimation.SetTrigger("Attack");
        _Player.gameObject.GetComponent<PlayerControl>().SetIsExist(false);

    }

    // エネミーを行く方向・かじる方向へ回転させる
    public void Rotate()
    {
        if (_IsFront)
            TargetMaterial.SetTexture("_MainTex", _FrontTexture);
        else
            TargetMaterial.SetTexture("_MainTex", _BackTexture);


        //_UpdatePosition = this.transform.position;

        //if(_IsFront)
        //    _UpdatePosition.y = _PosY;
        //else
        //    _UpdatePosition.y = _PosY * -1;

        //this.transform.position = _UpdatePosition;

        float y = 90.0f;
        if (_IsFront)
        {
            if (_EnemyDirection == new Vector2Int(0, 1))
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            if (_EnemyDirection == new Vector2Int(0, -1))
                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
            if (_EnemyDirection == new Vector2Int(-1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y * -1, 0.0f);
            if (_EnemyDirection == new Vector2Int(1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
        }
        else
        {
            if (_EnemyDirection == new Vector2Int(0, 1))
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
            if (_EnemyDirection == new Vector2Int(0, -1))
                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, -180.0f);
            if (_EnemyDirection == new Vector2Int(-1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y * -1, -180.0f);
            if (_EnemyDirection == new Vector2Int(1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y, -180.0f);
        }

    }

    // ブロック側で呼び出す　自分を回転させる関数
    public void RotateMySelf(Vector2Int position, float angle)
    {
        //Rotate時に呼び出される関数、自分の方向を変えるときにも自分で呼ぶ
        if (position != _EnemyLocalPosition)
            return;

        Vector3 direction = new Vector3(_EnemyDirection.x, 0f, _EnemyDirection.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        Vector2 tmp = new Vector2(direction.x, direction.z);
        //四捨五入して代入することでVector2Intにも無理やり代入させる
        _EnemyDirection = new Vector2Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y));
    }

    // ブロック側で呼び出す　自分をひっくり返す関数（表裏入れ替え）
    public void TurnOverMySelf(Vector2Int position)
    {
        //TurnOver時に呼び出される関数、ひっくり返すのはSetIsFrontで良いのでは？
        if (position != _EnemyLocalPosition)
            return;

        //ひっくり返す
        if (_IsFront)
            _IsFront = false;

        else
            _IsFront = true;

    }

    // ブロック側で呼び出す　自分の位置を入れ替える関数
    public void SwapMySelf(List<Vector2Int> position)
    {
        // ブロックのローカル
        //Swap時に呼び出される関数、親オブジェクトであるブロックの移動についていくだけ
        foreach (Vector2Int pos in position)
        {
            if (pos == _EnemyLocalPosition)
            {
                var blockConfig = transform.parent.parent.GetComponent<BlockConfig>();
                _EnemyLocalPosition = blockConfig.GetBlockLocalPosition();
                return;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = _DebugColor;
        Vector3 size = new Vector3(1.0f, 0.01f, 1.0f);

        for (int x = 0; x < 2 * _CheeseSearchRange + 1; ++x)
        {
            for (int z = 0; z < 2 * _CheeseSearchRange + 1; ++z)
            {
                Vector2 pos = new Vector2(x - _CheeseSearchRange, z - _CheeseSearchRange);
                Vector3 position = new Vector3(pos.x, 0.0f, pos.y);
                Gizmos.DrawCube(transform.position + position, size);
            }
        }
    }

    // エネミーを削除する処理
    public void SetDestroy()
    {
        _IsExist = true;

        transform.parent = null;
        StartCoroutine("DelayCapturedAnimation");
    }

    private IEnumerator DelayCapturedAnimation()
    {
        yield return new WaitForSeconds(_CapturedDelayTime);

        float y = 90.0f;

        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector2Int playerlocalposition = _Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition();

        // プレイヤーがエネミーの下のパネルにいるとき上を向く
        if (_EnemyLocalPosition.y > playerlocalposition.y)
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        // プレイヤーがエネミーの上のパネルにいるとき下を向く
        if (_EnemyLocalPosition.y < playerlocalposition.y)
            this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);

        // 右にプレイヤーがいたら左を向く
        if (_EnemyLocalPosition.x < playerlocalposition.x)
            this.transform.rotation = Quaternion.Euler(0.0f, y * -1, 0.0f);

        // 左にプレイヤーがいたら右を向く
        if (_EnemyLocalPosition.x > playerlocalposition.x)
            this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);


        _EnemyAnimation.SetBool("Captured", true);
    }

    public void SetIsFront(bool isfront) { _IsFront = isfront; }
    public bool GetIsFront() { return _IsFront; }
    public void SetStartEnemyTurn(bool enemyturn) { _StartEnemyTurn = enemyturn; }           // エネミーターンに変わったときにターンマネージャーでtrueにしてほしい
    public void SetLocalPosition(Vector2Int position) { _EnemyLocalPosition = position; }    // 自分のいるブロックの座標を更新する
    public Vector2Int GetLocalPosition() { return _EnemyLocalPosition; }
    public void SetCheese(GameObject cheese) { _Cheese = cheese; }
    public int GetCheeseSearchRange() { return _CheeseSearchRange; }
}

