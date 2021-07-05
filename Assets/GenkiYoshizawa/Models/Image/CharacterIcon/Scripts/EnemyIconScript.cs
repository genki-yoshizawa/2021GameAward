using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIconScript : MonoBehaviour
{
    [Header("�k�����n�߂鋗��")]
    [SerializeField] private float _MinDistance = 0.0f;
    [Header("�T�C�Y��0�ɂȂ鋗��(����������ɐݒ肷�邱�Ƃő傫���̕ψʗʂ������ł���)")]
    [SerializeField] private float _MaxDistance = 20.0f;
    [Header("�g�b�v�r���[�̎��̃X�P�[��")]
    [SerializeField] private float _WhenTopViewScale = 0.5f;
    [Header("�G�l�~�[�A�C�R���A�j���[�V�����̎���(�b)")]
    [SerializeField] private float _EnemyIconAnimationCycle = 1.0f;

    private GameObject _CharacterIconObject1 = null;
    private GameObject _CharacterIconObject2 = null;
    private GameObject _CharacterArrowObject = null;

    private Image _CharacterIconImage1 = null;
    private Image _CharacterIconImage2 = null;
    private Image _CharacterArrowImage = null;

    private GameObject _GameManager = null;
    private List<GameObject> _EnemysObject = null;
    private GameObject _CameraObject = null;
    
    // ��ʉ�����ǂꂾ������邩
    private float _FrameDistance = 0f;

    // �ŏ��̃��[�J���X�P�[���̒l
    private Vector3 _DefaultLocalScale;
    
    [Header("Asset����K�p����\�G�l�~�[(enemyA)�A�C�R���e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite _FrontEnemyIconSprite;
    [Header("Asset����K�p����\�G�l�~�[���(enemyA)�e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite _FrontEnemyArrowSprite;

    [Header("Asset����K�p���闠�G�l�~�[���[�y�A�C�R��(enemyB)�e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite _BackEnemyLoupeIconSprite;
    [Header("Asset����K�p���闠�G�l�~�[��󃋁[�y�A�C�R��(enemyB)�e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite _BackEnemyLoupeArrowSprite;

    [Header("Asset����K�p����1�ڂ̗��G�l�~�[�s���A�C�R��(enemyC1)�e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite _BackEnemyPinIconSprite1;
    [Header("Asset����K�p���闠�G�l�~�[���s���A�C�R��(enemyC)�e�N�X�`�������Ă�������")]
    [SerializeField] private Sprite _BackEnemyPinArrowSprite;

    // Start is called before the first frame update
    void Start()
    {
        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = canvasWidth / Screen.width;

        _CharacterIconObject1 = transform.GetChild(0).gameObject;
        _CharacterIconObject2 = transform.GetChild(1).gameObject;
        _CharacterArrowObject = transform.GetChild(2).gameObject;

        _CharacterIconImage1 = _CharacterIconObject1.GetComponent<Image>();
        _CharacterIconImage2 = _CharacterIconObject2.GetComponent<Image>();
        _CharacterArrowImage = _CharacterArrowObject.GetComponent<Image>();

        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _EnemysObject = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
        _CameraObject = _GameManager.GetComponent<GameManagerScript>().GetCamera();

        _DefaultLocalScale = transform.GetComponent<RectTransform>().localScale;

        // �A�C�R����Pivot�ʒu�𒲐�����
        RectTransform characterRect = _CharacterIconObject1.GetComponent<RectTransform>();
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
        // �G�l�~�[�ƃJ�����������ʂ̂Ƃ��̓G�l�~�[�ʒu���Q�Ƃ�
        // �t�ʂ̂Ƃ��̓G�l�~�[�̏��p�l���ʒu���Q�Ƃ���

        // �G�l�~�[�������ɂȂ����ꍇ�A�C�R���\���𕡐��p�ӂ��ĂȂ����ߏ�肭�s���Ȃ����ƂɂȂ��Ă���
        foreach (GameObject enemyObject in _EnemysObject)
        {
            if (enemyObject == null)
                return;

            // �����ʂ��ǂ�����bool
            bool isSameSide = enemyObject.GetComponent<EnemyControl>().GetIsFront() == _CameraObject.GetComponent<MainCameraScript>().GetIsFront();

            Vector3 objectPosition = new Vector3();

            if (isSameSide)
            {
                objectPosition = enemyObject.transform.position;
                if (CheckPositionInDisplay(objectPosition))
                    EnemyInDisplay();
                else
                    EnemyOutDisplay(enemyObject, objectPosition);
            }
            else
            {
                objectPosition = _GameManager.GetComponent<GameManagerScript>().GetBlock(enemyObject.GetComponent<EnemyControl>().GetLocalPosition()).transform.position;
                if (CheckPositionInDisplay(objectPosition))
                    PanelOnEnemyInDisplay(enemyObject, objectPosition);
                else
                    PanelOnEnemyOutDisplay(enemyObject, objectPosition);
            }
        }
    }
    // �G�l�~�[����ʓ��ɂ��鎞�ɌĂяo���֐�
    private void EnemyInDisplay()
    {
        _CharacterIconImage1.enabled = false;
        _CharacterIconImage2.enabled = false;
        _CharacterArrowImage.enabled = false;
    }

    // �G�l�~�[����ʊO�ɂ��鎞�ɌĂяo���֐�
    private void EnemyOutDisplay(GameObject enemy, Vector3 position)
    {
        PointToPositionLoupe(enemy, position);
    }

    // �G�l�~�[�̏��p�l������ʓ��ɂ��鎞�ɌĂяo���֐�
    private void PanelOnEnemyInDisplay(GameObject enemy, Vector3 position)
    {
        _CharacterIconImage1.enabled = true;
        _CharacterArrowImage.enabled = true;

        if (enemy.GetComponent<EnemyControl>().GetIsFront())
        {
            _CharacterIconImage1.sprite = _FrontEnemyIconSprite;
            _CharacterArrowImage.sprite = _FrontEnemyArrowSprite;
        }
        else
        {
            _CharacterIconImage2.enabled = true;

            _CharacterIconImage1.sprite = _BackEnemyPinIconSprite1;
            _CharacterArrowImage.sprite = _BackEnemyPinArrowSprite;
            StartCoroutine(EnemyPinIconAnimation(enemy));
        }

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
                _CharacterIconImage1.enabled = false;
                _CharacterIconImage2.enabled = false;
                _CharacterArrowImage.enabled = false;
                return;
            }
        }
        newLocalScale = _DefaultLocalScale * scale;
        transform.GetComponent<RectTransform>().localScale = newLocalScale;

        // ��]���f�t�H���g�ɐݒ�
        _CharacterArrowObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
        // iconPosition���w���悤�Ɉʒu�𒲐�
        float newPositionY = iconPosition.y + (_CharacterArrowObject.GetComponent<RectTransform>().sizeDelta.y + _CharacterIconObject1.GetComponent<RectTransform>().sizeDelta.y * 0.5f) * modifyScale * newLocalScale.y;
        transform.GetComponent<RectTransform>().position = new Vector3(iconPosition.x, newPositionY, 0.0f);
    }

    // �G�l�~�[�̏��p�l������ʊO�ɂ��鎞�ɌĂяo���֐�
    private void PanelOnEnemyOutDisplay(GameObject enemy, Vector3 position)
    {
        PointToPositionLoupe(enemy, position);
    }

    // �G�l�~�[�̂���ʒu�����[�y�Ŏw���֐�
    private void PointToPositionLoupe(GameObject enemy, Vector3 position)
    {
        _CharacterIconImage1.enabled = true;
        _CharacterArrowImage.enabled = true;

        if (enemy.GetComponent<EnemyControl>().GetIsFront())
        {
            _CharacterIconImage1.sprite = _FrontEnemyIconSprite;
            _CharacterArrowImage.sprite = _FrontEnemyArrowSprite;
        }
        else
        {
            _CharacterIconImage1.sprite = _BackEnemyLoupeIconSprite;
            _CharacterArrowImage.sprite = _BackEnemyLoupeArrowSprite;
        }

        // �傫�������ɖ߂�
        transform.GetComponent<RectTransform>().localScale = _DefaultLocalScale;

        // �X�N���[�����W���擾����
        Vector3 iconPosition = ConvertPositionToScreenSpaceOverlayAndDepth(position);
        // �X�N���[�����W���S����̃x�N�g�������
        Vector2 screenCenter = new Vector2(Screen.width, Screen.height);
        screenCenter *= 0.5f;
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

        if (angle >= -135.0f && angle < -45.0f)
        {//��
            angle += 135.0f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.y = Screen.height - _FrameDistance - (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = _FrameDistance;

        }
        else if (angle >= -45.0f && angle < 45.0f)
        {//��
            angle += 45f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.x = _FrameDistance + (angle / 90.0f) * (Screen.width - 2 * _FrameDistance);
            newPosition.y = _FrameDistance;
        }
        else if (angle >= 45.0f && angle < 135.0f)
        {//�E
            angle -= 45.0f;
            // 90�x�����ڂȂ̂Ń}�W�b�N�i���o�[
            newPosition.y = _FrameDistance + (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = Screen.width - _FrameDistance;
        }
        else
        {//��
            if (angle < 0f)
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

    // �G�l�~�[�s���A�C�R���̃A�j���[�V�����R���[�`��
    IEnumerator EnemyPinIconAnimation(GameObject enemy)
    {
        float passedTime = 0.0f;

        while (true)
        {
            Vector3 objectPosition = _GameManager.GetComponent<GameManagerScript>().GetBlock(enemy.GetComponent<EnemyControl>().GetLocalPosition()).transform.position;

            // �G�l�~�[���\�A�������̓G�l�~�[�ƃJ�����������ʁA�������̓G�l�~�[�̏��p�l�����J�����O�ɗ�����A�j���[�V�����R���[�`���I��
            // CheckPositionInDisplay�������ɒu���̂͏����I�ɖ��ʁH
            if (enemy.GetComponent<EnemyControl>().GetIsFront() ||
                !_CameraObject.GetComponent<MainCameraScript>().GetIsFront() ||
                !CheckPositionInDisplay(objectPosition))
            {
                _CharacterIconImage2.enabled = false;
                break;
            }
            // �R���[�`���J�n�t���[�����牽�b�o�߂�����
            passedTime += Time.deltaTime;

            if (passedTime > _EnemyIconAnimationCycle)
                passedTime -= _EnemyIconAnimationCycle;

            // �w�肳�ꂽ���Ԃɑ΂��Čo�߂������Ԃ̊���
            float ratio = passedTime / _EnemyIconAnimationCycle;

            Color newColor1 = _CharacterIconImage1.color;
            Color newColor2 = _CharacterIconImage2.color;

            newColor1.a = Mathf.Sin(Mathf.Abs(0.5f - ratio) * Mathf.PI);
            newColor2.a = Mathf.Sin(ratio * Mathf.PI);

            _CharacterIconImage1.color = newColor1;
            _CharacterIconImage2.color = newColor2;

            yield return null;
        }
    }
}
