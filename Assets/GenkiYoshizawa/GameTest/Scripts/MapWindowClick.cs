using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapWindowClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Hierarchy��EventSystem�����Ă�������")]
    [SerializeField] private EventSystem _EventSystem;
    [Header("�_�u���N���b�N�̗P�\����(�b)")]
    [SerializeField] private float _DoubleClickTime = 0.2f;

    private bool _isClick;

    private GameObject _GameSceneControllerObject;
    private GameSceneController _GameSceneControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        _isClick = false;

        // find�֐��͏d���̂ŏ��������ɍs��
        _GameSceneControllerObject = GameObject.Find("GameSceneController");
        _GameSceneControllerScript = _GameSceneControllerObject.GetComponent<GameSceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �N���b�N���ꂽ���̊֐�
    public void OnPointerClick(PointerEventData pointerData)
    {
        Debug.Log(gameObject.name + " ���N���b�N���ꂽ!");
        if (!_isClick)
        {
            _isClick = true;
            StartCoroutine(MeasureTime());
        }
        else
            DoubleClick();
    }

    // �_�u���N���b�N�p�^�C���J�E���g�֐�(�R���[�`��)
    IEnumerator MeasureTime()
    {
        // ����^�C���̏�����
        float times = 0f;
        while (_isClick)
        {
            times += Time.deltaTime;
            if (times < _DoubleClickTime)
                yield return null;
            else
            {
                // 0.2f�̊ԂɃN���b�N���Ȃ��ƃN���b�N����𗎂Ƃ�
                _isClick = false;
                yield break;
            }
        }
    }

    // �_�u���N���b�N���̊֐�
    public void DoubleClick()
    {
        _isClick = false;
        if (!_GameSceneControllerScript.GetisCubeSelectMode())
            _GameSceneControllerScript.ChangeCubeSelectMode();
    }
}
