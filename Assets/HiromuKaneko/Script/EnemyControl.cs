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

    protected enum EnemyLevel
    {
        LEVEL1,     // �s�����Ȃ��B�ҋ@���[�V�����̂�
        LEVEL2,     // 2�^�[���Ɉ�x�s������B�����邱�Ƃ͂��Ȃ�
        LEVEL3,     // 2�^�[���Ɉ�x�s���A���܂�1�^�[���Ɉ�x�s������B�����邱�Ƃ͂��Ȃ�
        LEVEL4,     // 2�^�[���Ɉ�x�s���A���܂�1�^�[���Ɉ�x�s������B���x���P�̍d���̃I�u�W�F�N�g��������
        LEVEL5,     // ���^�[���s������B���x��1�̍d���̃I�u�W�F�N�g��������
        LEVEL6,     // ���^�[���s������B���x���Q�̍d���̃I�u�W�F�N�g��������
        LEVEL7      // ���^�[���s������B���x��3�̍d���̃I�u�W�F�N�g��������
    }

    protected enum BreakTurn
    {
        RANDOM,
        EVERYTURN
    }


    // �K�v�����ȕϐ����Ƃ肠�����p��
    [SerializeField] protected EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] protected EnemyLevel _EnemyLevel = EnemyLevel.LEVEL1;
    [SerializeField] protected EnemyState _NextState = EnemyState.IDLE;


    [Header("�P�^�[���̍s����")]
    [SerializeField, TooltipAttribute("�P�^�[���̍s����"), Range(0, 2)] protected int _ActCount;           // �G�̍s����    �i�P�`�Q�H�j

    [Header("�ǂ������郌�x��")]
    [SerializeField, TooltipAttribute("�������ǂ̃��x��"), Range(0, 3)] protected int _BreakLevel;       // �����郌�x��    �i�O�`�R�H�j

    [Header("�����_��or�^�[����")]
    [SerializeField] protected BreakTurn _BreakTurn = BreakTurn.RANDOM;

    [Header("���f�o�b�O�p")]

    // ���ꂼ���Get�ESet���쐬�H
    [SerializeField] private Vector2Int _EnemyLocalPosition;      // �l�Y�~�̂���u���b�N�̍��W
    [SerializeField] private Vector2Int _EnemyDirection;          // �l�Y�~�̌����Ă����
    [SerializeField] private bool _IsFront;                       // �\������
    [SerializeField] private bool _StartEnemyTurn;                // �G�l�~�[�^�[�����n�܂����ŏ��ɏ�������p

    private GameObject _GameManager;                              // �Q�[���}�l�[�W���[��ێ�
    [SerializeField]
    protected GameObject _Player;                                 // �v���C���[��ێ�

    // �f�o�b�O�p�ɕ\�������Ă����Ȃ̂Ō�XSerializeField�͏����\��
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // �ړ��\�u���b�N�̕ێ��A�i�ސ�̃u���b�N��ێ�


    private float _PosY = 0.05f;    // Y���W�Œ�p
    [SerializeField] private int _WallCount;          // �ǂ̐��J�E���g�p
    [SerializeField] private int _NullBlockCount;     // nullblock�J�E���g�p
    [SerializeField] private int _TurnCount;
    [SerializeField] private int _Count;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        GameObject parent = transform.root.gameObject;
        _EnemyLocalPosition = parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
        _IsFront = true;
        _WallCount = 0;
        _TurnCount = 0;
        _Count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_EnemyState == EnemyState.IDLE)
            Idle();

        if (Input.GetKeyDown(KeyCode.K))
        {
            _WallCount = 0;
        }

    }

    //public virtual void ChangeState()
    //{


    //}

    void Wait()
    {
        Vector2Int pos = _EnemyLocalPosition;

        // �O�㍶�E�Ƀu���b�N�����邩
        if (_IsFront)
        {

            // ��̃u���b�N
            _EnemyDirection = new Vector2Int(0, 1);
            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));
            if (_Up != null)
            {
                if (!_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                {
                    Debug.Log("up");
                    _WallCount++;

                }
            }
            else
            {
                _NullBlockCount++;
            }

            // ���̃u���b�N
            _EnemyDirection = new Vector2Int(0, -1);
            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));
            if (_Down != null)
            {
                if (!_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                    _WallCount++;
            }
            else
            {
                _NullBlockCount++;

            }
            // ���̃u���b�N
            _EnemyDirection = new Vector2Int(-1, 0);
            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));
            if (_Left != null)
            {
                if (!_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                    _WallCount++;
            }
            else
            {
                _NullBlockCount++;

            }
            // �E�̃u���b�N
            _EnemyDirection = new Vector2Int(1, 0);
            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));
            if (_Right != null)
            {
                if (!_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                    _WallCount++;
            }
            else
            {
                _NullBlockCount++;

            }
        }
        else
        {

            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y + 1));


            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x, pos.y - 1));



            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x + 1, pos.y));

            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(new Vector2Int(pos.x - 1, pos.y));
        }


        // �����ŕǂ�����p�l�����擾���āA�����ȏ゠�����炩����悤�ɂ���H
        // �擾�����u���b�N�ɕǂ����邩�𔻒肷��
        // ChangeState�ŕǂ��󂷂��A����
        // Move����O�ɂ��̃u���b�N�Ƀv���C���[�����邩���Ȃ����𔻒肳���Ă��Ȃ���ΐi�߂�悤�ɏ�����ς���
        // 


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

        if (_Count > 0)
        {
            EnemyTurn();
        }
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

    // �ړ�������������Ȃ��ҋ@���[�V�����݂̂Ń^�[���I��
    void Stay()
    {
        // �Ȃɂ����Ȃ������H
        // �ҋ@���[�V���������s�H
        _Count--;
        _EnemyState = EnemyState.IDLE;
    }
    
    // �ړ��֐�
    void Move()
    {

        //
        if (_IsFront)
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(0).gameObject;
            transform.parent = o.transform;

            this.transform.position = o.transform.position;

            // �l�Y�~�̃��[�J���|�W�V������
            _EnemyLocalPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();


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
            _EnemyLocalPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();


            // �l�Y�~�̈ʒu�𒲐�����
            Vector3 Pos = this.transform.position;
            Pos.y = -_PosY;
            this.transform.position = Pos;

        }
        _Count--;
        _NextBlock = null;
        // �X�e�[�g��IDLE�Ɉڍs����
        _EnemyState = EnemyState.IDLE;

        // �l�Y�~�̃^�[�����I������

    }

    // �ǂ�������֐�
    void Break()
    {


        // �ǂ������鏈�������
        _EnemyDirection = new Vector2Int(0, 1);
        _Up.gameObject.GetComponent<BlockControl>().BreakWall(_IsFront, _EnemyLocalPosition, _EnemyDirection, _BreakLevel);
        //                Debug.Log("���ׂ����킷");
        _EnemyDirection = new Vector2Int(0, -1);
        _Down.gameObject.GetComponent<BlockControl>().BreakWall(_IsFront, _EnemyLocalPosition, _EnemyDirection, _BreakLevel);

        _EnemyDirection = new Vector2Int(-1, 0);
        _Left.gameObject.GetComponent<BlockControl>().BreakWall(_IsFront, _EnemyLocalPosition, _EnemyDirection, _BreakLevel);

        _EnemyDirection = new Vector2Int(1, 0);
        _Right.gameObject.GetComponent<BlockControl>().BreakWall(_IsFront, _EnemyLocalPosition, _EnemyDirection, _BreakLevel);

        if (_BreakTurn == BreakTurn.RANDOM)
        {

        }

        _Count--;

        _EnemyState = EnemyState.IDLE;

    }


    public void EnemyTurn()
    {
        if(_StartEnemyTurn)
        {
            _Count = _ActCount;
            _StartEnemyTurn = false;
        }


            Wait();
            // �G�l�~�[�X�e�[�g��ύX����֐����Ă�


            // �EShift�ŗ��ɍs���i�߂�Ȃ��j
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                _IsFront = false;
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

                GameObject ob;
                GameObject parent = transform.root.gameObject;
                ob = parent.transform.GetChild(1).gameObject;
                transform.parent = ob.transform;

            }

            // �����Ń��x���ʂɏ����������H
            // ChangeState();
            switch (_EnemyLevel)
            {
                case EnemyLevel.LEVEL1:
                    Level1();
                    break;
                case EnemyLevel.LEVEL2:
                    Level2();
                    break;
                case EnemyLevel.LEVEL3:
                    Level3();
                    break;
                case EnemyLevel.LEVEL4:
                    Level4();
                    break;
                case EnemyLevel.LEVEL5:
                    Level5();
                    break;
                case EnemyLevel.LEVEL6:
                    Level6();
                    break;
                case EnemyLevel.LEVEL7:
                    Level7();
                    break;
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


    public void ChangeState()
    {

    }

    // �����s�����Ȃ��@�ҋ@���[�V�����̂�
    public void Level1()
    {
        _EnemyState = EnemyState.STAY;
    }

    // �Q�^�[���ɂP�x�s������@�����邱�Ƃ͂��Ȃ�
    public void Level2()
    {
        // �v���C���[�̂���u���b�N���擾����
        // �v���C���[�����ԉ����u���b�N�֓�����
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject obj = null;
        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float y = 90;
        float random;

        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;
        }
        else
        {
            if (_IsFront)
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(playerpos, _Up.transform.position);
                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                obj = _Up;
                                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {

                            obj = _Up;
                            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                            distance = tmp;
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);


                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                obj = _Down;
                                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            obj = _Down;
                            this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                            distance = tmp;
                        }
                    }

                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                obj = _Left;
                                this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            obj = _Left;
                            this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                            distance = tmp;
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                obj = _Right;
                                this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {

                            obj = _Right;
                            this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                            distance = tmp;
                        }
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

            _TurnCount = 0;
            _NextBlock = obj;
            _EnemyState = EnemyState.MOVE;
        }
    }

    // �Q�^�[���ɂP�x�s������@���܂ɂP�^�[���ɂP�x�s���i����30%�ʁj�@�����邱�Ƃ͂��Ȃ�
    public void Level3()
    {
        // �v���C���[�̂���u���b�N���擾����
        // �v���C���[�����ԉ����u���b�N�֓�����
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject obj = null;
        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float y = 90;
        float random;

        random = Random.value;

        if (random < 0.3)
        {
            _TurnCount++;
        }

        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;
        }
        else
        {
            if (_IsFront)
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(playerpos, _Up.transform.position);
                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                obj = _Up;
                                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {

                            obj = _Up;
                            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                            distance = tmp;
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);


                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                obj = _Down;
                                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            obj = _Down;
                            this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                            distance = tmp;
                        }
                    }

                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                obj = _Left;
                                this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            obj = _Left;
                            this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                            distance = tmp;
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                obj = _Right;
                                this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {

                            obj = _Right;
                            this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                            distance = tmp;
                        }
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

            _TurnCount = 0;
            if (obj != null)
            {
                _NextBlock = obj;

                Debug.Log(_NextBlock);
                _EnemyState = EnemyState.MOVE;
            }
            else
            {
                _EnemyState = EnemyState.STAY;
            }
        }

    }

    // �Q�^�[���ɂP�x�s������@���܂ɂP�^�[���ɂP�x�s���i����50%�ʁj�@���x���P�̕ǂ�������
    public void Level4()
    {

<<<<<<< HEAD
        //// �v���C���[�̂���u���b�N���擾����
        //// �v���C���[�����ԉ����u���b�N�֓�����
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        GameObject breakobj = null;

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float y = 90;
        float random;

        // ���ӂ����ׂĕ� or �ǁ{�p�l�����Ȃ��ꍇ�͕K���������I��
        if ((_WallCount == 4) ||
           (_WallCount == 3 && _NullBlockCount == 1) ||
           (_WallCount == 2 && _NullBlockCount == 2) ||
           (_WallCount == 1 && _NullBlockCount == 3))
        {
            // �ǂ��̕ǂ������邩
            Debug.Log("�Ƃ������H");


            _WallCount = 0;
            _NullBlockCount = 0;
            _EnemyState = EnemyState.BREAK;
        }

        if (_WallCount == 3)
        {
            _WallCount = 0;
            _NullBlockCount = 0;
            _EnemyState = EnemyState.BREAK;
        }
        // ������p�l�����ꖇ�݂̂����ǃl�R�ɋ߂Â��Ă��܂��Ƃ��ɂ�����I��������i��ԗ������ꏊ�̕ǂ��j
        // ��ԉ����ւ�����p�l���ւ̓��ɕǂ��������炩����H�@�v���C���[�Ƃ̋����ɂ���Ă͗����ׂ��H


        random = Random.value;

        if (random < 0.5)
        {
            _TurnCount++;
        }

        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;
        }
        else
        {
            if (_IsFront)
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(playerpos, _Up.transform.position);
                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                moveobj = _Up;
                                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                                distance = tmp;
                            }
                            else
                            {
                                breakobj = _Up;
                                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            // �ǂ��Ȃ���΂�����
                            moveobj = _Up;
                            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                            distance = tmp;
                        }
                        else
                        {
                            // �ǂ��������炩����
                            breakobj = _Up;
                            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                            distance = tmp;
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);


                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                moveobj = _Down;
                                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                                distance = tmp;
                            }
                            else
                            {
                                breakobj = _Down;
                                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            moveobj = _Down;
                            this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                            distance = tmp;
                        }
                        else
                        {
                            breakobj = _Down;
                            this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
                            distance = tmp;
                        }
                    }

                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                moveobj = _Left;
                                this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                                distance = tmp;
                            }
                            else
                            {
                                breakobj = _Left;
                                this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            moveobj = _Left;
                            this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                            distance = tmp;
                        }
                        else
                        {
                            breakobj = _Left;
                            this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
                            distance = tmp;
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);

                    if (tmp == distance)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {

                                moveobj = _Right;
                                this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                                distance = tmp;
                            }
                            else
                            {
                                breakobj = _Right;
                                this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                                distance = tmp;
                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {

                            moveobj = _Right;
                            this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                            distance = tmp;
                        }
                        else
                        {
                            breakobj = _Right;
                            this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
                            distance = tmp;
                        }
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
                        moveobj = _Up;
                        distance2 = tmp;
                    }
                }

                if (_Down != null)
                {
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);
                    if (tmp < distance2)
                    {
                        moveobj = _Down;
                        distance2 = tmp;
                    }
                }

                if (_Left != null)
                {
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);
                    if (tmp < distance2)
                    {
                        moveobj = _Left;
                        distance2 = tmp;
                    }
                }

                if (_Right != null)
                {
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);
                    if (tmp < distance2)
                    {
                        moveobj = _Right;
                        distance2 = tmp;
                    }
                }
            }
