using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconScrip : MonoBehaviour
{
    [Header("�k�����n�߂鋗��")]
    [SerializeField] private float _MinDistance = 0.0f;
    [Header("�T�C�Y��0�ɂȂ鋗��(����������ɐݒ肷�邱�Ƃő傫���̕ψʗʂ������ł���)")]
    [SerializeField] private float _MaxDistance = 20.0f;
    [Header("�g�b�v�r���[�̎��̃X�P�[��")]
    [SerializeField] private float _WhenTopViewScale = 0.5f;

    private GameObject _CharacterIconObject = null;
    private GameObject _CharacterArrowObject = null;

    private GameObject _GameManager = null;
    private GameObject _PlayerObject = null;
    private GameObject _CameraObject = null;

    // ��ʉ�����ǂꂾ������邩
    private float _FrameDistance = 0f;

    // �ŏ��̃��[�J���X�P�[���̒l
    private Vector3 _DefaultLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = canvasWidth / Screen.width;

        _CharacterIconObject = transform.GetChild(0).gameObject;
        _CharacterArrowObject = transform.GetChild(1).gameObject;

        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _PlayerObject = _GameManager.GetComponent<GameManagerScript>().GetPlayer();
        _CameraObject = _GameManager.GetComponent<GameManagerScript>().GetCamera();

        _DefaultLocalScale = transform.GetComponent<RectTransform>().localScale;

        // �A�C�R����Pivot�ʒu�𒲐�����
        RectTransform characterRect = _CharacterIconObject.GetComponent<RectTransform>();
        RectTransform arrowRect = _CharacterArrowObject.GetComponent<RectTransform>();

        // ��ʉ�����̋������v�Z�����肷��
        // ����height+�L�����N�^�[�A�C�R���Ɩ�󂪗���Ă���(�d�Ȃ��Ă���)����
        arrowRect = SetPivotKeepPosition(arrowRect, new Vector2(0.5f, 0.5f));
        //��ʉ����痣������ = ���A�C�R��height�@+ �L�����A�C�R���̔��� + (�A�C�R���̗���Ă���(�d�Ȃ��Ă���)����  = ���S�ʒu�̋��� - ���ꂼ��̔����̃T�C�Y�̍��v)
        //_FrameDistance = arrowRect.sizeDelta.y * _DefaultLocalScale.y / modifyScale + characterRect.sizeDelta.y * _DefaultLocalScale.y / modifyScale + (characterRect.position.y - arrowRect.position.y) / modifyScale - (arrowRect.sizeDelta.y * 0.5f / modifyScale + characterRect.sizeDelta.y * 0.5f / modifyScale) * _DefaultLocalScale.y;
        _FrameDistance = ((arrowRect.sizeDelta.y + characterRect.sizeDelta.y * 0.5f) * _DefaultLocalScale.y * 0.5f + (characterRect.position.y - arrowRect.position.y)) / modifyScale;
       
        // �V����Pivot�ʒu(����Pivot�̓L�����N�^�[�A�C�R���̒��S)���v�Z����
        arrowRect = SetPivotKeepPosition(arrowRect, new Vector2(0f, 0f));

        Vector3 offsetArrowToCharacter = (characterRect.position - arrowRect.position) * modifyScale;

        arrowRect = SetPivotKeepPosition(arrowRect, new Vector2(offsetArrowToCharacter.x / arrowRect.sizeDelta.x, offsetArrowToCharacter.y / arrowRect.sizeDelta.y));
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[�ƃJ�����������ʂ̂Ƃ��̓v���C���[�ʒu���Q�Ƃ�
        // �t�ʂ̂Ƃ��̓v���C���[�̏��p�l���ʒu���Q�Ƃ���

        // �����ʂ��ǂ�����bool
        bool isSameSide = _PlayerObject.GetComponent<PlayerControl>().GetIsFront() == _CameraObject.GetComponent<MainCameraScript>().GetIsFront();

        Vector3 objectPosition = new Vector3();

        if (isSameSide)
        {

            objectPosition = _PlayerObject.transform.position;
            if (CheckPositionInDisplay(objectPosition))
                PlayerInDisplay();
            else
                PlayerOutDisplay(objectPosition);

        }
        else
        {
            objectPosition = _GameManager.GetComponent<GameManagerScript>().GetBlock(_PlayerObject.GetComponent<PlayerControl>().GetLocalPosition()).transform.position;
            if (CheckPositionInDisplay(objectPosition))
                PanelOnPlayerInDisplay(objectPosition);
            else
                PanelOnPlayerOutDisplay(objectPosition);
        }
    }

    // �v���C���[����ʓ��ɂ��鎞�ɌĂяo���֐�
    private void PlayerInDisplay()
    {
        _CharacterIconObject.GetComponent<Image>().enabled = false;
        _CharacterArrowObject.GetComponent<Image>().enabled = false;
    }

    // �v���C���[����ʊO�ɂ��鎞�ɌĂяo���֐�
    private void PlayerOutDisplay(Vector3 position)
    {
        PointToPositionLoupe(position);
    }

    // �v���C���[�̏��p�l������ʓ��ɂ��鎞�ɌĂяo���֐�
    private void PanelOnPlayerInDisplay(Vector3 position)
    {
        _CharacterIconObject.GetComponent<Image>().enabled = true;
        _CharacterArrowObject.GetComponent<Image>().enabled = true;

        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = Screen.width / canvasWidth;

        // �X�N���[�����W���擾����
        Vector3 iconPosition = ConvertPositionToScreenSpaceOverlayAndDepth(position);

        Vector3 newLocalScale = new Vector3();
        float scale = 1.0f;

        if (_CameraObject.GetComponent<MainCameraScript>().GetIsTop()) //�g�b�v�r���[�̎�
            scale = _WhenTopViewScale;
        else
        {
            // depth�ɉ����ăX�P�[���ϊ�
            // 0~1�̒l
            if (iconPosition.z > _MinDistance)
                scale = (iconPosition.z - _MaxDistance) / (_MinDistance - _MaxDistance);
            else
                scale = 1.0f;
            if (scale <= 0f)
            {
                _CharacterIconObject.GetComponent<Image>().enabled = false;
                _CharacterArrowObject.GetComponent<Image>().enabled = false;
                return;
            }
        }
        newLocalScale = _DefaultLocalScale * scale;
        transform.GetComponent<RectTransform>().localScale = newLocalScale;

        // ��]���f�t�H���g�ɐݒ�
        _CharacterArrowObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
        // iconPosition���w���悤�Ɉʒu�𒲐�
        float newPositionY = iconPosition.y + (_CharacterArrowObject.GetComponent<RectTransform>().sizeDelta.y + _CharacterIconObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f) * modifyScale * newLocalScale.y;
        transform.GetComponent<RectTransform>().position = new Vector3(iconPosition.x, newPositionY, 0.0f);
    }

    // �v���C���[�̏��p�l������ʊO�ɂ��鎞�ɌĂяo���֐�
    private void PanelOnPlayerOutDisplay(Vector3 position)
    {
        PointToPositionLoupe(position);
    }

    // �v���C���[�̂���ʒu�����[�y�Ŏw���֐�
    private void PointToPositionLoupe(Vector3 position)
    {
        _CharacterIconObject.GetComponent<Image>().enabled = true;
        _CharacterArrowObject.GetComponent<Image>().enabled = true;

        // �傫�������ɖ߂�
        transform.GetComponent<RectTransform>().localScale = _DefaultLocalScale;

        // �X�N���[�����W���擾����
        Vector3 iconPosition = ConvertPositionToScreenSpaceOverlayAndDepth(position);
        // �X�N���[�����W���S����̃x�N�g�������
        Vector2 screenCenter = new Vector2(Screen.width, Screen.height);
        screenCenter*= 0.5f;
        iconPosition = iconPosition - new Vector3(screenCenter.x, screenCenter.y, 0.0f);

        // �������x�N�g�����f�t�H���g�Ȃ̂ŁA���̃x�N�g���Ƃ̊p�x�𒲂ׂ�
        Vector2 down = new Vector2(0.0f, -1.0f);
        float angle = Vector2.SignedAngle(down, iconPosition);

        // �J�����̐��ʕ����̋t�ɂ����ꍇ�p�x�������̂Ŕ��]������
        // �J�����̐��ʃx�N�g����position�ւ̃x�N�g���̓��ς����Ȃ�t�ɂ���
        if (Vector3.Dot(_CameraObject.transform.forward, position - _CameraObject.transform.position) < 0f)
            angle += 180 * (angle < 0 ? 1 : -1);

        // �p�x��K�p���A�����v���C���[�̂���ʒu�Ɍ�����
        _CharacterArrowObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, angle);

        Vector3 newPosition = new Vector3();

        if(angle >= -135.0f && angle < -45.0f)
        {//��
            angle += 135.0f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.y = Screen.height - _FrameDistance - (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = _FrameDistance;

        }
        else if(angle >= -45.0f && angle < 45.0f)
        {//��
            angle += 45f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.x = _FrameDistance + (angle / 90.0f) * (Screen.width - 2 * _FrameDistance);
            newPosition.y = _FrameDistance;
        }
        else if(angle >= 45.0f && angle < 135.0f)
        {//�E
            angle -= 45.0f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.y = _FrameDistance + (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = Screen.width - _FrameDistance;
        }
        else
        {//��
            if(angle < 0f)            
                angle = 360f - Mathf.Abs(angle);
            
            angle -= 135f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.x = Screen.width - _FrameDistance - (angle / 90.0f) * (Screen.width - 2 * _FrameDistance);
            newPosition.y = Screen.height - _FrameDistance;
        }

        newPosition.z = 0f;
        transform.GetComponent<RectTransform>().position = newPosition;
    }

    // �����ŗ^����ꂽ���W���`��͈͂����ׂ�֐�
    private bool CheckPositionInDisplay(Vector3 position)
    {
        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        for (int i = 0; i < 6; i++)
        {
            if (!plane[i].GetSide(position))
                return false;
        }

        return true;
    }

    // �X�N���[�����W��Depth���݂ŕϊ�����
    private Vector3 ConvertPositionToScreenSpaceOverlayAndDepth(Vector3 position)
    {
        Vector2 uiPos;
        uiPos = RectTransformUtility.WorldToScreenPoint(Camera.main, position);

        float depth = Vector3.Distance(_CameraObject.transform.position, position);

        Vector3 pos = new Vector3(uiPos.x, uiPos.y, depth);

        return pos;
    }

    // �����ړI�ȍ��W��ς���Pivot���ړ�������֐�
    RectTransform SetPivotKeepPosition(RectTransform rect, Vector2 newPivot)
    {
        float width = rect.sizeDelta.x;
        float height = rect.sizeDelta.y;

        Vector3 oldPos = rect.position;
        Vector2 oldPivot = rect.pivot;

        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = Screen.width / canvasWidth;

        Vector2 pivotOffset = newPivot - oldPivot;
        Vector3 positionOffset = new Vector3(pivotOffset.x * width, pivotOffset.y * height, 0.0f) * modifyScale;

        Vector3 newPosition = oldPos + positionOffset;

        RectTransform newRect = rect;
        newRect.pivot = newPivot;
        newRect.position = newPosition;

        return newRect;

    }
}
