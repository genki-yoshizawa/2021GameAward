using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("使いたい音声ファイルを入れてください。")]
    [SerializeField] private AudioClip audioClip;

    [Header("プレイヤーの向きを入れてください。")]
    [SerializeField]private Vector2Int _Direction;

    private Vector2Int _LocalPosition;
    private GameObject _FrontBlock;
    private GameObject _BackBlock;

    private GameManagerScript _GameManagerScript;

    private bool _IsFront;

    void Start()
    {
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("右が押されたよ");
            RotateMySelf(_LocalPosition, 90.0f);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("左が押されたよ");
            RotateMySelf(_LocalPosition, -90.0f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("上が押されたよ");
            Move(_Direction);
        }

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

        //現在位置の取得
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //親を切り替える
        transform.parent = block.transform.GetChild(0).transform;

        //移動
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _LocalPosition += _Direction;
    }

    public void RotateMySelf(Vector2Int position, float angle)
    {
        if (position != _LocalPosition)
            return;

        Vector3 direction = new Vector3(_Direction.x, 0f, _Direction.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        Vector2 tmp = new Vector2(direction.x, direction.z);

        _Direction = new Vector2Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y));
    }


    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }

    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
}