=======
        ////// �v���C���[�̂���u���b�N���擾����
        ////// �v���C���[�����ԉ����u���b�N�֓�����
        //_Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        //Vector3 playerpos = _Player.transform.position;

        //GameObject moveobj = new GameObject();
        //GameObject breakobj = new GameObject();

        //float distance = 0.0f;
        //float distance2 = 10000.0f;
        //float tmp = 0.0f;
        //float y = 90;
        //float random;

        //// ���ӂ����ׂĕ� or �ǁ{�p�l�����Ȃ��ꍇ�͕K���������I��
        //if ((_WallCount == 4) ||
        //   (_WallCount == 3 && _NullBlockCount == 1) ||
        //   (_WallCount == 2 && _NullBlockCount == 2) ||
        //   (_WallCount == 1 && _NullBlockCount == 3))
        //{
        //    // �ǂ��̕ǂ������邩
        //    Debug.Log("�Ƃ������H");


        //    _WallCount = 0;
        //    _NullBlockCount = 0;
        //    _EnemyState = EnemyState.BREAK;
        //}

        //if (_WallCount == 3)
        //{
        //    _WallCount = 0;
        //    _NullBlockCount = 0;
        //    _EnemyState = EnemyState.BREAK;
        //}
        //// ������p�l�����ꖇ�݂̂����ǃl�R�ɋ߂Â��Ă��܂��Ƃ��ɂ�����I��������i��ԗ������ꏊ�̕ǂ��j
        //// ��ԉ����ւ�����p�l���ւ̓��ɕǂ��������炩����H�@�v���C���[�Ƃ̋����ɂ���Ă͗����ׂ��H

