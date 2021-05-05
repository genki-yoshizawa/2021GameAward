using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{

    private enum EnemyState
    {
        IDLE,
        WAIT,
        MOVE,
        GNAWING
    }
    private enum GnawingTurn
    {
        RANDOM,
        EVERYTURN
    }


    // 必要そうな変数をとりあえず用意
    [SerializeField] EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] EnemyState _NextState = EnemyState.WAIT;


    [Header("１ターンの行動回数")]
    [SerializeField]
    private int _ActCount;           // 敵の行動回数    （０～２？）

    [Header("壁をかじるレベル")]
    [SerializeField]
    private int _GnawingLevel;       // かじるレベル    （０～３？）

    [Header("ランダムorターン毎")]
    [SerializeField] GnawingTurn _GnawingTurn = GnawingTurn.RANDOM;


    // それぞれにGet・Setを作成？
    private Vector2Int _BlockPosition;     // ブロックの座標
    private Vector3 _EnemyDirection;    // ネズミの向いてる方向
    private bool _isFront;              // 表か裏か

    int _Count;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        ChangeState();

    }

    void ChangeState()
    {

        switch (_EnemyState)
        {
            case EnemyState.IDLE:
                Idle();
                break;
            case EnemyState.WAIT:
                Wait();
                break;
            case EnemyState.MOVE:
                Move();
                break;
            case EnemyState.GNAWING:
                Gnawing();
                break;

        }
    }

    // 待機関数
    void Idle()
    {
        // 待機モーションをさせる
        // プレイヤーのターンが終わったら次のステートに移行

        // 現状はターン制度がないためエンターキーでステートを移行させている
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
            _NextState = EnemyState.MOVE;
        }
    }

    // 思考関数？（必要かわからない）
    void Wait()
    {
        // 移動する方向を少し考えてる時間を作る？
        // ターンが変わって即移動でもよいのか？

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
            _NextState = EnemyState.GNAWING;
        }
    }

    // 移動関数
    void Move()
    {
        // まず、表世界にいるのか裏世界にいるのかをみて、表なら逃げる　裏なら追うように作る
        // 移動できるパネルを参照して、進める方向が多いパネルかつ
        // プレイヤーのいるパネルの位置を見て、プレイヤーから離れられる場所に移動する
        // 上記の条件が複数ある場合はランダム？

        // 表世界
        if (_isFront == true)
        {

        }
        // 裏世界
        else
        {

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            this.transform.position += new Vector3(0.0f,0.0f,1.0f);
            _EnemyState = _NextState;
            _NextState = EnemyState.IDLE;
            _Count++;
            Debug.Log(_Count);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.transform.position += new Vector3(0.0f,0.0f,-1.0f);
            _EnemyState = _NextState;
            _NextState = EnemyState.IDLE;
            _Count++;
            Debug.Log(_Count);

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.transform.position += new Vector3(-1.0f,0.0f,0.0f);
            _EnemyState = _NextState;
            _NextState = EnemyState.IDLE;
            _Count++;
            Debug.Log(_Count);

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            this.transform.position += new Vector3(1.0f,0.0f,0.0f);
            _EnemyState = _NextState;
            _NextState = EnemyState.IDLE;
            _Count++;
            Debug.Log(_Count);

        }


    }

    // 壁をかじる関数
    void Gnawing()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
            _NextState = EnemyState.WAIT;
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
}
