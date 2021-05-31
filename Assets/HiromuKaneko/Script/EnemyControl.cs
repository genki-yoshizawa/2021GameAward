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
    protected EnemyState _EnemyState = EnemyState.IDLE;
    [SerializeField] protected EnemyLevel _EnemyLevel = EnemyLevel.LEVEL1;
    protected EnemyState _NextState = EnemyState.IDLE;


    [Header("�P�^�[���̍s����")]
    [SerializeField, TooltipAttribute("�P�^�[���̍s����"), Range(0, 2)] protected int _ActCount;           // �G�̍s����    �i�P�`�Q�H�j

    [Header("�ǂ������郌�x��")]
    [SerializeField, TooltipAttribute("�������ǂ̃��x��"), Range(0, 3)] protected int _BreakLevel;       // �����郌�x��    �i�O�`�R�H�j

    [Header("�����_��or�^�[����")]
    [SerializeField] protected BreakTurn _BreakTurn = BreakTurn.RANDOM;

    [Header("�k�o�͈�")]
    [SerializeField] private int _CheeseSearchRange = 0;

    [Header("�f�o�b�O�J���[�̐ݒ�")]
    [SerializeField] private Color _DebugColor = new Color(1f, 1f, 0, 0.5f);

    [Header("���b�����Ĉړ����邩")]
    [SerializeField] protected float _WalkTime = 1.0f;

    [Header("���b�����Ă����邩")]
    [SerializeField] protected float _BiteTime = 1.0f;

    [Header("���b�ԃp�j�b�N���邩")]
    [SerializeField] protected float _PanicTime = 1.0f;

    [Header("���b�Ԍ��Captured���Đ����邩")]
    [SerializeField] private float _CapturedDelayTime = 1.0f;

    [Header("�\���E���̃e�N�X�`��")]
    [SerializeField] Texture _FrontTexture;
    [Header("�����E���̃e�N�X�`��")]
    [SerializeField] Texture _BackTexture;
    [Header("�l�Y�~�Ɏg���Ă�}�e���A��������")]

    public Material TargetMaterial;
    [Header("���f�o�b�O�p")]


    // ���ꂼ���Get�ESet���쐬�H
    [SerializeField] private Vector2Int _EnemyLocalPosition;      // �l�Y�~�̂���u���b�N�̍��W
    private Vector2Int _EnemyDirection;          // �l�Y�~�̌����Ă����
    private bool _StartEnemyTurn;                // �G�l�~�[�^�[�����n�܂����ŏ��ɏ�������p

    private GameObject _GameManager;                              // �Q�[���}�l�[�W���[��ێ�
    protected GameObject _Player;                                 // �v���C���[��ێ�
    private GameObject _Cheese;                                   // �`�[�Y��ێ�

    // �f�o�b�O�p�ɕ\�������Ă����Ȃ̂Ō�XSerializeField�͏����\��
    [SerializeField] protected GameObject _Up, _Down, _Left, _Right, _NextBlock; // �ړ��\�u���b�N�̕ێ��A�i�ސ�̃u���b�N��ێ�

    private int _TurnCount;
    private int _Count;
    private float _PosY = 0.075f;    // Y���W�Œ�p

    private Animator _EnemyAnimation;
    private Vector3 _StartPoint;
    private Vector3 _TargetPoint;
    private Vector3 _UpdatePosition;
    private float _PassedTime;
    private bool _CheeseBite;
    private bool _PlayerBite;
    private bool _IsExist;
    private bool _IsFront;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        GameObject parent = transform.root.gameObject;
        _EnemyLocalPosition = parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
        _IsFront = (transform.parent == transform.parent.parent.GetChild(0));
        _EnemyAnimation = gameObject.GetComponent<Animator>();
        _CheeseBite = false;
        _IsExist = false;
        _Count = 0;
        _PassedTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {

        if (_EnemyState == EnemyState.IDLE)
        {
            Idle();
        }

        // �p�j�b�N�A�j���[�V����
        if (_EnemyAnimation.GetBool("Panic"))
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _PanicTime)
            {
                _EnemyAnimation.SetBool("Panic", false);
                _PassedTime = 0.0f;

            }

        }

        // �����A�j���[�V����
        if (_EnemyAnimation.GetBool("Walk"))
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                _PassedTime = _WalkTime;
                _EnemyAnimation.SetBool("Walk", false);
            }

            Rotate();
            transform.position = _StartPoint + (_TargetPoint - _StartPoint) * (_PassedTime / _WalkTime);

            if (!_EnemyAnimation.GetBool("Walk"))
                _PassedTime = 0.0f;

        }

        // ������A�j���[�V����
        if (_EnemyAnimation.GetBool("Bite"))
        {
            // �`�[�Y��������Ƃ��̃A�j���[�V�����@������Ȃ���ړ�
            if (_CheeseBite)
            {
                _Cheese.gameObject.GetComponent<CheeseControl>().Eaten();
                float time = Time.deltaTime;
                if ((_PassedTime += time) > _WalkTime)
                {
                    _PassedTime = _WalkTime;
                    _EnemyAnimation.SetBool("Bite", false);
                }

                Rotate();
                transform.position = _StartPoint + (_TargetPoint - _StartPoint) * (_PassedTime / _WalkTime);

                if (!_EnemyAnimation.GetBool("Bite"))
                {
                    _PassedTime = 0.0f;
                    _CheeseBite = false;
                }

            }
            else
            {
                float time = Time.deltaTime;
                Rotate();
                if ((_PassedTime += time) > _BiteTime)
                {
                    _EnemyAnimation.SetBool("Bite", false);
                    _PassedTime = 0.0f;

                }
            }

        }

        var clipInfo = _EnemyAnimation.GetCurrentAnimatorClipInfo(0)[0];   // ������Layer�ԍ��A�z���0�Ԗ�

        // ���݂̃A�j���[�V������Dead��������I�u�W�F�N�g���폜����
        if (clipInfo.clip.name == "Dead")
        {
            Destroy(this.gameObject);
        }

    }

    void Wait()
    {
        Vector2Int pos = _EnemyLocalPosition;

        // �O�㍶�E�Ƀu���b�N�����邩
        if (_IsFront)
        {

            // ��̃u���b�N
            _EnemyDirection = new Vector2Int(0, 1);
            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // ���̃u���b�N
            _EnemyDirection = new Vector2Int(0, -1);
            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // ���̃u���b�N
            _EnemyDirection = new Vector2Int(-1, 0);
            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // �E�̃u���b�N
            _EnemyDirection = new Vector2Int(1, 0);
            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));


        }
        else
        {
            // ��̃u���b�N
            _EnemyDirection = new Vector2Int(0, 1);
            _Up = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // ���̃u���b�N
            _EnemyDirection = new Vector2Int(0, -1);
            _Down = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // ���̃u���b�N
            _EnemyDirection = new Vector2Int(-1, 0);
            _Left = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));

            // �E�̃u���b�N
            _EnemyDirection = new Vector2Int(1, 0);
            _Right = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock((pos + _EnemyDirection));
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
        // _EnemyAnimation.SetBool("Wait", true);
        if (_Count > 0)
        {
            EnemyTurn();
        }

    }

    // �ړ�������������Ȃ��ҋ@���[�V�����݂̂Ń^�[���I��
    void Stay()
    {
        _EnemyAnimation.SetBool("Panic", true);
        _Count--;
        _EnemyState = EnemyState.IDLE;
    }

    // �ړ��֐�
    void Move()
    {

        if (_IsFront)
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(0).gameObject;
            transform.parent = o.transform;

            _TargetPoint = o.transform.position;
            _StartPoint = this.transform.position;
            _TargetPoint.y = _PosY;

            // �l�Y�~�̃��[�J���|�W�V�������X�V
            _EnemyLocalPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();

            if (_Cheese != null)
            {
                Vector2Int cheeselocalposition = _Cheese.gameObject.GetComponent<CheeseConfig>().GetCheeseLocalPosition();

                if (cheeselocalposition == _EnemyLocalPosition)
                {
                    _CheeseBite = true;
                    _EnemyAnimation.SetBool("Bite", true);
                }
                else
                {
                    _EnemyAnimation.SetBool("Walk", true);
                }
            }
            else
            {
                _EnemyAnimation.SetBool("Walk", true);

            }
        }
        else
        {
            GameObject o;
            o = _NextBlock.transform.GetChild(1).gameObject;
            transform.parent = o.transform;

            _TargetPoint = o.transform.position;
            _StartPoint = this.transform.position;
            _TargetPoint.y = -_PosY;

            // �l�Y�~�̃��[�J���|�W�V�������X�V
            _EnemyLocalPosition = _NextBlock.GetComponent<BlockConfig>().GetBlockLocalPosition();

            if (_Cheese != null)
            {
                Vector2Int cheeselocalposition = _Cheese.gameObject.GetComponent<CheeseConfig>().GetCheeseLocalPosition();

                if (cheeselocalposition == _EnemyLocalPosition)
                {
                    _CheeseBite = true;
                    _EnemyAnimation.SetBool("Bite", true);
                }
                else
                {
                    _EnemyAnimation.SetBool("Walk", true);
                }
            }
            else
            {
                _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();

                if (!_Player.gameObject.GetComponent<PlayerControl>().GetIsFront())
                {
                    if (_Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition() == _EnemyLocalPosition)
                    {
                        PlayerKill();

                    }
                    else
                        _EnemyAnimation.SetBool("Walk", true);
                }

                _EnemyAnimation.SetBool("Walk", true);

            }

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
        Rotate();

        _EnemyAnimation.SetBool("Bite", true);

        _NextBlock.gameObject.GetComponent<BlockControl>().BreakWall(_IsFront, _EnemyLocalPosition, _EnemyDirection, _BreakLevel);

        if (_BreakTurn == BreakTurn.RANDOM)
        {

        }

        _Count--;

        _EnemyState = EnemyState.IDLE;

    }


    public void EnemyTurn()
    {
        // �����Ă���Γ�����
        if (_IsExist == false)
        {
            _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
            if (_Player.GetComponent<PlayerControl>().GetLocalPosition() != _EnemyLocalPosition)
            {
                if (_StartEnemyTurn)
                {
                    _Count = _ActCount;
                    _StartEnemyTurn = false;
                }

                Wait();
                // �G�l�~�[�X�e�[�g��ύX����֐����Ă�


                // �EShift�ŗ��ɍs���i�߂�Ȃ��j
                //if (Input.GetKeyDown(KeyCode.RightShift))
                //{
                //    _IsFront = false;
                //    this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);

                //    GameObject ob;
                //    GameObject parent = transform.root.gameObject;
                //    ob = parent.transform.GetChild(1).gameObject;
                //    transform.parent = ob.transform;

                //}

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
            else
            {
                if (!_Player.gameObject.GetComponent<PlayerControl>().GetIsFront())
                {
                    PlayerKill();

                }

            }

        }



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

        GameObject moveobj = null;
        Vector2Int movedirection = new Vector2Int();

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;


        if (_TurnCount == 0)
        {
            _TurnCount++;
            _EnemyState = EnemyState.STAY;
        }
        else
        {

            // �`�[�Y�݂��Ă�
            if (_Cheese != null)
            {
                // �\
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }
                // ��
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }

                // �`�[�Y�ƃl�Y�~�̂���ʒu���r
                tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
                if (moveobj != null)
                {
                    // �`�[�Y�ƃ��[�u��̃p�l���̈ʒu���r
                    tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }
                }
                else
                {
                    _EnemyState = EnemyState.STAY;

                }
            }
            // �`�[�Y�����ĂȂ�
            else
            {
                // �\
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                        }
                    }

                    // �v���C���[�ƃl�Y�~�̋������擾
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // �v���C���[�ƃ��[�u��̃p�l���̋������擾
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                        _EnemyState = EnemyState.STAY;

                }
                // ��
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }

                    // �v���C���[�ƃl�Y�~�̋������擾
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // �v���C���[�ƃ��[�u��̃p�l���̋������擾
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                        _EnemyState = EnemyState.STAY;

                }
            }
            _TurnCount = 0;
        }
    }

    // �Q�^�[���ɂP�x�s������@���܂ɂP�^�[���ɂP�x�s���i����30%�ʁj�@�����邱�Ƃ͂��Ȃ�
    public void Level3()
    {
        // �v���C���[�̂���u���b�N���擾����
        // �v���C���[�����ԉ����u���b�N�֓�����
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        Vector2Int movedirection = new Vector2Int();

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
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

            // �`�[�Y�݂��Ă�
            if (_Cheese != null)
            {
                // �\
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }
                // ��
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }
                }

                // �`�[�Y�ƃl�Y�~�̂���ʒu���r
                tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
                if (moveobj != null)
                {
                    // �`�[�Y�ƃ��[�u��̃p�l���̈ʒu���r
                    tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }
                }
                else
                {
                    _EnemyState = EnemyState.STAY;

                }
            }
            // �`�[�Y�����ĂȂ�
            else
            {
                // �\
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance = tmp;
                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                        }
                    }

                    // �v���C���[�ƃl�Y�~�̋������擾
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // �v���C���[�ƃ��[�u��̃p�l���̋������擾
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                         _EnemyState = EnemyState.STAY;

                }
                // ��
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                        }
                    }

                    // �v���C���[�ƃl�Y�~�̋������擾
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        // �v���C���[�ƃ��[�u��̃p�l���̋������擾
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                            _EnemyState = EnemyState.STAY;

                    }
                    else
                        _EnemyState = EnemyState.STAY;

                }
            }
            _TurnCount = 0;
        }
    }


    // �Q�^�[���ɂP�x�s������@���܂ɂP�^�[���ɂP�x�s���i����50%�ʁj�@���x���P�̕ǂ�������
    public void Level4()
    {
        // �v���C���[�̂���u���b�N���擾����
        // �v���C���[�����ԉ����u���b�N�֓�����
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        GameObject breakobj = null;

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float breakdistance = 0.0f;
        float breakdistance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;
        Vector2Int movedirection = new Vector2Int();
        Vector2Int breakdirection = new Vector2Int();


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


            // �`�[�Y�݂��Ă�
            if (_Cheese != null)
            {
                // �\
                if (_IsFront)
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }
                }
                // ��
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }
                }

                // �`�[�Y�ƃl�Y�~�̂���ʒu���r
                tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
                if (moveobj != null)
                {
                    // �`�[�Y�ƃ��[�u��̃p�l���̈ʒu���r
                    tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                    if (breakobj != null)
                    {
                        // �`�[�Y�Ɖ󂷕ǂ̈ʒu���r
                        float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            if (tmp3 < tmp)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }

                        }

                    }
                    else
                    {
                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }

                    }
                }
                else
                {
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                        if (tmp3 < tmp2)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }

                }
            }
            // �`�[�Y�����ĂȂ�
            else
            {
                // �\
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance = tmp;
                                }
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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance = tmp;
                                }

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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance = tmp;
                                    }


                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance = tmp;
                                }

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
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance = tmp;
                                }
                                else
                                {
                                    if (tmp > breakdistance)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp > distance)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }

                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp2 < tmp)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                if (tmp < tmp3)
                                {
                                    _EnemyDirection = breakdirection;
                                    _NextBlock = breakobj;
                                    _EnemyState = EnemyState.BREAK;
                                }
                                else
                                {
                                    _EnemyState = EnemyState.STAY;
                                }
                            }

                        }
                        else
                        {
                            if (tmp2 < tmp)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }
                    }
                    else
                    {
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp2 < tmp3)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }

                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
                // ��
                else
                {
                    if (_Up != null)
                    {
                        _EnemyDirection = new Vector2Int(0, 1);
                        tmp = Vector3.Distance(playerpos, _Up.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Up;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Up;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Down != null)
                    {
                        _EnemyDirection = new Vector2Int(0, -1);
                        tmp = Vector3.Distance(playerpos, _Down.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Down;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Down;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Left != null)
                    {
                        _EnemyDirection = new Vector2Int(-1, 0);
                        tmp = Vector3.Distance(playerpos, _Left.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Left;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Left;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    if (_Right != null)
                    {
                        _EnemyDirection = new Vector2Int(1, 0);
                        tmp = Vector3.Distance(playerpos, _Right.transform.position);
                        if (tmp == distance2)
                        {
                            random = Random.value;
                            if (random < 0.5f)
                            {
                                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                                {
                                    movedirection = _EnemyDirection;
                                    moveobj = _Right;
                                    distance2 = tmp;
                                }
                                else
                                {
                                    if (tmp < breakdistance2)
                                    {
                                        breakdirection = _EnemyDirection;
                                        breakobj = _Right;
                                        breakdistance2 = tmp;
                                    }

                                }
                            }
                        }
                        else if (tmp < distance2)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }
                            }
                        }
                    }

                    // �v���C���[�ƃl�Y�~�̈ʒu�̋������擾
                    tmp2 = Vector3.Distance(playerpos, this.transform.position);
                    if (moveobj != null)
                    {
                        tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp < tmp2)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                if (tmp3 < tmp)
                                {
                                    _EnemyDirection = breakdirection;
                                    _NextBlock = breakobj;
                                    _EnemyState = EnemyState.BREAK;
                                }
                                else
                                {
                                    _EnemyState = EnemyState.STAY;
                                }
                            }

                        }
                        else
                        {
                            if (tmp < tmp2)
                            {
                                _EnemyDirection = movedirection;
                                _NextBlock = moveobj;
                                _EnemyState = EnemyState.MOVE;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }
                    }
                    else
                    {
                        if (breakobj != null)
                        {
                            float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                            if (tmp3 < tmp2)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }

                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
            }
            _TurnCount = 0;
        }
    }

    // ���^�[���s������B���x��1�̍d���̃I�u�W�F�N�g��������
    public void Level5()
    {


        // �v���C���[�̂���u���b�N���擾����
        // �v���C���[�����ԉ����u���b�N�֓�����
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        GameObject moveobj = null;
        GameObject breakobj = null;

        float distance = 0.0f;
        float distance2 = 10000.0f;
        float breakdistance = 0.0f;
        float breakdistance2 = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;
        Vector2Int movedirection = new Vector2Int();
        Vector2Int breakdirection = new Vector2Int();


        // �`�[�Y�݂��Ă�
        if (_Cheese != null)
        {
            // �\
            if (_IsFront)
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }
            }
            // ��
            else
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }
            }

            // �`�[�Y�ƃl�Y�~�̂���ʒu���r
            tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
            if (moveobj != null)
            {
                // �`�[�Y�ƃ��[�u��̃p�l���̈ʒu���r
                tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);
                if (breakobj != null)
                {
                    // �`�[�Y�Ɖ󂷕ǂ̈ʒu���r
                    float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }
                    else
                    {
                        if (tmp3 < tmp)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }

                }
                else
                {
                    if (tmp < tmp2)
                    {
                        _EnemyDirection = movedirection;
                        _NextBlock = moveobj;
                        _EnemyState = EnemyState.MOVE;
                    }

                }
            }
            else
            {
                if (breakobj != null)
                {
                    float tmp3 = Vector3.Distance(_Cheese.transform.position, breakobj.transform.position);

                    if (tmp3 < tmp2)
                    {
                        _EnemyDirection = breakdirection;
                        _NextBlock = breakobj;
                        _EnemyState = EnemyState.BREAK;
                    }

                }

            }


        }
        // �`�[�Y�����ĂȂ�
        else
        {
            // �\
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
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance = tmp;
                            }
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
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance = tmp;
                            }

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
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance = tmp;
                                }


                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance = tmp;
                            }

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
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance = tmp;
                            }
                            else
                            {
                                if (tmp > breakdistance)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp > distance)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance = tmp;
                        }
                        else
                        {
                            if (tmp > breakdistance)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance = tmp;
                            }

                        }
                    }
                }

                tmp2 = Vector3.Distance(playerpos, this.transform.position);
                if (moveobj != null)
                {
                    tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            if (tmp < tmp3)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }

                    }
                    else
                    {
                        if (tmp2 < tmp)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
                else
                {
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp2 < tmp3)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }
                    else
                    {
                        _EnemyState = EnemyState.STAY;
                    }
                }
            }
            // ��
            else
            {
                if (_Up != null)
                {
                    _EnemyDirection = new Vector2Int(0, 1);
                    tmp = Vector3.Distance(playerpos, _Up.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Up;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Up;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Up.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Up;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Up;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Down != null)
                {
                    _EnemyDirection = new Vector2Int(0, -1);
                    tmp = Vector3.Distance(playerpos, _Down.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Down;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Down;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Down.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Down;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Down;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Left != null)
                {
                    _EnemyDirection = new Vector2Int(-1, 0);
                    tmp = Vector3.Distance(playerpos, _Left.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Left;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Left;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Left.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Left;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Left;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                if (_Right != null)
                {
                    _EnemyDirection = new Vector2Int(1, 0);
                    tmp = Vector3.Distance(playerpos, _Right.transform.position);
                    if (tmp == distance2)
                    {
                        random = Random.value;
                        if (random < 0.5f)
                        {
                            if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                            {
                                movedirection = _EnemyDirection;
                                moveobj = _Right;
                                distance2 = tmp;
                            }
                            else
                            {
                                if (tmp < breakdistance2)
                                {
                                    breakdirection = _EnemyDirection;
                                    breakobj = _Right;
                                    breakdistance2 = tmp;
                                }

                            }
                        }
                    }
                    else if (tmp < distance2)
                    {
                        if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                        {
                            movedirection = _EnemyDirection;
                            moveobj = _Right;
                            distance2 = tmp;
                        }
                        else
                        {
                            if (tmp < breakdistance2)
                            {
                                breakdirection = _EnemyDirection;
                                breakobj = _Right;
                                breakdistance2 = tmp;
                            }
                        }
                    }
                }

                // �v���C���[�ƃl�Y�~�̈ʒu�̋������擾
                tmp2 = Vector3.Distance(playerpos, this.transform.position);
                if (moveobj != null)
                {
                    tmp = Vector3.Distance(playerpos, moveobj.transform.position);
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            if (tmp3 < tmp)
                            {
                                _EnemyDirection = breakdirection;
                                _NextBlock = breakobj;
                                _EnemyState = EnemyState.BREAK;
                            }
                            else
                            {
                                _EnemyState = EnemyState.STAY;
                            }
                        }

                    }
                    else
                    {
                        if (tmp < tmp2)
                        {
                            _EnemyDirection = movedirection;
                            _NextBlock = moveobj;
                            _EnemyState = EnemyState.MOVE;
                        }
                        else
                        {
                            _EnemyState = EnemyState.STAY;
                        }
                    }
                }
                else
                {
                    if (breakobj != null)
                    {
                        float tmp3 = Vector3.Distance(playerpos, breakobj.transform.position);

                        if (tmp3 < tmp2)
                        {
                            _EnemyDirection = breakdirection;
                            _NextBlock = breakobj;
                            _EnemyState = EnemyState.BREAK;
                        }

                    }
                    else
                    {
                        _EnemyState = EnemyState.STAY;
                    }
                }
            }





        }


    }

    // ���^�[���s������B���x���Q�̍d���̃I�u�W�F�N�g��������
    public void Level6()
    {

    }

    // ���^�[���s������B���x���R�̍d���̃I�u�W�F�N�g��������
    public void Level7()
    {

    }

    public void PlayerKill()
    {
        _EnemyAnimation.SetTrigger("Attack");
        _Player.gameObject.GetComponent<PlayerControl>().SetIsExist(false);

    }

    // �G�l�~�[���s�������E����������։�]������
    public void Rotate()
    {
        if (_IsFront)
            TargetMaterial.SetTexture("_MainTex", _FrontTexture);
        else
            TargetMaterial.SetTexture("_MainTex", _BackTexture);


        //_UpdatePosition = this.transform.position;

        //if(_IsFront)
        //    _UpdatePosition.y = _PosY;
        //else
        //    _UpdatePosition.y = _PosY * -1;

        //this.transform.position = _UpdatePosition;

        float y = 90.0f;
        if (_IsFront)
        {
            if (_EnemyDirection == new Vector2Int(0, 1))
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            if (_EnemyDirection == new Vector2Int(0, -1))
                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);
            if (_EnemyDirection == new Vector2Int(-1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y * -1, 0.0f);
            if (_EnemyDirection == new Vector2Int(1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);
        }
        else
        {
            if (_EnemyDirection == new Vector2Int(0, 1))
                this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
            if (_EnemyDirection == new Vector2Int(0, -1))
                this.transform.rotation = Quaternion.Euler(0.0f, y * 2, -180.0f);
            if (_EnemyDirection == new Vector2Int(-1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y * -1, -180.0f);
            if (_EnemyDirection == new Vector2Int(1, 0))
                this.transform.rotation = Quaternion.Euler(0.0f, y, -180.0f);
        }

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
    private void OnDrawGizmos()
    {
        Gizmos.color = _DebugColor;
        Vector3 size = new Vector3(1.0f, 0.01f, 1.0f);

        for (int x = 0; x < 2 * _CheeseSearchRange + 1; ++x)
        {
            for (int z = 0; z < 2 * _CheeseSearchRange + 1; ++z)
            {
                Vector2 pos = new Vector2(x - _CheeseSearchRange, z - _CheeseSearchRange);
                Vector3 position = new Vector3(pos.x, 0.0f, pos.y);
                Gizmos.DrawCube(transform.position + position, size);
            }
        }
    }

    // �G�l�~�[���폜���鏈��
    public void SetDestroy()
    {
        _IsExist = true;

        transform.parent = null;
        StartCoroutine("DelayCapturedAnimation");
    }

    private IEnumerator DelayCapturedAnimation()
    {
        yield return new WaitForSeconds(_CapturedDelayTime);

        float y = 90.0f;

        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector2Int playerlocalposition = _Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition();

        // �v���C���[���G�l�~�[�̉��̃p�l���ɂ���Ƃ��������
        if (_EnemyLocalPosition.y > playerlocalposition.y)
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        // �v���C���[���G�l�~�[�̏�̃p�l���ɂ���Ƃ���������
        if (_EnemyLocalPosition.y < playerlocalposition.y)
            this.transform.rotation = Quaternion.Euler(0.0f, y * 2, 0.0f);

        // �E�Ƀv���C���[�������獶������
        if (_EnemyLocalPosition.x < playerlocalposition.x)
            this.transform.rotation = Quaternion.Euler(0.0f, y * -1, 0.0f);

        // ���Ƀv���C���[��������E������
        if (_EnemyLocalPosition.x > playerlocalposition.x)
            this.transform.rotation = Quaternion.Euler(0.0f, y, 0.0f);


        _EnemyAnimation.SetBool("Captured", true);
    }

    public void SetIsFront(bool isfront) { _IsFront = isfront; }
    public bool GetIsFront() { return _IsFront; }
    public void SetStartEnemyTurn(bool enemyturn) { _StartEnemyTurn = enemyturn; }           // �G�l�~�[�^�[���ɕς�����Ƃ��Ƀ^�[���}�l�[�W���[��true�ɂ��Ăق���
    public void SetLocalPosition(Vector2Int position) { _EnemyLocalPosition = position; }    // �����̂���u���b�N�̍��W���X�V����
    public Vector2Int GetLocalPosition() { return _EnemyLocalPosition; }
    public void SetCheese(GameObject cheese) { _Cheese = cheese; }
    public int GetCheeseSearchRange() { return _CheeseSearchRange; }
}