>>>>>>> 63b5873ec038eefc3b30ce183f7bccef8c33f320

        //random = Random.value;

        //if (random < 0.5)
        //{
        //    _TurnCount++;
        //}

        //if (_TurnCount == 0)
        //{
        //    _TurnCount++;

        //}
        //else
        //{
        //    if (_IsFront)
        //    {
        //        if (_Up != null)
        //        {
        //            _EnemyDirection = new Vector2Int(0, 1);
        //            tmp = Vector3.Distance(playerpos, _Up.transform.position);
        //            if (tmp == distance)
        //            {
        //                random = Random.value;
        //                if (random < 0.5f)
        //                {
        //                    if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                    {

        //                        moveobj = _Up;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                        distance = tmp;
        //                    }
        //                    else
        //                    {
        //                        breakobj = _Up;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                        distance = tmp;
        //                    }
        //                }
        //            }
        //            else if (tmp > distance)
        //            {
        //                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                {
        //                    // �ǂ��Ȃ���΂�����
        //                    obj = _Up;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                    distance = tmp;
        //                }
        //                else
        //                {
        //                    // �ǂ��������炩����
        //                    breakobj = _Up;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //                    distance = tmp;
        //                }
        //            }
        //        }

        //        if (_Down != null)
        //        {
        //            _EnemyDirection = new Vector2Int(0, -1);
        //            tmp = Vector3.Distance(playerpos, _Down.transform.position);


        //            if (tmp == distance)
        //            {
        //                random = Random.value;
        //                if (random < 0.5f)
        //                {
        //                    if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                    {
        //                        moveobj = _Down;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
        //                        distance = tmp;
        //                    }
        //                    else
        //                    {
        //                        breakobj = _Down;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
        //                        distance = tmp;
        //                    }
        //                }
        //            }
        //            else if (tmp > distance)
        //            {
        //                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                {
        //                    obj = _Down;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
        //                    distance = tmp;
        //                }
        //                else
        //                {
        //                    breakobj = _Down;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
        //                    distance = tmp;
        //                }
        //            }

        //        }

        //        if (_Left != null)
        //        {
        //            _EnemyDirection = new Vector2Int(-1, 0);
        //            tmp = Vector3.Distance(playerpos, _Left.transform.position);

        //            if (tmp == distance)
        //            {
        //                random = Random.value;
        //                if (random < 0.5f)
        //                {
        //                    if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                    {

        //                        moveobj = _Left;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
        //                        distance = tmp;
        //                    }
        //                    else
        //                    {
        //                        breakobj = _Left;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
        //                        distance = tmp;
        //                    }
        //                }
        //            }
        //            else if (tmp > distance)
        //            {
        //                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                {
        //                    moveobj = _Left;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
        //                    distance = tmp;
        //                }
        //                else
        //                {
        //                    breakobj = _Left;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, -y, 0.0f);
        //                    distance = tmp;
        //                }
        //            }
        //        }

        //        if (_Right != null)
        //        {
        //            _EnemyDirection = new Vector2Int(1, 0);
        //            tmp = Vector3.Distance(playerpos, _Right.transform.position);

        //            if (tmp == distance)
        //            {
        //                random = Random.value;
        //                if (random < 0.5f)
        //                {
        //                    if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                    {

        //                        moveobj = _Right;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
        //                        distance = tmp;
        //                    }
        //                    else
        //                    {
        //                        breakobj = _Right;
        //                        this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
        //                        distance = tmp;
        //                    }
        //                }
        //            }
        //            else if (tmp > distance)
        //            {
        //                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
        //                {

        //                    obj = _Right;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
        //                    distance = tmp;
        //                }
        //                else
        //                {
        //                    breakobj = _Right;
        //                    this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
        //                    distance = tmp;
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        if (_Up != null)
        //        {
        //            tmp = Vector3.Distance(playerpos, _Up.transform.position);
        //            if (tmp < distance2)
        //            {
        //                obj = _Up;
        //                distance2 = tmp;
        //            }
        //        }

        //        if (_Down != null)
        //        {
        //            tmp = Vector3.Distance(playerpos, _Down.transform.position);
        //            if (tmp < distance2)
        //            {
        //                obj = _Down;
        //                distance2 = tmp;
        //            }
        //        }

        //        if (_Left != null)
        //        {
        //            tmp = Vector3.Distance(playerpos, _Left.transform.position);
        //            if (tmp < distance2)
        //            {
        //                obj = _Left;
        //                distance2 = tmp;
        //            }
        //        }

        //        if (_Right != null)
        //        {
        //            tmp = Vector3.Distance(playerpos, _Right.transform.position);
        //            if (tmp < distance2)
        //            {
        //                obj = _Right;
        //                distance2 = tmp;
        //            }
        //        }
        //    }


        //    float movetmp = Vector3.Distance(playerpos, moveobj.transform.position);
        //    float breaktmp = Vector3.Distance(playerpos, breakobj.transform.position);

        //    if (movetmp < breaktmp)
        //    {
        //        _NextBlock = breakobj;
        //        _EnemyState = EnemyState.BREAK;

        //    }
        //    else
        //    {
        //        _NextBlock = moveobj;
        //        _EnemyState = EnemyState.MOVE;
        //    }
        //    _TurnCount = 0;
        //}

    }

    // ���^�[���s������B���x��1�̍d���̃I�u�W�F�N�g��������
    public void Level5()
    {

    }

    // ���^�[���s������B���x���Q�̍d���̃I�u�W�F�N�g��������
    public void Level6()
    {

    }

    // ���^�[���s������B���x���R�̍d���̃I�u�W�F�N�g��������
    public void Level7()
    {

    }


    // �u���b�N���ŌĂяo���@��������]������֐�
    public void RotateMySelf(Vector2Int position, float angle)
    {
        //Rotate���ɌĂяo�����֐��A�����̕�����ς���Ƃ��ɂ������ŌĂ�
        if (position != _EnemyLocalPosition)
            return;

        Vector3 direction = new Vector3(_EnemyDirection.x, 0f, _EnemyDirection.y);
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        Vector2 tmp = new Vector2(direction.x, direction.z);
        //�l�̌ܓ����đ�����邱�Ƃ�Vector2Int�ɂ����������������
        _EnemyDirection = new Vector2Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y));
    }

    // �u���b�N���ŌĂяo���@�������Ђ�����Ԃ��֐��i�\������ւ��j
    public void TurnOverMySelf(Vector2Int position)
    {
        //TurnOver���ɌĂяo�����֐��A�Ђ�����Ԃ��̂�SetIsFront�ŗǂ��̂ł́H
        if (position != _EnemyLocalPosition)
            return;

        //�Ђ�����Ԃ�
        if (_IsFront)
            _IsFront = false;
        else
            _IsFront = true;
    }

    // �u���b�N���ŌĂяo���@�����̈ʒu�����ւ���֐�
    public void SwapMySelf(List<Vector2Int> position)
    {
        // �u���b�N�̃��[�J��
        //Swap���ɌĂяo�����֐��A�e�I�u�W�F�N�g�ł���u���b�N�̈ړ��ɂ��Ă�������
        foreach (Vector2Int pos in position)
        {
            if (pos == _EnemyLocalPosition)
            {
                var blockConfig = transform.parent.parent.GetComponent<BlockConfig>();
                _EnemyLocalPosition = blockConfig.GetBlockLocalPosition();
                return;
            }
        }
    }

    public void EnemyDestroy() { Destroy(this.gameObject); }                                 // �G�l�~�[���폜���鏈��
    public void SetIsFront(bool isfront) { _IsFront = isfront; }
    public void SetStartEnemyTurn(bool enemyturn) { _StartEnemyTurn = enemyturn; }           // �G�l�~�[�^�[���ɕς�����Ƃ��Ƀ^�[���}�l�[�W���[��true�ɂ��Ăق���
    public void SetLocalPosition(Vector2Int position) { _EnemyLocalPosition = position; }    // �����̂���u���b�N�̍��W���X�V����

}

//class Level1 :  EnemyControl
//{
//    public override void ChangeState()
//    {
//        _EnemyState = EnemyState.STAY;
//    }
//}

