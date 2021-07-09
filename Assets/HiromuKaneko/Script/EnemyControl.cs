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
    public struct Panel
    {
        public GameObject PanelObj;
        public Vector2Int Direction;
        public bool Exist;
    };

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

    private EnemyState _EnemyState = EnemyState.IDLE;          // �G�l�~�[�X�e�[�g�ϐ�
    private List<Panel> _MovePanel = new List<Panel>();        // ��������̃u���b�N��ێ�����ϐ�
    private GameObject _Up, _Down, _Left, _Right, _NextBlock;  // �㉺���E�u���b�N�̕ێ��A�i�ސ�̃u���b�N��ێ�
    private Vector2Int _EnemyLocalPosition;      // �l�Y�~�̂���u���b�N�̍��W
    private Vector2Int _EnemyDirection;          // �l�Y�~�̌����Ă����
    private GameObject _GameManager;             // �Q�[���}�l�[�W���[��ێ�
    private GameObject _Player;                  // �v���C���[��ێ�
    private GameObject _Cheese;                  // �`�[�Y��ێ�
    private Animator _EnemyAnimation;            // �l�Y�~�̃A�j���[�V�����̕ێ�
    private Vector3 _StartPoint;                 // �ړ��O�|�W�V����
    private Vector3 _TargetPoint;                // �ړ���|�W�V����
    private float _PosY = 0.075f;                // Y���W�Œ�p
    private float _PassedTime;                   // �A�j���[�V�����p�^�C���ϐ�
    private bool _CheeseBite;                    // �`�[�Y��������Ƃ�
    private bool _PlayerBite;                    // �v���C���[��������Ƃ�
    private bool _IsExist;                       // �����m�F�p
    private bool _IsFront;                       // �\���ǂ����ɂ��邩
    private bool _IsMovePanel = false;           // ������p�l�������邩�ǂ����𔻒�
    private bool _IsTwoMax = false;

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
        _PassedTime = 0.0f;


    }

    // Update is called once per frame
    void Update()
    {
        if (_IsFront)
            TargetMaterial.SetTexture("_MainTex", _FrontTexture);
        else
            TargetMaterial.SetTexture("_MainTex", _BackTexture);

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
                if (_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() != _IsFront)
                {
                    if (_Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition() == _EnemyLocalPosition)
                    {
                        StartCoroutine("PlayerDown");
                    }
                }
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

    // �ҋ@�֐�
    void Idle()
    {
        _NextBlock = null;
        _MovePanel.Clear();
        _IsMovePanel = false;
        _IsTwoMax = false;
    }

    // �ړ�������������Ȃ��ҋ@���[�V�����݂̂Ń^�[���I��
    void Stay()
    {
        _EnemyAnimation.SetBool("Panic", true);
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

        // �X�e�[�g��IDLE�Ɉڍs����
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
                Wait();

                MoveTest();

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
                    default:
                        break;
                }
            }
            else
            {

                if (_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() != _IsFront)
                {
                    if (_Player.gameObject.GetComponent<PlayerControl>().GetLocalPosition() == _EnemyLocalPosition)
                    {
                        StartCoroutine("PlayerDown");
                    }
                }
                if (!_Player.gameObject.GetComponent<PlayerControl>().GetIsFront() && !_IsFront)
                {
                    PlayerKill();

                }
            }
        }
    }

    // �o�H�T���@�ߊ��
    public void FarRouteSearch(GameObject Panel, Vector2Int Direction)
    {
        Panel _Panel = new Panel() { PanelObj = null, Direction = new Vector2Int(0, 0), Exist = false };

        // �����̃Q�[���I�u�W�F�N�g��null�̏ꍇnull�œ����
        if (Panel == null)
        {
            _MovePanel.Add(_Panel);
            return;
        }

        // �v���C���[�̂���|�W�V�������擾
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        // �v���C���[�ƃG�l�~�[�̋������擾
        float player_enemy_distance = Vector3.Distance(playerpos, this.transform.position);

        // �㉺���E�̃p�l���Ə�L���r���āA�Z���̂���������
        float tmp = Vector3.Distance(playerpos, Panel.transform.position);

        _Panel = new Panel() { PanelObj = Panel, Direction = Direction, Exist = true };

        // �v���C���[�ƃG�l�~�[�̋�������������΃��X�g�ŕێ�����H
        if (player_enemy_distance < tmp)
        {
            _IsMovePanel = true;
            _MovePanel.Add(_Panel);

        }
        else
        {
            _Panel.Exist = false;
            _MovePanel.Add(_Panel);
        }

    }

    // �o�H�T���@�����
    public void NearRouteSearch(GameObject Panel, Vector2Int Direction)
    {
        Panel _Panel = new Panel() { PanelObj = null, Direction = new Vector2Int(0, 0), Exist = false };

        // �����̃Q�[���I�u�W�F�N�g��null�̏ꍇnull�œ����
        if (Panel == null)
        {
            _MovePanel.Add(_Panel);
            return;
        }

        // �v���C���[�̂���|�W�V�������擾
        _Player = _GameManager.gameObject.GetComponent<GameManagerScript>().GetPlayer();
        Vector3 playerpos = _Player.transform.position;

        // �v���C���[�ƃG�l�~�[�̋������擾
        float player_enemy_distance = Vector3.Distance(playerpos, this.transform.position);

        // �㉺���E�̃p�l���Ə�L���r���āA�Z���̂���������
        float tmp = Vector3.Distance(playerpos, Panel.transform.position);

        _Panel = new Panel() { PanelObj = Panel, Direction = Direction, Exist = true };

        // �v���C���[�ƃG�l�~�[�̋��������߂���΃��X�g�ŕێ�����H
        if (player_enemy_distance > tmp)
        {
            _IsMovePanel = true;
            _MovePanel.Add(_Panel);

        }
        else
        {
            _Panel.Exist = false;
            _MovePanel.Add(_Panel);
        }

    }
    // �ǂ̐��𐔂��ĕǖ��x�����^�[��
    public float WallCount(Panel moveobj)
    {

        // �ړ���̃p�l�����Ȃ��@�ړ���ɕǂ�����ꍇ0.0f��Ԃ�
        if (moveobj.PanelObj == null || !moveobj.PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, moveobj.Direction) || moveobj.Exist == false)
            return 0.0f;

        Vector2Int Dir = new Vector2Int();

        // �Ǒ��������p�l�������郊�X�g
        List<Panel> WallCountPanel = new List<Panel>();

        // �ォ���Ȃ獶�E�̂w�ɂP������  ����ȊO�͂x�ɂP������
        if (moveobj.Direction == new Vector2Int(0, 1) || moveobj.Direction == new Vector2Int(0, -1))
            Dir = new Vector2Int(1, 0);
        else
            Dir = new Vector2Int(0, 1);

        // �G�l�~�[�ɗאڂ��Ă���p�l����Ǒ��胊�X�g�ɓ����
        WallCountPanel.Add(moveobj);

        Panel obj = new Panel();
        Panel MinusDirPanel = new Panel();
        Panel PlusDirPanel = new Panel();

        // �㉺�Ȃ獶�@���E�Ȃ牺�̃p�l�����擾
        MinusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + (-Dir));
        // �㉺�Ȃ�E�@���E�Ȃ��̃p�l�����擾
        PlusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + Dir);
        // ������̉��̃p�l�����擾
        obj.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + moveobj.Direction);

        // null����Ȃ������炻�̃p�l�������X�g�ɓ����
        if (MinusDirPanel.PanelObj != null)
            WallCountPanel.Add(MinusDirPanel);
        if (PlusDirPanel.PanelObj != null)
            WallCountPanel.Add(PlusDirPanel);
        if (obj.PanelObj != null)
            WallCountPanel.Add(obj);

        // �㉺�Ȃ獶���@���E�Ȃ牺���̃p�l�����擾
        MinusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + (-Dir + moveobj.Direction));
        // �㉺�Ȃ�E���@���E�Ȃ�㉜�̃p�l�����擾
        PlusDirPanel.PanelObj = _GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(moveobj.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + (Dir + moveobj.Direction));

        // null����Ȃ������炻�̃p�l�������X�g�ɓ����
        if (MinusDirPanel.PanelObj != null)
            WallCountPanel.Add(MinusDirPanel);
        if (PlusDirPanel.PanelObj != null)
            WallCountPanel.Add(PlusDirPanel);

        int _WallCount = 0;

        // �ǂ̐��𐔂���
        foreach (Panel panel in WallCountPanel)
        {
            // ���̃p�l���ɃI�u�W�F�N�g�̕ǂ����邩�𒲂ׂ�B����΃J�E���g�𑝂₷
            _WallCount += CountWall(panel.PanelObj);

            // �O�ǂ𐔂���@���̃I�u�W�F�N�g�̑O�㍶�E���݂Ăm�t�k�k�Ȃ�O�ǂƂ��ăJ�E���g����
            // �ォ��㉺���E
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(0, 1)) == null)
                _WallCount++;
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(0, -1)) == null)
                _WallCount++;
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(-1, 0)) == null)
                _WallCount++;
            if (_GameManager.gameObject.GetComponent<GameManagerScript>().GetBlock(panel.PanelObj.GetComponent<BlockConfig>().GetBlockLocalPosition() + new Vector2Int(1, 0)) == null)
                _WallCount++;
        }

        // �ǖ��x���v�Z���ă��^�[��
        //          ��   /     �p�l��
        float retDinsity = (float)_WallCount / WallCountPanel.Count;
        //Debug.Log(_WallCount);
        //Debug.Log(WallCountPanel.Count);
        Debug.Log(retDinsity);

        return retDinsity;
    }

    public void MoveTest()
    {

        // �`�[�Y�݂��Ă�
        if (_Cheese != null)
        {
            CheeseMove();
        }
        // �`�[�Y�����ĂȂ�
        else
        {
            if (_IsFront == true)
            {
                // �o�H�T���@���H�^�E�p�Ɂ@�E������������̏��ŒT��
                FarRouteSearch(_Right, new Vector2Int(1, 0));
                FarRouteSearch(_Down, new Vector2Int(0, -1));
                FarRouteSearch(_Left, new Vector2Int(-1, 0));
                FarRouteSearch(_Up, new Vector2Int(0, 1));

                float[] WallDensity = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

                // �����p�l�����Ȃ����ɋ߂Â��p�l���Ɉړ�����p��Exist��true�ɂ��Ă��邾�� 
                if (_IsMovePanel == false)
                {
                    for (int i = 0; i < _MovePanel.Count; i++)
                    {
                        Panel _Panel = new Panel() { PanelObj = _MovePanel[i].PanelObj, Direction = _MovePanel[i].Direction, Exist = true };
                        _MovePanel[i] = _Panel;
                    }
                }

                // �Ǒ���
                for (int i = 0; i < WallDensity.Length; i++)
                {
                    WallDensity[i] = WallCount(_MovePanel[i]);
                }

                int MaxElement = 0;
                float Max = -1.0f;

                // �z��̒��ň�ԕǖ��x�̍����p�l���𒊏o
                for (int i = 0; i < WallDensity.Length; i++)
                {
                    if (Max < WallDensity[i])
                    {
                        MaxElement = i;
                        Max = WallDensity[i];

                    }
                    else if (Max == WallDensity[i])
                    {
                        _IsTwoMax = true;
                    }

                }

                // �ǖ��x�̍ő�l����ȏ゠��Ƃ��̏���
                if (_IsTwoMax == true)
                {
                    if (Max == WallDensity[0])
                    {
                        if(_MovePanel[0].PanelObj != null)
                        {
                            if (_MovePanel[0].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[0].Direction))
                            {
                                _NextBlock = _MovePanel[0].PanelObj;
                                _EnemyDirection = _MovePanel[0].Direction;
                            }
                        }

                    }

                    if (_NextBlock == null)
                    {
                        if (Max == WallDensity[1])
                        {
                            if (_MovePanel[1].PanelObj != null)
                            {
                                if (_MovePanel[1].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[1].Direction))
                                {
                                    _NextBlock = _MovePanel[1].PanelObj;
                                    _EnemyDirection = _MovePanel[1].Direction;
                                }
                            }
                        }
                    }

                    if (_NextBlock == null)
                    {
                        if (Max == WallDensity[2])
                        {
                            if (_MovePanel[2].PanelObj != null)
                            {
                                if (_MovePanel[2].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[2].Direction))
                                {
                                    _NextBlock = _MovePanel[2].PanelObj;
                                    _EnemyDirection = _MovePanel[2].Direction;
                                }
                            }
                        }
                    }

                    if (_NextBlock == null)
                    {
                        if (Max == WallDensity[3])
                        {
                            if (_MovePanel[3].PanelObj != null)
                            {
                                if (_MovePanel[3].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[3].Direction))
                                {

                                    _NextBlock = _MovePanel[3].PanelObj;
                                    _EnemyDirection = _MovePanel[3].Direction;
                                }
                            }
                        }
                    }

                }
                else
                {
                    if (_MovePanel[MaxElement].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[MaxElement].Direction))
                    {
                        _NextBlock = _MovePanel[MaxElement].PanelObj;
                        _EnemyDirection = _MovePanel[MaxElement].Direction;
                    }

                }

            }
            // ����
            else
            {
                // �o�H�T��
                NearRouteSearch(_Right, new Vector2Int(1, 0));
                NearRouteSearch(_Down, new Vector2Int(0, -1));
                NearRouteSearch(_Left, new Vector2Int(-1, 0));
                NearRouteSearch(_Up, new Vector2Int(0, 1));

                // �����p�l�����Ȃ����ɋ߂Â��p�l���Ɉړ�����p��Exist��true�ɂ��Ă��邾�� 
                if (_IsMovePanel == false)
                {
                    for (int i = 0; i < _MovePanel.Count; i++)
                    {
                        Panel _Panel = new Panel() { PanelObj = _MovePanel[i].PanelObj, Direction = _MovePanel[i].Direction, Exist = true };
                        _MovePanel[i] = _Panel;
                    }
                }

                // ���H�^�E�Ńv���C���[�ɋ߂Â�
                if (_NextBlock == null)
                {
                    // �E��T��
                    if (_MovePanel[0].Exist == true)
                    {
                        if (_MovePanel[0].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[0].Direction))
                        {
                            _NextBlock = _MovePanel[0].PanelObj;
                            _EnemyDirection = _MovePanel[0].Direction;
                        }
                    }
                }

                if (_NextBlock == null)
                {
                    // ����T��
                    if (_MovePanel[1].Exist == true)
                    {
                        if (_MovePanel[1].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[1].Direction))
                        {
                            _NextBlock = _MovePanel[1].PanelObj;
                            _EnemyDirection = _MovePanel[1].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // ����T��
                    if (_MovePanel[2].Exist == true)
                    {
                        if (_MovePanel[2].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[2].Direction))
                        {
                            _NextBlock = _MovePanel[2].PanelObj;
                            _EnemyDirection = _MovePanel[2].Direction;
                        }
                    }
                }
                if (_NextBlock == null)
                {
                    // ���T��
                    if (_MovePanel[3].Exist == true)
                    {
                        if (_MovePanel[3].PanelObj.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _MovePanel[3].Direction))
                        {
                            _NextBlock = _MovePanel[3].PanelObj;
                            _EnemyDirection = _MovePanel[3].Direction;
                        }
                    }
                }

            }


            if (_NextBlock == true)
                _EnemyState = EnemyState.MOVE;
            else
                _EnemyState = EnemyState.STAY;
        }
    }

    // �p�l���ɕǂ����邩�𒲂ׂ�
    public int CountWall(GameObject block)
    {
        GameManagerScript gmScript = _GameManager.GetComponent<GameManagerScript>();
        int count = 0;
        for (int i = 0; i < block.transform.GetChild(0).transform.childCount; ++i)
        {
            // �G�l�~�[�ƈ�v�����玟�̎q�I�u�W�F�N�g�Ɉڂ�
            List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
            bool isThrow = false;
            foreach (GameObject enemy in enemys)
                if (block.transform.GetChild(0).transform.GetChild(i).gameObject == enemy)
                {
                    isThrow = true;
                    break;
                }
            if (isThrow)
                continue;

            // �v���C���[�ł��Ȃ��M�~�b�N�̃`�F�b�N�G���^�[���ʂ�����
            if (gmScript.GetPlayer() != block.transform.GetChild(0).transform.GetChild(i).gameObject &&
                block.transform.GetChild(0).transform.GetChild(i).GetComponent<GimmicControl>().IsWall())
                count++;
        }
        return count;
    }

    // �`�[�Y���݂��Ă�����`�[�Y�̕��֌�����
    private void CheeseMove()
    {


        GameObject moveobj = null;
        Vector2Int movedirection = new Vector2Int();

        float distance = 10000.0f;
        float tmp = 0.0f;
        float tmp2 = 0.0f;
        float random;

        if (_Up != null)
        {
            _EnemyDirection = new Vector2Int(0, 1);
            tmp = Vector3.Distance(_Cheese.transform.position, _Up.transform.position);
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
            else if (tmp < distance)
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
            tmp = Vector3.Distance(_Cheese.transform.position, _Down.transform.position);
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
            else if (tmp < distance)
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
            tmp = Vector3.Distance(_Cheese.transform.position, _Left.transform.position);
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
            else if (tmp < distance)
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
            tmp = Vector3.Distance(_Cheese.transform.position, _Right.transform.position);
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
            else if (tmp < distance)
            {
                if (_Right.gameObject.GetComponent<BlockConfig>().CheckPanelMove(_IsFront, _EnemyLocalPosition, _EnemyDirection))
                {
                    movedirection = _EnemyDirection;
                    moveobj = _Right;
                    distance = tmp;
                }
            }
        }

        // �����̏����ԈႦ�ď����Ă��܂������炠�ƂŊm�F���K�v
        // �`�[�Y�ƃl�Y�~�̋������擾
        tmp2 = Vector3.Distance(_Cheese.transform.position, this.transform.position);
        if (moveobj != null)
        {
            // �`�[�Y�ƃ��[�u��̃p�l���̋������擾
            tmp = Vector3.Distance(_Cheese.transform.position, moveobj.transform.position);

            if (tmp2 < tmp)
            {
                _EnemyDirection = movedirection;
                _NextBlock = moveobj;
                _EnemyState = EnemyState.MOVE;
            }

        }
        else
            _EnemyState = EnemyState.STAY;


    }

    public void PlayerKill()
    {
        _EnemyAnimation.SetTrigger("Attack");
        _Player.gameObject.GetComponent<PlayerControl>().SetIsExist(false);
    }

    // �G�l�~�[���s�������E����������։�]������
    public void Rotate()
    {

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

    // �v���C���[�𗠂���U������H
    private IEnumerator PlayerDown()
    {
        this.GetComponent<GameOverEnemy>().StartGameOverEnemyAnimation();

        yield return new WaitForSeconds(2.0f);

        _Player.gameObject.GetComponent<PlayerControl>().SetIsExist(false);

        yield return null;
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
    public void SetLocalPosition(Vector2Int position) { _EnemyLocalPosition = position; }    // �����̂���u���b�N�̍��W���X�V����
    public Vector2Int GetLocalPosition() { return _EnemyLocalPosition; }
    public void SetCheese(GameObject cheese) { _Cheese = cheese; }
    public int GetCheeseSearchRange() { return _CheeseSearchRange; }
}
