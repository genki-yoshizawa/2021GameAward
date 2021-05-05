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


    // �K�v�����ȕϐ����Ƃ肠�����p��
    [SerializeField] EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] EnemyState _NextState = EnemyState.WAIT;


    [Header("�P�^�[���̍s����")]
    [SerializeField]
    private int _ActCount;           // �G�̍s����    �i�O�`�Q�H�j

    [Header("�ǂ������郌�x��")]
    [SerializeField]
    private int _GnawingLevel;       // �����郌�x��    �i�O�`�R�H�j

    [Header("�����_��or�^�[����")]
    [SerializeField] GnawingTurn _GnawingTurn = GnawingTurn.RANDOM;


    // ���ꂼ���Get�ESet���쐬�H
    private Vector2Int _BlockPosition;     // �u���b�N�̍��W
    private Vector3 _EnemyDirection;    // �l�Y�~�̌����Ă����
    private bool _isFront;              // �\������

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

    // �ҋ@�֐�
    void Idle()
    {
        // �ҋ@���[�V������������
        // �v���C���[�̃^�[�����I������玟�̃X�e�[�g�Ɉڍs

        // ����̓^�[�����x���Ȃ����߃G���^�[�L�[�ŃX�e�[�g���ڍs�����Ă���
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
            _NextState = EnemyState.MOVE;
        }
    }

    // �v�l�֐��H�i�K�v���킩��Ȃ��j
    void Wait()
    {
        // �ړ���������������l���Ă鎞�Ԃ����H
        // �^�[�����ς���đ��ړ��ł��悢�̂��H

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
            _NextState = EnemyState.GNAWING;
        }
    }

    // �ړ��֐�
    void Move()
    {
        // �܂��A�\���E�ɂ���̂������E�ɂ���̂����݂āA�\�Ȃ瓦����@���Ȃ�ǂ��悤�ɍ��
        // �ړ��ł���p�l�����Q�Ƃ��āA�i�߂�����������p�l������
        // �v���C���[�̂���p�l���̈ʒu�����āA�v���C���[���痣�����ꏊ�Ɉړ�����
        // ��L�̏�������������ꍇ�̓����_���H

        // �\���E
        if (_isFront == true)
        {

        }
        // �����E
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

    // �ǂ�������֐�
    void Gnawing()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
            _NextState = EnemyState.WAIT;
        }
    }


    // �u���b�N�ŌĂяo���@��������]������֐�
    public void RotateMySelf()
    {

    }

    // �u���b�N�ŌĂяo���@�������Ђ�����Ԃ��֐��i�\������ւ��j
    public void TurnOverMySelf()
    {

    }

    // �u���b�N�ŌĂяo���@�����̈ʒu�����ւ���֐�
    public void SwapMySelf()
    {

    }
}
