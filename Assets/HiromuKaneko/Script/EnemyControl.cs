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
    [SerializeField] private Vector2Int _EnemyDirection;          // ネズミの向いてる方向
    [SerializeField] private bool _isFront;                       // 表か裏か

    private GameObject[][] _Blocks;                               // ブロックを保持する
    private GameObject _GameManager;                              // ゲームマネージャーを保持
    protected GameObject _Player;                                 // プレイヤーを保持

    // デバッグ用に表示させてだけなので後々SerializeFieldは消す予定
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // 移動可能ブロックの保持、進む先のブロックを保持


    float _PosY = 0.05f;    // Y座標固定用

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _Player = GameObject.FindGameObjectWithTag("Player");
        GameObject parent = transform.root.gameObject;
        _EnemyBlockPosition = parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
        _isFront = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    //public virtual void ChangeState()
    //{


    //}

    void Wait()
    {
        Vector2Int pos = _EnemyBlockPosition;

        // 前後左右にブロックがあるか
        if (_isFront)
        {

            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y + 1));


            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y - 1));



            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x - 1, pos.y));

            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x + 1, pos.y));


        }
        else
        {

            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y + 1));


            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y - 1));



            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x + 1, pos.y));

            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x - 1, pos.y));
        }


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

        //
        if (_isFront)
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(0).gameObject;
            transform.parent = o.transform;

            this.transform.position = o.transform.position;

            // ネズミのローカルポジションを
            _EnemyBlockPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();


            // ネズミの位置を調整する
            Vector3 Pos = this.transform.position;
            Pos.y = _PosY;
            this.transform.position = Pos;


        }
        else
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(1).gameObject;
            transform.parent = o.transform;

            this.transform.position = o.transform.position;

            // ネズミのローカルポジションを
            _EnemyBlockPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();


            // ネズミの位置を調整する
            Vector3 Pos = this.transform.position;
            Pos.y = -_PosY;
            this.transform.position = Pos;


        }
        // ステートをIDLEに移行する
        _EnemyState = EnemyState.IDLE;

        // ネズミのターンを終了する

    }

    // 壁をかじる関数
    void Break()
    {
        if (_BreakTurn == BreakTurn.RANDOM)
        {

        }


        // 壁をかじる処理を作る

    }


    // ともきのスクリプトを見て真似る感じで
    // ブロックで呼び出す　自分を回転させる関数
    public void RotateMySelf(Vector2Int position, float angle)
    {
        // 向きベクトルを更新させる処理
        //Rotate時に呼び出される関数、自分の方向を変えるときにも自分で呼ぶ
        if (position != _EnemyBlockPosition)
            return;

        Vector3 direction = new Vector3(_EnemyDirection.x, 0f, _EnemyDirection.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        Vector2 tmp = new Vector2(direction.x, direction.z);
        //四捨五入して代入することでVector2Intにも無理やり代入させる
        _EnemyDirection = new Vector2Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y));

    }

    // ブロックで呼び出す　自分をひっくり返す関数（表裏入れ替え）
    public void TurnOverMySelf(Vector2Int position)
    {
        // _isFrontをfalse
        // 
        //TurnOver時に呼び出される関数、ひっくり返すのはSetIsFrontで良いのでは？
        if (position != _EnemyBlockPosition)
            return;

        //ひっくり返す
        if (_isFront)
            _isFront = false;
        else
            _isFront = true;
    }

    // ブロックで呼び出す　自分の位置を入れ替える関数
    public void SwapMySelf(List<Vector2Int> position)
    {
        // ブロックのローカル
        //Swap時に呼び出される関数、親オブジェクトであるブロックの移動についていくだけ
        foreach (Vector2Int pos in position)
        {
            if (pos == _EnemyBlockPosition)
            {
                var blockConfig = transform.parent.parent.GetComponent<BlockConfig>();
                _EnemyBlockPosition = blockConfig.GetBlockLocalPosition();
                return;
            }
        }
    }





    public void ChangeState()
    {
        // プレイヤーのいるブロックを取得して
        // プレイヤーから一番遠いブロックへ逃げる
        Vector3 playerpos = _Player.transform.position;

        GameObject obj = new GameObject();
        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float y = 90;
        if (_isFront)
        {
            if (_Up != null)
            {
                tmp = Vector3.Distance(playerpos, _Up.transform.position);
                if (tmp > distance)
                {
                    obj = _Up;
                    this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    distance = tmp;
                }
            }
            if (_Down != null)
            {
                tmp = Vector3.Distance(playerpos, _Down.transform.position);
                if (tmp > distance)
                {
                    obj = _Down;
                    this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                    distance = tmp;
                }
            }

            if (_Left != null)
            {
                tmp = Vector3.Distance(playerpos, _Left.transform.position);
                if (tmp > distance)
                {
                    obj = _Left;
                    this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                    distance = tmp;
                }
            }
            if (_Right != null)
            {
                tmp = Vector3.Distance(playerpos, _Right.transform.position);
                if (tmp > distance)
                {
                    obj = _Right;
                    this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                    distance = tmp;
                }
            }

        }
        else
        {
            if (_Up != null)
            {
                tmp = Vector3.Distance(playerpos, _Up.transform.position);
                if (tmp < distance2)
                {
                    obj = _Up;
                    distance2 = tmp;
                }
            }
            if (_Down != null)
            {
                tmp = Vector3.Distance(playerpos, _Down.transform.position);
                if (tmp < distance2)
                {
                    obj = _Down;
                    distance2 = tmp;
                }
            }

            if (_Left != null)
            {
                tmp = Vector3.Distance(playerpos, _Left.transform.position);
                if (tmp < distance2)
                {
                    obj = _Left;
                    distance2 = tmp;
                }
            }
            if (_Right != null)
            {
                tmp = Vector3.Distance(playerpos, _Right.transform.position);
                if (tmp < distance2)
                {
                    obj = _Right;
                    distance2 = tmp;
                }
            }
        }

        _NextBlock = obj;
        _EnemyState = EnemyState.MOVE;
    }


    public void EnemyTurn()
    {

        Wait();
        // エネミーステートを変更する関数を呼ぶ


        // 右Shiftで裏に行く（戻れない）
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            _isFront = false;
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

            GameObject ob;
            GameObject parent = transform.root.gameObject;
            ob = parent.transform.GetChild(1).gameObject;
            transform.parent = ob.transform;
            
        }

        
        ChangeState();
                
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


    public void SetIsFront(bool isfront) { _isFront = isfront; }
    public void SetLocalPosition(Vector2Int position) { _EnemyBlockPosition = position; }    // 自分のいるブロックの座標を更新する
    // public void GetLocalPosition() { return _EnemyBlockPosition; }

}



//class Level1 :  EnemyControl
//{
//    public override void ChangeState()
//    {
//        _EnemyState = EnemyState.STAY;
//    }
//}

