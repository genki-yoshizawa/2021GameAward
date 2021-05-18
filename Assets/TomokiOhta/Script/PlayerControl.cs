using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //�v���C���[�̃A�j���[�V����
    private FukidasiAnimationUI _FukidasiAnimationUI;

    //�\��
    private bool _IsFront;

    //��������
    private bool _IsExist;

    //�O�̃u���b�N���
    private GameObject _FrontBlock;

    //�ǂ̍s�����\�ł�7�̕������i�[���邩���Ǘ�����
    private List<int> _CanActionList = new List<int>();

    void Start()
    {
        //����FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _FukidasiAnimationUI = GameObject.Find("fukidasi_0").GetComponent<FukidasiAnimationUI>();
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

    public void TurnOverMySelf(Vector2Int position/*, Vector3 axis*/)
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
        RotateMySelf(_LocalPosition, 180.0f);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        SetFromtBlock();
    }

    private void PlayerRotate(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Rotate(_IsFront, 90);
        SetFromtBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.Swap(_IsFront);
        SetFromtBlock();
    }

    private void PlayerTurnOver(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        blockScript.TurnOver(_IsFront);
        SetFromtBlock();
    }

    public bool PlayerTurn()
    {
        if (_FrontBlock == null)
            SetFromtBlock();

        bool turnEnd = false;

        //�v���C���[���E��]
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateMySelf(_LocalPosition, 90.0f);
            transform.Rotate(0.0f, 90.0f, 0.0f);
            SetFromtBlock();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateMySelf(_LocalPosition, -90.0f);
            transform.Rotate(0.0f, -90.0f, 0.0f);
            SetFromtBlock();
        }

        //�㉺���ŃR�}���h�I��
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _FukidasiAnimationUI.SetCount(_CanActionList.Count);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _FukidasiAnimationUI.SetCount(0);
        }

        //Enter�L�[�ōs�� �^�[����i�߂�
        if (Input.GetKeyDown(KeyCode.Return))
        {

            turnEnd = true;
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }

    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }

    //�O�̃u���b�N�̏��擾
    private void SetFromtBlock()
    {
        _FrontBlock = _GameManagerScript.GetBlock(_LocalPosition + _Direction);
        if (_FrontBlock == null)
            return;

        var blockScript = _FrontBlock.GetComponent<BlockConfig>();

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

}

