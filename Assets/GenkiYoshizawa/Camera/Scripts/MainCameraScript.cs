using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MainCameraScript : MonoBehaviour
{

    [Header("���͂̃f�b�h�]�[��")]
    [SerializeField, Range(0.0f, 1.0f)] float _InputDeadZone = 0.3f;

    [Header("�v���C���[�Ƃ̗���鋗���i�ݒ肵���l���ŏ��̋����ɂȂ�j")]
    [SerializeField] private float _ToPlayerOffset = 1.0f;
    [Header("�v���C���[�Ƃ̗���鋗���̍ő�l")]
    [SerializeField] private float _ToPlayerOffsetMax = 10.0f;
    [Header("�v���C���[�Ƃ̗���鋗���̍ŏ��l")]
    [SerializeField] private float _ToPlayerOffsetMin = 0.5f;
    [Header("�������̃J�����ړ��ő呬�x")]
    [SerializeField] private float _CameraVelocityHorizontal = 0.5f;
    [Header("�O������̃J�����ړ��ő呬�x")]
    [SerializeField] private float _CameraVelocityVertical = 0.5f;

    private GameObject _GameManager = null;
    private GameObject _TopViewObject = null;
    private GameObject _PlayerObject = null;

    // �\��
    private bool _isFront = true;
    // �g�b�v�r���[��
    private bool _isTop = false;

    // �J�����̈ړ������̔���Ɏg�����E�㉺�̃p�l���̌��E�ʒu
    private float _UpLimitPanelPosition;
    private float _DownLimitPanelPosition;
    private float _RightLimitPanelPosition;
    private float _LeftLimitPanelPosition;

    // �Y�[���̌��ݑ��x
    private float _ZoomVelocity = 0.0f;

    // �J�����̌��ݒ����_
    private Vector3 _CameraLookAt;

    // ���ʃx�N�g���̕ۑ�(�ҏW���ɂ��g��)
    private Vector3 _SaveForward;

    [Header("�g�b�v�r���[�̃J�������_(�ҏW�p��bool)")]
    [SerializeField] private bool _EditTopView;
    private bool _CurEditTopView;
    // �ҏW�p�v���C���[�Ƃ̋�����ۑ�����
    private float _EditSaveOffset;

    // Start is called before the first frame update
    void Start()
    {
        // ExecuteAlways���g���Ă��邽�߁A�Q�[�������Ŕ��ʂ��s��
        if (!Application.IsPlaying(gameObject))
        {
            // �G�f�B�^(�ҏW)���̏���
            return;
        }

        _GameManager = GameObject.FindGameObjectWithTag("Manager");

        _PlayerObject = _GameManager.GetComponent<GameManagerScript>().GetPlayer();

        // �r���[�|�C���g�����������ɑΉ����₷�����邽�߂�for��
        for (int i = 0; i < transform.parent.childCount; ++i)
        {
            if (!transform.parent.GetChild(i).CompareTag("MainCamera"))
            {
                _TopViewObject = transform.parent.GetChild(i).gameObject;
            }
        }

        _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();

        GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();

        Vector2 minPos = new Vector2(0.0f, 0.0f);
        Vector2 maxPos = new Vector2(0.0f, 0.0f);

        bool breakFlg = false;

        foreach (GameObject[] block in blocks)
        {
            foreach (GameObject b in block)
            {
                if (b == null)
                    continue;
                minPos = new Vector2(b.transform.position.x, b.transform.position.z);
                maxPos = new Vector2(b.transform.position.x, b.transform.position.z);
                breakFlg = true;
                break;
            }
            if (breakFlg)
                break;
        }

        foreach (GameObject[] block in blocks)
        {
            foreach (GameObject b in block)
            {
                if (b == null)
                    continue;

                if (minPos.x > b.transform.position.x)
                    minPos.x = b.transform.position.x;
                if (minPos.y > b.transform.position.z)
                    minPos.y = b.transform.position.z;

                if (maxPos.x < b.transform.position.x)
                    maxPos.x = b.transform.position.x;
                if (maxPos.y < b.transform.position.z)
                    maxPos.y = b.transform.position.z;
            }
        }
        _UpLimitPanelPosition = maxPos.y;
        _DownLimitPanelPosition = minPos.y;
        _RightLimitPanelPosition = maxPos.x;
        _LeftLimitPanelPosition = minPos.x;

        _ZoomVelocity = 0.0f;
        _CameraLookAt = _PlayerObject.transform.position;

        // �{���̓g�b�v�r���[����̃A�j���[�V�������s��
        // ���ʃx�N�g�������O�ɐݒ肵�Ă��Ȃ��Ƃ����Ȃ��̂͂��傢����
        //_CameraObject.transform.LookAt(_CameraLookAt);
        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;

    }

    // Update is called once per frame
    void Update()
    {
        // ExecuteAlways���g���Ă��邽�߁A�Q�[�������Ŕ��ʂ��s��
        if (!Application.IsPlaying(gameObject))
        {
            // �G�f�B�^(�ҏW)���̏���
            if (_CurEditTopView && !_EditTopView)
            {
                transform.forward = _SaveForward;
                if (CheckPlayerObjectWhenEdit())
                {
                    transform.position = _PlayerObject.transform.position - transform.forward * _EditSaveOffset;
                    _ToPlayerOffset = _EditSaveOffset;
                }
            }

            if (CheckPlayerObjectWhenEdit())
            {
                transform.LookAt(_PlayerObject.transform.position, new Vector3(0.0f, 1.0f, 0.0f));

                _ToPlayerOffset = Vector3.Distance(_PlayerObject.transform.position, transform.position);
            }

            if (!_CurEditTopView && _EditTopView)
            {
                _SaveForward = transform.forward;
                _EditSaveOffset = _ToPlayerOffset;
            }

            _CurEditTopView = _EditTopView;

            if (!_EditTopView)
                return;

            // �r���[�|�C���g�����������ɑΉ����₷�����邽�߂�for��
            for (int i = 0; i < transform.parent.childCount; ++i)
            {
                if (!transform.parent.GetChild(i).CompareTag("MainCamera"))
                {
                    _TopViewObject = transform.parent.GetChild(i).gameObject;
                }
            }

            if (_TopViewObject == null)
                return;
            transform.position = _TopViewObject.transform.position;
            transform.rotation = _TopViewObject.transform.transform.rotation;

            return;
        }

        float trigger = Input.GetAxis("Controller_L_R_Trigger");
        float rightStickVertical = Input.GetAxis("Controller_R_Stick_Vertical");
        float rightStickHorizontal = Input.GetAxis("Controller_R_Stick_Horizontal");

        if (Input.GetButtonDown("Controller_Y"))
        {
            ResetCamera();
        }

        if (Input.GetButtonDown("Controller_RB"))
        {
            ExchangeTopToFollowPlayer();
        }

        if (Input.GetButtonDown("Controller_LB"))
        {
            ReturnCamera();
        }

        if (_isTop)
            return;
        // ����ȍ~�̏����̓g�b�v�r���[�̎��s��Ȃ�

        if (trigger < -_InputDeadZone)
            ZoomInOut(/*_isZoomIn = */ false);
        else if (trigger > _InputDeadZone)
            ZoomInOut(/*_isZoomIn = */ true);
        else
            _ZoomVelocity = 0.0f;

        if ((rightStickVertical < -_InputDeadZone || rightStickVertical > _InputDeadZone) || (rightStickHorizontal < -_InputDeadZone || rightStickHorizontal > _InputDeadZone))
        {
            FreeCamera(new Vector2(rightStickHorizontal, rightStickVertical));
            Debug.Log(new Vector2(rightStickHorizontal, rightStickVertical));
        }
    }


    private void FollowPlayer() { }

    private void FreeCamera(Vector2 stick)
    {
        // �g�b�v�r���[�Ȃ瓮�����Ȃ�
        if (_isTop)
            return;

        // �J�����̒����_���ړ�������
        _CameraLookAt += new Vector3(-stick.x * _CameraVelocityHorizontal, 0.0f, stick.y * _CameraVelocityVertical * (_isFront ? 1 : -1));

        //�ʒu�̕␳
        // �p�l���̈ʒu�����ׂ�y=0�O��
        // �J�����̍��W���王�������ɐ��������Ay=0��x,z���W�����߂�
        float t = /*_CameraLookAt.y - 0.0f*/_CameraLookAt.y / transform.forward.y; //�}��ϐ�
        Vector3 hitPanelPos = _CameraLookAt + transform.forward * t;
        if (hitPanelPos.x < _LeftLimitPanelPosition)
        {
            hitPanelPos.x = _LeftLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }
        else if (hitPanelPos.x > _RightLimitPanelPosition)
        {
            hitPanelPos.x = _RightLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }

        if (hitPanelPos.z < _DownLimitPanelPosition)
        {
            hitPanelPos.z = _DownLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }
        else if (hitPanelPos.z > _UpLimitPanelPosition)
        {
            hitPanelPos.z = _UpLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }

        // �J�����̒����_����J�����̃|�W�V���������߂�
        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
    }


    // �Y�[���C���E�A�E�g�̊֐�
    private void ZoomInOut(bool isZoomIn)
    {
        // �g�b�v�r���[�Ȃ瓮�����Ȃ�
        if (_isTop)
            return;

        // �����x�̒l�ݒ�
        float maxDistance = _ToPlayerOffsetMax - _ToPlayerOffsetMin;
        float accel = (2 * maxDistance) / (90.0f * 90.0f);// 90�t���[��

        if ((isZoomIn && _ZoomVelocity < 0.0f) || (!isZoomIn && _ZoomVelocity > 0.0f))
        {
            _ZoomVelocity = 0.0f;
            return;
        }

        _ZoomVelocity = _ZoomVelocity + accel * (isZoomIn ? 1.0f : -1.0f);
        _ToPlayerOffset += _ZoomVelocity;


        // �l�̕␳
        if (_ToPlayerOffset > _ToPlayerOffsetMax)
        {
            _ToPlayerOffset = _ToPlayerOffsetMax;
            _ZoomVelocity = 0.0f;
        }

        if (_ToPlayerOffset < _ToPlayerOffsetMin)
        {
            _ToPlayerOffset = _ToPlayerOffsetMin;
            _ZoomVelocity = 0.0f;
        }

        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
    }

    private void ExchangeTopToFollowPlayer()
    {
        // �ʏ�J�������g�b�v�r���[
        if (!_isTop)
        {
            _SaveForward = transform.forward;
            transform.rotation = _TopViewObject.transform.transform.rotation;

            if (_isFront)
                transform.position = _TopViewObject.transform.position;
            else
            {
                // ���ʂ̏ꍇ��TopView��xz���ʂɋ��ʔ��˂��Az����]
                transform.position = new Vector3(_TopViewObject.transform.position.x, -_TopViewObject.transform.position.y, -_TopViewObject.transform.position.z);
                Vector3 newForward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);
                transform.LookAt(transform.position + newForward, -Vector3.up);

            }
        }
        else
        {
            // �g�b�v�r���[���ʏ�J����
            _CameraLookAt = _PlayerObject.transform.position;
            // �\����y�̈ʒu��ς���
            _CameraLookAt.y = _CameraLookAt.y * (_isFront ? 1 : -1);

            transform.position = _CameraLookAt - _SaveForward * _ToPlayerOffset;
            transform.LookAt(_CameraLookAt, Vector3.up * (_isFront ? 1 : -1));
        }
        _isTop = !_isTop;
    }

    private void ReturnCamera()
    {
        _CameraLookAt = new Vector3(_CameraLookAt.x, -_CameraLookAt.y, _CameraLookAt.z);
        Vector3 newForward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);


        if (_isTop)
        {
            // �g�b�v�r���[
            transform.position = new Vector3(transform.position.x, -transform.position.y, -transform.position.z);
            _SaveForward = new Vector3(_SaveForward.x, -_SaveForward.y, -_SaveForward.z);
        }
        else
        {
            // ��g�b�v�r���[
            transform.position = _CameraLookAt - newForward * _ToPlayerOffset;
        }

        _isFront = !_isFront;
        // �ݒ�ɂ���Ă�(�g�b�v�r���[��)���]�����������ō��E���]����̂łȂɂ��N�����炱��������
        transform.LookAt(transform.position + newForward, Vector3.up * (_isFront ? 1.0f : -1.0f));
    }

    private void ResetCamera()
    {
        if (_isTop)
        {
            _CameraLookAt = _PlayerObject.transform.position;

            if (_isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {
                _SaveForward = new Vector3(_SaveForward.x, -_SaveForward.y, -_SaveForward.z);
                _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();
            }

            transform.position = _CameraLookAt - _SaveForward * _ToPlayerOffset;
            transform.LookAt(_CameraLookAt);
            _isTop = false;
        }
        else
        {

            // ����ʒu���v���C���[�I�u�W�F�N�g�̈ʒu�ɂ���
            // �I�t�Z�b�g����ʒu�����߂�
            // �J������LookAt�Ɍ�������
            _CameraLookAt = _PlayerObject.transform.position;

            if (_isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {
                transform.forward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);
                _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();
            }
            transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
            //transform.LookAt(_CameraLookAt);
        }
    }

    private bool CheckIsDisplayPlayer()
    {
        return true;
    }


    private void SetCameraIsFront(bool front) { _isFront = front; }
    private bool GetIsFront() { return _isFront; }

    // ���̃X�N���v�g�����̒l���ύX���ꂽ���Ɏ����ŌĂяo�����
    private void OnValidate()
    {
        // �v���C���[�Ƃ�Offset���ύX���ꂽ����transform.position�����f����

        if (CheckPlayerObjectWhenEdit())
            transform.position = _PlayerObject.transform.position - transform.forward * _ToPlayerOffset;
    }

    // �ҏW���ɌĂ΂��֐��B�v���C���[�I�u�W�F�N�g���������true��Ԃ�
    private bool CheckPlayerObjectWhenEdit()
    {
        if (_PlayerObject == null)
            _PlayerObject = GameObject.FindGameObjectWithTag("Player");

        if (_PlayerObject != null)
            return true;

        return false;
    }
}
