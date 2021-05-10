using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("使いたい音声ファイルを入れてください。")]
    [SerializeField] private AudioClip audioClip;

    [Header("プレイヤーの向きを入れてください。")]
    [SerializeField]private Vector2Int _Direction;

    //プレイヤーの配列座標
    private Vector2Int _LocalPosition;

    //他のオブジェクトに触れるときの仲介役
    private GameManagerScript _GameManagerScript;

    //表裏
    private bool _IsFront;

    private bool _IsExist;

    void Start()
    {
        //初手FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _IsExist = true;
    }

    void Update()
    {
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

    private void Move(Vector2Int direction)
    {
        //Vector3 nowpos = transform.localPosition;
        //transform.localPosition = new Vector3(nowpos.x - _MoveDirection.x, nowpos.y - _MoveDirection.y, nowpos.z - _MoveDirection.z);
        //transform.localPosition = nowpos - _MoveDirection;



        //前のブロック取得
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //親を切り替える
        transform.parent = block.transform.GetChild(0).transform;

        //移動
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _LocalPosition += _Direction;
    }

    public void RotateMySelf(Vector2Int position, float angle)
    {
        //Rotate時に呼び出される関数、自分の方向を変えるときにも自分で呼ぶ
        if (position != _LocalPosition)
            return;

        Vector3 direction = new Vector3(_Direction.x, 0f, _Direction.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

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

    public void TurnOverMySelf(Vector2Int position)
    {
        //TurnOver時に呼び出される関数、ひっくり返すのはSetIsFrontで良いのでは？
        if (position != _LocalPosition)
            return;

        //ひっくり返す
        if (_IsFront)
            _IsFront = false;
        else
            _IsFront = true;
    }

    public bool PlayerTurn()
    {
        bool turnEnd = false;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("右が押されたよ");
            RotateMySelf(_LocalPosition, 90.0f);
            transform.Rotate(0.0f, 90.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("左が押されたよ");
            RotateMySelf(_LocalPosition, -90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("上が押されたよ");

            //前のブロック取得
            var blockScript = _GameManagerScript.GetBlock(_LocalPosition + _Direction).GetComponent<BlockConfig>();

            if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            {
                Move(_Direction);
                turnEnd = true;
            }
            else
            {
                Debug.Log("Move失敗");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            //Rotateを行うデバッグ用

            //前のブロック取得
            var block = _GameManagerScript.GetBlock(_LocalPosition + _Direction);

            //前のブロックがRotate可能かどうかを調べる
            var panelScript = block.transform.GetChild(_IsFront ? 0 : 1).GetComponent<PanelConfig>();

            if (panelScript.GetCanRotate())
            {
                var blockControlScript = block.GetComponent<BlockControl>();
                blockControlScript.Rotate(_IsFront, 90);
                Debug.Log("Rotateできたよ");
                turnEnd = true;
            }
            else
            {
                Debug.Log("Rotate失敗");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //Swapを行うデバッグ用

            //前のブロック取得
            var block = _GameManagerScript.GetBlock(_LocalPosition + _Direction);

            //前のブロックがSwap可能かどうかを調べる
            var panelScript = block.transform.GetChild(_IsFront ? 0 : 1).GetComponent<PanelConfig>();
            if (panelScript.GetCanSwap())
            {
                var blockControlScript = block.GetComponent<BlockControl>();
                blockControlScript.Swap(_IsFront);
                Debug.Log("Swapしたよ");
                turnEnd = true;
            }
            else
            {
                Debug.Log("Swap失敗");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            //TurnOverを行うデバッグ用

            //前のブロック取得
            var block = _GameManagerScript.GetBlock(_LocalPosition + _Direction);

            //前のブロックがTurnOver可能かどうかを調べる
            var panelScript = block.transform.GetChild(_IsFront ? 0 : 1).GetComponent<PanelConfig>();
            if (panelScript.GetCanTurnOver())
            {
                var blockControlScript = block.GetComponent<BlockControl>();
                blockControlScript.TurnOver(_IsFront);
                Debug.Log("TurnOverできたよ");
                turnEnd = true;
            }
            else
            {
                Debug.Log("TurnOver失敗");
            }
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }

    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
}
