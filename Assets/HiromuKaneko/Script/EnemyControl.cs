using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{

    protected enum EnemyState
    {
        IDLE,   // ����̃^�[����
        STAY,   // �����̃^�[���ɉ������Ȃ�
        MOVE,   // �ړ�
        BREAK   // �ǂ���
    }
    protected enum BreakTurn
    {
        RANDOM,
        EVERYTURN
    }


    // �K�v�����ȕϐ����Ƃ肠�����p��
    [SerializeField] protected EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] protected EnemyState _NextState = EnemyState.IDLE;


    [Header("�P�^�[���̍s����")]
    [SerializeField]
    protected int _ActCount;           // �G�̍s����    �i�O�`�Q�H�j

    [Header("�ǂ������郌�x��")]
    [SerializeField]
    protected int _BreakLevel;       // �����郌�x��    �i�O�`�R�H�j

    [Header("�����_��or�^�[����")]
    [SerializeField] BreakTurn _BreakTurn = BreakTurn.RANDOM;


    // ���ꂼ���Get�ESet���쐬�H
    [SerializeField] private Vector2Int _EnemyBlockPosition;      // �l�Y�~�̂���u���b�N�̍��W
    private Vector3 _EnemyDirection;        // �l�Y�~�̌����Ă����
    private bool _isFront;                  // �\������
    private GameObject[][] _Blocks;         // �u���b�N��ێ�����

    private GameObject _GameManager;        // �Q�[���}�l�[�W���[��ێ�
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // �ړ��\�u���b�N�̕ێ��A�i�ސ�̃u���b�N��ێ�
    protected GameObject _Player;
    private int _Count;         // �X�e�[�g�ڍs�p�Ƀt���[���J�E���g

    float _PosY = 0.3f;    // Y���W�Œ�p

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
        // �G�l�~�[�X�e�[�g��ύX����֐����Ă�


        // ����̓^�[�����x���Ȃ����߃G���^�[�L�[�ŃX�e�[�g���ڍs�����Ă���
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

        // �O�㍶�E�Ƀu���b�N�����邩
        Vector2Int pos = _EnemyBlockPosition;

            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y + 1));


            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y - 1));



            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x - 1, pos.y));

            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x + 1, pos.y));



        //List<GameObject> moveblocks = new List<GameObject>();

        //// �ړ��ł���u���b�N�������
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

    // �ҋ@�֐�
    void Idle()
    {
        // �ҋ@���[�V������������
        // �v���C���[�̃^�[�����I������玟�̃X�e�[�g�Ɉڍs

        // �v���C���[�̃^�[�����I����āA�G�̃^�[���ɂȂ�����X�e�[�g���ڍs
        // ����̓^�[�����x���Ȃ����߃t���[���ŊǗ�



        // ����̓^�[�����x���Ȃ����߃G���^�[�L�[�ŃX�e�[�g���ڍs�����Ă���
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    _EnemyState = _NextState;
        //    _NextState = EnemyState.MOVE;

        //}
    }

    void Stay()
    {
        // �Ȃɂ����Ȃ������H
        // �ҋ@���[�V���������s�H
    }


    // �ړ��֐�
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

        // �܂��A�\���E�ɂ���̂������E�ɂ���̂����݂āA�\�Ȃ瓦����@���Ȃ�ǂ��悤�ɍ��
        // �ړ��ł���p�l�����Q�Ƃ��āA�i�߂�����������p�l������
        // �v���C���[�̂���p�l���̈ʒu�����āA�v���C���[���痣�����ꏊ�Ɉړ�����
        // ��L�̏�������������ꍇ�̓����_���H
        //  _Block = _GameManager.GetComponent<GameManagerScript>().GetBlocks();

        // �l�Y�~�̂���ʒu�̃u���b�N���擾�@gameeobject nowblock
        // �擾�����u���b�N�̒���BlockConfig.GetBlockLocalPosition
        // �S��GameObject���ꎞ�I�ɕϐ���p�Ӂi�O�㍶�E

        // �ǂ���������`���̏����������Ă���

        // �ړ���̃u���b�N�̒��̃p�l���̎q�I�u�W�F�N�g��

        // �ړ���̌���@
        // �ړ���̃u���b�N�̍��W���擾���ā@���S���W�ֈړ�
        // �x���W���ォ��w�肷��K�v������
        // �u���b�N�̎q�ǂ���



    }

    // �ǂ�������֐�
    void Break()
    {


        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyState = _NextState;
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

    // �����̂���u���b�N�̍��W���X�V����
    public void SetLocalPosition(Vector2Int position)
    {
        _EnemyBlockPosition = position;
    }

   // public void GetLocalPosition() { return _EnemyBlockPosition; }
    // �������\�����ǂ����ɂ��邩
    public void SetIsFront(bool isfront)
    {
        _isFront = isfront;
    }

    public void ChangeState()
    {
        // �v���C���[�̂���u���b�N���铿����
        // �v���C���[�����ԉ����u���b�N�֓�����
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

