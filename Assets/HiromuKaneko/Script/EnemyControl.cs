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
        public bool Exist;
    };

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

    private EnemyState _EnemyState = EnemyState.IDLE;          // エネミーステート変数
    private List<Panel> _MovePanel = new List<Panel>();        // 逃げ先候補のブロックを保持する変数
    private GameObject _Up, _Down, _Left, _Right, _NextBlock;  // 上下左右ブロックの保持、進む先のブロックを保持
    private Vector2Int _EnemyLocalPosition;      // ネズミのいるブロックの座標
    private Vector2Int _EnemyDirection;          // ネズミの向いてる方向
    private GameObject _GameManager;             // ゲームマネージャーを保持
    private GameObject _Player;                  // プレイヤーを保持
    private GameObject _Cheese;                  // チーズを保持
    private Animator _EnemyAnimation;            // ネズミのアニメーションの保持
    private Vector3 _StartPoint;                 // 移動前ポジション
    private Vector3 _TargetPoint;                // 移動先ポジション
    private float _PosY = 0.075f;                // Y座標固定用
    private float _PassedTime;                   // アニメーション用タイム変数
    private bool _CheeseBite;                    // チーズをかじるとき
    private bool _PlayerBite;                    // プレイヤーをかじるとき
    private bool _IsExist;                       // 生存確認用
    private bool _IsFront;                       // 表裏どっちにいるか
    private bool _IsMovePanel = false;           // 逃げるパネルがあるかどうかを判定
    private bool _IsTwoMax = false;

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
        _PassedTime = 0.0f;
        // マネージャーのスタートアニメーションを呼ぶ
        _GameManager.gameObject.GetComponent<GameManagerScript>().StartEnemyMovie(_IsFront);


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
                if (_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() != _IsFront)
                {
                    if (_Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition() == _EnemyLocalPosition)
                    {
                        StartCoroutine("PlayerDown");
                    }
                }
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

    // 待機関数
    void Idle()
    {
        _NextBlock = null;
        _MovePanel.Clear();
        _IsMovePanel = false;
        _IsTwoMax = false;
    }

    // 移動もかじるもしない待機モーションのみでターン終了
    void Stay()
    {
        _EnemyAnimation.SetBool("Panic", true);
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
                Wait();

                MoveTest();

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

                if (_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() != _IsFront)
                {
                    if (_Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition() == _EnemyLocalPosition)
                    {
                        StartCoroutine("PlayerDown");
                    }
                }
                if (!_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() && !_IsFront)
                {
                    PlayerKill();

                }
            }
        }
    }

    // 経路探索　近寄る
    public void FarRouteSearch(GameObject Panel, Vector2Int Direction)
    {
        Panel _Panel = new Panel() { PanelObj = null, Direction = new Vector2Int(0, 0), Exist = false };

        // 引数のゲームオブジェクトがnullの場合nullで入れる
        if (Panel == null)
        {
            _MovePanel.Add(_Panel);
            return;
        }

        // プレイヤーのいるポジションを取得
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        // プレイヤーとエネミーの距離を取得
        float player_enemy_distance = Vector3.Distance(playerpos, this.transform.position);

        // 上下左右のパネルと上記を比較して、短いのを除去する
        float tmp = Vector3.Distance(playerpos, Panel.transform.position);

        _Panel = new Panel() { PanelObj = Panel, Direction = Direction, Exist = true };

        // プレイヤーとエネミーの距離よりも長ければリストで保持する？
        if (player_enemy_distance < tmp)
        {
            _IsMovePanel = true;
            _MovePanel.Add(_Panel);

        }
        else
        {
            _Panel.Exist = false;
            _MovePanel.Add(_Panel);
        }

    }

    // 経路探索　離れる
    public void NearRouteSearch(GameObject Panel, Vector2Int Direction)
    {
        Panel _Panel = new Panel() { PanelObj = null, Direction = new Vector2Int(0, 0), Exist = false };

        // 引数のゲームオブジェクトがnullの場合nullで入れる
        if (Panel == null)
        {
            _MovePanel.Add(_Panel);
            return;
        }

        // プレイヤーのいるポジションを取得
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        // プレイヤーとエネミーの距離を取得
        float player_enemy_distance = Vector3.Distance(playerpos, this.transform.position);

        // 上下左右のパネルと上記を比較して、短いのを除去する
        float tmp = Vector3.Distance(playerpos, Panel.transform.position);

        _Panel = new Panel() { PanelObj = Panel, Direction = Direction, Exist = true };

        // プレイヤーとエネミーの距離よりも近ければリストで保持する？
        if (player_enemy_distance > tmp)
        {
            _IsMovePanel = true;
            _MovePanel.Add(_Panel);

        }
        else
        {
            _Panel.Exist = false;
            _MovePanel.Add(_Panel);
        }

    }
    // 壁の数を数えて壁密度をリターン
    public float WallCount(Panel moveobj)
    {

        // 移動先のパネルがない　移動先に壁がある場合0.0fを返す
        if (moveobj.PanelObj == null || !moveobj.PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, moveobj.Direction) || moveobj.Exist == false)
            return 0.0f;

        Vector2Int Dir = new Vector2Int();

        // 壁測定をするパネルを入れるリスト
        List<Panel> WallCountPanel = new List<Panel>();

        // 上か下なら左右のＸに１を入れる  それ以外はＹに１を入れる
        if (moveobj.Direction == new Vector2Int(0, 1) || moveobj.Direction == new Vector2Int(0, -1))
            Dir = new Vector2Int(1, 0);
        else
            Dir = new Vector2Int(0, 1);

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

        int _WallCount = 0;

        // 壁の数を数える
        foreach (Panel panel in WallCountPanel)
        {
            // そのパネルにオブジェクトの壁があるかを調べる。あればカウントを増やす
            _WallCount += CountWall(panel.PanelObj);

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
        float retDinsity = (float)_WallCount / WallCountPanel.Count;
        //Debug.Log(_WallCount);
        //Debug.Log(WallCountPanel.Count);
        Debug.Log(retDinsity);

        return retDinsity;
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
                // 経路探索　順路型右用に　右→下→左→上の順で探索
                FarRouteSearch(_Right, new Vector2Int(1, 0));
                FarRouteSearch(_Down, new Vector2Int(0, -1));
                FarRouteSearch(_Left, new Vector2Int(-1, 0));
                FarRouteSearch(_Up, new Vector2Int(0, 1));

                float[] WallDensity = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

                // 離れるパネルがない時に近づくパネルに移動する用にExistをtrueにしているだけ 
                if (_IsMovePanel == false)
                {
                    for (int i = 0; i < _MovePanel.Count; i++)
                    {
                        Panel _Panel = new Panel() { PanelObj = _MovePanel[i].PanelObj, Direction = _MovePanel[i].Direction, Exist = true };
                        _MovePanel[i] = _Panel;
                    }
                }

                // 壁測定
                for (int i = 0; i < WallDensity.Length; i++)
                {
                    WallDensity[i] = WallCount(_MovePanel[i]);
                }

                int MaxElement = 0;
                float Max = -1.0f;

                // 配列の中で一番壁密度の高いパネルを抽出
                for (int i = 0; i < WallDensity.Length; i++)
                {
                    if (Max < WallDensity[i])
                    {
                        MaxElement = i;
                        Max = WallDensity[i];

                    }
                    else if (Max == WallDensity[i])
                    {
                        _IsTwoMax = true;
                    }

                }

                // 壁密度の最大値が二つ以上あるときの処理
                if (_IsTwoMax == true)
                {
                    if (Max == WallDensity[0])
                    {
                        if (_MovePanel[0].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[0].Direction))
                        {
                            _NextBlock = _MovePanel[0].PanelObj;
                            _EnemyDirection = _MovePanel[0].Direction;
                        }
                    }

                    if (_NextBlock == null)
                    {
                        if (Max == WallDensity[1])
                        {
                            if (_MovePanel[1].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[1].Direction))
                            {
                                _NextBlock = _MovePanel[1].PanelObj;
                                _EnemyDirection = _MovePanel[1].Direction;
                            }

                        }
                    }

                    if (_NextBlock == null)
                    {
                        if (Max == WallDensity[2])
                        {
                            if (_MovePanel[2].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[2].Direction))
                            {
                                _NextBlock = _MovePanel[2].PanelObj;
                                _EnemyDirection = _MovePanel[2].Direction;
                            }

                        }
                    }

                    if (_NextBlock == null)
                    {
                        if (Max == WallDensity[3])
                        {
                            if (_MovePanel[3].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[3].Direction))
                            {

                                _NextBlock = _MovePanel[3].PanelObj;
                                _EnemyDirection = _MovePanel[3].Direction;
                            }
                        }
                    }

                }
                else
                {
                    if (_MovePanel[MaxElement].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[MaxElement].Direction))
                    {
                        _NextBlock = _MovePanel[MaxElement].PanelObj;
                        _EnemyDirection = _MovePanel[MaxElement].Direction;
                    }

                }

            }
            // 裏面
            else
            {
                // 経路探索
                NearRouteSearch(_Right, new Vector2Int(1, 0));
                NearRouteSearch(_Down, new Vector2Int(0, -1));
                NearRouteSearch(_Left, new Vector2Int(-1, 0));
                NearRouteSearch(_Up, new Vector2Int(0, 1));

                // 離れるパネルがない時に近づくパネルに移動する用にExistをtrueにしているだけ 
                if (_IsMovePanel == false)
                {
                    for (int i = 0; i < _MovePanel.Count; i++)
                    {
                        Panel _Panel = new Panel() { PanelObj = _MovePanel[i].PanelObj, Direction = _MovePanel[i].Direction, Exist = true };
                        _MovePanel[i] = _Panel;
                    }
                }

                // 順路型右でプレイヤーに近づく
                if (_NextBlock == null)
                {
                    // 右を探す
                    if (_MovePanel[0].Exist == true)
                    {
                        if (_MovePanel[0].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[0].Direction))
                        {
                            _NextBlock = _MovePanel[0].PanelObj;
                            _EnemyDirection = _MovePanel[0].Direction;
                        }
                    }
                }

                if (_NextBlock == null)
                {
                    // 下を探す
                    if (_MovePanel[1].Exist == true)
                    {
                        if (_MovePanel[1].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[1].Direction))
                        {
                            _NextBlock = _MovePanel[1].PanelObj;
                            _EnemyDirection = _MovePanel[1].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // 左を探す
                    if (_MovePanel[2].Exist == true)
                    {
                        if (_MovePanel[2].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[2].Direction))
                        {
                            _NextBlock = _MovePanel[2].PanelObj;
                            _EnemyDirection = _MovePanel[2].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // 上を探す
                    if (_MovePanel[3].Exist == true)
                    {
                        if (_MovePanel[3].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[3].Direction))
                        {
                            _NextBlock = _MovePanel[3].PanelObj;
                            _EnemyDirection = _MovePanel[3].Direction;
                        }
                    }
                }

            }

            _EnemyState = EnemyState.MOVE;

        }
    }

    // パネルに壁があるかを調べる
    public int CountWall(GameObject block)
    {
        GameManagerScript gmScript = _GameManager.GetComponent<GameManagerScript>();
        int count = 0;
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
            if (gmScript.GetPlayer() != block.transform.GetChild(0).transform.GetChild(i).gameObject &&
                block.transform.GetChild(0).transform.GetChild(i).GetComponent<GimmicControl>().IsWall())
                count++;
        }
        return count;
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

        _GameManager.gameObject.GetComponent<GameManagerScript>().StartEnemyMovie(_IsFront);
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

    // プレイヤーを裏から攻撃する？
    private IEnumerator PlayerDown()
    {
        this.GetComponent<GameOverEnemy>().StartGameOverEnemyAnimation();

        yield return new WaitForSeconds(2.0f);

        _Player.gameObject.GetComponent<PlayerControl>().SetIsExist(false);

        yield return null;
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
    public void SetLocalPosition(Vector2Int position) { _EnemyLocalPosition = position; }    // 自分のいるブロックの座標を更新する
    public Vector2Int GetLocalPosition() { return _EnemyLocalPosition; }
    public void SetCheese(GameObject cheese) { _Cheese = cheese; }
    public int GetCheeseSearchRange() { return _CheeseSearchRange; }
}
