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
    protected enum BreakTurn
    {
        RANDOM,
        EVERYTURN
    }


    // 必要そうな変数をとりあえず用意
    [SerializeField] protected EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] protected EnemyState _NextState = EnemyState.IDLE;


    [Header("１ターンの行動回数")]
    [SerializeField]
    protected int _ActCount;           // 敵の行動回数    （０～２？）

    [Header("壁をかじるレベル")]
    [SerializeField]
    protected int _BreakLevel;       // かじるレベル    （０～３？）

    [Header("ランダムorターン毎")]
    [SerializeField] BreakTurn _BreakTurn = BreakTurn.RANDOM;


    // それぞれにGet・Setを作成？
    [SerializeField] private Vector2Int _EnemyBlockPosition;      // ネズミのいるブロックの座標
    private Vector3 _EnemyDirection;        // ネズミの向いてる方向
    private bool _isFront;                  // 表か裏か
    private GameObject[][] _Blocks;         // ブロックを保持する

    private GameObject _GameManager;        // ゲームマネージャーを保持
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // 移動可能ブロックの保持、進む先のブロックを保持
    protected GameObject _Player;
    private int _Count;         // ステート移行用にフレームカウント

    float _PosY = 0.3f;    // Y座標固定用

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        //Player = gameObject.GetComponent<GameManagerScript>().GetPlayer();
        _Player = GameObject.FindGameObjectWithTag("Player");
        GameObject parent = transform.root.gameObject;
        _EnemyBlockPosition = parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
        _Count = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        Wait();
        // エネミーステートを変更する関数を呼ぶ


        // 現状はターン制度がないためエンターキーでステートを移行させている
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeState();
            

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

    //public virtual void ChangeState()
    //{
        

    //}

    void Wait()
    {

        // 前後左右にブロックがあるか
        Vector2Int pos = _EnemyBlockPosition;

            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y + 1));


            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y - 1));



            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x - 1, pos.y));

            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x + 1, pos.y));



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
        // 待機モーションをさせる
        // プレイヤーのターンが終わったら次のステートに移行

        // プレイヤーのターンが終わって、敵のターンになったらステートを移行
        // 現状はターン制度がないためフレームで管理



        // 現状はターン制度がないためエンターキーでステートを移行させている
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    _EnemyState = _NextState;
        //    _NextState = EnemyState.MOVE;

        //}
    }

    void Stay()
    {
        // なにもしない処理？
        // 待機モーションを実行？
    }


    // 移動関数
    void Move()
    {

        GameObject o;
        o = _NextBlock.transform.GetChild(0).gameObject;
        transform.parent = o.transform;

        this.transform.position = o.transform.position;

        _EnemyBlockPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();

  
        Vector3 Pos = this.transform.position;
        ;
        Pos.y = _PosY;
        this.transform.position = Pos;

        _EnemyState = EnemyState.IDLE;

        // まず、表世界にいるのか裏世界にいるのかをみて、表なら逃げる　裏なら追うように作る
        // 移動できるパネルを参照して、進める方向が多いパネルかつ
        // プレイヤーのいるパネルの位置を見て、プレイヤーから離れられる場所に移動する
        // 上記の条件が複数ある場合はランダム？
        //  _Block = _GameManager.GetComponent<GameManagerScript>().GetBlocks();

        // ネズミのいる位置のブロックを取得　gameeobject nowblock
        // 取得したブロックの中にBlockConfig.GetBlockLocalPosition
        // ４つのGameObjectを一時的に変数を用意（前後左右

        // 壁があったら～等の条件が増えていく

        // 移動先のブロックの中のパネルの子オブジェクトに

        // 移動先の決定　
        // 移動先のブロックの座標を取得して　中心座標へ移動
        // Ｙ座標を後から指定する必要がある
        // ブロックの子どもに



    }

    // 壁をかじる関数
    void Break()
    {


        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
        }
    }


    // ブロックで呼び出す　自分を回転させる関数
    public void RotateMySelf()
    {

    }

    // ブロックで呼び出す　自分をひっくり返す関数（表裏入れ替え）
    public void TurnOverMySelf()
    {

    }

    // ブロックで呼び出す　自分の位置を入れ替える関数
    public void SwapMySelf()
    {

    }

    // 自分のいるブロックの座標を更新する
    public void SetLocalPosition(Vector2Int position)
    {
        _EnemyBlockPosition = position;
    }

   // public void GetLocalPosition() { return _EnemyBlockPosition; }
    // 自分が表か裏どっちにいるか
    public void SetIsFront(bool isfront)
    {
        _isFront = isfront;
    }

    public void ChangeState()
    {
        // プレイヤーのいるブロックを朱徳して
        // プレイヤーから一番遠いブロックへ逃げる
        Vector3 playerpos = _Player.transform.position;

        GameObject obj = new GameObject();
        float distance = 0.0f;
        float tmp = 0.0f;

        if (_Up != null)
        {
            tmp = Vector3.Distance(playerpos, _Up.transform.position);
            if (tmp > distance)
            {
                obj = _Up;
                distance = tmp;
            }
        }
        if (_Down != null)
        {
            tmp = Vector3.Distance(playerpos, _Down.transform.position);
            if (tmp > distance)
            {
                obj = _Down;
                distance = tmp;
            }
        }

        if (_Left != null)
        {
            tmp = Vector3.Distance(playerpos, _Left.transform.position);
            if (tmp > distance)
            {
                obj = _Left;
                distance = tmp;
            }
        }
        if (_Right != null)
        {
            tmp = Vector3.Distance(playerpos, _Right.transform.position);
            if (tmp > distance)
            {
                obj = _Right;
                distance = tmp;
            }
        }

        _NextBlock = obj;
        _EnemyState = EnemyState.MOVE;
    }

    public void EnemyTurn( )
    {

        ChangeState();
    }

}

//class Level1 :  EnemyControl
//{
//    public override void ChangeState()
//    {
//        _EnemyState = EnemyState.STAY;
//    }
//}

