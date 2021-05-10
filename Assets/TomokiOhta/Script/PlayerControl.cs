using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("�g�����������t�@�C�������Ă��������B")]
    [SerializeField] private AudioClip audioClip;

    [Header("�v���C���[�̌��������Ă��������B")]
    [SerializeField]private Vector2Int _Direction;

    //�v���C���[�̔z����W
    private Vector2Int _LocalPosition;

    //���̃I�u�W�F�N�g�ɐG���Ƃ��̒����
    private GameManagerScript _GameManagerScript;

    //�\��
    private bool _IsFront;

    private bool _IsExist;

    void Start()
    {
        //����FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _IsExist = true;
    }

    void Update()
    {
        //���R������œ����Ǝv��
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



        //�O�̃u���b�N�擾
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //�e��؂�ւ���
        transform.parent = block.transform.GetChild(0).transform;

        //�ړ�
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _LocalPosition += _Direction;
    }

    public void RotateMySelf(Vector2Int position, float angle)
    {
        //Rotate���ɌĂяo�����֐��A�����̕�����ς���Ƃ��ɂ������ŌĂ�
        if (position != _LocalPosition)
            return;

        Vector3 direction = new Vector3(_Direction.x, 0f, _Direction.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        Vector2 tmp = new Vector2(direction.x, direction.z);
        //�l�̌ܓ����đ�����邱�Ƃ�Vector2Int�ɂ����������������
        _Direction = new Vector2Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y));
    }

    public void SwapMySelf(List<Vector2Int> position)
    {
        //Swap���ɌĂяo�����֐��A�e�I�u�W�F�N�g�ł���u���b�N�̈ړ��ɂ��Ă�������
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
        //TurnOver���ɌĂяo�����֐��A�Ђ�����Ԃ��̂�SetIsFront�ŗǂ��̂ł́H
        if (position != _LocalPosition)
            return;

        //�Ђ�����Ԃ�
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
            Debug.Log("�E�������ꂽ��");
            RotateMySelf(_LocalPosition, 90.0f);
            transform.Rotate(0.0f, 90.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("���������ꂽ��");
            RotateMySelf(_LocalPosition, -90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("�オ�����ꂽ��");

            //�O�̃u���b�N�擾
            var blockScript = _GameManagerScript.GetBlock(_LocalPosition + _Direction).GetComponent<BlockConfig>();

            if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            {
                Move(_Direction);
                turnEnd = true;
            }
            else
            {
                Debug.Log("Move���s");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            //Rotate���s���f�o�b�O�p

            //�O�̃u���b�N�擾
            var block = _GameManagerScript.GetBlock(_LocalPosition + _Direction);

            //�O�̃u���b�N��Rotate�\���ǂ����𒲂ׂ�
            var panelScript = block.transform.GetChild(_IsFront ? 0 : 1).GetComponent<PanelConfig>();

            if (panelScript.GetCanRotate())
            {
                var blockControlScript = block.GetComponent<BlockControl>();
                blockControlScript.Rotate(_IsFront, 90);
                Debug.Log("Rotate�ł�����");
                turnEnd = true;
            }
            else
            {
                Debug.Log("Rotate���s");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //Swap���s���f�o�b�O�p

            //�O�̃u���b�N�擾
            var block = _GameManagerScript.GetBlock(_LocalPosition + _Direction);

            //�O�̃u���b�N��Swap�\���ǂ����𒲂ׂ�
            var panelScript = block.transform.GetChild(_IsFront ? 0 : 1).GetComponent<PanelConfig>();
            if (panelScript.GetCanSwap())
            {
                var blockControlScript = block.GetComponent<BlockControl>();
                blockControlScript.Swap(_IsFront);
                Debug.Log("Swap������");
                turnEnd = true;
            }
            else
            {
                Debug.Log("Swap���s");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            //TurnOver���s���f�o�b�O�p

            //�O�̃u���b�N�擾
            var block = _GameManagerScript.GetBlock(_LocalPosition + _Direction);

            //�O�̃u���b�N��TurnOver�\���ǂ����𒲂ׂ�
            var panelScript = block.transform.GetChild(_IsFront ? 0 : 1).GetComponent<PanelConfig>();
            if (panelScript.GetCanTurnOver())
            {
                var blockControlScript = block.GetComponent<BlockControl>();
                blockControlScript.TurnOver(_IsFront);
                Debug.Log("TurnOver�ł�����");
                turnEnd = true;
            }
            else
            {
                Debug.Log("TurnOver���s");
            }
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }

    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
}
