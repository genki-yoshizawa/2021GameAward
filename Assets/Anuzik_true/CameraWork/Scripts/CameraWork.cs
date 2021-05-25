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

    // TopView�n
    private List<Transform> _TopViewTransform = new List<Transform>();                                        // ���Վ��_�̃g�����X�t�H�[���A0:�\���̘��Վ��_���W�@1:�����̘��Վ��_���W

    // �J�������]�p
    private List<Transform> _TurnOverTransform = new List<Transform>();                                       // ���]�p�̃g�����X�t�H�[���A0:�\���̃M���M���@1:���傤�ǃX�e�[�W�̐^�񒆁@2:�����̃M���M��

    private float CameraRotateOffset;                                                                           // ���x���f�U�C�����~�X������

    // �J�������\����t���O
    [SerializeField] private bool _IsFront;                                                                    // �J���������\�Ȃ�true

    // �J�������Վ��_����t���O
    private bool _IsTopView;                                                                    // �J�����������Վ��_�Ȃ�true



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

    }

    // Update is called once per frame
    void Update()
    {
        //// �e�X�g�p ////
        // ������������u�v���C���[�̘��Վ��_�v
        if(Input.GetKeyDown(KeyCode.T))
        {
            TopViewToPlayerViewCameraWork();
        }
        // ������������u�v���C���[�����v
        if (Input.GetKeyDown(KeyCode.F))
        {
            //PlayerMoveCameraWork();     // �v���C���[���Ă΂Ȃ��ƌ��ؕs�B���Ԃ���v�B

        }
        // ������������u�Q�[���X�^�[�g�v
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameStartCameraWork();
        }
        // ������������u���\���]�v
        if (Input.GetKeyDown(KeyCode.R))
        {
            TurnOverCameraWork();
        }
        //// �e�X�g�p ////
        


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
                Vector3 rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
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
                Vector3 rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
                this.gameObject.transform.transform.position = (_PlayerOldTransform.position + rPVposOffset);
                this.gameObject.transform.rotation = Quaternion.Euler(_rPlayerViewRotOffset.x, _rPlayerViewRotOffset.y, _rPlayerViewRotOffset.z);

                // ���Վ��_�ɃJ�������[�N
                iTween.MoveTo(this.gameObject, _TopViewTransform[1].position, _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TopViewToPlayerView_RotateTime));
            }

            _IsTopView = true;

        }
        
    }


    // �u�v���C���[�ړ��I�����̃J�������[�N(�����Fint �ړ���̃u���b�N�z��̗v�f��)
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
            iTween.MoveTo(this.gameObject, iTween.Hash("x", NextMoveToBlock_transform.position.x + _PlayerViewPosOffset.x, "y", _PlayerCurTransform.position.y + _PlayerViewPosOffset.y, "z", NextMoveToBlock_transform.position.z + _PlayerViewPosOffset.z, "time", _TopViewToPlayerView_RotateTime));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
            
        }
        else
        {   // �J���������̎��̏���

            // ���̈ړ���u���b�N���W�ɃJ�������[�N
            Vector3 rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);     // ���ʗp�I�t�Z�b�g
            iTween.MoveTo(this.gameObject, iTween.Hash("x", NextMoveToBlock_transform.position.x + rPVposOffset.x, "y", _PlayerCurTransform.position.y + rPVposOffset.y, "z", NextMoveToBlock_transform.position.z + rPVposOffset.z, "time", _TopViewToPlayerView_RotateTime));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
            
        }

        _IsTopView = false;

    }


    // �u���\�؂�ւ��v�̃J�������[�N
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


    // �u�Q�[���X�^�[�g���v�̃J�������[�N
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
    
}
