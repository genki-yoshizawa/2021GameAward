using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    // �ŏ��̓v���C���[�h�Ŏn�܂邽�߃L���[�u�I�����[�h���ʃt���O
    private bool _isCubeSelectMode;

    [Header("Hierarchy��Canvas�����Ă�������")]
    [SerializeField] private GameObject _CanvasObject;
    [Header("Hierarchy��DesktopMask�����Ă�������")]
    [SerializeField] private GameObject _DesktopMaskObject;
    private Animator _DesktopMaskAnimator;
    [Header("Hierarchy��LifeWindow�����Ă�������")]
    [SerializeField] private GameObject _LifeWindowObject;
    private Animator _LifeWindowAnimator;
    [Header("Hierarchy��ScoreWindow�����Ă�������")]
    [SerializeField] private GameObject _ScoreWindowObject;
    private Animator _ScoreWindowAnimator;
    [Header("Hierarchy��DescriptWindow�����Ă�������")]
    [SerializeField] private GameObject _DescriptWindowObject;
    private Animator _DescriptWindowAnimator;
    [Header("Hierarchy��PlayWindow�����Ă�������")]
    [SerializeField] private GameObject _PlayWindowObject;
    private Animator _PlayWindowAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _isCubeSelectMode = false;
        
        _DesktopMaskAnimator = _DesktopMaskObject.GetComponent<Animator>();
        _LifeWindowAnimator = _LifeWindowObject.GetComponent<Animator>();
        _ScoreWindowAnimator = _ScoreWindowObject.GetComponent<Animator>();
        _DescriptWindowAnimator = _DescriptWindowObject.GetComponent<Animator>();
        _PlayWindowAnimator = _PlayWindowObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �L���[�u�I�����[�h�ւ̕ύX
    public void ChangeCubeSelectMode()
    {
        _isCubeSelectMode = true;
        _DesktopMaskAnimator.SetBool("isCubeSelectMode", true);
        _LifeWindowAnimator.SetBool("isCubeSelectMode", true);
        _ScoreWindowAnimator.SetBool("isCubeSelectMode", true);
        _DescriptWindowAnimator.SetBool("isCubeSelectMode", true);
        _PlayWindowAnimator.SetBool("isCubeSelectMode", true);

        // �e�q�֌W�̍č\�z
        _PlayWindowObject.transform.parent = _DesktopMaskObject.transform;
        // �q�G�����L�[���Ԃ̍Đݒ�
        _PlayWindowObject.transform.SetSiblingIndex(5);
    }

    // �v���C���[�h�ւ̕ύX
    public void ChangePlayMode()
    {
        _isCubeSelectMode = false;
        _DesktopMaskAnimator.SetBool("isCubeSelectMode", false);
        _LifeWindowAnimator.SetBool("isCubeSelectMode", false);
        _ScoreWindowAnimator.SetBool("isCubeSelectMode", false);
        _DescriptWindowAnimator.SetBool("isCubeSelectMode", false);
        _PlayWindowAnimator.SetBool("isCubeSelectMode", false);
        
        // �e�q�֌W�̍č\�z
        _PlayWindowObject.transform.parent = _CanvasObject.transform;
        // �q�G�����L�[���Ԃ̍Đݒ�
        _PlayWindowObject.transform.SetSiblingIndex(1);
    }

    public bool GetisCubeSelectMode()
    {
        return _isCubeSelectMode;
    }
}
