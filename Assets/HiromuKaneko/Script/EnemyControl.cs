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


    public struct Panel
    {
        public GameObject PanelObj;
        public Vector2Int Direction;
    };

    // 必要そうな変数をとりあえず用意
    protected EnemyState _EnemyState = EnemyState.IDLE;

    [Header("１ターンの行動回数")]
    [SerializeField, TooltipAttribute("１ターンの行動回数"), Range(0, 2)] protected int _ActCount;         // 敵の行動回数    （１～２？）

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
    private int _WallCount;
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
    public float[] _WallDensity;

    // 逃げ先候補のブロックを保持する変数
    List<Panel> MovePanel = new List<Panel>();

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
        _WallCount = 0;
        _PassedTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {

        if (_IsFront)
            TargetMaterial.SetTexture("_MainTex", _FrontTexture);
        else
            TargetMaterial.SetTexture("_MainTex", _BackTexture);


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
    }

    // 待機関数
    void Idle()
    {

    }

    // 移動もかじるもしない待機モーションのみでターン終了
    void Stay()
    {
        _EnemyAnimation.SetBool("Panic", true);
        _WallCount--;
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
        _WallCount--;
        _NextBlock = null;
        MovePanel.Clear();
        _WallDensity = null;

        // ステートをIDLEに移行する
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
                    _WallCount = _ActCount;
                    _StartEnemyTurn = false;
                }

                Wait();

                MoveTest();

                Debug.Log("うぇいとむーぶ");

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
                    default:
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

    // 経路探索関数　
    public void FarRouteSearch(GameObject Panel, Vector2Int Direction)
    {
        if (Panel == null)
            return;

        // プレイヤーのいるポジションを取得
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        // プレイヤーとエネミーの距離を取得
        float player_enemy_distance = Vector3.Distance(playerpos, this.transform.position);

        // 上下左右のパネルと上記を比較して、短いのを除去する
        float tmp = Vector3.Distance(playerpos, Panel.transform.position);

        Panel _Panel = new Panel() { PanelObj = Panel, Direction = Direction };

        // プレイヤーとエネミーの距離よりも長ければリストで保持する？
        if (player_enemy_distance < tmp)
        {
            MovePanel.Add(_Panel);
        }

    }

    public void NearRouteSearch(GameObject Panel, Vector2Int Direction)
    {
        if (Panel == null)
            return;

        // プレイヤーのいるポジションを取得
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        // プレイヤーとエネミーの距離を取得
        float player_enemy_distance = Vector3.Distance(playerpos, this.transform.position);

        // 上下左右のパネルと上記を比較して、短いのを除去する
        float tmp = Vector3.Distance(playerpos, Panel.transform.position);

        Panel _Panel = new Panel() { PanelObj = Panel, Direction = Direction };

        // プレイヤーとエネミーの距離よりも長ければリストで保持する？
        if (player_enemy_distance > tmp)
        {
            MovePanel.Add(_Panel);
        }

    }
    // 壁の数を数えて壁密度をリターン
    public float WallCount(Panel moveobj)
    {
        Vector2Int Dir = new Vector2Int();

        // 壁測定をするパネルを入れるリスト
        List<Panel> WallCountPanel = new List<Panel>();

        // 上か下なら左右のＸに１を入れる  それ以外はＹに１を入れる
        if (moveobj.Direction == new Vector2Int(0, 1) || moveobj.Direction == new Vector2Int(0, -1))
        {
            Dir = new Vector2Int(1, 0);
        }
        else
        {
            Dir = new Vector2Int(0, 1);

        }

        // エネミーに隣接しているパネルを壁測定リストに入れる
        WallCountPanel.Add(moveobj);

        Panel obj = new Panel();
        Panel MinusDirPanel = new Panel();
        Panel PlusDirPanel = new Panel();

        // 上下なら左　左右なら下のパネルを取得
        MinusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + (-Dir));
        // 上下なら右　左右なら上のパネルを取得
        PlusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + Dir);
        // 逃げ先の奥のパネルを取得
        obj.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + moveobj.Direction);

        // nullじゃなかったらそのパネルをリストに入れる
        if (MinusDirPanel.PanelObj != null)
            WallCountPanel.Add(MinusDirPanel);
        if (PlusDirPanel.PanelObj != null)
            WallCountPanel.Add(PlusDirPanel);
        if (obj.PanelObj != null)
            WallCountPanel.Add(obj);

        // 上下なら左奥　左右なら下奥のパネルを取得
        MinusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + (-Dir + moveobj.Direction));
        // 上下なら右奥　左右なら上奥のパネルを取得
        PlusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + (Dir + moveobj.Direction));

        // nullじゃなかったらそのパネルをリストに入れる
        if (MinusDirPanel.PanelObj != null)
            WallCountPanel.Add(MinusDirPanel);
        if (PlusDirPanel.PanelObj != null)
            WallCountPanel.Add(PlusDirPanel);


        // 壁の数を数える
        foreach (Panel panel in WallCountPanel)
        {
            // そのパネルにオブジェクトの壁があるかを調べる。あればカウントを増やす
            CountWall(panel.PanelObj);

            // 外壁を数える　そのオブジェクトの前後左右をみてＮＵＬＬなら外壁としてカウントする
            // 上から上下左右
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(0, 1)) == null)
                _WallCount++;
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(0, -1)) == null)
                _WallCount++;
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(-1, 0)) == null)
                _WallCount++;
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(1, 0)) == null)
                _WallCount++;
        }

        // 壁密度を計算してリターン
        //          壁   /     パネル
        return _WallCount / WallCountPanel.Count;

    }

    public void MoveTest()
    {

        // チーズみつけてる
        if (_Cheese != null)
        {
            CheeseMove();
        }
        // チーズ見つけてない
        else
        {
            if (_IsFront == true)
            {
                // 経路探索
                FarRouteSearch(_Up, new Vector2Int(0, 1));
                FarRouteSearch(_Down, new Vector2Int(0, -1));
                FarRouteSearch(_Left, new Vector2Int(-1, 0));
                FarRouteSearch(_Right, new Vector2Int(1, 0));

                _WallDensity = new float[MovePanel.Count];
                // 壁測定
                for (int i = 0; i < MovePanel.Count; i++)
                {
                    _WallDensity[i] = WallCount(MovePanel[i]);
                }


                // 壁密度比較
                switch (_WallDensity.Length)
                {
                    case 1:
                        // 配列が１つの場合
                        _NextBlock = MovePanel[0].PanelObj;
                        _EnemyDirection = MovePanel[0].Direction;
                        break;
                    case 2:
                        // 配列が二つの場合
                        if (_WallDensity[0] < _WallDensity[1])
                        {
                            _NextBlock = MovePanel[1].PanelObj;
                            _EnemyDirection = MovePanel[1].Direction;
                        }
                        else if (_WallDensity[0] > _WallDensity[1])
                        {
                            _NextBlock = MovePanel[0].PanelObj;
                            _EnemyDirection = MovePanel[0].Direction;
                        }
                        else
                        {
                            // 右
                            if (MovePanel[0].Direction == new Vector2Int(1, 0))
                            {
                                _NextBlock = MovePanel[0].PanelObj;
                                _EnemyDirection = MovePanel[0].Direction;
                            }
                            else if(MovePanel[1].Direction == new Vector2Int(1, 0))
                            {
                                _NextBlock = MovePanel[1].PanelObj;
                                _EnemyDirection = MovePanel[1].Direction;
                            }

                            // 下
                            if (_NextBlock == null)
                            {
                                if (MovePanel[0].Direction == new Vector2Int(0, -1))
                                {
                                    _NextBlock = MovePanel[0].PanelObj;
                                    _EnemyDirection = MovePanel[0].Direction;
                                }
                                else if(MovePanel[1].Direction == new Vector2Int(0, -1))
                                {
                                    _NextBlock = MovePanel[1].PanelObj;
                                    _EnemyDirection = MovePanel[1].Direction;
                                }

                            }

                            // 左
                            if (_NextBlock == null)
                            {
                                if (MovePanel[0].Direction == new Vector2Int(-1, 0))
                                {
                                    _NextBlock = MovePanel[0].PanelObj;
                                    _EnemyDirection = MovePanel[0].Direction;
                                }
                                else if(MovePanel[1].Direction == new Vector2Int(-1, 0))
                                {
                                    _NextBlock = MovePanel[1].PanelObj;
                                    _EnemyDirection = MovePanel[1].Direction;
                                }

                            }

                            // 上
                            if (_NextBlock == null)
                            {
                                if (MovePanel[0].Direction == new Vector2Int(0, -1))
                                {
                                    _NextBlock = MovePanel[0].PanelObj;
                                    _EnemyDirection = MovePanel[0].Direction;
                                }
                                else if(MovePanel[1].Direction == new Vector2Int(0, -1))
                                {
                                    _NextBlock = MovePanel[1].PanelObj;
                                    _EnemyDirection = MovePanel[1].Direction;
                                }
                            }
                        }
                        break;
                    case 3:
                        // 配列が３つの場合
                        if (_WallDensity[0] < _WallDensity[1])
                        {
                            if (_WallDensity[1] < _WallDensity[2])
                            {
                                _NextBlock = MovePanel[2].PanelObj;
                                _EnemyDirection = MovePanel[2].Direction;
                            }
                            else
                            {
                                _NextBlock = MovePanel[1].PanelObj;
                                _EnemyDirection = MovePanel[1].Direction;
                            }
                        }
                        else
                        {
                            if (_WallDensity[0] < _WallDensity[2])
                            {
                                _NextBlock = MovePanel[2].PanelObj;
                                _EnemyDirection = MovePanel[2].Direction;

                            }
                            else
                            {
                                _NextBlock = MovePanel[0].PanelObj;
                                _EnemyDirection = MovePanel[0].Direction;
                            }
                        }

                        int[] maxElement = new int[3] { 0, 0, 0 };
                        float[] max = new float[3] { 0.0f, 0.0f, 0.0f };

                        // 配列の中で一番壁密度の高いパネルを抽出
                        for (int i = 0; i < _WallDensity.Length; i++)
                        {
                            if (max[0] <= _WallDensity[i])
                            {
                                maxElement[i] = i;
                                max[i] = _WallDensity[i];
                            }
                        }

                        if (_NextBlock == null)
                        {
                            // 右を探す
                            for (int i = 0; i < maxElement.Length; i++)
                            {
                                if (max[i] != 0.0f)
                                {
                                    if (MovePanel[maxElement[i]].Direction == new Vector2Int(1, 0))
                                    {
                                        _NextBlock = MovePanel[maxElement[i]].PanelObj;
                                        _EnemyDirection = MovePanel[maxElement[i]].Direction;
                                    }
                                }
                            }
                        }

                        if (_NextBlock == null)
                        {
                            // 下を探す
                            for (int i = 0; i < maxElement.Length; i++)
                            {
                                if (max[i] != 0.0f)
                                {
                                    if (MovePanel[maxElement[i]].Direction == new Vector2Int(0, -1))
                                    {
                                        _NextBlock = MovePanel[maxElement[i]].PanelObj;
                                        _EnemyDirection = MovePanel[maxElement[i]].Direction;
                                    }
                                }
                            }
                        }

                        if (_NextBlock == null)
                        {
                            // 左を探す
                            for (int i = 0; i < maxElement.Length; i++)
                            {
                                if (max[i] != 0.0f)
                                {
                                    if (MovePanel[maxElement[i]].Direction == new Vector2Int(-1, 0))
                                    {
                                        _NextBlock = MovePanel[maxElement[i]].PanelObj;
                                        _EnemyDirection = MovePanel[maxElement[i]].Direction;
                                    }
                                }

                            }
                        }

                        if (_NextBlock == null)
                        {
                            // 上を探す
                            for (int i = 0; i < maxElement.Length; i++)
                            {
                                if (max[i] != 0.0f)
                                {
                                    if (MovePanel[maxElement[i]].Direction == new Vector2Int(0, 1))
                                    {
                                        _NextBlock = MovePanel[maxElement[i]].PanelObj;
                                        _EnemyDirection = MovePanel[maxElement[i]].Direction;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
            else
            {
                NearRouteSearch(_Up, new Vector2Int(0, 1));
                NearRouteSearch(_Down, new Vector2Int(0, -1));
                NearRouteSearch(_Left, new Vector2Int(-1, 0));
                NearRouteSearch(_Right, new Vector2Int(1, 0));

                if (_NextBlock == null)
                {
                    // 右を探す
                    for (int i = 0; i < MovePanel.Count; i++)
                    {
                        if (MovePanel[i].Direction == new Vector2Int(1, 0))
                        {
                            _NextBlock = MovePanel[i].PanelObj;
                            _EnemyDirection = MovePanel[i].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // 下を探す
                    for (int i = 0; i < MovePanel.Count; i++)
                    {
                        if (MovePanel[i].Direction == new Vector2Int(0, -1))
                        {
                            _NextBlock = MovePanel[i].PanelObj;
                            _EnemyDirection = MovePanel[i].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // 左を探す
                    for (int i = 0; i < MovePanel.Count; i++)
                    {
                        if (MovePanel[i].Direction == new Vector2Int(-1, 0))
                        {
                            _NextBlock = MovePanel[i].PanelObj;
                            _EnemyDirection = MovePanel[i].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // 上を探す
                    for (int i = 0; i < MovePanel.Count; i++)
                    {
                        if (MovePanel[i].Direction == new Vector2Int(0, 1))
                        {
                            _NextBlock = MovePanel[i].PanelObj;
                            _EnemyDirection = MovePanel[i].Direction;
                        }
                    }
                }
            }
            _EnemyState = EnemyState.MOVE;
        }
    }

    

    // 順路型右用関数
    //private void RouteTypeRight(Vector2Int Dir)
    //{
    //    if (_NextBlock == null)
    //    {
    //        // 上を探す
    //        for (int i = 0; i < maxElement.Length; i++)
    //        {
    //            if (max[i] != 0.0f)
    //            {
    //                if (MovePanel[maxElement[i]].Direction == new Vector2Int(0, 1))
    //                {
    //                    _NextBlock = MovePanel[maxElement[i]].PanelObj;
    //                    _EnemyDirection = MovePanel[maxElement[i]].Direction;
    //                }
    //            }
    //        }
    //    }

    //}

    // パネルに壁があるかを調べる
    public void CountWall(GameObject block)
    {
        GameManagerScript gmScript = _GameManager.GetComponent<GameManagerScript>();

        for (int i = 0; i < block.transform.GetChild(0).transform.childCount; ++i)
        {
            // エネミーと一致したら次の子オブジェクトに移る
            List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
            bool isThrow = false;
            foreach (GameObject enemy in enemys)
                if (block.transform.GetChild(0).transform.GetChild(i).gameObject == enemy)
                {
                    isThrow = true;
                    break;
                }
            if (isThrow)
                continue;

            // プレイヤーでもなくギミックのチェックエンターも通ったら
            if (gmScript.GetPlayer() != transform.GetChild(i).gameObject &&
                block.transform.GetChild(0).transform.GetChild(i).GetComponent<GimmicControl>().IsWall())
                _WallCount++;
        }

    }

    // チーズをみつけていたらチーズの方へ向かう
    private void CheeseMove()
    {


        GameObject moveobj = null;
        Vector2Int movedirection = new Vector2Int();

        float distance = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;

        if (_Up != null)
        {
            _EnemyDirection = new Vector2Int(0, 1);
            tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
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
            else if (tmp < distance)
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
            tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
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
            else if (tmp < distance)
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
            tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
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
            else if (tmp < distance)
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
            tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
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
            else if (tmp < distance)
            {
                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                {
                    movedirection = _EnemyDirection;
                    moveobj = _Right;
                    distance = tmp;
                }
            }
        }

        // ここの処理間違えて消してしまったからあとで確認が必要
        // チーズとネズミの距離を取得
        tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
        if (moveobj != null)
        {
            // チーズとムーブ先のパネルの距離を取得
            tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);

            if (tmp2 < tmp)
            {
                _EnemyDirection = movedirection;
                _NextBlock = moveobj;
                _EnemyState = EnemyState.MOVE;
            }

        }
        else
            _EnemyState = EnemyState.STAY;


    }

    public void PlayerKill()
    {
        _EnemyAnimation.SetTrigger("Attack");
        _Player.gameObject.GetComponent<PlayerControl>().SetIsExist(false);

    }

    // エネミーを行く方向・かじる方向へ回転させる
    public void Rotate()
    {

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

    private GameObject SelectMovePanel(GameObject panelobj, GameObject movepanel, Vector2Int direction, float distance)
    {
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        float tmp = 0.0f;
        float random;

        if (panelobj != null)
        {
            //プレイヤーと上パネルの距離を取得
            tmp = Vector3.Distance(playerpos, panelobj.transform.position);

            random = Random.value;

            if (_IsFront)
            {
                // 壁がない・離れる or　同等の距離の時ランダムで決める
                if (panelobj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, direction) &&
                   ((tmp == distance && random < 0.5f) || tmp > distance))
                {
                    _EnemyDirection = direction;
                    movepanel = panelobj;
                    distance = tmp;
                }
            }
            else
            {
                if (panelobj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, direction) &&
                    ((tmp == distance && random < 0.5f) || tmp < distance))
                {
                    _EnemyDirection = direction;
                    movepanel = panelobj;
                    distance = tmp;
                }
            }
        }
        return movepanel;
    }


}
