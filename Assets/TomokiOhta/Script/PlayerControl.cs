using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerControl : MonoBehaviour
{
    [Header("�g�����������t�@�C�������Ă��������B")]
    [SerializeField] private AudioClip audioClip;

    [Header("�v���C���[�̌��������Ă��������B")]
    [SerializeField]private Vector2Int _Direction;

    [Header("�����o��")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("�R�}���h")]
    [SerializeField] private GameObject _ActObj;

    //�v���C���[�̔z����W
    private Vector2Int _LocalPosition;

    //���̃I�u�W�F�N�g�ɐG���Ƃ��̒����
    private GameManagerScript _GameManagerScript;

    //�\��
    private bool _IsFront;

    //��������
    private bool _IsExist;

    //�O�̃u���b�N���
    private GameObject _FrontBlock;

    //�ǂ̍s�����\�ł�7�̕������i�[���邩���Ǘ�����
    private List<int> _CanActionList = new List<int>();

    //
    private FukidasiAnimationUI _FukidasiScript;

    private int _CommandSelect = 0;

    void Start()
    {
        //����FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        //_FukidasiAnimationUI = GameObject.Find("fukidasi_0").GetComponent<FukidasiAnimationUI>();

        _IsExist = true;

        _FukidasiScript = _FukidasiObj.GetComponent<FukidasiAnimationUI>();
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
        //�O�̃u���b�N�擾
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //�e��؂�ւ���
        transform.parent = block.transform.GetChild(0).transform;

        //�ړ�
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _LocalPosition += _Direction;
    }

    public void RotateMySelf(Vector2Int position, float angle, float axisX = 0.0f, float axisY =1.0f, float axisZ = 0.0f)
    {
        Vector3 axis = new Vector3(axisX, axisY, axisZ);

        //Rotate���ɌĂяo�����֐��A�����̕�����ς���Ƃ��ɂ������ŌĂ�
        if (position != _LocalPosition)
            return;

        Vector3 direction = new Vector3(_Direction.x, 0f, _Direction.y);
        direction = Quaternion.Euler(axis * angle) * direction;

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

    public void TurnOverMySelf(Vector2Int position, Vector3 axis)
    {
        //TurnOver���ɌĂяo�����֐�
        if (position != _LocalPosition)
            return;

        //�Ђ�����Ԃ�
        if (_IsFront)
            _IsFront = false;
        else
            _IsFront = true;

        //�����𔽓]
        //_Direction *= -1;
        //���f���𔽓]
        RotateMySelf(_LocalPosition, 180.0f, axis.x, axis.y, axis.z);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        SetFrontBlock();
    }

    private void PlayerRotate(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Rotate(_IsFront, 90);
        SetFrontBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Swap(_IsFront);
        SetFrontBlock();
    }

    private void PlayerTurnOver(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.TurnOver(_IsFront, _Direction);
        SetFrontBlock();
    }

    public bool PlayerTurn()
    {
        bool turnEnd = false;

        //Start�Ŏ擾����̂Ń^�[���J�n���Ɏ蓮�Ŏ擾
        if (_FrontBlock == null)
            SetFrontBlock();

        //�����o���̃A�j���[�V�����I�����m�F�����琶������
        if (_FukidasiScript.GetCount() == 0)
        {
            _FukidasiScript.SetCount(_CanActionList.Count);
            _FukidasiScript.SetPanel(_CanActionList);
        }

        //�v���C���[���E��]
        //����낫��낵������ƃR�}���h���������܂܏o�Ă��Ȃ��Ȃ�̂Œ���
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateMySelf(_LocalPosition, _IsFront ? 90.0f : -90.0f);
            transform.Rotate(0.0f, 90.0f, 0.0f);
            SetFrontBlock();

            _FukidasiScript.ResetCount();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateMySelf(_LocalPosition, _IsFront ? -90.0f : 90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
            SetFrontBlock();

            _FukidasiScript.ResetCount();
        }

        //�㉺���ŃR�}���h�I��
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(_CommandSelect > 0)
            _CommandSelect--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(_CommandSelect < _CanActionList.Count)
            _CommandSelect++;
        }

        //Enter�L�[�ōs�� �^�[����i�߂�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CommandAction(_CommandSelect);
            _CommandSelect = 0;
            _FukidasiScript.ResetCount();
            turnEnd = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PlayerTurnOver(_FrontBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            PlayerRotate(_FrontBlock);
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }
    public bool GetIsExist() { return _IsExist; }


    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
    public void SetIsExist(bool isExist) { _IsExist = isExist; }


    //�O�̃u���b�N�̏��擾
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

        //���g�����Z�b�g���ĐV���ɏ���n��
        _CanActionList = new List<int>();

        //��]�\�Ȃ�0������
        if (blockScript.CheckPanelRotate(_IsFront))
            _CanActionList.Add(0);

        //�ړ��\�Ȃ�1������
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            _CanActionList.Add(1);

        //���]�\�Ȃ�2������
        if (blockScript.CheckPanelTurnOver(_IsFront))
            _CanActionList.Add(2);

        //���։\�Ȃ�3������
        if (blockScript.CheckPanelSwap(_IsFront))
            _CanActionList.Add(3);
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
                        PlayerRotate(_FrontBlock);
                        Debug.Log("Rotate");
                        break;
                    case 1:
                        PlayerMove();
                        Debug.Log("Move");
                        break;
                    case 2:
                        PlayerTurnOver(_FrontBlock);
                        Debug.Log("TurnOver");
                        break;
                    case 3:
                        PlayerSwap(_FrontBlock);
                        Debug.Log("Swap");
                        break;
                }
                break;
            }

            count++;
        }
    }

}

