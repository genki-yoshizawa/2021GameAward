using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("�g�����������t�@�C�������Ă��������B")]
    [SerializeField] private AudioClip[] _AudioClip;

    [Header("�v���C���[�̌��������Ă��������B")]
    [SerializeField]private Vector2Int _Direction;
    private Vector2Int _StartDirection;

    [Header("�����o��")]
    [SerializeField] private GameObject _FukidasiObj;

    [Header("��������")]
    [SerializeField] private float _WalkTime = 1.0f;

    [Header("�����]���ɂ����鎞��")]
    [SerializeField] private float _RotateTime = 0.5f;

    [Header("�N���A���"), SerializeField] private GameObject _ClearScreen;
    [Header("�Q�[���I�[�o�[���"), SerializeField] private GameObject _GameOverScreen;

    private bool _NowWalkAnim = false;

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

    //�L�[���͂ŉE���������̂��H
    private bool _IsRight = false;

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

        _CorsorStartPosition = _FukidasiObj.transform.GetChild(_AnimMax).localPosition;

        _TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();
    }

    public void Update()
    {
        // �����A�j���[�V����
        if (_Animator.GetBool("Walk") && _NowWalkAnim == false)
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
        if (_Animator.GetBool("Walk") && _NowWalkAnim == true)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _RotateTime)
            {
                _Animator.SetBool("Walk", false);
            }

            transform.Rotate(0.0f, 90.0f * (time / _RotateTime) * (_IsRight ? 1.0f : -1.0f), 0.0f);

            if (!_Animator.GetBool("Walk"))
            {
                _PassedTime = 0.0f;
                _NowWalkAnim = false;
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
            var gameOverScript = _GameOverScreen.GetComponent<GameOverScreen>();
            gameOverScript.DisplayGameOverScreen();
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
        _GameManagerScript.GetCamera().transform.GetComponent<CameraWork>().PlayerMoveCameraWork(_LocalPosition + direction);

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
        //�J�����ʒu�̍X�V
        _GameManagerScript.GetCamera().GetComponent<CameraWork>().PlayerSwapCameraWork();

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
        var camera = _GameManagerScript.GetCamera().GetComponent<CameraWork>();
        if (camera != null)
            camera.PlayerTurnCameraWork();
        else
            Debug.Log("���߂�Ȃ���I�I�I�I");

        //�Ђ�����Ԃ�
        _IsFront = !_IsFront;

        //���f���𔽓]
        RotateMySelf(_LocalPosition, 180.0f, axis.x, axis.y, axis.z);
    }

    private void PlayerMove()
    {
        Move(_Direction);
        AudioManager.Instance.PlaySE(_AudioClip[0]);
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
        bool turnEnd = false;

        //�A�j���[�V�����J�ڒ��������瓮���Ȃ�����
        if (_Animator.IsInTransition(0))
            return turnEnd;

        //���݂̃A�j���[�V���������擾
        var clipInfo = _Animator.GetCurrentAnimatorClipInfo(0)[0];

        //�A�j���[�V�������Đ����̓R�}���h������󂯕t���Ȃ�
        if (clipInfo.clip.name == "Walk" || clipInfo.clip.name == "PanelAction" || clipInfo.clip.name == "Capture")
            return turnEnd;

        //����
        if (clipInfo.clip.name == "GameOver")
            return turnEnd;

        //Start�Ŏ擾����̂Ń^�[���J�n���Ɏ蓮�Ŏ擾
        if (_FrontBlock == null)
            SetFrontBlock();

        //�����o���̃A�j���[�V�����I�����m�F�����琶������
        if (_FukidasiScript.GetAnimPattern() == -1 && _IsExist)
        {
            var enemys = _GameManagerScript.GetEnemys();

            if (!(_FrontBlock == null || enemys.Count == 0))
            {
                AudioManager.Instance.PlaySE(_AudioClip[1]);

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
                var icon = _FukidasiObj.transform.GetChild(_AnimMax).GetComponent<RectTransform>();
                icon.anchoredPosition =
                    new Vector3(icon.localPosition.x, _CorsorStartPosition.y + (20.0f * _CommandSelect), _CorsorStartPosition.z);
            }
        }

        //�v���C���[���E��]
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _Animator.SetBool("Walk", true);
            _NowWalkAnim = true;
            _IsRight = true;

            RotateMySelf(_LocalPosition, _IsFront ? 90.0f : -90.0f);
            
            SetFrontBlock();

            _FukidasiScript.ResetAnimPattern();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //�����ύX���ɕ��s�A�j���[�V�����Đ�
            _Animator.SetBool("Walk", true);
            _NowWalkAnim = true;
            _IsRight = false;

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
                var icon = _FukidasiObj.transform.GetChild(_AnimMax).GetComponent<RectTransform>();
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
                var icon = _FukidasiObj.transform.GetChild(_AnimMax).GetComponent<RectTransform>();
                icon.anchoredPosition = new Vector3(icon.localPosition.x, icon.localPosition.y - 20.0f, icon.localPosition.z);

                //������sprite�ύX
                _FukidasiScript.SetActPattern(_CanActionList, false, _CommandSelect + 1);
            }
        }

        //Enter�L�[�ōs�� �^�[����i�߂�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_FrontBlock == null)
                return turnEnd;

            var enemy = CheckEnemy(_LocalPosition + _Direction);
            if (enemy != null)
            {
                AudioManager.Instance.PlaySE(_AudioClip[3]);

                _Animator.SetTrigger("Capture");
                _GameManagerScript.KillEnemy(enemy);
                var remainEnemy = _GameManagerScript.GetEnemys();

                //�G�����Ȃ��Ȃ������Ƃ��m�F������Q�[�����I��点�ɍs��
                if (remainEnemy.Count <= 0)
                {
                    _Animator.SetBool("Clear", true);
                }
            }
            else
            {
                AudioManager.Instance.PlaySE(_AudioClip[2]);

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

