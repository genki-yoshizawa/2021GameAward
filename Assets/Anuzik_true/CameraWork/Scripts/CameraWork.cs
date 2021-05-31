using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraWork : MonoBehaviour
{
    // �ݒ肷��e�J�������W�pGameObject
    [SerializeField] private GameObject[] _TopViewCameraPoint;           // 0:�\���̘��Վ��_���W�@1:�����̘��Վ��_���W
    [SerializeField] private GameObject[] _TurnOverPoint;                     // 0:�\���̃M���M���@1:���傤�ǃX�e�[�W�̐^�񒆁@2:�����̃M���M��

    // �e�J�������[�N�̎��Ԏw��
    [SerializeField] private float _TopViewToPlayerView_MoveTime;         // �u���Վ��_���v���C���[�v�̃J�������[�N�̈ړ�����
    [SerializeField] private float _TopViewToPlayerView_RotateTime;       // �u���Վ��_���v���C���[�v�̃J�������[�N�̉�]����
    [SerializeField] private float _TurnOverCameraWork_MoveTime;        // �u�Ђ�����Ԃ��v�̃J�������[�N�̈ړ�����
    [SerializeField] private float _TurnOverCameraWork_RotateTime;      // �u�Ђ�����Ԃ��v�̃J�������[�N�̉�]����


    // �Q�[���}�l�[�W���[�擾
    private GameManagerScript _GameManagerScript;

    // �v���C���[�n
    private GameObject _PlayerObject;                                               // �v���C���[�I�u�W�F�N�g
    private Transform _PlayerOldTransform;                                        // �v���C���[�̑O�g�����X�t�H�[��
    private Transform _PlayerCurTransform;                                        // �v���C���[�̓���������̃g�����X�t�H�[��
    [SerializeField] private Vector3 _PlayerViewPosOffset;                   // �v���C���[�Ǐ]�J�����ŗ���鋗��(x, y, z)
    [SerializeField] private Vector3 _PlayerViewRotOffset;                   // �v���C���[�Ǐ]�J�����̌���(x, y, z)
    [SerializeField] private Vector3 _rPlayerViewRotOffset;                  // ���ʂ̃v���C���[�Ǐ]�J�����̌���(x, y, z)
    private Vector3 rPVposOffset;                                                       // ���ʗp�v���C���[�I�t�Z�b�g

    // TopView�n
    private List<Transform> _TopViewTransform = new List<Transform>();                                        // ���Վ��_�̃g�����X�t�H�[���A0:�\���̘��Վ��_���W�@1:�����̘��Վ��_���W

    // �J�������]�p
    private List<Transform> _TurnOverTransform = new List<Transform>();                                       // ���]�p�̃g�����X�t�H�[���A0:�\���̃M���M���@1:���傤�ǃX�e�[�W�̐^�񒆁@2:�����̃M���M��

    private float CameraRotateOffset;                                                                           // ���x���f�U�C�����~�X������
    private float CameraRotateOffset_z;                                                                           // �u���]�v�p

    // �J�������\����t���O
    [SerializeField] private bool _IsFront;                                                                    // �J���������\�Ȃ�true

    // �J�������Վ��_����t���O
    private bool _IsTopView;                                                                    // �J�����������Վ��_�Ȃ�true

    // ����ւ��E�Ђ�����Ԃ�
    [SerializeField] private float _PlayerTurnSwapCameraWorkTime = 1.0f;                    // �J�������[�N����
    private bool _PlayerIsSwap;                 // �u����ւ����v�t���O
    private bool _PlayerIsTurn;                 // �u���]���v�t���O

    private float _PreLRTrigger;
    private float _PreRStick;

    //------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        // ����FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        // PlayerObject���擾
        _PlayerObject = _GameManagerScript.GetPlayer();
        

        // �v���C���[�̏����g�����X�t�H�[����ݒ�
        _PlayerOldTransform = _PlayerObject.transform.transform;
        _PlayerCurTransform = _PlayerObject.transform.transform;
        // TopView�̃g�����X�t�H�[���ݒ�
        for(int i = 0; i < _TopViewCameraPoint.Length; i++)
        {
            _TopViewTransform.Add(_TopViewCameraPoint[i].transform.transform);
        }
        // TurnOver�̃g�����X�t�H�[���ݒ�
        for (int i = 0; i < _TurnOverPoint.Length; i++)
        {
            _TurnOverTransform.Add(_TurnOverPoint[i].transform.transform);
        }

        // �������_����Վ��_�ɐݒ�
        _IsTopView = true;

        CameraRotateOffset = 90.0f;

        _PreLRTrigger = 0.0f;
        _PreRStick = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float curLRTrigger = Mathf.Abs(Input.GetAxis("Controller_L_R_Trigger"));
        float curRStick = Mathf.Abs(Input.GetAxis("Controller_R_Stick_Vertical"));

        //// �e�X�g�p ////
        // ������������u�v���C���[�̘��Վ��_�v
        if (Input.GetKeyDown(KeyCode.T) || (curLRTrigger > 0.7f && _PreLRTrigger <= 0.7f)) 
        {
            TopViewToPlayerViewCameraWork();
        }
        // ������������u�v���C���[���쎞�v
        if (Input.GetKeyDown(KeyCode.F))
        {
            //PlayerMoveCameraWork();     // �u�ړ��v��
            //PlayerSwapCameraWork();     // �u����ւ��v��
            PlayerTurnCameraWork();     // �u���]�v��
        }
        // ������������u�Q�[���X�^�[�g�v
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameStartCameraWork();
        }
        // ������������u���\���]�v
        if (Input.GetKeyDown(KeyCode.R) || (curRStick > 0.5f && _PreRStick <= 0.5f))
        {
            TurnOverCameraWork();
        }
        //// �e�X�g�p ////
        



        // ����ւ��Ńv���C���[�ɒǏ]���鏈��
        if(_PlayerIsSwap)
        {
            // ���W�����v���C���[�ɒǏ]
            if (_IsFront)
            {
                this.gameObject.transform.position = _GameManagerScript.GetPlayer().transform.position + _PlayerViewPosOffset;
            }
            else
            {
                this.gameObject.transform.position = _GameManagerScript.GetPlayer().transform.position + rPVposOffset;
            }
        }

        _PreLRTrigger = curLRTrigger;
        _PreRStick = curRStick;
    }

    //------------------------------------------------------------------------------------------------------------
    // �u���Վ��_�̃v���C���[�v�̃J�������[�N
    public void TopViewToPlayerViewCameraWork()
    {

        if(_IsTopView)
        {   // �u���Վ��_���v���C���[�v�̃J�������[�N

            if (_IsFront && _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {   // �J�������\�̎��̏���
                // ���Վ��_�ɃJ�������Z�b�g
                this.gameObject.transform.transform.position = _TopViewTransform[0].position;
                this.gameObject.transform.transform.rotation = _TopViewTransform[0].rotation;

                // �v���C���[�Ǐ]���_�ɃJ�������[�N
                iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + _PlayerViewPosOffset), _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
                
                _IsTopView = false;

            }
            else if (!_IsFront && !_PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {   // �J���������̎��̏���
                // ���Վ��_�ɃJ�������Z�b�g
                this.gameObject.transform.transform.position = _TopViewTransform[1].position;
                this.gameObject.transform.transform.rotation = _TopViewTransform[1].rotation;

                // �v���C���[�Ǐ]���_�ɃJ�������[�N
                // ���ʗp�Ƀv���C���[�Ǐ]�J�����̃I�t�Z�b�g��ݒ�
                rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
                iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + rPVposOffset), _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _rPlayerViewRotOffset.x, "y", _rPlayerViewRotOffset.y, "z", _rPlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));

                _IsTopView = false;

            }

        }
        else
        {   // �u�v���C���[�����Վ��_�v�̃J�������[�N
            if (_IsFront)
            {   // �J�������\�̎��̏���
                // �v���C���[�Ǐ]���_�ɃJ�������Z�b�g
                this.gameObject.transform.transform.position = (_PlayerOldTransform.position + _PlayerViewPosOffset);
                this.gameObject.transform.rotation = Quaternion.Euler(_PlayerViewRotOffset.x, _PlayerViewRotOffset.y, _PlayerViewRotOffset.z);

                // ���Վ��_�ɃJ�������[�N
                iTween.MoveTo(this.gameObject, _TopViewTransform[0].position, _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].localEulerAngles.x, "y", (_TopViewTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[0].localEulerAngles.z, "time", _TopViewToPlayerView_RotateTime));
            }
            else
            {   // �J���������̎��̏���
                // �v���C���[�Ǐ]���_�ɃJ�������Z�b�g
                rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
                this.gameObject.transform.transform.position = (_PlayerOldTransform.position + rPVposOffset);
                this.gameObject.transform.rotation = Quaternion.Euler(_rPlayerViewRotOffset.x, _rPlayerViewRotOffset.y, _rPlayerViewRotOffset.z);

                // ���Վ��_�ɃJ�������[�N
                iTween.MoveTo(this.gameObject, _TopViewTransform[1].position, _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TopViewToPlayerView_RotateTime));
            }

            _IsTopView = true;

        }
        
    }


    // �u�v���C���[�ړ��I�����v�̃J�������[�N(�����Fint �ړ���̃u���b�N�z��̗v�f��)-----------------------------------------------------------------------
    public void PlayerMoveCameraWork(Vector2Int BlockNum)
    {
        // �v���C���[�̌��݃g�����X�t�H�[�����X�V
        _PlayerObject = _GameManagerScript.GetPlayer();
        _PlayerCurTransform = _PlayerObject.transform.transform;
        _PlayerOldTransform = _PlayerCurTransform;
        
        // ���̈ړ���u���b�N��Transform���擾
        Transform NextMoveToBlock_transform;
        NextMoveToBlock_transform = _GameManagerScript.GetBlock(BlockNum).transform;
        
        if (_IsFront)
        {   // �J�������\�̎��̏���

            // ���̈ړ���u���b�N���W�ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", NextMoveToBlock_transform.position.x + _PlayerViewPosOffset.x, "y", _PlayerCurTransform.position.y + _PlayerViewPosOffset.y, "z", NextMoveToBlock_transform.position.z + _PlayerViewPosOffset.z, "time", _TopViewToPlayerView_MoveTime));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
            
        }
        else
        {   // �J���������̎��̏���

            // ���̈ړ���u���b�N���W�ɃJ�������[�N
            rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);     // ���ʗp�I�t�Z�b�g
            iTween.MoveTo(this.gameObject, iTween.Hash("x", NextMoveToBlock_transform.position.x + rPVposOffset.x, "y", _PlayerCurTransform.position.y + rPVposOffset.y, "z", NextMoveToBlock_transform.position.z + rPVposOffset.z, "time", _TopViewToPlayerView_MoveTime));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _rPlayerViewRotOffset.x, "y", _rPlayerViewRotOffset.y, "z", _rPlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
            
        }

        _IsTopView = false;

    }


    // �v���C���[����ւ��p�J�������[�N------------------------------------------------------------------------------------------------------
    public void PlayerSwapCameraWork()
    {
        // ����ւ��t���OON
        _PlayerIsSwap = true;

        // �J������rotation�����킹��
        if(_IsFront)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(_PlayerViewRotOffset.x, _PlayerViewRotOffset.y, _PlayerViewRotOffset.z);
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler(_rPlayerViewRotOffset.x, _rPlayerViewRotOffset.y, _rPlayerViewRotOffset.z);

            // �����̂݃I�t�Z�b�g���X�V
            rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
        }
        
        // �w�莞�Ԍ�Ɋ֐����Ăяo��
        Invoke(nameof(PlayerSwap_EndFunc), _PlayerTurnSwapCameraWorkTime);
    }
    void PlayerSwap_EndFunc()
    {
        // ����ւ��t���OOFF
        _PlayerIsSwap = false;

        _IsTopView = false;
    }


    // �v���C���[�Ђ�����Ԃ��p�J�������[�N----------------------------------------------------------------------------------------------
    public void PlayerTurnCameraWork()
    {
        
        if (_IsFront)
        {
            // �����̂݃I�t�Z�b�g���X�V
            rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _PlayerObject.transform.position.x + rPVposOffset.x, "y", _PlayerObject.transform.position.y + rPVposOffset.y, "z", _PlayerObject.transform.position.z + rPVposOffset.z, "time", 0.3f
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _rPlayerViewRotOffset.x, "y", _rPlayerViewRotOffset.y, "z", _rPlayerViewRotOffset.z, "time", _PlayerTurnSwapCameraWorkTime
                    , "easeType", iTween.EaseType.linear));
        }
        else
        {
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _PlayerObject.transform.position.x + _PlayerViewPosOffset.x, "y", _PlayerObject.transform.position.y + _PlayerViewPosOffset.y, "z", _PlayerObject.transform.position.z + _PlayerViewPosOffset.z, "time", 0.3f
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _PlayerTurnSwapCameraWorkTime
                    , "easeType", iTween.EaseType.linear));
        }

        _IsFront = !_IsFront;
        _IsTopView = false;
        
    }


    // �u���\�؂�ւ��v�̃J�������[�N------------------------------------------------------------------------------------------------------------
    public void TurnOverCameraWork()
    {
        if(!_IsTopView)     // ���Վ��_����Ȃ��ꍇ���Վ��_�ɂȂ�J�������[�N
        {
            if (_IsFront)
            {   // �J�������\�̎��̏���
                // ���Վ��_�ɃJ�������[�N
                iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].position.x, "y", _TopViewTransform[0].position.y, "z", _TopViewTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
                
                iTween.RotateTo(this.gameObject
                    ,iTween.Hash("x", _TopViewTransform[0].localEulerAngles.x, "y", (_TopViewTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_MoveTime
                    , "oncomplete", "TurnOverCameraWork2"
                    , "oncompletetarget", this.gameObject));
            }
            else
            {   // �J���������̎��̏���
                // ���Վ��_�ɃJ�������[�N
                iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].position.x, "y", _TopViewTransform[1].position.y, "z", _TopViewTransform[1].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
                
                iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TurnOverCameraWork_MoveTime
                    , "oncomplete", "TurnOverCameraWork2"
                    , "oncompletetarget", this.gameObject));
            }
        }
        else
        {
            this.TurnOverCameraWork2();
        }
    }
    void TurnOverCameraWork2()
    {
        if (_IsFront)
        {   // �J�������\�̎��̏���
            // �\���̃M���M���ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[0].position.x, "y", _TurnOverTransform[0].position.y, "z", _TurnOverTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[0].localEulerAngles.x, "y", (_TurnOverTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork3"
                    , "oncompletetarget", this.gameObject));
        }
        else
        {   // �J���������̎��̏���
            // �����̃M���M���ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[2].position.x, "y", _TurnOverTransform[2].position.y, "z", _TurnOverTransform[2].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[2].localEulerAngles.x, "y", (_TurnOverTransform[2].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[2].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork3"
                    , "oncompletetarget", this.gameObject));
        }
    }
    void TurnOverCameraWork3()
    {
        // ���傤�ǃX�e�[�W�̐^�񒆂ɃJ�������[�N
        iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[1].position.x, "y", _TurnOverTransform[1].position.y, "z", _TurnOverTransform[1].position.z, "time", _TurnOverCameraWork_MoveTime
            , "easeType", iTween.EaseType.linear));
        iTween.RotateTo(this.gameObject
                , iTween.Hash("x", _TurnOverTransform[1].localEulerAngles.x, "y", (_TurnOverTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[1].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                , "easeType", iTween.EaseType.linear
                , "oncomplete", "TurnOverCameraWork4"
                , "oncompletetarget", this.gameObject));
    }
    void TurnOverCameraWork4()
    {
        if (_IsFront)
        {   // �J�������\�̎��̏���
            // �����̃M���M���ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[2].position.x, "y", _TurnOverTransform[2].position.y, "z", _TurnOverTransform[2].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[2].localEulerAngles.x, "y", (_TurnOverTransform[2].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[2].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork5"
                    , "oncompletetarget", this.gameObject));
        }
        else
        {   // �J���������̎��̏���
            // �\���̃M���M���ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[0].position.x, "y", _TurnOverTransform[0].position.y, "z", _TurnOverTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[0].localEulerAngles.x, "y", (_TurnOverTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork5"
                    , "oncompletetarget", this.gameObject));
        }
    }
    void TurnOverCameraWork5()
    {
        if (_IsFront)
        {   // �J�������\�̎��̏���
            // �����̘��Վ��_�ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].position.x, "y", _TopViewTransform[1].position.y, "z", _TopViewTransform[1].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                , "easeType", iTween.EaseType.linear));
        }
        else
        {   // �J���������̎��̏���
            // �\���̘��Վ��_�ɃJ�������[�N
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].position.x, "y", _TopViewTransform[0].position.y, "z", _TopViewTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].localEulerAngles.x, "y", (_TopViewTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                , "easeType", iTween.EaseType.linear));
        }

        _IsFront = !_IsFront;
        _IsTopView = true;

    }


    // �u�Q�[���X�^�[�g���v�̃J�������[�N------------------------------------------------------------------------------------------------------------------------
    public void GameStartCameraWork()
    {
        // �b��I�Ɂu���Վ��_���v���C���[�v�̃J�������[�N��ݒ�

        if (_PlayerObject.GetComponent<PlayerControl>().GetIsFront())
        {   // �v���C���[���\�X�^�[�g�̎��̏���
            // ���Վ��_�ɃJ�������Z�b�g
            this.gameObject.transform.transform.position = _TopViewTransform[0].position;
            this.gameObject.transform.transform.rotation = _TopViewTransform[0].rotation;

            // �v���C���[�Ǐ]���_�ɃJ�������Z�b�g 
            iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + _PlayerViewPosOffset), _TopViewToPlayerView_MoveTime);
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));

            _IsTopView = false;

        }
        else if (!_PlayerObject.GetComponent<PlayerControl>().GetIsFront())
        {   // �v���C���[�����X�^�[�g�̎��̏���
            // ���Վ��_�ɃJ�������Z�b�g
            this.gameObject.transform.transform.position = _TopViewTransform[1].position;
            this.gameObject.transform.transform.rotation = _TopViewTransform[1].rotation;

            // �v���C���[�Ǐ]���_�ɃJ�������Z�b�g 
            // ���ʗp�Ƀv���C���[�Ǐ]�J�����̃I�t�Z�b�g��ݒ�
            Vector3 rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
            iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + rPVposOffset), _TopViewToPlayerView_MoveTime);
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _rPlayerViewRotOffset.x, "y", _rPlayerViewRotOffset.y, "z", _rPlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));

            _IsTopView = false;

        }

    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------
    // IsFront���擾(�J�������\���f���Ă��邩�ۂ�)
    public bool GetCameraWorkIsFront()
    {
        return _IsFront;
    }

    // IsFront��ݒ�(�p�l�����쎞�ɗ��ʂɍs�������Ɏg��)
    public void SetCameraWorkIsFront(bool FrontOrBack)
    {
        _IsFront = FrontOrBack;
    }

    // �J�������Վ��_����t���O
    public bool GetIsTopView()
    {
        return _IsTopView;
    }
    
    public bool GetIsFront() { return _IsFront; }
}
