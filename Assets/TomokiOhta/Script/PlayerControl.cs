using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("�g�����������t�@�C�������Ă��������B")]
    [SerializeField] private AudioClip[] _AudioClip;

    [Header("�v���C���[�̌��������Ă��������B")]
    [SerializeField] private Vector2Int _Direction;
    private Vector2Int _StartDirection;

    [Header("��������")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("�����]���ɂ����鎞��")]
    [SerializeField] private float _RotateTime90 = 0.5f;

    [Header("�����]���ɂ����鎞��")]
    [SerializeField] private float _RotateTime180 = 0.5f;

    [Header("�N���A���"), SerializeField] private GameObject _ClearScreen;
    [Header("�Q�[���I�[�o�[���"), SerializeField] private GameObject _GameOverScreen;

    private bool _IsWalk = false;

    //�A�j���[�V�����X�^�[�g����̌o�ߎ���
    private float _PassedTime;

    //�����n�߂̍��W
    private Vector3 _WalkStartPosition;

    //�ړI�n�̍��W
    private Vector3 _WalkTargetPosition;

    //�v���C���[�̃A�j���[�^�[
    private Animator _Animator;

    //�v���C���[�̔z����W
    private Vector2Int _LocalPosition;
    private Vector2Int _StartPostion;

    //���̃I�u�W�F�N�g�ɐG���Ƃ��̒����
    private GameManagerScript _GameManagerScript;

    //�\��
    private bool _IsFront;

    //��������
    private bool _IsExist;

    //�O�̃u���b�N���
    private GameObject _FrontBlock;

    //�ǂ̍s�����\�łǂ̕������i�[���邩���Ǘ�����
    private List<int> _CanActionList = new List<int>();

    //�R�}���h�I���ŏ��I��ł��邩
    private bool _CommandTop = true;  

    //�^�[���}�l�[�W���[
    private TurnManager _TurnManager;

    //�N���A�y��GameOver����
    private bool _IsClear;
    private bool _IsGameOver;

    //�֐��|�C���^�̂悤�Ȃ���
    private System.Action<GameObject> _CommandAction;

    //�R�}���h�p�̃X�N���v�g
    private CommandUI _CommandScript;

    private bool _SelectDirection = false;
    private bool _SelectCommand = false;

    //���ꂩ���������
    private float _TurnAngle = 0.0f;

    //���ʃp�l���Ɉړ��ł��邩
    private bool _CanMove = false;

    public void Start()
    {
        //����FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Animator = GetComponent<Animator>();

        //commandScript�擾
        _CommandScript = GameObject.FindGameObjectWithTag("Command").GetComponent<CommandUI>();


        _StartPostion = _LocalPosition;
        _StartDirection = _Direction;
        _IsExist = true;

        //�A�j���[�V�����p�̕ϐ�
        _WalkStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;


        _TurnManager = _GameManagerScript.GetTurnManager().GetComponent<TurnManager>();

        AudioManager.Instance.PlayGameBGM(_AudioClip[0], _AudioClip[1]);

        _IsClear = false;
        _IsGameOver = false;
    }

    public void Update()
    {
        //���݂̃A�j���[�V���������擾
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        // �����A�j���[�V����
        if (_Animator.GetBool("Walk") && _IsWalk == false)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                _PassedTime = _WalkTime;
                _Animator.SetBool("Walk", false);
            }

            transform.position = _WalkStartPosition + (_WalkTargetPosition - _WalkStartPosition) * (_PassedTime / _WalkTime);

            if (!_Animator.GetBool("Walk"))
            {
                _PassedTime = 0.0f;
                _GameManagerScript.GetCamera().transform.GetComponent<MainCameraScript>().SetIsPlayerMove(false);
            }
        }


        //�����ύX
        if (_Animator.GetBool("Walk") && _IsWalk == true)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _RotateTime90)
            {
                _Animator.SetBool("Walk", false);
            }

            transform.Rotate(0.0f, Mathf.Abs(_TurnAngle) * (time / _RotateTime90) * (_TurnAngle > 0.0f ? 1.0f : -1.0f), 0.0f);

            if (!_Animator.GetBool("Walk"))
            {
                if(_Direction == Vector2Int.up)
                    transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                else if(_Direction == Vector2Int.down)
                    transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                else if(_Direction == Vector2Int.right)
                    transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                else if(_Direction == Vector2Int.left)
                    transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);

                _PassedTime = 0.0f;
                _TurnAngle = 90.0f;
                _IsWalk = false;
            }
        }

        //�N���A�m�F
        if (clipInfo.clip.name == "Clear")
        {
            if (!_IsClear)
            {
                var clearScreenScript = _ClearScreen.GetComponent<ClearScreen>();
                clearScreenScript.DisplayClearScreen(_TurnManager.GetTurnCount());
                _IsClear = true;
            }
        }

        //�Q�[���I�[�o�[�m�F
        if (clipInfo.clip.name == "GameOvered")
        {
            if (!_IsGameOver)
            {
                var gameOverScript = _GameOverScreen.GetComponent<GameOverScreen>();
                gameOverScript.DisplayGameOverScreen();
                _IsGameOver = true;
            }
        }

    }

    private void Move(Vector2Int direction)
    {
        //�O�̃u���b�N�擾
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        var camera = _GameManagerScript.GetCamera();

        if (camera)
            camera.GetComponent<MainCameraScript>().SetIsPlayerMove(true);

        //�������������߂Ɍ��ݒn�ƖړI�n���擾
        _WalkStartPosition = transform.position;
        _WalkTargetPosition = block.transform.position;
        _WalkTargetPosition.y = transform.position.y;

        //Walk�̃A�j���[�V�����J�n
        _Animator.SetBool("Walk", true);

        //�e��؂�ւ���
        transform.parent = block.transform.GetChild(0).transform;

        //�ړ�
        _LocalPosition += _Direction;

    }

    public void RotateMySelf(Vector2Int position, float angle, float axisX = 0.0f, float axisY = 1.0f, float axisZ = 0.0f)
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
        //�J�����ʒu�̍X�V
        var camera = _GameManagerScript.GetCamera();
        if (camera)
            camera.GetComponent<MainCameraScript>().SetIsPlayerSwap();

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

        //�J�����ʒu�̍X�V
        var camera = _GameManagerScript.GetCamera().GetComponent<MainCameraScript>();
        if (camera)
            camera.SetIsPlayerTurnOver();
        else
            Debug.Log("���߂�Ȃ���I�I�I�I");

        //�Ђ�����Ԃ�
        _IsFront = !_IsFront;

        //���f���𔽓]
        RotateMySelf(_LocalPosition, 180.0f, axis.x, axis.y, axis.z);

        AudioManager.Instance.ReverseBGM(_IsFront);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        AudioManager.Instance.PlaySE(_AudioClip[2]);
        SetFrontBlock();
    }

    private void PlayerRotate(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        _Animator.SetTrigger("Action");
        blockScript.Rotate(_IsFront, 90);
        SetFrontBlock();
    }

    private void PlayerSwap(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        _Animator.SetTrigger("Action");
        blockScript.Swap(_IsFront);
        SetFrontBlock();
    }

    private void PlayerTurnOver(GameObject block)
    {
        var blockScript = block.GetComponent<BlockControl>();
        block.GetComponent<BlockConfig>().PanelRemoveAttention(_IsFront);
        blockScript.TurnOver(_IsFront, _Direction);
        _Animator.SetBool("Action", true);
        SetFrontBlock();
    }

    public bool PlayerTurn()
    {
        //�A�j���[�V�����J�ڒ��������瓮���Ȃ�����
        if (_Animator.IsInTransition(0))
            return false;

        //������������ł�����^�[���͗��Ȃ�
        if (!_IsExist)
            return false;


        //���݂̃A�j���[�V���������擾
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        //�A�j���[�V�������Đ����̓R�}���h������󂯕t���Ȃ�
        if (clipInfo.clip.name == "Walk" || clipInfo.clip.name == "PanelAction" || clipInfo.clip.name == "Capture" || clipInfo.clip.name == "GameOver")
            return false;

        //�^�[���J�n���Ɏ蓮�Ŏ擾
        if (_FrontBlock == null)
            SetFrontBlock();

        //�R�}���h�̕`��
        //Under���ݒ肵�Ȃ���΂����Ȃ��̂ŗv�ύX
        //�O�Ƀu���b�N�͂��邪�����ł��Ȃ��Ƃ����`�悵�����Ȃ�
        if (!_CommandScript.IsDraw() && _FrontBlock != null)
        {
            var enemy = CheckEnemy(_LocalPosition + _Direction);

            bool isUnder = (enemy != null || _CommandAction == null);
            
            var blockScript = _FrontBlock.GetComponent<BlockConfig>();

            _CommandScript.SetActPattern(blockScript, enemy, _IsFront, _CanMove);
            _CommandScript.SetUnder(!isUnder);
            _CommandScript.SetDraw(true);
        }


        //���������܂��Ă��Ȃ���Ό��߂�
        if (!_SelectDirection)
        {
            //�v���C���[���E��] WASD�Ō������ς��悤�ɂ���
            if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("Controller_L_Stick_Horizontal") > 0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") > 0.5f)
                SelectDirection(Vector2Int.right);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("Controller_L_Stick_Horizontal") < -0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") < -0.5f)
                SelectDirection(Vector2Int.left);
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("Controller_L_Stick_Vertical") > 0.5f || Input.GetAxis("Controller_D_Pad_Vertical") > 0.5f)
                SelectDirection(Vector2Int.up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("Controller_L_Stick_Vertical") < -0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") < -0.5f)
                SelectDirection(Vector2Int.down);

            //����������ăR�}���h�I�т̍��ڂ֍s��
            if (Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return))
            {
                //�O�Ƀp�l�����Ȃ���΃R�}���h��\�����Ȃ�
                if (!_FrontBlock)
                    return false;

                //�O�̃p�l���������ł��Ȃ��Ȃ�
                if (!_CanMove)
                {
                    if (_CommandAction == null)
                        return false;

                    _CommandTop = false;
                }

                var enemy = CheckEnemy(_LocalPosition + _Direction);

                _CommandScript.ActiveSelectCommand(_CanMove, enemy);
                _SelectDirection = true;
            }
        }
        //���܂��Ă���΃R�}���h����
        else if (!_SelectCommand)
        {
            //�㉺���ŃR�}���h�I��
            if (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("Controller_L_Stick_Vertical") > 0.5f || Input.GetAxis("Controller_D_Pad_Vertical") > 0.5f)
            {
                if (_CanMove && CheckEnemy(_LocalPosition + _Direction) == null)
                {
                    _CommandTop = true;
                    _CommandScript.CommandSelect(_CommandTop);
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("Controller_L_Stick_Vertical") < -0.5f || Input.GetAxis("Controller_D_Pad_Vertical") < -0.5f)
            {
                if (_CommandAction != null && CheckEnemy(_LocalPosition + _Direction) == null)
                {
                    _CommandTop = false;
                    _CommandScript.CommandSelect(_CommandTop);
                }
            }

            //����������ă^�[����i�߂�
            if (Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return))
            {
                var enemy = CheckEnemy(_LocalPosition + _Direction);

                //�O�ɓG��������߂܂���
                if (enemy)
                {
                    CaptureEnemy(enemy);
                    return true;
                }

                //�㕔�Ȃ�ړ��A�����Ȃ�act���s��
                if (_CommandTop)
                    PlayerMove();
                else
                    _CommandAction(_FrontBlock);

                _CommandTop = true;
                _CommandScript.SetDraw(false);
                _SelectDirection = false;
                return true;
            }

            //���������ߒ���
            else if (Input.GetButtonDown("Controller_A") || Input.GetKeyDown(KeyCode.Backspace))
            {
                _SelectDirection = false;

                var enemy = CheckEnemy(_LocalPosition + _Direction);

                _CommandScript.UnactiveCommand(_CommandTop, enemy);
            }
        }

        return false;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }
    public bool GetIsExist() { return _IsExist; }


    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront) { _IsFront = isFront; }
    public void SetIsExist(bool isExist)
    {
        if (!isExist)
            SetDead();
        _IsExist = isExist;
    }


    private bool SetFrontBlock()
    {
        BlockConfig blockScript;

        _CanMove = false;

        //���Ɏ擾���Ă���u���b�N���͍폜���Ă���
        if (_FrontBlock)
        {
            blockScript = _FrontBlock.GetComponent<BlockConfig>();
            blockScript.PanelRemoveAttention(_IsFront);
        }

        //�V�����u���b�N�����擾
        _FrontBlock = _GameManagerScript.GetBlock(_LocalPosition + _Direction);
        if (_FrontBlock == null)
            return false;

        blockScript = _FrontBlock.GetComponent<BlockConfig>();
        blockScript.PanelAttention(_IsFront);

        //�ړ�
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            _CanMove = true;
        //��]
        if (blockScript.CheckPanelRotate(_IsFront))
        {
            _CommandAction = PlayerRotate;
            return true;
        }
        //����
        if (blockScript.CheckPanelSwap(_IsFront))
        {
            _CommandAction = PlayerSwap;
            return true;
        }
        //���]
        if (blockScript.CheckPanelTurnOver(_IsFront))
        {
            _CommandAction = PlayerTurnOver;
            return true;
        }

        //�ǂ���ł��Ȃ����null�����Ă���
        _CommandAction = null;

        return false;
    }

    public void SetTired(bool flag)
    {
        _Animator.SetBool("Tired", flag);
    }

    public void SetDead()
    {
        _Animator.SetTrigger("GameOver");
    }

    private GameObject CheckEnemy(Vector2Int position)
    {
        //�O�ɕǂ����݂��Ă��邩�𒲂ׂ�
        if (_FrontBlock == null)
            return null;

        var frontBlockScript = _FrontBlock.GetComponent<BlockConfig>();

        bool noWall = frontBlockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction);
        if (!noWall)
            return null;

        //���ʂ�enemy�����邩�𒲂ׂ�
        var enemys = _GameManagerScript.GetEnemys();

        EnemyControl enemyScript;
        foreach (var enemy in enemys)
        {
            enemyScript = enemy.GetComponent<EnemyControl>();
            if (position == enemyScript.GetLocalPosition() && _IsFront && enemyScript.GetIsFront())
                return enemy;
        }

        return null;
    }

    public Vector3 GetTargetPosition() { return _WalkTargetPosition; }

    private void SelectDirection(Vector2Int dir)
    {
        //�������������Ă�������牟���������Ɍ�����ς���

        //���̌����Ƃ��ꂩ����������������Ȃ牽�����Ȃ�
        if (dir == _Direction)
            return;

        _CommandScript.SetDraw(false);

        _Animator.SetBool("Walk", true);
        _IsWalk = true;

        //���ς��g���Čv�Z�Ō��������Ɗp�x�����߂�
        _TurnAngle = Vector2.SignedAngle(_Direction, dir) * -1;

        RotateMySelf(_LocalPosition, _IsFront ? _TurnAngle : -_TurnAngle);

        SetFrontBlock();
    }

    private void CaptureEnemy(GameObject enemy)
    {
        //�G�l�~�[�����ʂɂ���Ε߂܂���
        if (enemy != null)
        {
            AudioManager.Instance.PlaySE(_AudioClip[5]);

            _Animator.SetTrigger("Capture");
            _GameManagerScript.KillEnemy(enemy);
            var remainEnemy = _GameManagerScript.GetEnemys();

            //�G�����Ȃ��Ȃ������Ƃ��m�F������Q�[�����I��点�ɍs��
            if (remainEnemy.Count <= 0)
                _Animator.SetBool("Clear", true);
        }
    }

}

