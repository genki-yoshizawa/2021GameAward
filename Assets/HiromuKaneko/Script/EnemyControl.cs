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

    [SerializeField] protected bool _EnemyTurn;
    // ���ꂼ���Get�ESet���쐬�H
    [SerializeField] private Vector2Int _EnemyBlockPosition;      // �l�Y�~�̂���u���b�N�̍��W
    [SerializeField] private Vector2Int _EnemyDirection;          // �l�Y�~�̌����Ă����
    [SerializeField] private bool _isFront;                       // �\������

    private GameObject[][] _Blocks;                               // �u���b�N��ێ�����
    private GameObject _GameManager;                              // �Q�[���}�l�[�W���[��ێ�
    protected GameObject _Player;                                 // �v���C���[��ێ�

    // �f�o�b�O�p�ɕ\�������Ă����Ȃ̂Ō�XSerializeField�͏����\��
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // �ړ��\�u���b�N�̕ێ��A�i�ސ�̃u���b�N��ێ�


    float _PosY = 0.05f;    // Y���W�Œ�p

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _Player = GameObject.FindGameObjectWithTag("Player");
        GameObject parent = transform.root.gameObject;
        _EnemyBlockPosition = parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
        _isFront = true;
        _EnemyTurn = false;

    }

    // Update is called once per frame
    void Update()
    {
        Wait();
        // �G�l�~�[�X�e�[�g��ύX����֐����Ă�


        // ����̓^�[�����x���Ȃ����߃G���^�[�L�[�ŃX�e�[�g���ڍs�����Ă���
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            _isFront = false;
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

            GameObject ob;
            GameObject parent = transform.root.gameObject;
            ob = parent.transform.GetChild(1).gameObject;
            transform.parent = ob.transform;


        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _EnemyTurn = true;

        }

        if (_EnemyTurn)
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
        Vector2Int pos = _EnemyBlockPosition;

        // �O�㍶�E�Ƀu���b�N�����邩
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



            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x +1, pos.y));

            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x -1, pos.y));
        }


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

        _EnemyTurn = false;
    }


    // �ړ��֐�
    void Move()
    {

        //
        if (_isFront)
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(0).gameObject;
            transform.parent = o.transform;

            this.transform.position = o.transform.position;

            // �l�Y�~�̃��[�J���|�W�V������
            _EnemyBlockPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();


            // �l�Y�~�̈ʒu�𒲐�����
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

            // �l�Y�~�̃��[�J���|�W�V������
            _EnemyBlockPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();


            // �l�Y�~�̈ʒu�𒲐�����
            Vector3 Pos = this.transform.position;
            Pos.y = -_PosY;
            this.transform.position = Pos;


        }
        // �X�e�[�g��IDLE�Ɉڍs����
        _EnemyState = EnemyState.IDLE;

        // �l�Y�~�̃^�[�����I������
        _EnemyTurn = false;

    }

    // �ǂ�������֐�
    void Break()
    {
        if (_BreakTurn == BreakTurn.RANDOM)
        {

        }


        // �ǂ������鏈�������

        _EnemyTurn = false;
    }


    // �Ƃ����̃X�N���v�g�����Đ^���銴����
    // �u���b�N�ŌĂяo���@��������]������֐�
    public void RotateMySelf(Vector2Int position, float angle)
    {
        // �����x�N�g�����X�V�����鏈��
       
    }

    // �u���b�N�ŌĂяo���@�������Ђ�����Ԃ��֐��i�\������ւ��j
    public void TurnOverMySelf(Vector2Int position)
    {
        // _isFront��false
        // 
    }

    // �u���b�N�ŌĂяo���@�����̈ʒu�����ւ���֐�
    public void SwapMySelf(Vector2Int position)
    {
        // �u���b�N�̃��[�J��
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

    public bool GetEnemyTurn()
    {
        return _EnemyTurn;
    }

    public void ChangeState()
    {
        // �v���C���[�̂���u���b�N���擾����
        // �v���C���[�����ԉ����u���b�N�֓�����
        Vector3 playerpos = _Player.transform.position;

        GameObject obj = new GameObject();
        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float y = 90;
        if(_isFront)
        {
            if (_Up != null)
            {
                tmp = Vector3.Distance(playerpos, _Up.transform.position);
                if (tmp > distance)
                {
                    obj = _Up;
                    this.transform.rotation = Quaternion.Euler(0.0f,0.0f, 0.0f);
                    distance = tmp;
                }
            }
            if (_Down != null)
            {
                tmp = Vector3.Distance(playerpos, _Down.transform.position);
                if (tmp > distance)
                {
                    obj = _Down;
                    this.transform.rotation = Quaternion.Euler(0.0f, y*2, 0.0f);
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
        _EnemyTurn = true;
    }

}

//class Level1 :  EnemyControl
//{
//    public override void ChangeState()
//    {
//        _EnemyState = EnemyState.STAY;
//    }
//}

