using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("�g�����������t�@�C�������Ă��������B")]
    [SerializeField] private AudioClip audioClip;

    [Header("�v���C���[�̌��������Ă��������B")]
    [SerializeField]private Vector2Int _Direction;
    private Vector2Int _StartDirection;

    [Header("�����o��")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("��������")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("�s���I����̑ҋ@����")]
    [SerializeField] private float _ActionTime = 0.01f;

    [Header("�N���A���"), SerializeField] private GameObject _ClearScreen;

    [Header("�����ύX���ɕ��s�A�j���[�V�������Đ����邩"), SerializeField] private bool IsWalkAnim;
    private bool NowWalkAnim = false;

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

    //�ӂ�������UI���Ǘ�����
    private FukidasiAnimationUI _FukidasiScript;
    private Vector3 _CorsorStartPosition;

    //�R�}���h�I�����ɏォ�牽�Ԗڂɂ��邩
    private int _CommandSelect = 3;

    //�s���R�}���h�̎�ސ�
    private readonly int _AnimMax = 4;

    //�^�[���}�l�[�W���[
    private TurnManager _TurnManager;

    public void Start()
    {
        //����FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Animator = GetComponent<Animator>();

        _IsExist = true;


        _FukidasiScript = _FukidasiObj.GetComponent<FukidasiAnimationUI>();

        _StartPostion = _LocalPosition;
        _StartDirection = _Direction;
        _IsExist = true;

        //�A�j���[�V�����p�̕ϐ�
        _WalkStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;

        _CorsorStartPosition = _FukidasiObj.transform.GetChild(4).localPosition;

        _TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();
    }

    public void Update()
    {
        // �����A�j���[�V����
        if (_Animator.GetBool("Walk") && NowWalkAnim == false)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                _PassedTime = _WalkTime;
                _Animator.SetBool("Walk", false);
            }

            transform.position = _WalkStartPosition + (_WalkTargetPosition - _WalkStartPosition) * (_PassedTime / _WalkTime);

            if (!_Animator.GetBool("Walk"))
                _PassedTime = 0.0f;
        }


        //�����ύX
        if (_Animator.GetBool("Walk") && NowWalkAnim == true)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _WalkTime)
            {
                //_PassedTime = _WalkTime;

                _Animator.SetBool("Walk", false);
            }

            transform.Rotate(0.0f, 90.0f * 0.01f, 0.0f);

            if (!_Animator.GetBool("Walk"))
            {
                _PassedTime = 0.0f;
                NowWalkAnim = false;
            }
        }

        //���݂̃A�j���[�V���������擾
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        //�N���A�m�F
        if (clipInfo.clip.name == "Clear")
        {
            var clearScreenScript = _ClearScreen.GetComponent<ClearScreen>();
            clearScreenScript.DisplayClearScreen(_TurnManager.GetTurnCount());
        }

        //�Q�[���I�[�o�[�m�F
        if (clipInfo.clip.name == "GameOvered")
        {
            //������gameover���Ăяo��
            //var gameOverScript = _ClearScreen.GetComponent<GameOverScreen>();
            //gameOverScript.DisplayGameOverScreen();
        }

    }

    public void PlayerInit()
    {
        _IsExist = true;
        _LocalPosition = _StartPostion;
        _Direction = _StartDirection;
        _WalkStartPosition  = new Vector3(0.0f, 0.0f, 0.0f);
        _WalkTargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _PassedTime = 0.0f;

        if (_Animator.GetBool("Tired"))
            SetTired(false);
    }

    private void Move(Vector2Int direction)
    {
        //�O�̃u���b�N�擾
        var block = _GameManagerScript.GetBlock(_LocalPosition + direction);

        //�������������߂Ɍ��ݒn�ƖړI�n���擾
        _WalkStartPosition = transform.position;
        _WalkTargetPosition = block.transform.position;

        //Walk�̃A�j���[�V�����J�n
        _Animator.SetBool("Walk", true);

        //�e��؂�ւ���
        transform.parent = block.transform.GetChild(0).transform;

        //�ړ�
        _LocalPosition += _Direction;

        //_GameManagerScript.GetCamera().transform.GetComponent<CameraWork>().PlayerMoveCameraWork(_LocalPosition + direction);
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
        blockScript.TurnOver(_IsFront, _Direction);
        _Animator.SetBool("Action", true);
        SetFrontBlock();
    }

    public bool PlayerTurn()
    {
        bool turnEnd = false;

        //Start�Ŏ擾����̂Ń^�[���J�n���Ɏ蓮�Ŏ擾
        if (_FrontBlock == null)
            SetFrontBlock();

        //�����o���̃A�j���[�V�����I�����m�F�����琶������
        if (_FukidasiScript.GetAnimPattern() == -1)
        {
            _FukidasiScript.SetAnimPattern(_CanActionList.Count);
            if (CheckEnemy(_LocalPosition + _Direction) != null)
            {
                //�O�ɓG�������̂ł���p�̉摜���o��
                _FukidasiScript.SetActPattern(_CanActionList, true);
            }
            else
                _FukidasiScript.SetActPattern(_CanActionList);

            //�J�[�\������ԏ�ɐݒ�
            _CommandSelect = _CanActionList.Count - 1;
            var icon = _FukidasiObj.transform.GetChild(4).GetComponent<RectTransform>();
            icon.anchoredPosition = 
                new Vector3(icon.localPosition.x, _CorsorStartPosition.y + (20.0f * _CommandSelect), _CorsorStartPosition.z);
        }

        //�v���C���[���E��]
        //����낫��낵������ƃR�}���h���������܂܏o�Ă��Ȃ��Ȃ�̂Œ���
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //�����ύX���ɕ��s�A�j���[�V�����Đ�
            if (IsWalkAnim)
            {
                _Animator.SetBool("Walk", true);
                NowWalkAnim = true;
            }
            else
                transform.Rotate(0.0f, 90.0f, 0.0f);

            RotateMySelf(_LocalPosition, _IsFront ? 90.0f : -90.0f);
            
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //�����ύX���ɕ��s�A�j���[�V�����Đ�
            if (IsWalkAnim)
            {
                _Animator.SetBool("Walk", true);
                NowWalkAnim = true;
            }
            else
                transform.Rotate(0.0f, -90.0f, 0.0f);

            RotateMySelf(_LocalPosition, _IsFront ? -90.0f : 90.0f);
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }

        //�㉺���ŃR�}���h�I��
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_CommandSelect < _CanActionList.Count - 1)
            {
                _CommandSelect++;

                //�A�C�R����transform�擾
                var icon = _FukidasiObj.transform.GetChild(4).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y + 20.0f, icon.localPosition.z);

                //������sprite�ύX
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_CommandSelect > 0)
            {
                _CommandSelect--;

                //�A�C�R����transform�擾
                var icon = _FukidasiObj.transform.GetChild(4).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y - 20.0f, icon.localPosition.z);

                //������sprite�ύX
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }

        //Enter�L�[�ōs�� �^�[����i�߂�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var enemy = CheckEnemy(_LocalPosition + _Direction);
            if (enemy != null)
            {
                _Animator.SetTrigger("Capture");
                _GameManagerScript.KillEnemy(enemy);
                var remainEnemy = _GameManagerScript.GetEnemys();

                //�G�����Ȃ��Ȃ������Ƃ��m�F������Q�[�����I��点�ɍs��
                if (remainEnemy.Count <= 0)
                    _Animator.SetBool("Clear", true);
            }
            else
            {
                CommandAction(_CommandSelect);
            }

            //_CommandSelect = 3;   ���݈Ӌ`��������Ȃ����ǈꉞ�c���Ă���
            _FukidasiScript.ResetAnimPattern();
            turnEnd = true;
        }

        return turnEnd;
    }

    public Vector2Int GetLocalPosition() { return _LocalPosition; }
    public bool GetIsFront() { return _IsFront; }
    public bool GetIsExist() { return _IsExist; }


    public void SetLocalPosition(Vector2Int position) { _LocalPosition = position; }
    public void SetIsFront(bool isFront){ _IsFront = isFront; }
    public void SetIsExist(bool isExist)
    {
        if (isExist)
            SetDead();
        _IsExist = isExist;
    }


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

        //���։\�Ȃ�3������
        if (blockScript.CheckPanelSwap(_IsFront))
            _CanActionList.Add(3);

        //���]�\�Ȃ�2������
        if (blockScript.CheckPanelTurnOver(_IsFront))
            _CanActionList.Add(2);

        //�ړ��\�Ȃ�1������
        if (blockScript.CheckPanelMove(_IsFront, _LocalPosition, _Direction))
            _CanActionList.Add(1);

        //��]�\�Ȃ�0������
        if (blockScript.CheckPanelRotate(_IsFront))
            _CanActionList.Add(0);
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
                        break;
                    case 1:
                        PlayerMove();
                        break;
                    case 2:
                        PlayerTurnOver(_FrontBlock);
                        break;
                    case 3:
                        PlayerSwap(_FrontBlock);
                        break;
                }
                break;
            }

            count++;
        }
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
        if(_FrontBlock == null)
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

}

